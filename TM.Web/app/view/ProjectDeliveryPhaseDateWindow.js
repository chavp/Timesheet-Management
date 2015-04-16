Ext.define('TM.view.ProjectDeliveryPhaseDateWindow', {
    extend: 'Ext.window.Window',
    xtype: 'projectdeliveryphasedatewindow',
    width: 400,
    title: 'เพิ่มช่วงการส่งมอบโครงการ / Add Project Delivery Phase Date',
    resizable: false,
    closable: false,
    constrain: true,
    config: {
        editData: null,
        projectdeliveryphasestore: null,
        mindate: null,
        maxdate: null,
        projectID: null,
        projectsavewindow: null
    },

    initComponent: function () {
        var self = this;

        var projectDeliveryPhases = Ext.create('Ext.data.Store', {
            fields: ['value', 'name'],
            data: [
                { "value": "InProgress", "name": "In Progress" },
                { "value": "Done", "name": "Done" }
            ]
        });

        var messageAlert = function () {
            Ext.MessageBox.show({
                title: TextLabel.validationTitle,
                msg: "วันที่ช่วงส่งมอบนี้ถูกกำหนดแล้ว กรุณาระบุวันที่ช่วงส่งมอบใหม่",
                buttons: Ext.MessageBox.OK,
                icon: Ext.MessageBox.WARNING
            });
        }

        var addData = function (saveData) {
            self.close();
            self.projectdeliveryphasestore.add(saveData);
            //self.projectdeliveryphasestore.commitChanges();
            self.projectdeliveryphasestore.sort([{
                property: 'DeliveryPhaseDate',
                direction: 'ASC'
            }]);
            self.projectsavewindow.setProjectDeliveryPhaseCount(self.projectdeliveryphasestore.getCount());
        }

        var addAction = Ext.create('Ext.Action', {
            text: TextLabel.saveActionText,
            disabled: false,
            handler: function (widget, event) {
                //event.preventEvent();

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
                //console.log(vals);

                //Ext.MessageBox.wait("กำลังบันทึกข้อมูล...", 'กรุณารอ');
                var saveData = Ext.create('widget.projectdeliveryphase', {
                    ID: vals.ID,
                    ProjectID: vals.ProjectID,
                    DeliveryPhaseDate: Ext.Date.parse(vals.DeliveryPhaseDate, "d/m/Y"),
                    StatusOfProjectDeliveryPhase: vals.StatusOfProjectDeliveryPhase
                });

                if (self.editData == null) {
                    var dup = self.projectdeliveryphasestore.findRecord("DeliveryPhaseDate", saveData.get("DeliveryPhaseDate"));
                    if (dup !== null) {
                        messageAlert();
                        return false;
                    }
                    addData(saveData);
                } else {
                    var dup = self.projectdeliveryphasestore.findRecord("DeliveryPhaseDate", saveData.get("DeliveryPhaseDate"));
                    if (dup !== null && self.editData.data.ID !== dup.get("ID")) {
                        messageAlert();
                        return false;
                    }

                    //console.log(self.editData);
                    //var update = self.projectdeliveryphasestore.findRecord("ID", vals.ID);
                    //if (update != null) {
                    //    update.set("DeliveryPhaseDate", saveData.get("DeliveryPhaseDate"));
                    //    update.set("StatusOfProjectDeliveryPhase", saveData.get("StatusOfProjectDeliveryPhase"));
                    //} else {
                    //    self.editData.set("DeliveryPhaseDate", saveData.get("DeliveryPhaseDate"));
                    //    self.editData.set("StatusOfProjectDeliveryPhase", saveData.get("StatusOfProjectDeliveryPhase"));
                    //}

                    self.editData.set("DeliveryPhaseDate", saveData.get("DeliveryPhaseDate"));
                    self.editData.set("StatusOfProjectDeliveryPhase", saveData.get("StatusOfProjectDeliveryPhase"));

                    self.close();
                    self.projectdeliveryphasestore.sort([{
                        property: 'DeliveryPhaseDate',
                        direction: 'ASC'
                    }]);
                }

                //Ext.Ajax.request({
                //    url: paramsView.urlSaveProjectDeliveryPhase,
                //    success: function (transport) {
                //        Ext.MessageBox.hide();
                //        var respose = Ext.decode(transport.responseText);
                //        if (respose.success) {
                //            Ext.MessageBox.show({
                //                title: TextLabel.successTitle,
                //                msg: 'บันทึกข้อมูลเสร็จสมบูรณ์',
                //                //width: 300,
                //                buttons: Ext.MessageBox.OK,
                //                icon: Ext.MessageBox.INFO,
                //                fn: function (btn) {

                //                    if (self.projectdeliveryphasestore)
                //                        self.projectdeliveryphasestore.load({
                //                            callback: function (records, operation, success) {
                //                                //console.log(self.projectdeliveryphasestore.getCount());
                //                                if (self.projectsavewindow) {
                //                                    self.projectsavewindow.setProjectDeliveryPhaseCount(self.projectdeliveryphasestore.getCount());
                //                                }
                //                            }
                //                        });

                //                    self.close();
                //                }
                //            });
                //        } else {
                //            Ext.MessageBox.show({
                //                title: TextLabel.errorAlertTitle,
                //                msg: respose.message,
                //                //width: 300,
                //                buttons: Ext.MessageBox.OK,
                //                icon: Ext.MessageBox.ERROR
                //            });
                //        }
                //    },
                //    failure: function (transport) {
                //        Ext.MessageBox.show({
                //            title: TextLabel.errorAlertTitle,
                //            msg: "เกิดข้อผิดพลาดในการบันทึกตำแหน่ง " + transport.responseText,
                //            //width: 300,
                //            buttons: Ext.MessageBox.OK,
                //            icon: Ext.MessageBox.ERROR
                //        });
                //    },
                //    jsonData: saveData.data
                //});
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
                labelWidth: 250,
                allowBlank: false,
                labelAlign: 'right'
            },
            items: [
                { name: 'ID', xtype: 'textarea', allowBlank: true, hidden: true },
                { name: 'ProjectID', xtype: 'textarea', allowBlank: true, hidden: true, value: self.projectID },
                {
                    xtype: 'datefield',
                    fieldLabel: 'วันที่ช่วงส่งมอบ / Delivery Phase Date<span class="required">*</span>',
                    name: 'DeliveryPhaseDate',
                    padding: '0 20 0 0',
                    editable: false,
                    minValue: self.mindate,
                    maxValue: self.maxdate,
                    format: "d/m/Y"
                    //colspan: 1,
                    //maxLength: 100
                },
                {
                    xtype: 'combo',
                    hidden: true,
                    fieldLabel: 'สถานะช่วงส่งมอบ / Delivery Phase Status',
                    name: 'StatusOfProjectDeliveryPhase',
                    store: projectDeliveryPhases,
                    queryMode: 'local',
                    displayField: 'name',
                    valueField: 'value',
                    padding: '0 20 0 0',
                    editable: false,
                    value: "InProgress"
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
            
        }
    }
});