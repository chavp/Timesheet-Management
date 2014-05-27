Ext.onReady(function () {
    var searchReportsItems = [];

    var departmentStore = Ext.create('widget.departmentStore');
    var cmbDepartment = {
        xtype: 'combo',
        id: 'Department',
        name: 'Department',
        fieldLabel: 'ฝ่าย - แผนก / Division - Department',
        store: departmentStore,
        emptyText: 'กรุณาระบุข้อมูล',
        padding: "0 100 0 0",
        //maxLength: 30,
        hideTrigger: false,
        displayField: 'Name',
        valueField: 'ID',
        //pageSize: 10,
        minChars: 1,
        allowBlank: false,
        disabled: true,
        editable: true,
        forceSelection: true
    };

    var projectStore = Ext.create('widget.projectStore');
    var cmbProjectCode = {
        xtype: 'combo',
        id: 'ProjectCode',
        name: 'ProjectCode',
        fieldLabel: 'โครงการ / Project',
        store: projectStore,
        emptyText: 'กรุณาระบุข้อมูล',
        padding: "0 100 0 0",
        //maxLength: 30,
        hideTrigger: false,
        displayField: 'Display',
        valueField: 'Code',
        pageSize: 10,
        minChars: 1,
        allowBlank: false,
        disabled: true,
        editable: true,
        forceSelection: true
    };

    var empStore = Ext.create('widget.employeeStore');
    var cmbEmployeeID = {
        xtype: 'combo',
        id: 'EmployeeID',
        name: 'EmployeeID',
        fieldLabel: 'พนักงาน / Employee',
        store: empStore,
        emptyText: 'กรุณาระบุข้อมูล',
        padding: "0 100 0 0",
        //maxLength: 20,
        hideTrigger: false,
        displayField: 'Display',
        valueField: 'EmployeeID',
        pageSize: 10,
        minChars: 1,
        allowBlank: false,
        disabled: true,
        editable: true,
        forceSelection: true
    };

    var timesheetReportStore = Ext.create('widget.timesheetreportStore');
    timesheetReportStore.load();

    var searchReportsFieldset = {
        xtype: 'fieldset',
        title: '<h5>เงื่อนไขในการค้นหา / Criterion</h5>',
        border: false,
        defaultType: 'field',
        defaults: {
            labelWidth: 300,
            anchor: '-20 -20',
            allowBlank: false,
            labelAlign: 'right'
        },
        bodyPadding: 10,
        items: searchReportsItems
    };

    searchReportsItems.push({
        xtype: 'combo',
        id: 'Name',
        name: 'Name',
        padding: "0 180 0 0",
        store: timesheetReportStore,
        fieldLabel: 'ชื่อรายงาน / Report Name',
        displayField: 'Name',
        valueField: 'ID',
        forceSelection: true,
        triggerAction: 'all',
        queryMode: 'local',
        allowBlank: false,
        editable: false,
        emptyText: "กรุณาเลือก",
        listeners: {
            change: function (cmb, newValue, oldValue, eOpts) {
                var cmpDepartment = Ext.getCmp('Department');
                var cmpProjectCode = Ext.getCmp('ProjectCode');
                var cmpEmployeeID = Ext.getCmp('EmployeeID');

                cmpDepartment.reset();
                cmpProjectCode.reset();
                cmpEmployeeID.reset();

                var cmpCheckAllTime = Ext.getCmp('CheckAllTime');
                if (cmpCheckAllTime) {
                    cmpCheckAllTime.reset();
                    cmpCheckAllTime.setDisabled(true);
                }

                cmpDepartment.setDisabled(true);
                cmpProjectCode.setDisabled(true);
                cmpEmployeeID.setDisabled(true);

                if (newValue === 1) { // Actual Cost for Person
                    cmpEmployeeID.setDisabled(false);
                } else if (newValue === 2) { // Actual Cost for Department
                    cmpDepartment.setDisabled(false);
                } else if (newValue === 3) { // Actual Cost for Project
                    cmpProjectCode.setDisabled(false);
                } else if (newValue === 4) { // Actual Cost for All Project
                    if (cmpCheckAllTime) {
                        cmpCheckAllTime.setDisabled(false);
                    }
                } else if (newValue === 5) { // Actual Effort for Person
                    cmpEmployeeID.setDisabled(false);
                } else if (newValue === 6) { // Actual Effort for Department
                    cmpDepartment.setDisabled(false);
                }
            }
        }
    });

    searchReportsItems.push({
        xtype: 'radiogroup',
        fieldLabel: 'รูปแบบรายงาน / Report Type',
        anchor: '100%',
        columns: 4,
        items: [
                {
                    boxLabel: 'Excel',
                    name: 'Type',
                    inputValue: 1,
                    checked: true
                },
                {
                    boxLabel: 'PDF',
                    name: 'Type',
                    inputValue: 2,
                    disabled: true
                }
        ]
    });

    searchReportsItems.push({
        xtype: 'radiogroup',
        fieldLabel: 'ข้อมูลรายงาน / Report Data',
        anchor: '100%',
        columns: 4,
        items: [
                {
                    boxLabel: 'ทั้งหมด',
                    name: 'Data',
                    inputValue: 1,
                    checked: true
                },
                {
                    boxLabel: 'Summary',
                    name: 'Data',
                    inputValue: 2,
                    disabled: true
                },
                {
                    boxLabel: 'Detail',
                    name: 'Data',
                    inputValue: 3,
                    disabled: true
                }
        ]
    });

    searchReportsItems.push({
        xtype: 'fieldcontainer',
        fieldLabel: 'ช่วงวันที่ / Date (วันแรก - วันสุดท้าย ของเดือนปัจจุบัน)',
        name: 'dateBetween',
        layout: 'hbox',
        defaults: {
            allowBlank: false
        },
        items: [{
            xtype: 'datefield',
            id: 'fromStartTimesheet',
            name: 'fromStartTimesheet',
            //anchor: '100%',
            fieldLabel: '',
            flex: 1,
            format: 'd/m/Y',
            editable: false,
            vtype: 'daterange',
            endDateField: 'toStartTimesheet'
        }, {
            xtype: 'datefield',
            id: 'toStartTimesheet',
            name: 'toStartTimesheet',
            //anchor: '100%',
            fieldLabel: '',
            flex: 1,
            format: 'd/m/Y',
            padding: "0 100 0 0",
            editable: false,
            vtype: 'daterange',
            startDateField: 'fromStartTimesheet'
        }]
    });

    if (paramsView.canCheckAllTime) {
        searchReportsItems[3].items.push({
            xtype: 'checkbox',
            id: 'CheckAllTime',
            name: 'CheckAllTime',
            fieldLabel: '',
            padding: "0 100 0 0",
            labelWidth: 0,
            labelAlign: 'right',
            disabled: true,
            boxLabel: 'ทั้งหมด / All',
            inputValue: true,
            listeners: {
                change: function (el, newValue, oldValue, eOpts) {
                    Ext.getCmp('fromStartTimesheet').setDisabled(false);
                    Ext.getCmp('toStartTimesheet').setDisabled(false);
                    if (newValue) {
                        Ext.getCmp('fromStartTimesheet').setDisabled(true);
                        Ext.getCmp('toStartTimesheet').setDisabled(true);
                    }
                }
            }
        });
    }

    // set Default
    var setDefault = function () {
        Ext.getCmp('fromStartTimesheet').setValue(paramsView.firstDayOfMonth);
        Ext.getCmp('toStartTimesheet').setValue(paramsView.lastDayOfMonth);
    }

    searchReportsItems.push(cmbDepartment);
    searchReportsItems.push(cmbProjectCode);
    searchReportsItems.push(cmbEmployeeID);

    Ext.create('Ext.panel.Panel', {
        layout: {
            type: 'hbox',
            pack: 'start',
            align: 'stretch'
        },
        renderTo: 'searchReportsPanel',
        //height: 230,
        width: 1150,
        border: 1,
        defaults: {
            frame: false,
            split: false
        },
        items: [
                {
                    xtype: 'form',
                    id: 'searchReportsForm',
                    width: '100%',
                    title: '',
                    region: 'center',
                    //height: 160,
                    bodyPadding: "0 150 0 150",
                    collapsible: false,
                    items: [searchReportsFieldset],
                    buttonAlign: 'center',
                    border: 0,
                    buttons: [
                                    {
                                        text: '<i class="glyphicon glyphicon-print"></i> ส่งออกข้อมูล / Export',
                                        handler: function (btn) {
                                            var form = Ext.getCmp('searchReportsForm');

                                            if (!form.isValid()) {
                                                Ext.MessageBox.show({
                                                    title: messagesForm.validationTitle,
                                                    msg: messagesForm.validationWarning,
                                                    buttons: Ext.MessageBox.OK,
                                                    icon: Ext.MessageBox.WARNING
                                                });
                                                return false;
                                            }

                                            var values = form.getValues();
                                            console.log(values);

                                            var timesheetreport = Ext.create('widget.timesheetreport', {
                                                Name: values.Name,
                                                Type: values.Type,
                                                Data: values.Data,
                                                FromDate: values.fromStartTimesheet,
                                                ToDate: values.toStartTimesheet,
                                                CheckAllTime: values.CheckAllTime,
                                                ProjectCode: values.ProjectCode,
                                                EmployeeID: values.EmployeeID,
                                                DepartmentID: values.Department
                                            });

                                            //console.log(timesheetreport.data);
                                            Ext.MessageBox.wait("กำลังสร้าง Report...", 'กรุณารอ');
                                            Ext.Ajax.request({
                                                url: paramsView.urlExportReport,
                                                jsonData: timesheetreport.data,
                                                failure: function (xhr) {
                                                    //alert('failed  !');
                                                    Ext.MessageBox.hide();

                                                    Ext.MessageBox.show({
                                                        title: messagesForm.errorAlertTitle,
                                                        msg: xhr.responseText,
                                                        //width: 300,
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.ERROR
                                                    });
                                                },
                                                success: function (xhr) {
                                                    //alert('success!');
                                                    Ext.MessageBox.hide();
                                                    var response = Ext.decode(xhr.responseText);
                                                    if (response.success) {
                                                        Ext.MessageBox.show({
                                                            title: messagesForm.successTitle,
                                                            msg: 'Export Success',
                                                            //width: 300,
                                                            buttons: Ext.MessageBox.OK,
                                                            icon: Ext.MessageBox.INFO,
                                                            fn: function (btn) {
                                                                window.location = response.exportUrl;
                                                            }
                                                        });
                                                    } else {
                                                        Ext.MessageBox.show({
                                                            title: messagesForm.errorAlertTitle,
                                                            msg: response.message,
                                                            //width: 300,
                                                            buttons: Ext.MessageBox.OK,
                                                            icon: Ext.MessageBox.ERROR
                                                        });
                                                    }

                                                },
                                            });
                                        }
                                    }, {
                                        text: '<i class="glyphicon glyphicon-trash"></i> ล้างข้อมูล / Clear',
                                        handler: function (btn) {
                                            var form = Ext.getCmp('searchReportsForm').getForm();
                                            form.reset();
                                            setDefault();
                                        }
                                    }
                    ]
                }
        ]
    });

    setDefault();
});