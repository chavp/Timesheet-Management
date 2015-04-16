Ext.define('TM.view.ProjectWindow', {
    extend: 'Ext.window.Window',
    xtype: 'projectWindow',
    width: 1000,
    title: 'เพิ่มผู้รับผิดชอบ / Add Member',
    resizable: false,
    closable: false,
    constrain: true,
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

        var errorLoadEmployee = "เกิดข้อผิดพลาดในการค้นข้อมูล Employee";

        var employeeStore = Ext.create('widget.employeeStore', {
            pageSize: 50,
            url: paramsView.urlReadEmployee
        });
        me.employeeStore = employeeStore;
        employeeStore.reset();

        var searchEmployeeAction = function (obj) {
            var form = Ext.getCmp('searchEmployeeForm');
            if (form.isValid()) {
                var values = form.getValues();

                var newProjectMembers = memberStore.getNewRecords();
                var modifiedProjectMembers = memberStore.getModifiedRecords();
                var removedProjectMembers = memberStore.getRemovedRecords();

                var division = values.Division;
                var department = values.Department;
                var employeeID = values.EmployeeID;
                var firstName = values.FirstName;
                var lastName = values.LastName;

                employeeStore.currentPage = 1;
                employeeStore.proxy.extraParams.divisionID = division;
                employeeStore.proxy.extraParams.departmentID = department;
                employeeStore.proxy.extraParams.employeeID = employeeID;
                employeeStore.proxy.extraParams.employeeFirstName = firstName;
                employeeStore.proxy.extraParams.employeeLastName = lastName;
                employeeStore.proxy.extraParams.projectCode = me.projectCode;

                //var listOfProjectMemberList = [];
                //memberStore.each(function (rec) {
                //    listOfProjectMemberList.push(rec.data.EmployeeID);
                //});
                //employeeStore.proxy.extraParams.withoutEmpIDList = listOfProjectMemberList;
                me.setWithoutEmpIDList();
                if (employeeStore.proxy.extraParams.withoutEmpIDList.length <= paramsConst.limitMember) {
                    employeeStore.load({
                        url: paramsView.urlReadEmployee,
                        callback: function (records, operation, success) {
                            if (success) {

                            } else {
                                //Ext.MessageBox.show({
                                //    title: messagesForm.errorAlertTitle,
                                //    msg: errorLoadEmployee,
                                //    //width: 300,
                                //    buttons: Ext.MessageBox.OK,
                                //    icon: Ext.MessageBox.ERROR
                                //});
                            }
                        }
                    });
                }
            }
        };

        me.searchEmployeeAction = searchEmployeeAction;

        var projectRoleStore = Ext.create('widget.projectRoleStore');
        //console.log(paramsView.urlReadProjectRole);
        projectRoleStore.load({
            url: paramsView.urlReadProjectRole
        });

        var memberStore = Ext.create('widget.projectMemberStore', {
            pageSize: 9999,
            listeners: {
                datachanged: function (st, eOpts) {
                    //me.projectData.set("Members", memberStore.data.length);
                    //me.projectData.commit();
                }
            }
        });
        memberStore.proxy.extraParams.projectCode = me.projectCode;
        me.memberStore = memberStore;

        var searchEmployeeFieldset = {
            xtype: 'fieldset',
            title: '<h5>เงื่อนไขในการค้นหา / Criterion</h5>',
            border: false,
            defaultType: 'field',
            defaults: {
                labelWidth: 200,
                anchor: '-20 -20',
                allowBlank: false,
                margin: "0 100 5 0",
                labelAlign: 'right'
            },
            items: [
                {
                    xtype: 'combo',
                    id: 'Division',
                    name: 'Division',
                    fieldLabel: 'ฝ่าย / Division',
                    store: me.divisionStore,
                    displayField: 'Name',
                    valueField: 'ID',
                    //forceSelection: true,
                    typeAhead: true,
                    allowBlank: true,
                    editable: false,
                    value: -1,
                    listeners: {
                        change: function (combo, newValue, oldValue, eOpts) {
                            if (newValue !== -1) {
                                me.departmentStore.proxy.extraParams.divisionID = newValue;
                                me.departmentStore.load({
                                    scope: this,
                                    callback: function (records, operation, success) {
                                        if (success) {
                                            Ext.getCmp('Department').setValue(-1);
                                            if (paramsView.userDivisionID) {
                                                var departmentID = parseInt(paramsView.userDepartmentID);
                                                Ext.getCmp('Department').setValue(departmentID);
                                            }
                                        } else {
                                            //Ext.MessageBox.show({
                                            //    title: messagesForm.errorAlertTitle,
                                            //    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Department',
                                            //    //width: 300,
                                            //    buttons: Ext.MessageBox.OK,
                                            //    icon: Ext.MessageBox.ERROR
                                            //});
                                        }
                                    }
                                });
                            } else {
                                me.departmentStore.removeAll();
                                me.departmentStore.add({ "ID": -1, "DivisionID": -1, "Name": "ทั้งหมด" });
                                Ext.getCmp('Department').setValue(-1);
                            }
                        }
                    }
                },
                {
                    xtype: 'combo',
                    id: 'Department',
                    name: 'Department',
                    fieldLabel: 'แผนก / Department',
                    store: me.departmentStore,
                    displayField: 'Name',
                    valueField: 'ID',
                    forceSelection: true,
                    queryMode: 'local',
                    allowBlank: true,
                    editable: false,
                    value: -1
                },
                {
                    xtype: 'fieldcontainer',
                    fieldLabel: 'พนักงาน / Employee',
                    name: 'project',
                    layout: 'hbox',
                    bodyPadding: 5,
                    defaults: {
                        labelWidth: 50,
                        allowBlank: true
                    },
                    items: [{
                        xtype: 'textfield',
                        id: 'EmployeeID',
                        name: 'EmployeeID',
                        fieldLabel: '',
                        emptyText: 'รหัส / ID',
                        flex: 1,
                        maxLength: 50,
                        maskRe: /^\d+$/,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    searchEmployeeAction();
                                }
                            }
                        }
                    }, {
                        xtype: 'textfield',
                        id: 'FirstName',
                        name: 'FirstName',
                        fieldLabel: '',
                        emptyText: 'ชื่อ / Name',
                        maxLength: 255,
                        flex: 2,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    searchEmployeeAction();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'textfield',
                        id: 'LastName',
                        name: 'LastName',
                        fieldLabel: '',
                        emptyText: 'นามสกุล / Last Name',
                        maxLength: 255,
                        flex: 3,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    searchEmployeeAction();
                                }
                            }
                        }
                    }]
                }
            ]
        };

        var cancleAction = Ext.create('Ext.Action', {
            //iconCls: 'add-button',
            text: messagesForm.closeActionText,
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
            height: 600,
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
                            title: 'ค้นหาพนักงาน / Search Employee',
                            region: 'north',
                            items: [searchEmployeeFieldset],
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '<i class="glyphicon glyphicon-search"></i> ค้นหา / Search',
                                    handler: searchEmployeeAction
                                }, {
                                    text: '<i class="glyphicon glyphicon-trash"></i> ล้างข้อมูล / Clear',
                                    handler: function (btn) {
                                        var form = Ext.getCmp('searchEmployeeForm').getForm();
                                        form.reset();
                                        me.setDefaultDivisionAndDepartment();
                                        searchEmployeeAction();
                                    }
                                }
                            ]
                        },
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
                                    id: 'gridEmployee',
                                    title: 'ผลการค้นหาพนักงาน / Employee Result',
                                    flex : 18,
                                    //margin: 3,
                                    selType: 'checkboxmodel',
                                    store: employeeStore,
                                    columns: [
                                        { xtype: 'rownumberer', width: 35, sortable: false, locked: true },
                                        { text: 'ID', dataIndex: 'ID', flex: 1, hidden: true, sortable: false },
                                        { text: 'รหัส<br/>Emp ID', dataIndex: 'EmployeeID', sortable: true, width: 100 },
                                        { text: 'ชื่อ-นามสกุล<br/>Name', dataIndex: 'FullName', sortable: true, width: 160 },
                                        { text: 'ตำแหน่ง<br/>Position', dataIndex: 'Position', sortable: true, width: 200 }
                                    ],
                                    // paging bar on the bottom
                                    bbar: Ext.create('Ext.PagingToolbar', {
                                        //store: timesheetStore,
                                        displayInfo: true,
                                        store: employeeStore,
                                        displayMsg: 'ข้อมูล {0} - {1} of {2}',
                                        emptyMsg: "ไม่มีพนักงาน"
                                    }),
                                    listeners: {
                                        selectionchange: function (obj, selected, eOpts) {
                                            var gridEmployee = Ext.getCmp('gridEmployee');
                                            var selected = gridEmployee.getSelectionModel().getSelection();
                                            var cmdAddProjectMember = Ext.getCmp('cmdAddProjectMember');
                                            if (selected.length > 0) {
                                                cmdAddProjectMember.setDisabled(false);
                                                return;
                                            }
                                            cmdAddProjectMember.setDisabled(true);
                                        }
                                    }
                                },
                                {
                                    xtype: 'container',
                                    title: '',
                                    layout: 'vbox',
                                    margin: "120 0 0 0",
                                    flex : 1,
                                    items: [
                                        {
                                            xtype: 'button',
                                            id: 'cmdAddProjectMember',
                                            margin: "3 0 0 0",
                                            text: '>',
                                            disabled: true,
                                            handler: function (btn) {
                                                var gridProjectMember = Ext.getCmp('gridProjectMember');
                                                var gridEmployee = Ext.getCmp('gridEmployee');
                                                var selected = gridEmployee.getSelectionModel().getSelection();
                                                if (selected.length > 0) {
                                                    var listOfSelected = [];
                                                    for (var i = 0; i < selected.length; i++) {
                                                        var emp = selected[i].data;
                                                        var id = emp.ID;
                                                        listOfSelected.push(id);

                                                        //add new Project Member
                                                        var newPM = Ext.create('widget.projectMember', {
                                                            EmployeeID: emp.EmployeeID,
                                                            FullName: emp.FullName,
                                                            Position: emp.Position,
                                                            CanEditProjectRole: true,
                                                            CanRemove: true
                                                        });
                                                        
                                                        newPM.commit();
                                                        newPM.set('ProjectRoleName', null);

                                                        memberStore.insert(0, newPM);
                                                        gridProjectMember.getView().refresh();
                                                    }
                                                    
                                                    searchEmployeeAction();
                                                }
                                            }
                                        },
                                        {
                                            xtype: 'button',
                                            id: 'cmdRemoveProjectMember',
                                            margin: "3 3 0 0",
                                            text: '<',
                                            disabled: true,
                                            handler: function (btn) {
                                                var gridProjectMember = Ext.getCmp('gridProjectMember');
                                                var selected = gridProjectMember.getSelectionModel().getSelection();
                                                if (selected.length > 0) {
                                                    var listOfSelected = [];
                                                    for (var i = 0; i < selected.length; i++) {
                                                        var id = selected[i].data.ID;
                                                        //console.log(id);
                                                        if (id !== "" && id !== null && id !== undefined) {
                                                            listOfSelected.push(id);
                                                        }
                                                        //console.log(id);
                                                    }

                                                    memberStore.remove(selected);
                                                    gridProjectMember.getView().refresh();

                                                    if (listOfSelected.length === 0) {
                                                        searchEmployeeAction();
                                                        return;
                                                    }

                                                    var modifiedRecords = memberStore.getModifiedRecords();
                                                    
                                                    Ext.MessageBox.wait("กำลังบันทึกข้อมูล...", 'กรุณารอ');
                                                    Ext.Ajax.request({
                                                        url: paramsView.urlRemoveProjectMember,    // where you wanna post
                                                        success: function (transport) {
                                                            Ext.MessageBox.hide();
                                                            var response = Ext.decode(transport.responseText);
                                                            if (response.success) {
                                                                Ext.MessageBox.show({
                                                                    title: messagesForm.successTitle,
                                                                    msg: response.message,
                                                                    //width: 300,
                                                                    buttons: Ext.MessageBox.OK,
                                                                    icon: Ext.MessageBox.INFO,
                                                                    fn: function (btn) {
                                                                        searchEmployeeAction();
                                                                        me.refreshMemberStore(function () {
                                                                            memberStore.add(modifiedRecords);
                                                                        });
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

                                                            Ext.MessageBox.show({
                                                                title: messagesForm.errorAlertTitle,
                                                                msg: 'เกิดข้อผิดพลาดในการ remove Project Member',
                                                                //width: 300,
                                                                buttons: Ext.MessageBox.OK,
                                                                icon: Ext.MessageBox.ERROR
                                                            });
                                                        },
                                                        jsonData: {
                                                            projectCode: me.projectCode,
                                                            projectMemberIDList: listOfSelected
                                                        }  // your json data
                                                    });
                                                }
                                            }
                                        }
                                    ]
                                },
                                {
                                    xtype: 'gridpanel',
                                    title: 'พนักงานที่เป็นสมาชิกในโครงการ / Project Members',
                                    id: 'gridProjectMember',
                                    flex: 20,
                                    selType: 'checkboxmodel',
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
                                        },
                                        beforeselect: function (plugin, record, index, eOpts) {
                                            if (!record.get('CanRemove')) {
                                                return false;
                                            }
                                        },
                                        selectionchange: function (obj, selected, eOpts) {
                                            var gridProjectMember = Ext.getCmp('gridProjectMember');
                                            var selected = gridProjectMember.getSelectionModel().getSelection();
                                            var cmdRemoveProjectMember = Ext.getCmp('cmdRemoveProjectMember');
                                            if (selected.length > 0) {
                                                cmdRemoveProjectMember.setDisabled(false);
                                                return;
                                            }
                                            cmdRemoveProjectMember.setDisabled(true);
                                        }
                                    },
                                    columns: [
                                        //{ hidden: false, text: '*', dataIndex: 'CanEditProjectRole', width: 50 },
                                        //{ hidden: false, text: '-', dataIndex: 'CanRemove', width: 50 },
                                        //{xtype: 'rownumberer', width: 35, sortable: false, locked: true },
                                        {
                                            xtype: 'rownumberer', width: 20, sortable: false, locked: true,
                                            renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                                                if (!record.data.CanRemove) {
                                                    metaData.css = 'cellreadonly';
                                                }
                                                else {
                                                    metaData.css = 'cellediting';
                                                }
                                                return value;
                                            }
                                        },
                                        { text: 'ID', dataIndex: 'ID', flex: 1, hidden: true, sortable: false },
                                        {
                                            text: 'หน้าที่ในโครงการ<br/>Project Role',
                                            dataIndex: 'ProjectRoleName',
                                            width: 200,
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
                                            }, sortable: true
                                        },
                                        { text: 'รหัส<br/>Emp ID', dataIndex: 'EmployeeID', sortable: true, width: 100 },
                                        { text: 'ชื่อ-นามสกุล<br/>Name', dataIndex: 'FullName', sortable: true, width: 160 },
                                        { text: 'ตำแหน่ง<br/>Position', dataIndex: 'Position', sortable: true, width: 200 }
                                    ],
                                    buttonAlign: 'center',
                                    buttons: [
                                        {
                                            text: '<i class="glyphicon glyphicon-floppy-disk"></i> บันทึก / Save',
                                            handler: function (btn) {
                                                if (memberStore.count() > paramsConst.limitMember) {
                                                    Ext.MessageBox.show({
                                                        title: messagesForm.errorAlertTitle,
                                                        msg: 'กรุณากำหนดสมาชิกในโครงการไม่เกิน ' + paramsConst.limitMember + ' ท่าน',
                                                        //width: 300,
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.ERROR
                                                    });
                                                    return false;
                                                }

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
                                                        url: paramsView.urlSaveMemberProjectRole,    // where you wanna post
                                                        success: function (transport) {
                                                            Ext.MessageBox.hide();
                                                            var response = Ext.decode(transport.responseText);
                                                            if (response.success) {
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
                                                            Ext.MessageBox.show({
                                                                title: messagesForm.errorAlertTitle,
                                                                msg: 'เกิดข้อผิดพลาดในการเพิ่มข้อมูล Project Member',
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
                                    ]
                                    //,bbar: Ext.create('Ext.PagingToolbar', {
                                    //    //store: timesheetStore,
                                    //    displayInfo: true,
                                    //    displayMsg: 'สมาชิกในโครงการ ที่กำลังแสดงอยู่ {0} - {1} of {2}',
                                    //    emptyMsg: "ไม่มี สมาชิกในโครงการ"
                                    //})
                                }
                            ]
                        }
                    ]
                }
            ],
            buttonAlign: 'center',
            buttons: [new Ext.button.Button(cancleAction)]
        }];

        this.callParent();
    },

    setWithoutEmpIDList: function () {
        var self = this;
        var listOfProjectMemberList = [];
        self.memberStore.each(function (rec) {
            if (rec.data.ID === null || rec.data.ID === '') {
                listOfProjectMemberList.push(rec.data.EmployeeID);
            }
        });
        self.employeeStore.proxy.extraParams.withoutEmpIDList = listOfProjectMemberList;
    },

    setDefaultDivisionAndDepartment: function () {
        var self = this;
        if (paramsView.userDivisionID) {
            var cmbDivision = Ext.getCmp('Division');
            var cmbDepartment = Ext.getCmp('Department');

            cmbDivision.setValue(parseInt(paramsView.userDivisionID));
            cmbDivision.setReadOnly(true);
            cmbDepartment.setReadOnly(true);
        }
    },

    refreshMemberStore: function (callback) {
        var self = this;

        self.memberStore.load({
            callback: function (records, operation, success) {
                if (success) {
                    if (callback) callback();

                    self.setWithoutEmpIDList();
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
    },

    listeners: {
        show: function (w, opt) {
            this.searchEmployeeAction();
            this.memberStore.load({
                url: paramsView.urlReadProjectMember,
                callback: function (records, operation, success) {
                    if (success) {
                        //searchEmployeeAction();
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
        }
    }
});