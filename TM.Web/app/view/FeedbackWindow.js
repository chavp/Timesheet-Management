Ext.define('TM.view.FeedbackWindow', {
    extend: 'Ext.window.Window',
    xtype: 'feedbackWindow',
    title: '<i class="glyphicon glyphicon-comment"></i> ทุก feedback ของคุณ คือ วิวัฒนาการของเรา...',
    resizable: false,
    closable: false,
    width: 450,
    config: {
    },
    initComponent: function () {
        var self = this;

        var rating = new Ext.ux.widget.Rating({
            fieldLabel: 'ระดับความสำคัญ หรือ ระดับความพึงพอใจ',
            id: 'rating',
            name: 'rating',
            value: 2.5,
            titles: {
                '0': 'ปรับปรุงหรือไม่มีก็ได้ หรือ แย่',
                '1.5': 'ปรับปรุงก็จะดี หรือ กลางๆ',
                '3.5': 'จะต้องปรับปรุง หรือ ยอดเยี่ยม',
                '5': 'ต้องปรับปรุง หรือ สมบูรณ์แบบ'
            }
        });

        this.items = [{
            xtype: 'form',
            layout: 'form',
            fieldDefaults: {
                labelWidth: 290,
                allowBlank: false,
                labelAlign: 'right'
            },
            bodyPadding: '5 5 0',
            defaultType: 'textfield',
            items: [{
                xtype: 'textareafield',
                id: 'message',
                name: 'message',
                maxLength: 500,
                minLength: 5,
                fieldLabel: '',
                emptyText: 'กรุณาระบุ คำแนะนำ, ติชม, ปัญหาข้อบกพร่อง หรือ คุณสมบัติของซอฟแวร์ที่คุณต้องการให้ปรับปรุงเพิ่มเติม'
            },
            rating],
            buttons: [{
                text: '<i class="glyphicon glyphicon-floppy-disk"></i> บันทึก / Save',
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
                    Ext.MessageBox.wait("กำลังบันทึก feedback...", 'กรุณารอ');
                    Ext.Ajax.request({
                        url: paramsView.urlSaveFeedback,
                        jsonData: vals,
                        success: function (transport) {
                            Ext.MessageBox.hide();
                            var response = Ext.decode(transport.responseText);
                            if (response.success) {
                                Ext.MessageBox.show({
                                    title: messagesForm.successTitle,
                                    msg: response.message + "<br/>ขอบคุณค่ะ <i class='glyphicon glyphicon-heart'></i>",
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
                                    msg: 'เกิดข้อผิดพลาดในขั้นตอนบันทึก feedback <br/>' + response.message,
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
    },

    setMessageFocus: function () {
        Ext.getCmp('message').focus(false, 500);
    }
});