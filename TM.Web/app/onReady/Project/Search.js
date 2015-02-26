function actionCursorRenderer(val, metadata, record) {
    metadata.style = 'cursor: pointer;'
    return val;
}

function addSelectAllDepartmentModel(store) {
    store.add(Ext.create('widget.department', {
        ID: -1,
        DivisionID: -1,
        Name: TextLabel.allLabel,
    }));
}

Ext.onReady(function () {
    var projectStore = Ext.create('widget.projectStore', {
        pageSize: 14
    });
    var divisionStore = Ext.create('widget.divisionStore');

    var departmentStore = Ext.create('widget.departmentStore');
    //add All department
    addSelectAllDepartmentModel(departmentStore);

    var customerstore = Ext.create("widget.customerstore");
    customerstore.load({
        pageSize: 9999,
        url: paramsView.urlGetCustomer
    })
    var searchProject = function () {
        var form = Ext.getCmp('searchProjectForm');
        if (form.isValid()) {

            var values = form.getValues();
            var projectCode = values.ProjectCode;
            var projectName = values.ProjectName;
            projectStore.currentPage = 1;
            projectStore.proxy.extraParams.projectCode = projectCode;
            projectStore.proxy.extraParams.projectName = projectName;
            projectStore.load({
                callback: function (records, operation, success) {
                    if (success) {

                    }
                }
            });

        }
    }

    var searchProjectFieldset = {
        xtype: 'fieldset',
        title: '<h5>' + TextLabel.searchCriterionLabel + '</h5>',
        border: false,
        defaultType: 'field',
        defaults: {
            labelWidth: 150,
            anchor: '-20 -20',
            allowBlank: false,
            labelAlign: 'right'
        },
        bodyPadding: 10,
        items: [
            {
                xtype: 'fieldcontainer',
                fieldLabel: TextLabel.projectLabel,
                name: 'project',
                layout: 'hbox',
                bodyPadding: 5,
                defaults: {
                    labelWidth: 50,
                    allowBlank: true
                },
                items: [{
                    xtype: 'textfield',
                    id: 'ProjectCode',
                    name: 'ProjectCode',
                    fieldLabel: '',
                    emptyText: TextLabel.codeLabel,
                    flex: 1,
                    maxLength: 30,
                    listeners: {
                        change: function (obj, newValue) {
                            //console.log(newValue);
                            obj.setRawValue(newValue.toUpperCase());
                        },
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                searchProject();
                            }
                        }
                    }
                }, {
                    xtype: 'textfield',
                    id: 'ProjectName',
                    name: 'ProjectName',
                    fieldLabel: '',
                    emptyText: TextLabel.nameLabel,
                    maxLength: 255,
                    flex: 3,
                    listeners: {
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                searchProject();
                            }
                        }
                    }
                }]
            }
        ]
    };

    Ext.create('Ext.panel.Panel', {
        layout: 'border',
        renderTo: 'projectPanel',
        width: 'auto',
        height: WindowHeight.height,
        border: 1,
        defaults: {
            frame: false,
            split: false
        },
        items: [
            {
                xtype: 'form',
                id: 'searchProjectForm',
                title: '',
                region: 'north',
                bodyPadding: "0 20 0 20",
                collapsible: false,
                items: [searchProjectFieldset],
                buttonAlign: 'center',
                border: 0,
                buttons: [
                    {
                        text: TextLabel.cmdSearchText,
                        handler: function (btn) {
                            searchProject();
                        }
                    }, {
                        text: TextLabel.cmdClearText,
                        handler: function (btn) {
                            var form = Ext.getCmp('searchProjectForm').getForm();
                            form.reset();
                            searchProject();
                        }
                    }
                ]
            },
            {
                xtype: 'gridpanel',
                id: 'gridProject',
                frame: false,
                title: '',
                region: 'center',
                store: projectStore,
                columns: [
                    {
                        xtype: 'rownumberer',
                        width: 30,
                        sortable: false,
                        locked: true
                    }, {
                        xtype: 'actioncolumn',
                        sortable: false,
                        menuDisabled: true,
                        width: 100,
                        items: [{
                            // Use a URL in the icon config
                            xtype: 'button',
                            getTip: function (v, meta, rec) {
                                var members = rec.get('Members');
                                if (members > paramsConst.limitMember) {
                                    return 'แก้ไขผู้รับผิดชอบ / Edit Member <br/> *โครงการพิเศษ คุณจะไม่สามารถเพิ่ม หรือลบสมาชิกในโครงการนี้ได้';
                                }
                                return 'เพิ่มผู้รับผิดชอบ / Add Member';
                            },
                            iconCls: 'add-project-memebr-icon',
                            getClass: function (v, meta, rec) {
                                var members = rec.get('Members');
                                if (members > paramsConst.limitMember) {
                                    return 'edit-project-memebr-icon';
                                }
                                return 'add-project-memebr-icon';
                            },
                            handler: function (grid, rowIndex, colIndex, item, event, record, row) {
                                //var record = grid.getStore().getAt(rowIndex);
                                grid.getSelectionModel().select(record);
                                var members = record.get('Members');
                                if (members <= paramsConst.limitMember) {
                                    var projectForm = Ext.create('widget.projectWindow', {
                                        iconCls: 'add-project-memebr-icon',
                                        projectData: record,
                                        animateTarget: row,
                                        modal: true,
                                        divisionStore: divisionStore,
                                        departmentStore: departmentStore
                                    });
                                    departmentStore.removeAll();
                                    addSelectAllDepartmentModel(departmentStore);
                                    //console.log(record);
                                    if (paramsView.isManagerRole) {
                                        projectForm.setDefaultDivisionAndDepartment();
                                    }
                                    projectForm.show();
                                } else {
                                    var projectForm = Ext.create('widget.editProjectMemberWindow', {
                                        iconCls: 'edit-project-memebr-icon',
                                        projectData: record,
                                        animateTarget: row,
                                        projectStore: projectStore,
                                        title: 'แก้ไขผู้รับผิดชอบ / Edit Member <br/> *โครงการพิเศษ คุณจะไม่สามารถเพิ่ม หรือลบสมาชิกในโครงการนี้ได้',
                                        modal: true
                                    });
                                    projectForm.show();
                                }
                            }
                        },
                        {
                            xtype: 'button',
                            getTip: function (v, meta, rec) {
                                return 'แก้ไขโครงการ / Edit Project';
                            },
                            getClass: function (v, meta, rec) {
                                return 'edit-project-icon';
                            },
                            isDisabled: function (view, rowIndex, colIndex, item, record) {
                                if (paramsView.isAdminRole) return false;
                                var isOwner = record.get('IsOwner');
                                if (isOwner) return false;
                                if (paramsView.isManagerRole) return true;
                                return false;
                            },
                            handler: function (grid, rowIndex, colIndex, item, event, record, row) {
                                grid.getSelectionModel().select(record);
                                var editForm = Ext.create('widget.projectSaveWindow', {
                                    iconCls: 'edit-project-icon',
                                    editData: record,
                                    animateTarget: row,
                                    projectStore: projectStore,
                                    customerstore: customerstore,
                                    modal: true
                                });

                                editForm.setValues(record);
                                editForm.show();
                            }
                        },
                        {
                            xtype: 'button',
                            tooltip: 'Cumulative Flow Diagram (CFD)',
                            iconCls: 'chart-line-project-member-icon',
                            handler: function (grid, rowIndex, colIndex, item, event, record, row) {
                                grid.getSelectionModel().select(record);
                                var chartForm = Ext.create('widget.chartWindow', {
                                    title: "" + record.get('Code') + " (" + record.get('Name') + ")",
                                    iconCls: 'chart-line-project-member-icon',
                                    projectData: record,
                                    animateTarget: row,
                                    modal: true
                                });
                                chartForm.show();
                            }
                        },
                        {
                            xtype: 'button',
                            tooltip: 'Project Activities Bar',
                            iconCls: 'chart-bar-project-member-icon',
                            handler: function (grid, rowIndex, colIndex, item, event, record, row) {
                                grid.getSelectionModel().select(record);

                                var barChartForm = Ext.create('widget.projectactivitiesbarwindow', {
                                    title: "" + record.get('Code') + " (" + record.get('Name') + ")",
                                    iconCls: 'chart-bar-project-member-icon',
                                    projectData: record,
                                    animateTarget: row,
                                    modal: true
                                });
                                barChartForm.show();
                            }
                        }
                        ]
                    },
                    { text: 'ID', dataIndex: 'ID', flex: 1, hidden: true, sortable: true },
                    { text: TextLabel.projectCodeColumnText, dataIndex: 'Code', sortable: true, flex: 1 },
                    {
                        text: TextLabel.projectNameColumnText, dataIndex: 'Name', sortable: true, flex: 3,
                        renderer: function (value, metaData, record, rowIdx, colIdx, store) {
                            // Sample value: msimms & Co. "like" putting <code> tags around your code

                            value = Ext.String.htmlEncode(value);

                            // "double-encode" before adding it as a data-qtip attribute
                            metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';

                            return value;
                        }
                    },
                    { text: 'วันที่เริ่มโครงการ<br/>Start Date', dataIndex: 'StartDate', sortable: true, flex: 1, renderer: Ext.util.Format.dateRenderer('d/m/Y') },
                    { text: 'วันที่ปิดโครงการ<br/>End Date', dataIndex: 'EndDate', sortable: true, flex: 1, renderer: Ext.util.Format.dateRenderer('d/m/Y') },
                    { text: 'สมาชิกในโครงการ<br/>Project Members', dataIndex: 'Members', sortable: true, flex: 1, align: 'center' },
                    {
                        text: 'สถานะโครงการ<br/>Project Status', dataIndex: 'StatusDisplay', sortable: true, flex: 1, align: 'center',
                        renderer: function (value, metadata, record, rowIndex, colIndex, store) {
                            var status = record.get('StatusDisplay');

                            if (status === 'Open') {
                                metadata.style = "background-color:#99FF99;";
                            } else {
                                metadata.style = "background-color:#FFCCCC;";
                            }
                            return value;
                        }
                    },
                    {
                        xtype: 'actioncolumn',
                        sortable: false,
                        menuDisabled: true,
                        width: 30,
                        items: [{
                            xtype: 'button',
                            iconCls: 'delete-icon',
                            text: TextLabel.cmdDeleteText,
                            tooltip: TextLabel.cmdDeleteText,
                            scope: this,
                            xtype: 'button',
                            isDisabled: function (view, rowIndex, colIndex, item, record) {
                                if (record.get('TotalTimesheet') > 0) return true;
                                if (paramsView.isAdminRole) return false;
                                if (paramsView.isManagerRole) return true;
                                return false;
                            },
                            handler: function (grid, rowIndex, colIndex, item, event, record, row) {
                                grid.getSelectionModel().select(record);
                                Ext.MessageBox.confirm('ยืนยัน', 'คุณต้องการลบข้อมูลนี้ใช่ หรือ ไม่?',
                                function (btn) {
                                    if (btn === "yes") {
                                        Ext.MessageBox.wait("กำลังลบข้อมูล...", 'กรุณารอ');
                                        Ext.Ajax.request({
                                            url: paramsView.urlProjectApi + '/DeleteProject',
                                            type: 'json',
                                            method   : 'POST',
                                            jsonData : {projectID: record.data.ID},
                                            success  : function (response) {
                                                Ext.MessageBox.hide();
                                                var o = Ext.decode(response.responseText);
                                                if (o.success) {
                                                    Ext.MessageBox.show({
                                                        title: TextLabel.successTitle,
                                                        msg: 'ลบข้อมูลเสร็จสมบูรณ์',
                                                        //width: 300,
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.INFO,
                                                        fn: function (btn) {
                                                        }
                                                    });
                                                    projectStore.load();
                                                } else {
                                                    Ext.MessageBox.show({
                                                        title: messagesForm.errorAlertTitle,
                                                        msg: o.message,
                                                        //width: 300,
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.ERROR
                                                    });
                                                }
                                                //console.log(o);
                                            },
                                            failure  : function (response) {
                                                Ext.MessageBox.hide();
                                                //console.log(response);
                                                //var o = Ext.decode(response.responseText);
                                                //console.log(o);
                                                Ext.MessageBox.show({
                                                    title: messagesForm.errorAlertTitle,
                                                    msg: 'เกิดข้อผิดพลาดในการลบข้อมูล <br/>' + response.responseText,
                                                    //width: 300,
                                                    buttons: Ext.MessageBox.OK,
                                                    icon: Ext.MessageBox.ERROR
                                                });

                                                projectStore.load();
                                            }
                                        });

                                        //record.erase({
                                        //    success: function (record, operation) {
                                        //        self.projectStore.load();
                                                
                                        //        Ext.MessageBox.show({
                                        //            title: TextLabel.successTitle,
                                        //            msg: 'ลบข้อมูลเสร็จสมบูรณ์',
                                        //            //width: 300,
                                        //            buttons: Ext.MessageBox.OK,
                                        //            icon: Ext.MessageBox.INFO,
                                        //            fn: function (btn) {
                                        //            }
                                        //        });
                                        //    },
                                        //    failure: function (record, operation) {
                                        //        self.projectStore.load();
                                        //    }
                                        //});
                                    }
                                });
                            }
                        }]
                    }
                ],
                tbar: [{
                    cls: 'btn',
                    xtype: 'button',
                    iconCls: 'glyphicon glyphicon-plus',
                    text: "เพิ่มโครงการ / Add Project",
                    hidden: paramsView.isManagerRole,
                    handler: function (btn, evt) {
                        var projectSaveWindow = Ext.create('widget.projectSaveWindow', {
                            modal: true,
                            animateTarget: btn,
                            projectStore: projectStore,
                            customerstore: customerstore
                        });

                        projectSaveWindow.show();
                    }
                }]
                ,listeners: {
                    //'itemdblclick': function (grid, record, item, index, e, eOpts) {
                    //    //console.log("itemdblclick");
                    //    var projectForm = Ext.create('widget.projectWindow', {
                    //        projectData: record,
                    //        animateTarget: item,
                    //        modal: true
                    //    });
                    //    console.log(record);
                    //    projectForm.show();
                    //}
                },
                bbar: Ext.create('Ext.PagingToolbar', {
                    store: projectStore,
                    displayInfo: true,
                    displayMsg: 'Projects ที่กำลังแสดงอยู่ {0} - {1} of {2}',
                    emptyMsg: "ไม่มี Projects"
                })
            }
        ]
    });

    projectStore.load({ url: paramsView.urlReadProject });
    divisionStore.load({ url: paramsView.urlReadDivision });
});