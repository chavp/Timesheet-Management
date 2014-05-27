Ext.apply(Ext.form.field.VTypes, {
    password: function (val, field) {
        if (field.initialPassField) {
            var pwd = field.up('form').down('#' + field.initialPassField);
            return (val == pwd.getValue());
        }
        return true;
    },
    passwordText: 'คุณระบุรหัสเข้าระบบใหม่ไม่ตรงกัน'
});

Ext.define('TM.view.UserChangePasswordWindow', {
    extend: 'Ext.window.Window',
    xtype: 'userChangePasswordWindow',
    title: '<i class="glyphicon glyphicon-lock"></i> แก้ไขรหัสเข้าระบบ / Change Password',
    resizable: false,
    closable: false,
    width: 480,
    config: {
    },
    initComponent: function () {
        var self = this;

        this.items = [{
            xtype: 'form',
            layout: 'form',
            fieldDefaults: {
                labelWidth: 300,
                allowBlank: false,
                inputType: 'password',
                labelAlign: 'right',
                maxLength: 20,
                minLength: 1
            },
            bodyPadding: '5 5 0',
            defaultType: 'textfield',
            items: [{
                id: 'oldPassword',
                name: 'oldPassword',
                fieldLabel: 'รหัสเข้าระบบเดิม / Old Password',
                emptyText: messagesForm.requireInputEmptyText
            }, {
                id: 'newPassword',
                name: 'newPassword',
                fieldLabel: 'รหัสเข้าระบบใหม่ / New Password',
                emptyText: messagesForm.requireInputEmptyText
            }, {
                id: 'confirmNewPassword',
                name: 'confirmNewPassword',
                fieldLabel: 'ยืนยันรหัสเข้าระบบใหม่ / Confirm New Password',
                emptyText: messagesForm.requireInputEmptyText,
                vtype: 'password',
                initialPassField: 'newPassword' // id of the initial password field
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
                    Ext.MessageBox.wait("กำลังดำเนินการแก้ไขรหัสเข้าระบบ...", 'กรุณารอ');
                    Ext.Ajax.request({
                        url: paramsView.urlChangePassword,
                        jsonData: vals,
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
                                        self.close();
                                    }
                                });
                            } else {
                                Ext.MessageBox.show({
                                    title: messagesForm.errorAlertTitle,
                                    msg: 'เกิดข้อผิดพลาดในขั้นตอนแก้ไขรหัสเข้าระบบ <br/>' + response.message,
                                    //width: 300,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                            }
                        },
                        failure: function (transport) {
                            Ext.MessageBox.hide();
                            Ext.MessageBox.show({
                                title: messagesForm.errorAlertTitle,
                                msg: 'เกิดข้อผิดพลาดในการบันทึกข้อมูล',
                                //width: 300,
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                        }
                    });

                }
            }, {
                text: messagesForm.cancleActionText,
                handler: function () {
                    self.close();
                }
            }]
        }];

        this.callParent(arguments);
    }
});