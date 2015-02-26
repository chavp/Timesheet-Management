Ext.define('TM.view.DivisionWindow', {
    extend: 'Ext.window.Window',
    xtype: 'divisionwindow',
    width: 500,
    title: 'เพิ่มฝ่าย / Add Division',
    resizable: false,
    closable: false,
    constrain: true,
    config: {
        editData: null,
        divisionStore: null,
        departmentStore: null,
        employeeStore: null
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
                var saveData = Ext.create('widget.division', {
                    ID: vals.ID,
                    Name: vals.Name
                });

                Ext.Ajax.request({
                    url: paramsView.urlSaveDivision,
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
                labelWidth: 120,
                allowBlank: false,
                labelAlign: 'right'
            },
            items: [
                { name: 'ID', xtype: 'textarea', allowBlank: true, hidden: true },
                {
                    fieldLabel: TextLabel.divisionLabelText + '<span class="required">*</span>',
                    name: 'Name',
                    //colspan: 1,
                    maxLength: 100
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
        //console.log(form.isDirty());
    },

    listeners: {
        close: function (panel, eOpts) {
            var self = this;

            var form = self.down('form');
            if (form.isDirty()) {
                if (self.divisionStore) self.divisionStore.load();
                if (self.departmentStore) self.departmentStore.load();
                if (self.employeeStore) self.employeeStore.load();
            }
        }
    }
});