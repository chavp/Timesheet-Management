function actionCursorRenderer(val, metadata, record) {
    metadata.style = 'cursor: pointer;'
    //console.log(val);
    return val;
}

function addSelectAllDepartmentModel(store) {
    store.add(Ext.create('widget.department', {
        ID: -1,
        DivisionID: -1,
        Name: "ทั้งหมด",
    }));
}

Ext.onReady(function () {

    var projectStore = Ext.create('widget.projectStore', {
        pageSize: 10
    });
    var divisionStore = Ext.create('widget.divisionStore');

    var departmentStore = Ext.create('widget.departmentStore');
    //add All department
    addSelectAllDepartmentModel(departmentStore);

    var searchProjectFieldset = {
        xtype: 'fieldset',
        title: '<h5>เงื่อนไขในการค้นหา / Criterion</h5>',
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
                fieldLabel: 'โครงการ / Project',
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
                    emptyText: 'รหัส / Code',
                    flex: 1,
                    maxLength: 30,
                    listeners: {
                        change: function (obj, newValue) {
                            //console.log(newValue);
                            obj.setRawValue(newValue.toUpperCase());
                        }
                    }
                }, {
                    xtype: 'textfield',
                    id: 'ProjectName',
                    name: 'ProjectName',
                    fieldLabel: '',
                    emptyText: 'ชื่อ / Name',
                    maxLength: 255,
                    flex: 3
                }]
            }
        ]
    };

    var customerstore = Ext.create("widget.customerstore");
    customerstore.load({
        pageSize: 9999,
        url: paramsView.urlGetCustomer
    });

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

    var projectdeliveryphasestore = Ext.create('widget.projectdeliveryphasestore');

    Ext.create('Ext.panel.Panel', {
        layout: 'border',
        renderTo: 'projectPanel',
        width: 'auto',
        height: 540,
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
                        text: '<i class="glyphicon glyphicon-search"></i> ค้นหา / Search',
                        handler: function (btn) {
                            searchProject();
                        }
                    }, {
                        text: '<i class="glyphicon glyphicon-trash"></i> ล้างข้อมูล / Clear',
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
                            tooltip: 'แก้ไขผู้รับผิดชอบ / Edit Member',
                            iconCls: 'edit-project-memebr-icon',
                            handler: function (grid, rowIndex, colIndex, item, event, record, row) {
                                grid.getSelectionModel().select(record);
                                var projectForm = Ext.create('widget.editProjectMemberWindow', {
                                    iconCls: 'edit-project-memebr-icon',
                                    projectData: record,
                                    animateTarget: row,
                                    modal: true
                                });
                                projectForm.show();
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
                                    projectdeliveryphasestore: projectdeliveryphasestore,
                                    modal: true
                                });

                                editForm.setValues(record);
                                editForm.show();
                            }
                        },
                        {
                            xtype: 'button',
                            tooltip: 'Cumulative Cost Flow Diagram',
                            iconCls: 'chart-line-project-member-icon',
                            isDisabled: function (view, rowIndex, colIndex, item, record) {
                                var totalTimesheet = record.get('TotalTimesheet');
                                if (totalTimesheet > 0) return false;
                                return true;
                            },
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
                            isDisabled: function (view, rowIndex, colIndex, item, record) {
                                var totalTimesheet = record.get('TotalTimesheet');
                                if (totalTimesheet > 0) return false;
                                return true;
                            },
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
                    { text: 'ID', dataIndex: 'ID', flex: 1, sortable: false, hidden: true },
                    { text: 'รหัสโครงการ<br/>Project Code', dataIndex: 'Code', sortable: true, flex: 1 },
                    {
                        text: 'ชื่อโครงการ<br/>Project Name', dataIndex: 'Name', sortable: true, flex: 3,
                        renderer: function (value, metaData, record, rowIdx, colIdx, store) {
                            // Sample value: msimms & Co. "like" putting <code> tags around your code

                            value = Ext.String.htmlEncode(value);

                            // "double-encode" before adding it as a data-qtip attribute
                            metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';

                            return value;
                        }
                    },
                    { text: 'วันที่เริ่มโครงการ<br/>Start Date', dataIndex: 'StartDate', sortable: true, flex: 1, renderer: Ext.util.Format.dateRenderer('d/m/Y') },
                    { text: 'วันที่ปิดโครงการ<br/>End Date', dataIndex: 'EndDate', flex: 1, sortable: true, renderer: Ext.util.Format.dateRenderer('d/m/Y') },
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
                    }
                ],
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