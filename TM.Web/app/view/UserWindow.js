Ext.define('TM.view.UserWindow', {
    extend: 'Ext.window.Window',
    xtype: 'userWindow',
    width: 1000,
    title: 'เพิ่มข้อมูลพนักงาน / Add Employee',
    resizable: false,
    closable: false,
    constrain: true,
    config: {
        editData: null,
        employeeStore: null,
        titleNameStore: null,
        positionStore: null,
        divisionStore: null,
        departmentStore: null
    },

    initComponent: function () {
        var self = this;

        self.departmentStore.clearFilter();
        self.departmentStore.filter('DivisionID', -1);

        function closWindow() {
            self.departmentStore.clearFilter();
            self.close();
        }

        var addAction = Ext.create('Ext.Action', {
            //iconCls: 'add-button',
            text: TextLabel.saveActionText,
            disabled: false,
            handler: function (widget, event) {

                var form = self.down('form');
                if (!form.isValid()) {
                    Ext.MessageBox.show({
                        title: TextLabel.validationTitle,
                        msg: TextLabel.validationWarning,
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.WARNING
                    });

                    return false;
                }
                //console.log(self.getDtEndDate().getRawValue());
                //console.log(self.getDtStartDate().getRawValue());

                var endDateText = self.getDtEndDate().getRawValue(),
                    startDateText = self.getDtStartDate().getRawValue();

                if (form.isValid()) {
                    var vals = form.getValues();
                    //console.log(vals);
                    Ext.MessageBox.wait("กำลังบันทึกข้อมูล...", 'กรุณารอ');

                    var saveData = Ext.create('widget.employee', {
                        ID: vals.ID,
                        TitleID: vals.TitleID,
                        EmployeeID: vals.EmployeeID,
                        NameTH: vals.NameTH,
                        LastTH: vals.LastTH,
                        NameEN: vals.NameEN,
                        LastEN: vals.LastEN,
                        Nickname: vals.Nickname,
                        Email: vals.Email,
                        PositionID: vals.PositionID,
                        DivisionID: vals.DivisionID,
                        DepartmentID: vals.DepartmentID,
                        StartDateText: startDateText,
                        EndDateText: endDateText,
                        AppRole: vals['app-role'],
                        Status: vals.Status
                    });

                    //console.log(saveData);
                    Ext.Ajax.request({
                        url: paramsView.urlSaveEmployee,
                        success: function (transport) {
                            Ext.MessageBox.hide();
                            var respose = Ext.decode(transport.responseText);
                            if (respose.success) {
                                Ext.MessageBox.show({
                                    title: TextLabel.successTitle,
                                    msg: 'บันทึกข้อมูลเสร็จสมบูรณ์',
                                    //width: 300,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO,
                                    fn: function (btn) {
                                        closWindow();
                                    }
                                });
                            } else {
                                Ext.MessageBox.show({
                                    title: TextLabel.errorAlertTitle,
                                    msg: respose.message,
                                    //width: 300,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                            }
                        },
                        failure: function (transport) {
                            Ext.MessageBox.show({
                                title: TextLabel.errorAlertTitle,
                                msg: "เกิดข้อผิดพลาดในการบันทึกพนักงาน " + transport.responseText,
                                //width: 300,
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                        },
                        jsonData: saveData.data
                    });
                }
            }
        });

        var cancleAction = Ext.create('Ext.Action', {
            //iconCls: 'add-button',
            text: TextLabel.cancleActionText,
            disabled: false,
            handler: function (widget, event) {
                closWindow();
            }
        });

        self.items = [{
            xtype: 'panel',
            layout: {
                type: 'hbox',
                pack: 'start',
                align: 'stretch'
            },
            items: [{
                xtype: 'form',
                layout: {
                    type: 'table',
                    columns: 2,
                    tableAttrs: {
                        style: {
                            width: '100%'
                        }
                    }
                },
                bodyStyle: 'padding: 8px 5px 5px 10px',
                defaultType: 'textfield',
                fieldDefaults: {
                    labelWidth: 270,
                    allowBlank: false,
                    labelAlign: 'right'
                },
                items: [{
                    //id: 'ID',
                    name: 'ID',
                    xtype: 'textarea',
                    allowBlank: true,
                    hidden: true
                },{
                    xtype: 'combo',
                    fieldLabel: 'คำนำหน้าชื่อ / Title <span class="required">*</span>',
                    id: 'Title',
                    name: 'TitleID',
                    //colspan: 1,
                    queryMode: 'local',
                    displayField: 'Display',
                    valueField: 'ID',
                    emptyText: TextLabel.requireSelectEmptyText,
                    store: self.titleNameStore,
                    editable: false
                }, {
                    fieldLabel: 'รหัสพนักงาน / Employee ID <span class="required">*</span>',
                    id: 'EmployeeID',
                    name: 'EmployeeID',
                    validFlag: true,
                    maxLength: 10,
                    minLength: 3,
                    validator: function () {
                        return this.validFlag;
                    },
                    listeners: {
                        'change': function (textfield, newValue, oldValue) {
                            var me = this;
                            newValue = parseInt(newValue);
                            //me.setRawValue(newValue);
                            var checkDuplicate = true;

                            if (self.editData && self.editData.data.EmployeeID == newValue) {
                                checkDuplicate = false;
                                me.validFlag = true;
                                me.validate();
                            }

                            if (checkDuplicate) {
                                if (newValue > 999) {
                                    me.setLoading(true);
                                    me.setReadOnly(true);
                                    addAction.setDisabled(true);

                                    Ext.Ajax.request({
                                        url: paramsView.urlCheckDuplicatedEmployeeID + '?employeeID=' + newValue,
                                        success: function (response) {
                                            var result = Ext.decode(response.responseText);
                                            me.validFlag = result.valid ? true : 'รหัสพนักงาน / Employee ID นี้ มีอยู่ในระบบแล้ว!';
                                            me.validate();
                                            me.setLoading(false);
                                            me.setReadOnly(false);
                                            addAction.setDisabled(false);
                                        },
                                        failure: function (transport) {
                                            me.setLoading(false);
                                            me.setReadOnly(false);
                                            addAction.setDisabled(false);
                                            Ext.MessageBox.show({
                                                title: TextLabel.errorAlertTitle,
                                                msg: "เกิดข้อผิดพลาดในขั้นตอนการตรวจสอบ รหัสพนักงาน / Employee ID ซ้ำ " + transport.responseText,
                                                //width: 300,
                                                buttons: Ext.MessageBox.OK,
                                                icon: Ext.MessageBox.ERROR
                                            });
                                        }
                                    });
                                }
                            }
                        }
                    }
                }, {
                    fieldLabel: 'ชื่อ (ไทย) / First Name (TH) <span class="required">*</span>',
                    id: 'NameTH',
                    name: 'NameTH',
                    //colspan: 1,
                    maxLength: 100
                }, {
                    fieldLabel: 'นามสกุล (ไทย) / Last Name (TH) <span class="required">*</span>',
                    id: 'LastTH',
                    name: 'LastTH',
                    //colspan: 1,
                    maxLength: 100
                }, {
                    fieldLabel: 'ชื่อ (อังกฤษ) / First Name (EN) <span class="required">*</span>',
                    id: 'NameEN',
                    name: 'NameEN',
                    //colspan: 1,
                    maxLength: 100
                }, {
                    fieldLabel: 'นามสกุล (อังกฤษ) / Last Name (EN) <span class="required">*</span>',
                    id: 'LastEN',
                    name: 'LastEN',
                    //colspan: 1,
                    maxLength: 100
                }, {
                    fieldLabel: 'ชื่อเล่น / Nickname',
                    id: 'Nickname',
                    name: 'Nickname',
                    //colspan: 1,
                    maxLength: 30,
                    allowBlank: true
                }, {
                    fieldLabel: 'อีเมล์ / Email',
                    id: 'Email',
                    name: 'Email',
                    vtype: 'email',
                    //colspan: 1,
                    maxLength: 100,
                    allowBlank: true
                }, {
                    xtype: 'combo',
                    fieldLabel: 'ตำแหน่ง / Position <span class="required">*</span>',
                    id: 'Position',
                    name: 'PositionID',
                    width: '100%',
                    colspan: 2,
                    queryMode: 'local',
                    displayField: 'Display',
                    valueField: 'ID',
                    emptyText: TextLabel.requireInputEmptyText,
                    store: self.positionStore,
                    forceSelection: true,
                    editable: true,
                    listConfig: { itemTpl: highlightMatch.createItemTpl('Display', 'Position') }
                }, {
                    xtype: 'combo',
                    fieldLabel: 'ฝ่าย / Division <span class="required">*</span>',
                    id: 'Division',
                    name: 'DivisionID',
                    width: '100%',
                    colspan: 2,
                    queryMode: 'local',
                    displayField: 'Name',
                    valueField: 'ID',
                    emptyText: TextLabel.requireInputEmptyText,
                    store: self.divisionStore,
                    forceSelection: true,
                    editable: true,
                    listeners: {
                        change: function (combo, newValue, oldValue, eOpts) {
                            self.departmentStore.clearFilter();
                            self.departmentStore.filter({
                                property: 'DivisionID',
                                exactMatch: true,
                                value   : newValue
                            });

                            Ext.getCmp('Department').reset();
                        }
                    },
                    listConfig: { itemTpl: highlightMatch.createItemTpl('Name', 'Division') }
                }, {
                    xtype: 'combo',
                    fieldLabel: 'แผนก / Department <span class="required">*</span>',
                    id: 'Department',
                    name: 'DepartmentID',
                    width: '100%',
                    colspan: 2,
                    queryMode: 'local',
                    displayField: 'Name',
                    valueField: 'ID',
                    emptyText: TextLabel.requireInputEmptyText,
                    store: self.departmentStore,
                    forceSelection: true,
                    editable: true,
                    listConfig: { itemTpl: highlightMatch.createItemTpl('Name', 'Department') }
                }, {
                    xtype: 'fieldcontainer',
                    fieldLabel: 'วันที่เริ่ม - สิ้นสุดการทำงาน / Start - End Date',
                    id: 'startDateBetween',
                    name: 'startDateBetween',
                    layout: 'hbox',
                    colspan: 2,
                    items: [{
                        xtype: 'datefield',
                        id: 'EmpStartDate',
                        name: 'StartDate',
                        width: 100,
                        //fieldLabel: 'วันที่เริ่มต้นโครงการ / Project Start Date',
                        format: "d/m/Y",
                        allowBlank: true,
                        editable: false,
                        vtype: 'daterange',
                        endDateField: 'EmpEndDate'
                    }, {
                        xtype: 'displayfield',
                        margin: '0 5 0 5',
                        value: '-'
                    }, {
                        xtype: 'datefield',
                        id: 'EmpEndDate',
                        name: 'EndDate',
                        width: 100,
                        //fieldLabel: 'วันที่สิ้นสุดโครงการ / Project End Date',
                        format: "d/m/Y",
                        allowBlank: true,
                        editable: false,
                        vtype: 'daterange',
                        startDateField: 'EmpStartDate'
                    }, {
                        itemId: 'cmbEmployeeStatus',
                        name: 'Status',
                        xtype: 'combo',
                        fieldLabel: 'สถานะพนักงาน',
                        labelWidth: 100,
                        displayField: 'Name',
                        valueField: 'ID',
                        value: 'Work',
                        editable: false,
                        store: 'EmployeeStatusArrayStore'
                    }]
                }, {
                    xtype: 'fieldset',
                    title: '',
                    colspan: 2,
                    layout: 'anchor',
                    collapsible: false,
                    margin: '0 0 0 10',
                    padding: '0 0 0 0',
                    border: false,
                    hidden: true,
                    items: [{
                        xtype: 'radiogroup',
                        id: 'AppRole',
                        name: 'AppRole',
                        fieldLabel: 'ระดับผู้ใช้ / User Level',
                        columns: 5,
                        labelAlign: 'right',
                        labelWidth: 220,
                        //height : 100,
                        items: [{
                            checked: true,
                            boxLabel: 'Member',
                            name: 'app-role',
                            inputValue: 'Member',
                            width: 100
                        }, {
                            boxLabel: 'Admin',
                            name: 'app-role',
                            inputValue: 'Admin',
                            width: 100
                        }, {
                            boxLabel: 'Support',
                            name: 'app-role',
                            inputValue: 'Support',
                            width: 100
                        }, {
                            boxLabel: 'Manager',
                            name: 'app-role',
                            inputValue: 'Manager',
                            width: 100
                        }, {
                            boxLabel: 'Executive',
                            name: 'app-role',
                            inputValue: 'Executive',
                            width: 100
                        }]
                    }]
                }]
            }],
            buttonAlign: 'center',
            buttons: [
                new Ext.button.Button(addAction),
                new Ext.button.Button(cancleAction)
            ]
        }];

        this.callParent();
    },

    setValues: function (record) {
        var self = this;
        var form = self.down('form').getForm();
        form.trackResetOnLoad = true;
        form.setValues(record.data);
        
        //Ext.getCmp('Title').setValue(record.data.TitleID);
        //Ext.getCmp('Position').setValue(record.data.PositionID);
        //Ext.getCmp('Division').setValue(record.data.DivisionID);
        //Ext.getCmp('Department').setValue(record.data.DepartmentID);

        self.departmentStore.clearFilter();
        //console.log(record.data.DivisionID);
        self.departmentStore.filter({
            property: 'DivisionID',
            exactMatch: true,
            value: record.data.DivisionID
        });

        var grAppRole = Ext.getCmp('AppRole');
        var appRoles = record.data.AppRole.split('/');
        //console.log(appRoles);

        for (var i = 0; i < appRoles.length; i++) {
            if (appRoles[i] !== 'Member') {
                grAppRole.setValue({
                    'app-role': appRoles[i]
                });
                break;
            }
        }

    },

    getDtStartDate: function () {
        return Ext.getCmp('EmpStartDate');
    },

    getDtEndDate: function () {
        return Ext.getCmp('EmpEndDate');
    },

    listeners: {
        close: function (panel, eOpts) {
            var self = this;

            var form = self.down('form');

            if (form.isDirty()) {
                if (self.employeeStore) self.employeeStore.load();
                if (self.departmentStore) self.departmentStore.load();
                if (self.positionStore) self.positionStore.load();
            }
        }
    }
});