Ext.define('TM.view.PositionWindow', {
    extend: 'Ext.window.Window',
    xtype: 'positionwindow',
    width: 500,
    title: 'เพิ่มตำแหน่ง / Add Position',
    resizable: false,
    closable: false,
    constrain: true,
    config: {
        editData: null,
        positionStore: null
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
                var saveData = Ext.create('widget.position', {
                    ID: vals.ID,
                    Name: vals.Name,
                    PositionCost: vals.PositionCost
                });

                Ext.Ajax.request({
                    url: paramsView.urlSavePosition,
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
                            msg: "เกิดข้อผิดพลาดในการบันทึกตำแหน่ง " + transport.responseText,
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
                labelWidth: 120,
                allowBlank: false,
                labelAlign: 'right'
            },
            items: [
                { name: 'ID', xtype: 'textarea', allowBlank: true, hidden: true },
                {
                    fieldLabel: TextLabel.positionLabelText + '<span class="required">*</span>',
                    name: 'Name',
                    //colspan: 1,
                    maxLength: 100
                },
                {
                    xtype: 'numberfield',
                    fieldLabel: TextLabel.positionProjectRoleRateCostLabelText,
                    name: 'PositionCost',
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
                if (self.positionStore) self.positionStore.load();
            }
        }
    }
});