Ext.onReady(function () {

    var projectStore = Ext.create('widget.projectStore');
    projectStore.load({
        url: paramsView.urlReadProject
    });

    var timesheetStore = Ext.create('widget.timesheetStore');
    timesheetStore.proxy.extraParams.projectID = -1;
    timesheetStore.proxy.extraParams.fromDateText = paramsView.fromDateText;
    timesheetStore.proxy.extraParams.toDateText = paramsView.toDateText;
    timesheetStore.load({
        url: paramsView.urlGetTimesheet
    });

    var phaseStore = Ext.create('widget.phaseStore');
    phaseStore.load({
        url: paramsView.urlGetAllPhase
    });

    var tasktypeStore = Ext.create('widget.tasktypeStore');
    tasktypeStore.load({
        url: paramsView.urlGetAllTaskType
    });

    var maintaskStore = Ext.create('widget.maintaskStore');
    maintaskStore.load({
        url: paramsView.urlGetMainTask
    });

    var searchProjectCriFieldset = {
        xtype: 'fieldset',
        title: '<h5>' + TextLabel.searchCriterionLabel + '</h5>',
        border: false,
        defaultType: 'field',
        defaults: {
            labelWidth: 220,
            anchor: '95%',
            allowBlank: false,
            labelAlign: 'right'
        },
        bodyPadding: 10,
        items: [
            {
                xtype: 'combo',
                id: 'projectID',
                name: 'projectID',
                store: projectStore,
                fieldLabel: 'โครงการ / Project',
                displayField: 'Display',
                valueField: 'ID',
                forceSelection: true,
                minChars: 1,
                triggerAction: 'all',
                queryMode: 'local',
                allowBlank: true,
                editable: true,
                value: -1,
                initialValue: -1,
                listeners: {
                    scope: this,
                    afterRender: function (t, o) {
                        t.value = t.initialValue;
                    }
                },
                listConfig: { itemTpl: highlightMatch.createItemTpl('Display', 'projectID') }
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: 'ช่วงวันที่ / Date<br/>(วันอาทิตย์ - วันเสาร์ ของสัปดาห์ปัจจุบัน)',
                name: 'dateBetween',
                layout: 'hbox',
                items: [{
                    xtype: 'datefield',
                    id: 'fromStartTimesheet',
                    name: 'fromStartTimesheet',
                    anchor: '100%',
                    fieldLabel: '',
                    width: 100,
                    format: "d/m/Y",
                    editable: false,
                    vtype: 'daterange',
                    endDateField: 'toStartTimesheet'  // limited to the current date or prior
                }, {
                    xtype: 'displayfield',
                    margin: '0 5 0 5',
                    value: '-'
                }, {
                    xtype: 'datefield',
                    id: 'toStartTimesheet',
                    name: 'toStartTimesheet',
                    anchor: '100%',
                    fieldLabel: '',
                    width: 100,
                    format: "d/m/Y",
                    editable: false,
                    vtype: 'daterange',
                    startDateField: 'fromStartTimesheet'// defaults to today
                }]
            }
        ]
    };

    // set Default
    var setDefault = function () {
        Ext.getCmp('fromStartTimesheet').setValue(paramsView.fromDateText);
        Ext.getCmp('toStartTimesheet').setValue(paramsView.toDateText);
    };

    var searchTimesheet = function () {
        var form = Ext.getCmp('searchTimesheetForm');
        if (form.isValid()) {
            var values = form.getValues();

            var projectID = values.projectID;
            var fromDate = values.fromStartTimesheet;
            var toDate = values.toStartTimesheet;

            projectID = projectID || -1;

            timesheetStore.currentPage = 1;
            timesheetStore.proxy.extraParams.projectID = projectID;
            timesheetStore.proxy.extraParams.fromDateText = fromDate;
            timesheetStore.proxy.extraParams.toDateText = toDate;

            timesheetStore.load({
                url: paramsView.urlGetTimesheet
            });
        }
    };

    Ext.create('Ext.panel.Panel', {
        layout: 'border',
        renderTo: 'timesheetPanel',
        height: WindowHeight.height,
        width: 'auto',
        border: 1,
        defaults: {
            frame: false,
            split: false
        },
        items: [
            {
                xtype: 'form',
                id: 'searchTimesheetForm',
                title: '',
                region: 'north',
                //height: 160,
                collapsible: false,
                bodyPadding: "0 20 0 20",
                items: [searchProjectCriFieldset],
                buttonAlign: 'center',
                border: 0,
                buttons: [
                    {
                        text: TextLabel.cmdSearchText,
                        handler: function (btn) {
                            searchTimesheet();
                        }
                    }, {
                        text: TextLabel.cmdClearText,
                        handler: function (btn) {
                            var form = Ext.getCmp('searchTimesheetForm').getForm();
                            form.reset();

                            setDefault();
                            searchTimesheet();
                        }
                    }
                ]
            },
            {
                xtype: 'gridpanel',
                id: 'gridTimesheet',
                frame: false,
                region: 'center',
                title: '',
                store: timesheetStore,
                selType: 'checkboxmodel',
                checkOnly: true,
                mode: 'SINGLE',
                columns: [
                    {
                        xtype: 'rownumberer',
                        width: 30,
                        sortable: false,
                        locked: true
                    }, {
                        xtype: 'actioncolumn',
                        sortable: false,
                        menuDisabled: true,
                        width: 30,
                        items: [{
                            // Use a URL in the icon config
                            xtype: 'button',
                            tooltip: 'แก้ไขข้อมูล Timesheet / Edit Timesheet',
                            iconCls: 'edit-timesheet-icon',
                            handler: function (grid, rowIndex, colIndex, item, event, record, row) {
                                //var record = grid.getStore().getAt(rowIndex);
                                //grid.getSelectionModel().select(record);

                                var editForm = Ext.create('widget.timesheetWindow', {
                                    iconCls: 'edit-timesheet-icon',
                                    editData: record,
                                    animateTarget: row,
                                    modal: true,
                                    timesheetStore: timesheetStore,

                                    projectStore: projectStore,
                                    phaseStore: phaseStore,
                                    tasktypeStore: tasktypeStore,
                                    maintaskStore: maintaskStore

                                });
                                editForm.setValues(record);
                                editForm.show();
                            }
                        }]
                    },
                    { text: 'ID', dataIndex: 'ID', hidden: true, sortable: true },
                    { text: 'รหัสโครงการ<br/>Project Code', dataIndex: 'ProjectCode', width: 110, sortable: true },
                    { text: 'ProjectName', dataIndex: 'ProjectName', hidden: true, sortable: true },
                    { text: 'วันที่<br/>Date', dataIndex: 'StartDate', width: 90, sortable: true, renderer: Ext.util.Format.dateRenderer('d/m/Y') },
                    { text: 'ช่วงโครงการ<br/>Project Phase', dataIndex: 'Phase', sortable: true, width: 230 },
                    { text: 'ประเภทงาน<br/>Task Type', dataIndex: 'TaskType', sortable: true, width: 80 },
                    { text: 'งานหลัก<br/>Main Task', dataIndex: 'MainTaskDesc', sortable: true, width: 150 },
                    {
                        text: 'งานย่อย<br/>Sub Task', dataIndex: 'SubTaskDesc', sortable: true, width: 150,
                        renderer: function (value, metaData, record, rowIdx, colIdx, store) {
                            // Sample value: msimms & Co. "like" putting <code> tags around your code

                            value = Ext.String.htmlEncode(value);

                            // "double-encode" before adding it as a data-qtip attribute
                            metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';

                            return value;
                        }
                    },
                    { text: 'จำนวน ชม. ที่ใช้<br/>Used Hour(s)', dataIndex: 'HourUsed', sortable: true, flex: 1 }
                ],
                listeners: {
                    selectionchange: function (obj, selected, eOpts) {
                        var gridTimesheet = Ext.getCmp('gridTimesheet');
                        var selected = gridTimesheet.getSelectionModel().getSelection();
                        var cmdDeleteTimesheet = Ext.getCmp('cmdDeleteTimesheet');
                        if (selected.length > 0) {
                            cmdDeleteTimesheet.setDisabled(false);
                            return;
                        }
                        cmdDeleteTimesheet.setDisabled(true);
                    }
                },
                tbar: [{
                    cls: 'btn',
                    xtype: 'button',
                    iconCls: 'glyphicon glyphicon-plus',
                    text: "เพิ่มข้อมูล / Add Timesheet",
                    handler: function (btn, evt) {
                        var addForm = Ext.create('widget.timesheetWindow', {
                            animateTarget: btn,
                            modal: true,
                            timesheetStore: timesheetStore,

                            projectStore: projectStore,
                            phaseStore: phaseStore,
                            tasktypeStore: tasktypeStore,
                            maintaskStore: maintaskStore
                        });

                        addForm.show();
                    }
                }, {
                    cls: 'btn',
                    xtype: 'button',
                    id: 'cmdDeleteTimesheet',
                    iconCls: 'glyphicon glyphicon-minus',
                    text: "ลบข้อมูล / Delete Timesheet",
                    disabled: true,
                    handler: function (btn, evt) {

                        var gridTimesheet = Ext.getCmp('gridTimesheet');
                        var selected = gridTimesheet.getSelectionModel().getSelection();
                        var listOfDelete = [];
                        //console.log(selected);
                        for (var i = 0; i < selected.length; i++) {
                            var id = selected[i].data.ID;
                            listOfDelete.push(id);
                            //console.log(id);
                        }

                        if (listOfDelete.length > 0) {
                            Ext.MessageBox.confirm('ยืนยัน / Confirm', 'คุณต้องการลบ Timesheet ใช่ หรือ ไม่?',
                                function (btn) {
                                    if (btn === "yes") {
                                        Ext.MessageBox.wait("กำลังลบข้อมูล...", 'กรุณารอ / Please wait');
                                        Ext.Ajax.request({
                                            url: paramsView.urlDeleteTimesheet,
                                            success: function (transport) {
                                                Ext.MessageBox.hide();
                                                var respose = Ext.decode(transport.responseText);
                                                if (respose.success) {
                                                    Ext.MessageBox.show({
                                                        title: messagesForm.successTitle,
                                                        msg: respose.message,
                                                        //width: 300,
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.INFO,
                                                        fn: function (btn) {
                                                            searchTimesheet();
                                                        }
                                                    });
                                                } else {
                                                    Ext.MessageBox.show({
                                                        title: messagesForm.errorAlertTitle,
                                                        msg: 'เกิดข้อผิดพลาดในขั้นตอนการลบ Timesheet<br/>' + respose.message,
                                                        //width: 300,
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.ERROR
                                                    });
                                                }
                                            },   // function called on success
                                            failure: function (transport) {
                                                Ext.MessageBox.hide();
                                                Ext.MessageBox.show({
                                                    title: messagesForm.errorAlertTitle,
                                                    msg: "เกิดข้อผิดพลาดในขั้นตอนลบข้อมูล " + transport.responseText,
                                                    //width: 300,
                                                    buttons: Ext.MessageBox.OK,
                                                    icon: Ext.MessageBox.ERROR
                                                });
                                            },
                                            jsonData: listOfDelete  // your json data
                                        });
                                    }
                                });
                        }
                    }
                }],
                // paging bar on the bottom
                bbar: Ext.create('Ext.PagingToolbar', {
                    store: timesheetStore,
                    displayInfo: true,
                    displayMsg: 'Timesheet ที่กำลังแสดงอยู่ {0} - {1} of {2}',
                    emptyMsg: "ไม่มี Timesheet"
                })
            }
        ]
    });

    setDefault();
});