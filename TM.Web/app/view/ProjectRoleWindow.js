Ext.define('TM.view.ProjectRoleWindow', {
    extend: 'Ext.window.Window',
    xtype: 'projectrolewindow',
    width: 500,
    title: TextLabel.addProjectRoleText,
    resizable: false,
    closable: false,
    constrain: true,
    config: {
        editData: null,
        order: 0,
        projectRoleStore: null
    },

    initComponent: function () {
        var self = this;

        var addAction = Ext.create('Ext.Action', {
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

                //console.log(form);

                var vals = form.getValues();
                //console.log(vals.Name);

                Ext.MessageBox.wait("กำลังบันทึกข้อมูล...", 'กรุณารอ');
                var saveData = Ext.create('widget.projectRole', {
                    ProjectRoleID: vals.ProjectRoleID,
                    ProjectRoleName: vals.ProjectRoleName,
                    Order: vals.Order,
                    ProjectRoleCost: vals.ProjectRoleCost
                });

                //console.log(saveData);

                Ext.Ajax.request({
                    url: paramsView.urlSaveProjectRole,
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
                                    self.close();
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
                            msg: "เกิดข้อผิดพลาดในการบันทึกฝ่าย " + transport.responseText,
                            //width: 300,
                            buttons: Ext.MessageBox.OK,
                            icon: Ext.MessageBox.ERROR
                        });
                    },
                    jsonData: saveData.data
                });
            }
        });

        self.items = [{
            xtype: 'form',
            layout: {
                type: 'vbox',
                align: 'stretch'
            },
            defaultType: 'textfield',
            bodyStyle: 'padding: 6px',
            fieldDefaults: {
                labelWidth: 200,
                allowBlank: false,
                labelAlign: 'right'
            },
            items: [
                { name: 'ProjectRoleID', xtype: 'textarea', allowBlank: true, hidden: true },
                {
                    fieldLabel: TextLabel.projectRoleLabelText + ' <span class="required">*</span>',
                    name: 'ProjectRoleName',
                    //colspan: 1,
                    maxLength: 100
                },
                {
                    xtype: 'numberfield',
                    fieldLabel: TextLabel.projectRoleCostText,
                    name: 'ProjectRoleCost',
                    allowBlank: true,
                    maxLength: 9,
                    forcePrecision: true,
                    decimalPrecision: 0,
                    useThousandSeparator: true,
                    fieldCls: 'a-form-num-field',
                    step: 100,
                    //padding: '0 100 0 0',
                    listeners: {
                        blur: function (field) {
                            field.setRawValue(Ext.util.Format.number(field.getValue(), '0,000'));//0,000.00
                        }
                    }
                },
                {
                    xtype: 'numberfield',
                    fieldLabel: TextLabel.orderLabel,
                    name: 'Order',
                    allowBlank: true,
                    maxLength: 5,
                    forcePrecision: true,
                    decimalPrecision: 0,
                    useThousandSeparator: true,
                    fieldCls: 'a-form-num-field',
                    margin: '0 100 5 0',
                    step: 1,
                    value: self.order
                }
            ],
            buttonAlign: 'center',
            buttons: [
                new Ext.button.Button(addAction),
                new Ext.button.Button(CommandActionBuilder.cancleAction(self))
            ]
        }];

        self.callParent();
    },

    setValues: function (record) {
        var self = this;

        var form = self.down('form').getForm();
        form.trackResetOnLoad = true;
        form.setValues(record.data);
    },
    listeners: {
        close: function (panel, eOpts) {
            var self = this;

            var form = self.down('form');
            if (form.isDirty()) {
                if (self.projectRoleStore) self.projectRoleStore.load();
            }
        }
    }
});