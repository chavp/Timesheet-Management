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
    var projectStore = Ext.create('widget.projectStore');
    projectStore.load();
    var divisionStore = Ext.create('widget.divisionStore');
    divisionStore.load();

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

    Ext.create('Ext.panel.Panel', {
        layout: 'border',
        renderTo: 'projectPanel',
        width: 1150,
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
                bodyPadding: "0 150 0 150",
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
                        //align: 'center',
                        width: 30,
                        items: [{
                            // Use a URL in the icon config
                            xtype: 'button',
                            tooltip: 'แก้ไขผู้รับผิดชอบ / Edit Member',
                            iconCls: 'edit-project-memebr-icon',
                            handler: function (grid, rowIndex, colIndex, item, event, record, row) {
                                //var record = grid.getStore().getAt(rowIndex);
                                grid.getSelectionModel().select(record);

                                var projectForm = Ext.create('widget.editProjectMemberWindow', {
                                    projectData: record,
                                    animateTarget: row,
                                    modal: true
                                });
                                projectForm.show();
                            }
                        }]
                    },
                    { text: 'ID', dataIndex: 'ID', flex: 1, hidden: true },
                    { text: 'รหัสโครงการ<br/>Project Code', dataIndex: 'Code', flex: 1 },
                    { text: 'ชื่อโครงการ<br/>Project Name', dataIndex: 'Name', flex: 4 },
                    { text: 'วันที่เริ่มโครงการ<br/>Start Date', dataIndex: 'StartDate', flex: 1, renderer: Ext.util.Format.dateRenderer('d/m/Y') },
                    { text: 'วันที่ปิดโครงการ<br/>End Date', dataIndex: 'EndDate', flex: 1, renderer: Ext.util.Format.dateRenderer('d/m/Y') },
                    { text: 'สมาชิกในโครงการ<br/>Project Members', dataIndex: 'Members', flex: 1 }
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
});