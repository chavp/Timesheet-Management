Ext.define('TM.view.EditProjectMemberWindow', {
    extend: 'Ext.window.Window',
    xtype: 'editProjectMemberWindow',
    width: 1000,
    title: 'แก้ไขผู้รับผิดชอบ / Edit Member',
    resizable: false,
    closable: false,

    config: {
        projectData: null,
        departmentStore: null,
        divisionStore: null
    },

    initComponent: function () {
        var me = this;

        //Project / Project Member :  PJ-RVP013(PVR Service Pool)
        //me.title += ": " + me.projectData.data.Code;
        me.projectCode = me.projectData.data.Code;
        var project = me.projectData.data.Code + "(" + me.projectData.data.Name + ")";

        var projectRoleStore = Ext.create('widget.projectRoleStore',
            {
                autoLoad: true
            });

        var memberStore = Ext.create('widget.projectMemberStore', {
            pageSize: 11,
            listeners: {
                datachanged: function (st, eOpts) {
                    //me.projectData.set("Members", memberStore.data.length);
                    //me.projectData.commit();
                }
            }
        });
        memberStore.proxy.extraParams.projectCode = me.projectCode;
        memberStore.load({
            callback: function (records, operation, success) {
                if (success) {
                   
                } else {
                    //Ext.MessageBox.show({
                    //    title: messagesForm.errorAlertTitle,
                    //    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Project Member',
                    //    //width: 300,
                    //    buttons: Ext.MessageBox.OK,
                    //    icon: Ext.MessageBox.ERROR
                    //});
                }
            }
        });
        me.memberStore = memberStore;

        var cancleAction = Ext.create('Ext.Action', {
            //iconCls: 'add-button',
            text: '<i class="glyphicon glyphicon-remove"></i> ปิด / Close',
            disabled: false,
            handler: function (widget, event) {
                var modifiedRecords = me.memberStore.getModifiedRecords();
                var isAnswer = false;
                if (modifiedRecords.length > 0) {
                    Ext.MessageBox.alert({
                        title: 'แจ้งเตือน / Warning',
                        msg: 'มีการกำหนดหน้าที่ในโครงการ ท่านต้องการยกเลิกการดำเนินการนี้ ใช่ หรือ ไม่?',
                        //width: 300,
                        buttons: Ext.MessageBox.YESNO,
                        icon: Ext.MessageBox.QUESTION,
                        fn: function (btn) {
                            if (btn === 'yes') {
                                me.close();
                            }
                        }
                    });
                } else {
                    me.close();
                }
            }
        });

        this.items = [{
            xtype: 'panel',
            layout: 'border',
            frame: false,
            width: '100%',
            height: 520,
            border: 0,
            items: [
                {
                    xtype: 'form',
                    region: 'north',
                    defaults: {
                        frame: false,
                        split: false,
                        labelAlign: 'right'
                    },
                    items: [{
                        xtype: 'displayfield',
                        id: 'Project',
                        name: 'Project',
                        labelWidth: 210,
                        value: project,
                        fieldStyle: 'font-weight: bold;',
                        fieldLabel: 'โครงการ / Project',
                        frame: false
                    }]
                },
                {
                    region: 'center',
                    xtype: 'form',
                    id: 'searchEmployeeForm',
                    layout: 'border',
                    //height: 500,
                    border: 0,
                    width: '100%',
                    //bodyStyle: 'margin: 10px',
                    defaults: {
                        frame: false,
                        split: false
                    },
                    items: [
                        {
                            xtype: 'panel',
                            region: 'center',
                            layout: {
                                type: 'hbox',
                                pack: 'start',
                                align: 'stretch'
                            },
                            items: [
                                {
                                    xtype: 'gridpanel',
                                    title: 'พนักงานที่เป็นสมาชิกในโครงการ / Project Members',
                                    id: 'gridProjectMember',
                                    flex: 20,
                                    store: memberStore,
                                    plugins: [{
                                        ptype: 'cellediting',
                                        clicksToEdit: 1
                                    }],
                                    listeners: {
                                        beforeedit: function (plugin, edit) {
                                            if (!edit.record.get('CanEditProjectRole')) {
                                                return false;
                                            }
                                        }
                                    },
                                    columns: [
                                        //{ hidden: false, text: '*', dataIndex: 'CanEditProjectRole', width: 50 },
                                        //{ hidden: false, text: '-', dataIndex: 'CanRemove', width: 50 },
                                        { xtype: 'rownumberer', width: 35, sortable: false, locked: true },
                                        { text: 'ID', dataIndex: 'ID', flex: 1, hidden: true },
                                        {
                                            text: 'หน้าที่ในโครงการ<br/>Project Role',
                                            dataIndex: 'ProjectRoleName',
                                            flex: 1,
                                            emptyText: 'กรุณาเลือก',
                                            editor:
                                            {
                                                xtype: 'combobox',
                                                queryMode: 'local',
                                                store: projectRoleStore,
                                                displayField: 'ProjectRoleName',
                                                valueField: 'ProjectRoleName',
                                                editable: false
                                            },
                                            renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                                                if (!record.data.CanEditProjectRole) {
                                                    metaData.css = 'cellreadonly';
                                                }
                                                else {
                                                    metaData.css = 'cellediting';
                                                }
                                                return value;
                                            }
                                        },
                                        { text: 'รหัส<br/>Emp ID', dataIndex: 'EmployeeID', width: 100 },
                                        { text: 'ชื่อ-นามสกุล<br/>Name', dataIndex: 'FullName', width: 180 },
                                        { text: 'ตำแหน่ง<br/>Position', dataIndex: 'Position', flex: 1 }
                                    ],
                                    buttonAlign: 'center',
                                    buttons: [
                                        {
                                            text: '<i class="glyphicon glyphicon-floppy-disk"></i> บันทึก / Save',
                                            handler: function (btn) {
                                                var modifiedRecords = memberStore.getModifiedRecords();
                                                for (var i = 0; i < modifiedRecords.length; i++) {
                                                    var modified = modifiedRecords[i];
                                                    if (modified.get('ProjectRoleName') === null
                                                        || modified.get('ProjectRoleName') === undefined
                                                        || modified.get('ProjectRoleName') === '') {

                                                        Ext.MessageBox.show({
                                                            title: messagesForm.validationTitle,
                                                            msg: messagesForm.validationAddProjectMember,
                                                            //width: 300,
                                                            buttons: Ext.MessageBox.OK,
                                                            icon: Ext.MessageBox.WARNING
                                                        });

                                                        return false;
                                                    }
                                                }

                                                //console.log(modifiedRecords);
                                                if (modifiedRecords.length > 0) {
                                                    var modifiedProjectMembers = [];

                                                    for (var i = 0; i < modifiedRecords.length; i++) {
                                                        var record = modifiedRecords[i];
                                                        modifiedProjectMembers.push(record.data);
                                                    }

                                                    //console.log(modifiedProjectMembers);

                                                    Ext.MessageBox.wait("กำลังบันทึกข้อมูล...", 'กรุณารอ');
                                                    Ext.Ajax.request({
                                                        url: paramsView.urlSaveProjectRole,    // where you wanna post
                                                        success: function (transport) {
                                                            Ext.MessageBox.hide();
                                                            //Ext.MessageBox.alert('Success',
                                                            //                        "บันทึกข้อมูลเสร็จสมบูรณ์",
                                                            //                        function (btn) {
                                                            //                            memberStore.commitChanges();
                                                            //                        });
                                                            Ext.MessageBox.show({
                                                                title: messagesForm.successTitle,
                                                                msg: messagesForm.successMsg,
                                                                //width: 300,
                                                                buttons: Ext.MessageBox.OK,
                                                                icon: Ext.MessageBox.INFO,
                                                                fn: function (btn) {
                                                                    me.refreshMemberStore();
                                                                    //memberStore.commitChanges();
                                                                }
                                                            });

                                                        },   // function called on success
                                                        failure: function (transport) {
                                                            Ext.MessageBox.hide();
                                                            Ext.MessageBox.show({
                                                                title: messagesForm.errorAlertTitle,
                                                                msg: 'เกิดข้อผิดพลาดในการเพิ่มข้อมูล Project Member ',
                                                                //width: 300,
                                                                buttons: Ext.MessageBox.OK,
                                                                icon: Ext.MessageBox.ERROR
                                                            });
                                                        },
                                                        jsonData: {
                                                            projectCode: me.projectCode,
                                                            projectMemberViewList: modifiedProjectMembers
                                                        }
                                                    });
                                                }
                                            }
                                        }
                                    ],
                                    bbar: Ext.create('Ext.PagingToolbar', {
                                        //store: timesheetStore,
                                        displayInfo: true,
                                        store: memberStore,
                                        displayMsg: 'ข้อมูล {0} - {1} of {2}',
                                        emptyMsg: "ไม่มีสมาชิกในโครงการ"
                                    })
                                }
                            ]
                        }
                    ]
                }
            ],
            buttons: [new Ext.button.Button(cancleAction)]
        }];

        this.callParent();
    },

    refreshMemberStore: function (callback) {
        var self = this;

        self.memberStore.load({
            callback: function (records, operation, success) {
                if (success) {
                    if (callback) callback();

                    self.projectData.set("Members", records.length);
                    self.projectData.commit();
                } else {
                    //Ext.MessageBox.show({
                    //    title: messagesForm.errorAlertTitle,
                    //    msg: 'เกิดข้อผิดพลาดในการ load ข้อมูล Project Member',
                    //    //width: 300,
                    //    buttons: Ext.MessageBox.OK,
                    //    icon: Ext.MessageBox.ERROR
                    //});
                }
            }
        });
    }
});