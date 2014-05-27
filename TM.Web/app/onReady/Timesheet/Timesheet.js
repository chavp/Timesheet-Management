Ext.onReady(function () {

    var projectStore = Ext.create('widget.projectStore');

    var timesheetStore = Ext.create('widget.timesheetStore');
    timesheetStore.proxy.extraParams.projectID = -1;
    timesheetStore.proxy.extraParams.fromDateText = paramsView.fromDateText;
    timesheetStore.proxy.extraParams.toDateText = paramsView.toDateText;

    projectStore.load();
    timesheetStore.load({
        callback: function (records, operation, success) {
            if (success) {

            }
        }
    });

    var searchProjectCriFieldset = {
        xtype: 'fieldset',
        title: '<h5>เงื่อนไขในการค้นหา / Criterion</h5>',
        border: false,
        defaultType: 'field',
        defaults: {
            labelWidth: 400,
            anchor: '-20 -20',
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
                queryMode: 'remote',
                allowBlank: true,
                editable: true,
                value: -1
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: 'ช่วงวันที่ / Date (วันอาทิตย์ - วันเสาร์ ของสัปดาห์ปัจจุบัน)',
                name: 'dateBetween',
                layout: 'hbox',
                padding: "0 100 0 0",
                items: [{
                    xtype: 'datefield',
                    id: 'fromStartTimesheet',
                    name: 'fromStartTimesheet',
                    anchor: '100%',
                    fieldLabel: '',
                    flex: 1,
                    format: "d/m/Y",
                    editable: false,
                    vtype: 'daterange',
                    endDateField: 'toStartTimesheet'  // limited to the current date or prior
                }, {
                    xtype: 'datefield',
                    id: 'toStartTimesheet',
                    name: 'toStartTimesheet',
                    anchor: '100%',
                    fieldLabel: '',
                    flex: 1,
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
    }

    var searchTimesheet = function () {
        var form = Ext.getCmp('searchTimesheetForm');
        if (form.isValid()) {
            var values = form.getValues();

            var projectID = values.projectID;
            var fromDate = values.fromStartTimesheet;
            var toDate = values.toStartTimesheet;

            projectID = projectID || -1;

            timesheetStore.proxy.extraParams.projectID = projectID;
            timesheetStore.proxy.extraParams.fromDateText = fromDate;
            timesheetStore.proxy.extraParams.toDateText = toDate;

            timesheetStore.load({
                callback: function (records, operation, success) {
                    if (success) {

                    }
                }
            });
        }
    }

    Ext.create('Ext.panel.Panel', {
        layout: 'border',
        renderTo: 'timesheetPanel',
        height: 560,
        width: 1150,
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
                bodyPadding: "0 150 0 150",
                items: [searchProjectCriFieldset],
                buttonAlign: 'center',
                border: 0,
                buttons: [
                    {
                        text: '<i class="glyphicon glyphicon-search"></i> ค้นหา / Search',
                        handler: function (btn) {
                            searchTimesheet();
                        }
                    }, {
                        text: '<i class="glyphicon glyphicon-trash"></i> ล้างข้อมูล / Clear',
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
                        //align: 'center',
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
                                    editData: record,
                                    animateTarget: row,
                                    modal: true,
                                    timesheetStore: timesheetStore
                                });
                                editForm.setValues(record);
                                editForm.show();
                            }
                        }]
                    },
                    { text: 'ID', dataIndex: 'ID', hidden: true },
                    { text: 'รหัสโครงการ<br/>Project Code', dataIndex: 'ProjectCode', flex: 1 },
                    { text: 'ProjectName', dataIndex: 'ProjectName', hidden: true },
                    { text: 'วันที่<br/>Date', dataIndex: 'StartDate', width: 90, renderer: Ext.util.Format.dateRenderer('d/m/Y') },
                    { text: 'ช่วงโครงการ<br/>Project Phase', dataIndex: 'Phase', flex: 2 },
                    { text: 'ประเภทงาน<br/>Task Type', dataIndex: 'TaskType', width: 80 },
                    { text: 'งานหลัก<br/>Main Task', dataIndex: 'MainTaskDesc', width: 80 },
                    { text: 'งานย่อย<br/>Sub Task', dataIndex: 'SubTaskDesc', flex: 3 },
                    { text: 'จำนวน ชม. ที่ใช้<br/>Used Hour(s)', dataIndex: 'HourUsed', flex: 1 }
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
                            timesheetStore: timesheetStore
                        });

                        addForm.show();
                        addForm.setFocusProject();
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
                            Ext.MessageBox.confirm('ยืนยัน / Confirm', 'คุณต้องการลบ Timesheet ใช่หรือไม่?',
                                function (btn) {
                                    if (btn === "yes") {
                                        Ext.MessageBox.wait("กำลังลบข้อมูล...", 'กรุณารอ / Please wait');
                                        Ext.Ajax.request({
                                            url: paramsView.urlDeleteTimesheet,
                                            success: function (transport) {
                                                Ext.MessageBox.hide();
                                                timesheetStore.load();
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