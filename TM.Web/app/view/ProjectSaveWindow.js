Ext.define('TM.view.ProjectSaveWindow', {
    extend: 'Ext.window.Window',
    xtype: 'projectSaveWindow',
    width: 780,
    title: '<i class="glyphicon glyphicon-plus"></i> เพิ่มโครงการ / Add Project',
    resizable: false,
    closable: false,
    constrain: true,
    config: {
        editData: null,
        projectStore: null,
        customerstore: null
    },
    initComponent: function () {
        var self = this;
        
        var gridIniMember = { xtype: 'hiddenfield' };

        if (self.editData) {
            self.title = "แก้ไขข้อมูลโครงการ / Edit Project";
        } else {

            var initialMemberStore = Ext.create('widget.initialMemberStore', {
                url: paramsView.urlGetAllInitialMember
            });
            self.initialMemberStore = initialMemberStore;

            gridIniMember = Ext.create('Ext.grid.Panel', {
                height: 230,
                iconCls: 'add-project-memebr-icon',
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
                        StatusID: vals.StatusID
                    });

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
                self.close();
            }
        });

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
            }, {
                fieldLabel: 'รหัสโครงการ / Project Code <span class="required">*</span><br/>(ตัวอย่าง PJ-XXX001)',
                id: 'Code',
                name: 'Code',
                validFlag: true,
                padding: '0 200 0 0',
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
            }, {
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
                step: 100000,
                minValue: 0,
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
                    endDateField: 'EndDate'
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
                items: [{
                    xtype: 'datefield',
                    id: 'ContractStartDate',
                    name: 'ContractStartDate',
                    width: 100,
                    format: "d/m/Y",
                    allowBlank: true,
                    editable: false,
                    vtype: 'daterange',
                    endDateField: 'ContractEndDate'
                }, {
                    xtype: 'displayfield',
                    margin: '0 5 0 5',
                    value: '-'
                }, {
                    xtype: 'datefield',
                    id: 'ContractEndDate',
                    name: 'ContractEndDate',
                    width: 100,
                    format: "d/m/Y",
                    allowBlank: true,
                    editable: false,
                    vtype: 'daterange',
                    startDateField: 'ContractStartDate'
                }]
            },
            {
                fieldLabel: 'วันที่ส่งมอบงานจริง / Delivery Date',
                xtype: 'datefield',
                id: 'DeliverDate',
                name: 'DeliverDate',
                padding: '0 250 0 0',
                format: "d/m/Y",
                value: null,
                allowBlank: true,
                editable: false
            }, {
                xtype: 'fieldcontainer',
                fieldLabel: 'วันที่เริ่มต้น - สิ้นสุด Warranty / Start - End Date for Warranty',
                id: 'warrantyDateBetween',
                name: 'warrantyDateBetween',
                layout: 'hbox',
                items: [{
                    xtype: 'datefield',
                    id: 'WarrantyStartDate',
                    name: 'WarrantyStartDate',
                    width: 100,
                    format: "d/m/Y",
                    allowBlank: true,
                    editable: false,
                    vtype: 'daterange',
                    endDateField: 'WarrantyEndDate'
                }, {
                    xtype: 'displayfield',
                    margin: '0 5 0 5',
                    value: '-'
                }, {
                    xtype: 'datefield',
                    id: 'WarrantyEndDate',
                    name: 'WarrantyEndDate',
                    width: 100,
                    format: "d/m/Y",
                    allowBlank: true,
                    editable: false,
                    vtype: 'daterange',
                    startDateField: 'WarrantyStartDate'
                }]
            }, {
                xtype: 'combo',
                fieldLabel: 'สถานะโครงการ / Project Status',
                name: 'StatusID',
                padding: '0 250 0 0',
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
        var form = this.down('form').getForm();
        form.setValues(record.data);
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