Ext.define('TM.view.ProjectUpdateProgressSaveWindow', {
    extend: 'Ext.window.Window',
    xtype: 'projectUpdateProgressSaveWindow',
    title: '<i class="glyphicon glyphicon-edit"></i> อัพเด็ทโปรเกรซ / Update Progress',
    resizable: false,
    closable: false,
    constrain: true,
    width: 480,
    config: {
    },
    initComponent: function () {
        var self = this;

        var cancleAction = Ext.create('Ext.Action', {
            //iconCls: 'add-button',
            text: messagesForm.cancleActionText,
            disabled: false,
            handler: function (widget, event) {
                self.close();
            }
        });

        Ext.apply(self, {
            items: [{
                xtype: 'form',
                layout: 'form',
                fieldDefaults: {
                    labelWidth: 250,
                    allowBlank: false,
                    labelAlign: 'right',
                    maxLength: 20,
                    minLength: 1
                },
                bodyPadding: '5 5 0',
                defaultType: 'textfield',
                items: [{
                    name: 'ID',
                    allowBlank: true,
                    hidden: true
                }, {
                    name: 'ProjectID',
                    allowBlank: true,
                    hidden: true
                }, {
                    xtype: 'datefield',
                    name: 'UpdateProgress',
                    padding: '0 350 0 0',
                    fieldLabel: 'วันที่อัพเด็ทโปรเกรซ / Update Progress <span class="required">*</span>',
                    format: "d/m/Y",
                    value: paramsView.maxDateText,
                    maxValue: paramsView.maxDateText,
                    editable: false
                }, {
                    xtype: 'numberfield',
                    name: 'UpdatedValue',
                    fieldLabel: 'โปรเกรซ / Progress',
                    fieldCls: 'a-form-num-field',
                    //value: 0,
                    decimalPrecision: 0,
                    maxValue: 100
                }]
            }],
            //buttonAlign: 'center',
            buttons: [{
                text: messagesForm.okActionText,
                handler: function () {
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

                    var vals = form.getValues();
                    console.log(vals);
                }
            }, new Ext.button.Button(cancleAction)]
        });

        self.callParent();
    },

    setValues: function (record) {
        var self = this;

        var form = this.down('form').getForm();
        form.setValues(record);
    }
});