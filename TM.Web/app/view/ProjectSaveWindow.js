Ext.define('TM.view.ProjectSaveWindow', {
    extend: 'Ext.window.Window',
    xtype: 'projectSaveWindow',
    width: 850,
    title: '<i class="glyphicon glyphicon-plus"></i> เพิ่มโครงการ / Add Project',
    resizable: false,
    closable: false,
    constrain: true,
    config: {
        editData: null,
        projectStore: null,
        customerstore: null,
        projectdeliveryphasestore: null
    },
    initComponent: function () {
        var self = this,
            _autoCheckDuplicate = false;
        
        var gridIniMember = { xtype: 'hiddenfield' };
       
        self.isRegistered = false;
        if (self.editData) {
            self.title = "แก้ไขข้อมูลโครงการ / Edit Project";
            //console.log(self.editData);
            //self.projectdeliveryphasestore.removeAll();
            //self.projectdeliveryphasestore.proxy.extraParams.projectID = self.editData.data.ID;
            self.projectdeliveryphasestore.loadData(self.editData.raw.ProjectDeliveryPhases);
            //self.projectdeliveryphasestore.load({
            //    pageSize: 9999,
            //    url: paramsView.urlGetProjectDeliveryPhases
            //});

            var addProjectDeliveryPhaseDate = Ext.create("widget.button", {
                cls: 'btn',
                iconCls: 'glyphicon glyphicon-plus',
                text: "เพิ่มช่วงส่งมอบ / Add Delivery Phase",
                disabled: true,
                handler: function (btn, evt) {
                    var newDiv = Ext.create("widget.projectdeliveryphasedatewindow",
                            {
                                title: TextLabel.addProjectDeliveryPhaseDateTitle,
                                animateTarget: btn,
                                modal: true,
                                projectID: self.editData.data.ID,
                                projectdeliveryphasestore: self.projectdeliveryphasestore,
                                mindate: self.getContractStartDate().getValue(),
                                maxdate: self.getContractEndDate().getValue(),
                                projectsavewindow: self
                            });
                    newDiv.show();
                }
            });
            self.addProjectDeliveryPhaseDate = addProjectDeliveryPhaseDate;

        } else {

            self.isRegistered = true;

            var initialMemberStore = Ext.create('widget.initialMemberStore', {
                url: paramsView.urlGetAllInitialMember
            });
            self.initialMemberStore = initialMemberStore;

            gridIniMember = Ext.create('Ext.grid.Panel', {
                height: 330,
                iconCls: 'project-memebr-icon',
                title: 'พนักงานที่ต้องอยู่ทุกโครงการ',
                id: 'gridInitialMember',
                store: initialMemberStore,
                selType: 'checkboxmodel',
                columns: [
                    { xtype: 'rownumberer', width: 35, sortable: false, locked: true },
                    { text: 'ID', dataIndex: 'ID', hidden: true, sortable: false },
                    { text: 'รหัส<br/>Emp ID', dataIndex: 'EmployeeID', sortable: true, width: 100 },
                    { text: 'ชื่อ-นามสกุล<br/>Name', dataIndex: 'FullName', sortable: true, width: 160 },
                    { text: 'หน้าที่ในโครงการ<br/>Project Role', dataIndex: 'ProjectRoleName', sortable: true, flex: 1 }
                ],
                bbar: Ext.create('Ext.PagingToolbar', {
                    store: initialMemberStore,
                    displayInfo: true,
                    displayMsg: 'ข้อมูล {0} - {1} of {2}',
                    emptyMsg: "ไม่มีสมาชิกในโครงการ"
                })
                //,tbar: [{
                //    cls: 'btn',
                //    xtype: 'button',
                //    iconCls: 'add-project-memebr-icon',
                //    text: "เพิ่มสมาชิกที่ต้องอยู่ทุกโครงการ / Add Project Default Member",
                //    handler: function (btn, evt) {
                        
                //    }
                //}]
            });

            initialMemberStore.load({
                url: paramsView.urlGetAllInitialMember,
                callback: function (r, ops, success) {
                    gridIniMember.getSelectionModel().selectAll();
                }
            });
        }

        var projectStatusStore = Ext.create('widget.projectStatusStore');
        projectStatusStore.proxy.extraParams.includeAll = null;
        projectStatusStore.load({
            url: paramsView.urlGetProjectStatus
        });

        var addAction = Ext.create('Ext.Action', {
            //iconCls: 'add-button',
            text: messagesForm.saveActionText,
            disabled: false,
            handler: function (widget, event) {
                var form = self.down('form');
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

                    Ext.MessageBox.wait("กำลังบันทึกข้อมูล...", 'กรุณารอ');
                    var saveProject = Ext.create('widget.project', {
                        ID: vals.ID,
                        Code: vals.Code,
                        NameTH: vals.NameTH,
                        NameEN: vals.NameEN,
                        StartDate: Ext.Date.parse(vals.StartDate, 'd/m/Y'),
                        EndDate: Ext.Date.parse(vals.EndDate, 'd/m/Y'),
                        CustomerID: vals.CustomerID,

                        ContractStartDate: Ext.Date.parse(vals.ContractStartDate, 'd/m/Y'),
                        ContractEndDate: Ext.Date.parse(vals.ContractEndDate, 'd/m/Y'),
                        DeliverDate: Ext.Date.parse(vals.DeliverDate, 'd/m/Y'),
                        WarrantyStartDate: Ext.Date.parse(vals.WarrantyStartDate, 'd/m/Y'),
                        WarrantyEndDate: Ext.Date.parse(vals.WarrantyEndDate, 'd/m/Y'),
                        EstimateProjectValue: vals.EstimateProjectValue,
                        ProjectValue: vals.ProjectValue,
                        StatusID: vals.StatusID,
                        Progress: vals.Progress,
                        StateOfProgress: vals.StateOfProgress
                    });

                    if (self.editData) {
                        saveProject.data.ProjectDeliveryPhases = [];
                        //console.log(self.projectdeliveryphasestore.getModifiedRecords());
                        for (var i = 0; i < self.projectdeliveryphasestore.getCount() ; i++) {
                            var model = self.projectdeliveryphasestore.getAt(i);
                            
                            var saveData = Ext.create('widget.projectdeliveryphase', {
                                ID: model.get("ID"),
                                ProjectID: model.get("ProjectID"),
                                DeliveryPhaseDate: model.get("DeliveryPhaseDate"),
                                StatusOfProjectDeliveryPhase: model.get("StatusOfProjectDeliveryPhase")
                            });
                            //console.log(saveData);
                            saveProject.data.ProjectDeliveryPhases.push(saveData.data);
                        }
                        //saveProject.data.ProjectDeliveryPhases = self.projectdeliveryphasestore.getModifiedRecords();
                    }

                    saveProject.data.InitMembers = [];
                    if (gridIniMember.getSelectionModel) {
                        var selected = gridIniMember.getSelectionModel().getSelection();
                        if (selected.length > 0) {
                            for (var i = 0; i < selected.length; i++) {
                                var initMember = selected[i].data;
                                saveProject.data.InitMembers.push(initMember.ID);
                            }
                        }
                    }

                    Ext.Ajax.request({
                        url: paramsView.urlSaveProject,
                        success: function (transport) {
                            Ext.MessageBox.hide();
                            var respose = Ext.decode(transport.responseText);
                            if (respose.success) {
                                Ext.MessageBox.show({
                                    title: messagesForm.successTitle,
                                    msg: 'บันทึกข้อมูลเสร็จสมบูรณ์',
                                    //width: 300,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO,
                                    fn: function (btn) {
                                        if (self.projectStore) self.projectStore.load();
                                        if (self.editData) {// แก้ไขสำเร็จ
                                            self.close();
                                        } else {
                                            //form.getForm().reset();
                                            self.close();
                                        }
                                    }
                                });

                            } else {
                                Ext.MessageBox.show({
                                    title: messagesForm.errorAlertTitle,
                                    msg: 'เกิดข้อผิดพลาดในการบันทึกข้อมูล ' + respose.message,
                                    //width: 300,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                            }
                        },
                        failure: function (transport) {
                            Ext.MessageBox.show({
                                title: messagesForm.errorAlertTitle,
                                msg: "เกิดข้อผิดพลาดในการบันทึกโครงการ " + transport.responseText,
                                //width: 300,
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                        },
                        jsonData: saveProject.data
                    });
                }
            }
        });

        var cancleAction = Ext.create('Ext.Action', {
            //iconCls: 'add-button',
            text: messagesForm.cancleActionText,
            disabled: false,
            handler: function (widget, event) {
                //self.close();
                var form = self.down('form').getForm();

                if (self.editData) {
                    var modifiedRecords = self.projectdeliveryphasestore.getModifiedRecords();
                    var newRecords = self.projectdeliveryphasestore.getNewRecords();
                    var removedRecords = self.projectdeliveryphasestore.getRemovedRecords();

                    if (modifiedRecords.length > 0
                        || newRecords.length > 0
                        || removedRecords.length > 0
                        || form.isDirty()) {
                        Ext.MessageBox.alert({
                            title: 'แจ้งเตือน / Warning',
                            msg: 'มีการแก้ไขข้อมูลโครงการ ท่านต้องการยกเลิกการดำเนินการนี้ ใช่ หรือ ไม่?',
                            //width: 300,
                            buttons: Ext.MessageBox.YESNO,
                            icon: Ext.MessageBox.QUESTION,
                            fn: function (btn) {
                                if (btn === 'yes') {
                                    self.close();

                                    self.projectStore.load();
                                }
                            }
                        });
                    } else {
                        self.close();
                    }
                } else {
                    self.close();
                }
            }
        });

        var stateOfProgress = Ext.create('Ext.data.Store', {
            fields: ['value', 'name'],
            data: [
                { "value": "InProgress", "name": "InProgress" },
                { "value": "Done", "name": "Done" }
            ]
        });

        function days_between(date1, date2) {

            // The number of milliseconds in one day
            var ONE_DAY = 1000 * 60 * 60 * 24

            // Convert both dates to milliseconds
            var date1_ms = date1.getTime()
            var date2_ms = date2.getTime()

            // Calculate the difference in milliseconds
            var difference_ms = Math.abs(date1_ms - date2_ms)

            // Convert back to days and return
            return Math.round(difference_ms / ONE_DAY)

        }

        this.items = [{
            xtype: 'form',
            layout: {
                type: 'vbox',
                align: 'stretch'
            },
            frame: false,
            width: '100%',
            border: 0,
            bodyStyle: 'padding: 6px',
            defaultType: 'textfield',
            fieldDefaults: {
                labelWidth: 400,
                allowBlank: false,
                labelAlign: 'right'
            },
            items: [{
                id: 'ID',
                name: 'ID',
                xtype: 'textarea',
                allowBlank: true,
                hidden: true
            },
            {
                fieldLabel: 'รหัสโครงการ / Project Code <span class="required">*</span><br/>(ตัวอย่าง PJ-XXX001)',
                id: 'Code',
                name: 'Code',
                validFlag: true,
                padding: '0 230 0 0',
                minLength: 4,
                maxLength: 50,
                validator: function () {
                    return this.validFlag;
                },
                listeners: {
                    'change': function (textfield, newValue, oldValue) {
                        var me = this;
                        newValue = newValue.toUpperCase();
                        me.setRawValue(newValue);
                        var checkDuplicate = true;
                        
                        if (_autoCheckDuplicate) {
                            if (self.editData && self.editData.data.Code === newValue) {
                                checkDuplicate = false;
                                me.validFlag = true;
                                me.validate();
                            }

                            if (checkDuplicate) {
                                if (newValue.length > 4) {
                                    me.setLoading(true);
                                    me.setReadOnly(true);
                                    addAction.setDisabled(true);

                                    Ext.Ajax.request({
                                        url: paramsView.urlCheckDuplicatedProjectCode + '?projectCode=' + newValue,
                                        success: function (response) {
                                            var result = Ext.decode(response.responseText);
                                            me.validFlag = result.valid ? true : 'Project Code นี้ มีอยู่ในระบบแล้ว!';
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
                                                title: messagesForm.errorAlertTitle,
                                                msg: "เกิดข้อผิดพลาดในขั้นตรวจสอบ Project Code ซ้ำ " + transport.responseText,
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
                }
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: 'ความคืบหน้าโครงการ / Project Progress',
                layout: 'hbox',
                hidden: self.isRegistered,
                items: [{
                    xtype: 'numberfield',
                    id: 'Progress',
                    name: 'Progress',
                    minValue: 0,
                    maxValue: 100,
                    decimalPrecision: 0,
                    width: 70,
                    fieldCls: 'a-form-num-field',
                    step: 1,
                    allowBlank: true
                },
                {
                    xtype: 'displayfield',
                    margin: '0 5 0 5',
                    value: '%'
                }]
            },
            {
                xtype: 'numberfield',
                fieldLabel: 'มูลค่าโครงการโดยประมาณ / Estimated Value of Project',
                id: 'EstimateProjectValue',
                name: 'EstimateProjectValue',
                allowBlank: true,
                maxLength: 15,
                forcePrecision: true,
                decimalPrecision: 2,
                useThousandSeparator: true,
                fieldCls: 'a-form-num-field',
                padding: '0 137 0 0',
                step: 10000,
                minValue: 0,
                hidden: self.isRegistered,
                listeners: {
                    blur: function (field) {
                        field.setRawValue(Ext.util.Format.number(field.getValue(), '0,000.00'));
                    }
                }
            }, {
                xtype: 'numberfield',
                fieldLabel: 'มูลค่าโครงการ / Value of Project',
                id: 'ProjectValue',
                name: 'ProjectValue',
                allowBlank: true,
                maxLength: 15,
                forcePrecision: true,
                decimalPrecision: 2,
                useThousandSeparator: true,
                fieldCls: 'a-form-num-field',
                padding: '0 137 0 0',
                step: 10000,
                minValue: 0,
                hidden: self.isRegistered,
                listeners: {
                    blur: function (field) {
                        field.setRawValue(Ext.util.Format.number(field.getValue(), '0,000.00'));
                    }
                }
            }, {
                fieldLabel: 'ชื่อโครงการภาษาไทย / Project Thai Name <span class="required">*</span>',
                id: 'NameTH',
                name: 'NameTH',
                maxLength: 255
            }, {
                fieldLabel: 'ชื่อโครงการภาษาอังกฤษ / Project Eng Name <span class="required">*</span>',
                id: 'NameEN',
                name: 'NameEN',
                maxLength: 255
            },
            {
                xtype: 'combo',
                fieldLabel: 'ลูกค้า / Customer <span class="required">*</span>',
                id: 'Customer',
                name: 'CustomerID',
                allowBlank: false,
                queryMode: 'local',
                displayField: 'Name',
                valueField: 'ID',
                store: self.customerstore,
                forceSelection: true,
                editable: true,
                emptyText: TextLabel.requireInputEmptyText,
                listConfig: { itemTpl: highlightMatch.createItemTpl('Name', 'Customer') }
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: 'วันที่เริ่มต้น - สิ้นสุด ใน Share Point / Start - End Date in Share Point <span class="required">*</span>',
                id: 'startDateBetween',
                name: 'startDateBetween',
                layout: 'hbox',
                items: [ {
                    xtype: 'datefield',
                    id: 'StartDate',
                    name: 'StartDate',
                    width: 100,
                    //fieldLabel: 'วันที่เริ่มต้นโครงการ / Project Start Date',
                    format: "d/m/Y",
                    allowBlank: false,
                    editable: false,
                    vtype: 'daterange',
                    endDateField: 'EndDate',
                    listeners: {
                        change: function (dt, newValue, oldValue, eOpts) {

                            self.getContractStartDate().setMinValue(newValue);
                            self.getContractStartDate().setMaxValue(null);
                            self.getContractStartDate().setValue(null);

                            self.getContractEndDate().setValue(null);
                            self.getContractEndDate().setMaxValue(null);

                            self.getDeliverDate().setValue(null);

                            self.getWarrantyStartDate().setValue(null);
                            self.getWarrantyEndDate().setValue(null);

                        }
                    }
                }, {
                    xtype: 'displayfield',
                    margin: '0 5 0 5',
                    value: '-'
                }, {
                    xtype: 'datefield',
                    id: 'EndDate',
                    name: 'EndDate',
                    width: 100,
                    //fieldLabel: 'วันที่สิ้นสุดโครงการ / Project End Date',
                    format: "d/m/Y",
                    allowBlank: true,
                    editable: false,
                    vtype: 'daterange',
                    startDateField: 'StartDate'
                }]
            }, {
                xtype: 'fieldcontainer',
                fieldLabel: 'วันที่เริ่มต้น - สิ้นสุด ตามสัญญา / Start - End Date in Contract ',
                id: 'contractDateBetween',
                name: 'contractDateBetween',
                layout: 'hbox',
                hidden: self.isRegistered,
                items: [{
                    xtype: 'datefield',
                    id: 'ContractStartDate',
                    name: 'ContractStartDate',
                    itemId: 'ContractStartDate',
                    width: 100,
                    format: "d/m/Y",
                    allowBlank: true,
                    editable: false,
                    vtype: 'daterange',
                    endDateField: 'ContractEndDate',
                    listeners: {
                        change: function (dt, newValue, oldValue, eOpts) {
                            Ext.getCmp("ContractEndDate").setDisabled(true);
                            if (newValue) {
                                self.getContractEndDate().setDisabled(false);
                            }
                        }
                    }
                }, {
                    xtype: 'displayfield',
                    margin: '0 5 0 5',
                    value: '-'
                }, {
                    xtype: 'datefield',
                    id: 'ContractEndDate',
                    name: 'ContractEndDate',
                    itemId: 'ContractEndDate',
                    width: 100,
                    format: "d/m/Y",
                    allowBlank: true,
                    editable: false,
                    vtype: 'daterange',
                    startDateField: 'ContractStartDate',
                    disabled: true,
                    listeners: {
                        change: function (dt, newValue, oldValue, eOpts) {
                            self.getDeliverDate().setDisabled(true);
                            self.addProjectDeliveryPhaseDate.setDisabled(true);

                            if (newValue) {
                                self.getContractStartDate().setMaxValue(newValue);

                                self.getDeliverDate().setMinValue(newValue);
                                self.getDeliverDate().setMaxValue(null);
                                self.getDeliverDate().setDisabled(false);

                                self.addProjectDeliveryPhaseDate.setDisabled(false);
                            }
                        }
                    }
                },
                {
                    xtype: 'displayfield',
                    itemId: 'ContractDateDay',
                    margin: '0 5 0 5',
                    value: ''
                }]
            },
            {
                xtype: 'combo',
                fieldLabel: 'สถานะของช่วงการพัฒนา / State of Implement Phase',
                name: 'StateOfProgress',
                store: stateOfProgress,
                queryMode: 'local',
                displayField: 'name',
                valueField: 'value',
                padding: '0 320 0 0',
                editable: false,
                value: "InProgress",
                allowBlank: true,
                //hidden: self.isRegistered,
                hidden: true,
                listeners: {
                    change: function (cmb, newValue, oldValue, opts) {
                        //self.projectStatusNameDisplay.setValue(cmb.getRawValue());
                    }
                }
            },
            {
                xtype: 'grid',
                title: 'ช่วงการส่งมอบ / Delivery Phase ',
                width: 300,
                height: 230,
                //hidden: true,
                hidden: self.isRegistered,
                store: self.projectdeliveryphasestore,
                columns: {
                    items: [
                            {
                                xtype: 'rownumberer',
                                width: 30
                            },
                            {
                                xtype: 'actioncolumn',
                                width: 30,
                                items: [{
                                    xtype: 'button',
                                    iconCls: 'edit-icon',
                                    handler: function (grid, rowIndex, colIndex, item, event, record, row) {
                                        grid.getSelectionModel().select(record);

                                        var editForm = Ext.create('widget.projectdeliveryphasedatewindow', {
                                            iconCls: 'edit-icon',
                                            animateTarget: row,

                                            editData: record,
                                            modal: true,
                                            projectID: self.editData.data.ID,
                                            projectdeliveryphasestore: self.projectdeliveryphasestore,
                                            mindate: self.getContractStartDate().getValue(),
                                            maxdate: self.getContractEndDate().getValue(),
                                            projectsavewindow: self

                                        });
                                        editForm.setValues(record);
                                        editForm.show();
                                    }
                                }]
                            },
                            { text: 'ID', dataIndex: 'ID', flex: 1, hidden: true },
                            { text: "วันที่ช่วงส่งมอบ <br/> Delivery Phase Date", dataIndex: 'DeliveryPhaseDate', sortable: false, flex: 2, renderer: Ext.util.Format.dateRenderer('d/m/Y') },
                            {
                                text: "สถานะช่วงส่งมอบ <br/> Delivery Phase Status",
                                dataIndex: 'StatusOfProjectDeliveryPhase', sortable: false, flex: 1,
                                hidden: true,
                                renderer: function (value, metadata, record, rowIndex, colIndex, store) {
                                    var status = record.get('StatusOfProjectDeliveryPhase');

                                    if (status === 'Done') {
                                        metadata.style = "background-color:#99FF99;";
                                    } else {
                                        metadata.style = "background-color:#FFAD33;";
                                    }
                                    return value;
                                }
                            },
                            {
                                xtype: 'actioncolumn',
                                width: 30,
                                items: [{
                                    xtype: 'button',
                                    iconCls: 'delete-icon',
                                    handler: function (grid, rowIndex, colIndex, item, event, record, row) {
                                        grid.getSelectionModel().select(record);
                                        self.projectdeliveryphasestore.remove(record);
                                        self.setProjectDeliveryPhaseCount(self.projectdeliveryphasestore.getCount());
                                        //CommandActionBuilder.deleteData(
                                        //    record.data.ID,
                                        //    paramsView.urlDeleteProjectDeliveryPhase,
                                        //    self.projectdeliveryphasestore,
                                        //    null,
                                        //    function () {
                                        //        self.setProjectDeliveryPhaseCount(self.projectdeliveryphasestore.getCount());
                                        //    });

                                    }
                                }]
                            }
                    ],
                    defaults: {
                        sortable: false,
                        locked: true,
                        menuDisabled: false
                    }
                },
                bbar: Ext.create('Ext.PagingToolbar', {
                    store: self.projectdeliveryphasestore,
                    displayInfo: true,
                    displayMsg: 'ช่วงส่งมอบที่กำลังแสดงอยู่ {0} - {1} of {2}',
                    emptyMsg: "ไม่กำหนดช่วงส่งมอบ"
                }),
                tbar: [self.addProjectDeliveryPhaseDate]
            },
            {
                fieldLabel: 'วันที่ส่งมอบงานจริง / Delivery Date',
                xtype: 'datefield',
                id: 'DeliverDate',
                name: 'DeliverDate',
                itemId: 'DeliverDate',
                padding: '0 320 0 0',
                format: "d/m/Y",
                value: null,
                allowBlank: true,
                editable: false,
                hidden: self.isRegistered,
                disabled: true,
                listeners: {
                    change: function (dt, newValue, oldValue, eOpts) {
                        self.getWarrantyDateBetween().setDisabled(true);
                        if (newValue) {
                            self.getContractEndDate().setMaxValue(newValue);

                            self.getWarrantyDateBetween().setDisabled(false);
                            self.getWarrantyStartDate().setMinValue(newValue);
                            self.getWarrantyStartDate().setMaxValue(null);
                            self.getWarrantyStartDate().setValue(null);
                            self.getWarrantyEndDate().setValue(null);
                        }
                    }
                }
            }, {
                xtype: 'fieldcontainer',
                fieldLabel: 'วันที่เริ่มต้น - สิ้นสุด Warranty / Start - End Date for Warranty',
                id: 'warrantyDateBetween',
                name: 'warrantyDateBetween',
                itemId: 'warrantyDateBetween',
                layout: 'hbox',
                hidden: self.isRegistered,
                disabled: true,
                items: [{
                    xtype: 'datefield',
                    id: 'WarrantyStartDate',
                    name: 'WarrantyStartDate',
                    itemId: 'WarrantyStartDate',
                    width: 100,
                    format: "d/m/Y",
                    allowBlank: true,
                    editable: false,
                    vtype: 'daterange',
                    endDateField: 'WarrantyEndDate',
                    listeners: {
                        change: function (dt, newValue, oldValue, eOpts) {
                            self.getWarrantyEndDate().setDisabled(true);
                            if (newValue) {
                                self.getDeliverDate().setMaxValue(newValue);

                                self.getWarrantyEndDate().setDisabled(false);
                                self.getWarrantyEndDate().setValue(null);
                            }
                        }
                    }
                }, {
                    xtype: 'displayfield',
                    margin: '0 5 0 5',
                    value: '-'
                }, {
                    xtype: 'datefield',
                    id: 'WarrantyEndDate',
                    name: 'WarrantyEndDate',
                    itemId: 'WarrantyEndDate',
                    width: 100,
                    format: "d/m/Y",
                    allowBlank: true,
                    editable: false,
                    vtype: 'daterange',
                    startDateField: 'WarrantyStartDate',
                    disabled: true
                }]
            },
            {
                xtype: 'combo',
                fieldLabel: 'สถานะโครงการ / Project Status',
                name: 'StatusID',
                padding: '0 320 0 0',
                queryMode: 'local',
                displayField: 'Name',
                valueField: 'ID',
                store: projectStatusStore,
                value: 1,
                editable: false,
                listeners: {
                    change: function (cmb, newValue, oldValue, opts) {
                        //self.projectStatusNameDisplay.setValue(cmb.getRawValue());
                    }
                }
            },
            gridIniMember],
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

        var form = this.down('form').getForm();
        form.trackResetOnLoad = true;
        form.setValues(record.data);

        self.getContractStartDate().setMinValue(record.data.StartDate);

        //console.log(self.editData);

        if (self.editData) {
            if (self.editData.get("ProjectDeliveryPhaseCount") > 0) {
                self.getStartDate().setReadOnly(true);
                self.getContractStartDate().setReadOnly(true);
                self.getContractEndDate().setReadOnly(true);
                
            }
        }
    },

    setProjectDeliveryPhaseCount: function (count) {
        var self = this;

        self.getStartDate().setReadOnly(false);
        self.getContractStartDate().setReadOnly(false);
        self.getContractEndDate().setReadOnly(false);
        self.editData.set("ProjectDeliveryPhaseCount", count);
        if (count > 0) {
            self.getStartDate().setReadOnly(true);
            self.getContractStartDate().setReadOnly(true);
            self.getContractEndDate().setReadOnly(true);
        }
    },

    getStartDate: function () {
        return Ext.getCmp("StartDate");
    },

    getContractStartDate: function () {
        return Ext.getCmp("ContractStartDate");
    },
    getContractEndDate: function () {
        return Ext.getCmp("ContractEndDate");
    },

    getDeliverDate: function () {
        return Ext.getCmp("DeliverDate");
    },

    getWarrantyDateBetween: function () {
        return Ext.getCmp("warrantyDateBetween");
    },
    getWarrantyStartDate: function () {
        return Ext.getCmp("WarrantyStartDate");
    },
    getWarrantyEndDate: function () {
        return Ext.getCmp("WarrantyEndDate");
    }
});

Ext.define("ThousandSeparatorNumberField", {
    override: "Ext.form.field.Number",
    /**
    * @cfg {Boolean} useThousandSeparator
    */
    useThousandSeparator: false,

    /**
     * @inheritdoc
     */
    toRawNumber: function (value) {
        return String(value).replace(this.decimalSeparator, '.').replace(new RegExp(Ext.util.Format.thousandSeparator, "g"), '');
    },

    /**
     * @inheritdoc
     */
    getErrors: function (value) {
        if (!this.useThousandSeparator)
            return this.callParent(arguments);
        var me = this,
            errors = Ext.form.field.Text.prototype.getErrors.apply(me, arguments),
            format = Ext.String.format,
            num;

        value = Ext.isDefined(value) ? value : this.processRawValue(this.getRawValue());

        if (value.length < 1) { // if it's blank and textfield didn't flag it then it's valid
            return errors;
        }

        value = me.toRawNumber(value);

        if (isNaN(value.replace(Ext.util.Format.thousandSeparator, ''))) {
            errors.push(format(me.nanText, value));
        }

        num = me.parseValue(value);

        if (me.minValue === 0 && num < 0) {
            errors.push(this.negativeText);
        }
        else if (num < me.minValue) {
            errors.push(format(me.minText, me.minValue));
        }

        if (num > me.maxValue) {
            errors.push(format(me.maxText, me.maxValue));
        }

        return errors;
    },

    /**
     * @inheritdoc
     */
    valueToRaw: function (value) {
        if (!this.useThousandSeparator)
            return this.callParent(arguments);
        var me = this;

        var format = "000,000";
        for (var i = 0; i < me.decimalPrecision; i++) {
            if (i == 0)
                format += ".";
            format += "0";
        }
        value = me.parseValue(Ext.util.Format.number(value, format));
        value = me.fixPrecision(value);
        value = Ext.isNumber(value) ? value : parseFloat(me.toRawNumber(value));
        value = isNaN(value) ? '' : String(Ext.util.Format.number(value, format)).replace('.', me.decimalSeparator);
        return value;
    },

    /**
     * @inheritdoc
     */
    getSubmitValue: function () {
        if (!this.useThousandSeparator)
            return this.callParent(arguments);
        var me = this,
            value = me.callParent();

        if (!me.submitLocaleSeparator) {
            value = me.toRawNumber(value);
        }
        return value;
    },

    /**
     * @inheritdoc
     */
    setMinValue: function (value) {
        if (!this.useThousandSeparator)
            return this.callParent(arguments);
        var me = this,
            allowed;

        me.minValue = Ext.Number.from(value, Number.NEGATIVE_INFINITY);
        me.toggleSpinners();

        // Build regexes for masking and stripping based on the configured options
        if (me.disableKeyFilter !== true) {
            allowed = me.baseChars + '';

            if (me.allowExponential) {
                allowed += me.decimalSeparator + 'e+-';
            }
            else {
                allowed += Ext.util.Format.thousandSeparator;
                if (me.allowDecimals) {
                    allowed += me.decimalSeparator;
                }
                if (me.minValue < 0) {
                    allowed += '-';
                }
            }

            allowed = Ext.String.escapeRegex(allowed);
            me.maskRe = new RegExp('[' + allowed + ']');
            if (me.autoStripChars) {
                me.stripCharsRe = new RegExp('[^' + allowed + ']', 'gi');
            }
        }
    },

    /**
     * @private
     */
    parseValue: function (value) {
        if (!this.useThousandSeparator)
            return this.callParent(arguments);
        value = parseFloat(this.toRawNumber(value));
        return isNaN(value) ? null : value;
    }
});