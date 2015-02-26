Ext.define('TM.view.TimesheetWindow', {
    extend: 'Ext.window.Window',
    xtype: 'timesheetWindow',
    width: 700,
    title: '<i class="glyphicon glyphicon-plus"></i> เพิ่ม Timesheet / Add Timesheet',
    resizable: false,
    closable: false,
    constrain: true,
    config: {
        editData: null,
        timesheetStore: null,

        projectStore: null,
        phaseStore: null,
        tasktypeStore: null,
        maintaskStore: null
    },
    initComponent: function () {
        var me = this;

        me.saveTextAction = messagesForm.saveActionText;

        var projectCodeLabel = 'โครงการ / Project <span class="required">*</span>';

        var showProjectCombo = true;
        var projectDisplay = "";
        if (me.editData) {
            me.title = "แก้ไข Timesheet / Edit Timesheet";
            //me.saveTextAction = '<i class="glyphicon glyphicon-floppy-disk"></i> บันทึกข้อมูล / Save';
            projectCodeLabel = 'โครงการ / Project';
            projectDisplay = me.editData.data.ProjectCode + "(" + me.editData.data.ProjectName + ")";

            showProjectCombo = false;
        }

        me.projectStore.clearFilter();
        me.projectStore.filter([
            { filterFn: function (item) { return item.get("ID") > -1 && item.get("StatusDisplay") === 'Open'; } }
        ]);

        var addAction = Ext.create('Ext.Action', {
            //iconCls: 'add-button',
            text: me.saveTextAction,
            disabled: false,
            handler: function (widget, event) {
                var form = me.down('form');
                if (!form.isValid()) {
                    Ext.MessageBox.show({
                        title: messagesForm.validationTitle,
                        msg: messagesForm.validationWarning,
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.WARNING
                    });

                    return false;
                }
                if (form.isValid()) {
                    var vals = form.getValues();
                    var mainTaskDesc = Ext.getCmp('MainTask').getRawValue();
                    if (me.editData) {

                        var projectCode = Ext.getCmp('ProjectCode').getRawValue();
                        var phase = Ext.getCmp('Phase').getRawValue();
                        var taskType = Ext.getCmp('TaskType').getRawValue();

                        var saveTimesheet = Ext.create('widget.timesheet', {
                            ID: vals.ID,
                            ProjectCode: projectCode,
                            StartDateText: vals.StartDate,
                            Phase: phase,
                            TaskType: taskType,
                            MainTaskDesc: mainTaskDesc,
                            SubTaskDesc: vals.SubTaskDesc,
                            HourUsed: vals.HourUsed,
                            Remark: vals.Remark
                        });

                        //console.log(saveTimesheet);
                        Ext.MessageBox.wait("กำลังบันทึกข้อมูล...", 'กรุณารอ');
                        Ext.Ajax.request({
                            url: paramsView.urlSaveTimesheet,    // where you wanna post
                            success: function (transport) {
                                Ext.MessageBox.hide();
                                var response = Ext.decode(transport.responseText);
                                if (response.success) {
                                    Ext.MessageBox.show({
                                        title: messagesForm.successTitle,
                                        msg: messagesForm.successMsg,
                                        //width: 300,
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.INFO,
                                        fn: function (btn) {
                                            me.close();
                                            me.projectStore.clearFilter();
                                            if (me.timesheetStore) {
                                                me.timesheetStore.load();
                                            }
                                        }
                                    });
                                } else {
                                    Ext.MessageBox.show({
                                        title: messagesForm.errorAlertTitle,
                                        msg: 'เกิดข้อผิดพลาดในการบันทึกข้อมูล <br/>' + response.message,
                                        //width: 300,
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.ERROR
                                    });
                                }

                            },   // function called on success
                            failure: function (transport) {
                                Ext.MessageBox.hide();
                                var errorMsg = "";
                                if (transport.responseText.indexOf('anti-forgery') > 0) {
                                    errorMsg = "anti-forgery กรุณากดเข้า page นี้ใหม่";
                                }
                                Ext.MessageBox.show({
                                    title: messagesForm.errorAlertTitle,
                                    msg: 'เกิดข้อผิดพลาดในการบันทึกข้อมูล <br/>' + errorMsg,
                                    //width: 300,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                            },
                            jsonData: saveTimesheet.data  // your json data
                        });
                    }
                    else
                    {
                        var newTimesheet = Ext.create('widget.timesheet', {
                            ID: vals.ID,
                            ProjectCode: vals.ProjectCode,
                            StartDateText: vals.StartDate,
                            Phase: vals.Phase,
                            TaskType: vals.TaskType,
                            MainTaskDesc: mainTaskDesc,
                            SubTaskDesc: vals.SubTaskDesc,
                            HourUsed: vals.HourUsed,
                            Remark: vals.Remark
                        });
                        //console.log(newTimesheet.data);
                        Ext.MessageBox.wait("กำลังเพิ่มข้อมูล...", 'กรุณารอ');

                        var jsonData = newTimesheet.data;

                        Ext.Ajax.request({
                            url: paramsView.urlAddTimesheet,    // where you wanna post
                            success: function (transport) {
                                Ext.MessageBox.hide();
                                var respose = Ext.decode(transport.responseText);
                                
                                if (respose.success) {
                                    Ext.MessageBox.show({
                                        title: messagesForm.successTitle,
                                        msg: 'เพิ่มข้อมูลเสร็จสมบูรณ์',
                                        //width: 300,
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.INFO,
                                        fn: function (btn) {
                                            form.getForm().reset();
                                            Ext.getCmp('ProjectCode').focus();
                                            //console.log('success');
                                            if (me.timesheetStore) {
                                                me.timesheetStore.load();
                                            }
                                        }
                                    });

                                } else {
                                    Ext.MessageBox.show({
                                        title: messagesForm.errorAlertTitle,
                                        msg: 'เกิดข้อผิดพลาดในการเพิ่มข้อมูล ' + respose.message,
                                        //width: 300,
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.ERROR
                                    });
                                }
                            },   // function called on success
                            failure: function (transport) {
                                Ext.MessageBox.hide();
                                //console.log(transport);
                                var errorMsg = "";
                                if (transport.responseText.indexOf('anti-forgery') > 0) {
                                    errorMsg = "anti-forgery กรุณากดเข้า page นี้ใหม่";
                                }
                                Ext.MessageBox.show({
                                    title: messagesForm.errorAlertTitle,
                                    msg: 'เกิดข้อผิดพลาดในการเพิ่มข้อมูล <br/>' + errorMsg,
                                    //width: 300,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                            },
                            jsonData: jsonData  // your json data
                        });
                    }
                }
            }
        });

        var cancleAction = Ext.create('Ext.Action', {
            //iconCls: 'add-button',
            text: messagesForm.cancleActionText,
            disabled: false,
            handler: function (widget, event) {
                me.projectStore.clearFilter();
                me.close();
            }
        });

        //console.log(paramsView.maxDateText);

        var hourUsedStore = Ext.create('Ext.data.Store', {
            fields: ['value', 'text'],
            data: []
        });
        for (var i = 0.5; i <= 24; i+=0.5) {
            hourUsedStore.add({
                value: i,
                text: i
            });
        }
        me.items = [{
            xtype: 'form',
            layout: {
                type: 'vbox',
                align: 'stretch'
            },
            frame: false,
            width: '100%',
            border: 0,
            bodyStyle: 'padding: 6px',
            fieldDefaults: {
                labelWidth: 200,
                allowBlank: false,
                labelAlign: 'right'
            },
            items: [{
                name: 'ID',
                xtype: 'textarea',
                allowBlank: true,
                hidden: true
            }, {
                xtype: 'displayfield',
                id: 'Project',
                name: 'Project',
                fieldLabel: 'โครงการ / Project',
                fieldStyle: 'font-weight: bold;',
                value: projectDisplay,
                hidden: showProjectCombo
            }, {
                xtype: 'combo',
                id: 'ProjectCode',
                name: 'ProjectCode',
                store: me.projectStore,
                queryMode: 'local',
                displayField: 'Display',
                valueField: 'ID',
                forceSelection: true,
                minChars: 1,
                fieldLabel: projectCodeLabel,
                emptyText: messagesForm.requireInputEmptyText,
                editable: true,
                disabled: (me.editData),
                hidden: !showProjectCombo,
                anyMatch: true,
                listConfig: { itemTpl: highlightMatch.createItemTpl('Display', 'ProjectCode') }
            }, {
                xtype: 'datefield',
                id: 'StartDate',
                name: 'StartDate',
                padding: '0 350 0 0',
                fieldLabel: 'วันที่ / Date <span class="required">*</span>',
                format: "d/m/Y",
                value: new Date(),
                maxValue: paramsView.maxDateText,
                editable: false
            }, {
                xtype: 'combo',
                id: 'Phase',
                name: 'Phase',
                store: me.phaseStore,
                queryMode: 'local',
                displayField: 'Name',
                valueField: 'ID',
                fieldLabel: TextLabel.projectPhaseTitle + ' <span class="required">*</span>',
                emptyText: messagesForm.requireSelectEmptyText,
                editable: false
            }, {
                xtype: 'combo',
                id: 'TaskType',
                name: 'TaskType',
                store: me.tasktypeStore,
                queryMode: 'local',
                displayField: 'Name',
                valueField: 'ID',
                fieldLabel: TextLabel.projectTaskTypeTitle + ' <span class="required">*</span>',
                emptyText: messagesForm.requireSelectEmptyText,
                value: 1,
                editable: false
            }, {
                xtype: 'combo',
                name: 'MainTaskDesc',
                id: 'MainTask',
                store: me.maintaskStore,
                queryMode: 'local',
                displayField: 'Name',
                valueField: 'ID',
                fieldLabel: TextLabel.projectMainTaskTitle + ' <span class="required">*</span>',
                emptyText: messagesForm.requireInputEmptyText,
                minChars: 1,
                editable: true,
                anyMatch: true,
                listConfig: { itemTpl: highlightMatch.createItemTpl('Name', 'MainTask') }
            }, {
                xtype: 'textarea',
                id: 'SubTaskDesc',
                name: 'SubTaskDesc',
                fieldLabel: 'งานย่อย / Sub Task <span class="required">*</span>',
                height: 200,
                emptyText: messagesForm.requireInputEmptyText
            },
            {
                xtype: 'combo',
                name: 'HourUsed',
                id: 'HourUsed',
                fieldLabel: 'จำนวน ช.ม. ที่ใช้ / Used Hour(s) <span class="required">*</span>',
                emptyText: 'กรุณาระบุข้อมูล',
                padding: '0 350 0 0',
                forceSelection: true,
                store: hourUsedStore,
                queryMode: 'local',
                displayField: 'text',
                valueField: 'value',
                editable: true
            }
            //, {
            //    xtype: 'numberfield',
            //    name: 'HourUsed',
            //    fieldLabel: 'จำนวน ช.ม. ที่ใช้ / Used Hour(s) <span class="required">*</span>',
            //    emptyText: 'กรุณาระบุ',
            //    padding: '0 350 0 0',
            //    decimalPrecision: 1,
            //    step: 0.5,
            //    minValue: 0.5,
            //    maxValue: 24
            //}
            , {
                xtype: 'textarea',
                name: 'Remark',
                fieldLabel: 'หมายเหตุ / Remark',
                height: 50,
                maxLength: 255,
                allowBlank: true
            }],
            buttonAlign: 'center',
            buttons: [
                new Ext.button.Button(addAction),
                new Ext.button.Button(cancleAction)
            ]
        }];
        me.callParent();
    },
    
    setValues: function(record){
        var form = this.down('form').getForm();

        form.setValues(record.data);
        Ext.getCmp('ProjectCode').setReadOnly(true);
    },

    setFocusProject: function () {
        //Ext.getCmp('ProjectCode').focus();
        //Ext.getCmp('ProjectCode').expand();

        Ext.getCmp('ProjectCode').focus();
        
    },
    listeners: {
        show: function (w, eOpts) {
            Ext.getCmp('ProjectCode').focus(false, true);
        }
    }
});