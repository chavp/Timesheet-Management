Ext.define('TM.view.ProjectUpdateProgressWindow', {
    extend: 'Ext.window.Window',
    xtype: 'projectUpdateProgressWindow',
    width: 780,
    title: 'แก้ไขโปรเกรซ / Edit Project Progress',
    resizable: false,
    closable: false,
    constrain: true,
    config: {
        editData: null,
        projectStore: null
    },
    requires: [
        'Ext.selection.CellModel'
    ],
    initComponent: function () {
        var self = this;

        var projectProgressUpdateLogStore = Ext.create('widget.projectProgressUpdateLogStore', {
            pageSize: 11
        });
        self.projectProgressUpdateLogStore = projectProgressUpdateLogStore;

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
                width: '100%',
                layout: {
                    type: 'vbox',
                    align: 'stretch'
                },
                frame: false,
                bodyStyle: 'padding: 6px',
                defaultType: 'textfield',
                fieldDefaults: {
                    labelWidth: 350,
                    allowBlank: true,
                    labelAlign: 'right'
                },
                items: [{
                    name: 'ID',
                    xtype: 'textarea',
                    hidden: true
                }, {
                    fieldLabel: 'รหัสโครงการ / Project Code',
                    name: 'Code',
                    padding: '0 200 0 0',
                    readOnly: true
                }, {
                    xtype: 'numberfield',
                    fieldLabel: 'มูลค่าโครงการโดยประมาณ / Estimated Value of Project',
                    name: 'EstimateProjectValue',
                    maxLength: 15,
                    forcePrecision: true,
                    decimalPrecision: 2,
                    useThousandSeparator: true,
                    fieldCls: 'a-form-num-field',
                    padding: '0 137 0 0',
                    listeners: {
                        blur: function (field) {
                            field.setRawValue(Ext.util.Format.number(field.getValue(), '0,000.00'));
                        }
                    },
                    readOnly: true
                }, {
                    fieldLabel: 'ชื่อโครงการภาษาไทย / Project Thai Name',
                    name: 'NameTH',
                    readOnly: true
                }, {
                    fieldLabel: 'ชื่อโครงการภาษาอังกฤษ / Project Eng Name',
                    name: 'NameEN',
                    readOnly: true
                }, {
                    fieldLabel: 'ชื่อลูกค้า / Customer Name',
                    name: 'CustomerName',
                    readOnly: true
                }, {
                    xtype: 'gridpanel',
                    id: 'gridProjectProgressPanel',
                    height: 400,
                    title: 'ประวัติการอัพเด็ทโปรเกรซ / Updated Project Progress Histories',
                    store: self.projectProgressUpdateLogStore,
                    columns: {
                        items: [
                            { text: 'ID', dataIndex: 'ID', flex: 1, hidden: true},
                            {
                                text: 'วันที่อัพเด็ทล่าสุด<br/>Latest Update', dataIndex: 'UpdatedDate', width: 200, renderer: Ext.util.Format.dateRenderer('d/m/Y'),
                                editor: {
                                    allowBlank: false
                                }
                            },
                            {
                                text: 'โปรเกรซ<br/>Progress', dataIndex: 'UpdatedValue', flex: 1,
                                editor: {
                                    allowBlank: false
                                }
                            }
                        ],
                        defaults: {
                            sortable: false,
                            menuDisabled: true,
                            renderer: function (value, metaData, record, rowIdx, colIdx, store) {
                                if (value) {
                                    value = Ext.String.htmlEncode(value);
                                    metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';
                                }
                                return value;
                            }
                        }
                    },
                    // paging bar on the bottom
                    bbar: Ext.create('Ext.PagingToolbar', {
                        store: self.projectProgressUpdateLogStore,
                        displayInfo: true,
                        displayMsg: 'ข้อมูล {0} - {1} of {2}',
                        emptyMsg: "ไม่พบประวัติการอัพเด็ท"
                    }),
                    tbar: [{
                        cls: 'btn',
                        xtype: 'button',
                        iconCls: 'glyphicon glyphicon-edit',
                        text: 'อัพเด็ทโปรเกรซ / Update Progress',
                        scope: self,
                        handler: self.onAddClick
                    }]
                }]
            }],
            //buttonAlign: 'center',
            buttons: [new Ext.button.Button(cancleAction)]
        });

        self.callParent();
    },

    setValues: function (record) {
        var self = this;

        var form = this.down('form').getForm();
        form.setValues(record.data);

        self.projectProgressUpdateLogStore.proxy.extraParams.projectID = record.data.ID;
        self.projectProgressUpdateLogStore.load({ url: paramsView.urlFindProjectProgressUpdateLog });
    },

    onAddClick: function (btn) {
        var self = this;
        var form = this.down('form').getForm();
        var vals = form.getValues();

        var popup = Ext.create('widget.projectUpdateProgressSaveWindow', {
            animateTarget: btn,
            modal: true
        });

        popup.setValues({
            ID: vals.ID,
            ProjectID: vals.ProjectID
        });
        var po = btn.getPosition();
        //var offset = $(self).offset();
        popup.showAt(po[0], po[1]);
        //console.log(po);
        //popup.showAt(btn);
    }
});