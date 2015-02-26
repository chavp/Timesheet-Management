Ext.onReady(function () {

    var timesheetReportStore = Ext.create('widget.timesheetreportStore', {
        autoLoad: true
    });

    var searchReportsItems = [];

    var isManagerOrOwner = function () {
        return (paramsView.isManagerRole || paramsView.isOwner === 'True');
    };

    var projectStore = Ext.create('widget.projectStore');
    projectStore.proxy.extraParams.includeAll = null;
    var cmbProjectCode = {
        xtype: 'combo',
        id: 'ProjectCode',
        name: 'ProjectCode',
        fieldLabel: 'โครงการ / Project',
        store: projectStore,
        emptyText: 'กรุณาระบุข้อมูล',
        padding: "0 0 0 0",
        //maxLength: 30,
        hideTrigger: false,
        displayField: 'Display',
        valueField: 'Code',
        pageSize: 50,
        minChars: 1,
        allowBlank: false,
        disabled: true,
        editable: true,
        forceSelection: true,
        listConfig: { itemTpl: highlightMatch.createItemTpl('Display', 'ProjectCode') }
    };

    var empStore = Ext.create('widget.employeeStore');
    var txtEmployeeID = {
        xtype: 'combo',
        id: 'EmployeeID',
        name: 'EmployeeID',
        fieldLabel: 'พนักงาน / Employee',
        store: empStore,
        emptyText: 'กรุณาระบุข้อมูล',
        padding: "0 200 0 0",
        //maxLength: 20,
        hideTrigger: false,
        displayField: 'Display',
        valueField: 'EmployeeID',
        pageSize: 50,
        minChars: 1,
        allowBlank: false,
        disabled: true,
        editable: true,
        forceSelection: true,
        listConfig: { itemTpl: highlightMatch.createItemTpl('Display', 'EmployeeID') }
    };

    //cmbProjectCode.isVisible(false);
    //cmbProjectCode.setMargin("0 300 0 0");
    //txtEmployeeID.isVisible(false);

    searchReportsItems.push({
        xtype: 'combo',
        id: 'Name',
        name: 'Name',
        padding: "0 300 0 0",
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
                Ext.getCmp('ProjectCode').reset();
                Ext.getCmp('EmployeeID').reset();
                Ext.getCmp('ProjectCode').setDisabled(true);
                Ext.getCmp('EmployeeID').setDisabled(true);
                empStore.proxy.extraParams.isOwner = false;
                projectStore.proxy.extraParams.isOwner = false;
                if (isManagerOrOwner()) {
                    if (newValue === 1) { // Actual Effort for Person
                        if (paramsView.isManagerRole) {
                            Ext.getCmp('ProjectCode').setDisabled(true);
                            Ext.getCmp('EmployeeID').setDisabled(false);
                        }
                        empStore.load({
                            url: paramsView.urlReadEmployee
                        });
                    } else if (newValue === 2) { // Actual Effort for Department
                        Ext.getCmp('ProjectCode').setDisabled(false);
                        Ext.getCmp('EmployeeID').setDisabled(true);
                        projectStore.proxy.extraParams.isForDepartment = true;
                        projectStore.load({
                            url: paramsView.urlReadProject,
                            callback: function (records, operation, success) {
                                if (success) {
                                    Ext.getCmp('ProjectCode').setValue("");
                                }
                            }
                        });
                    } else if (newValue === 3) { // Actual Cost for Person
                        Ext.getCmp('ProjectCode').setDisabled(true);
                        Ext.getCmp('EmployeeID').setDisabled(false);
                        empStore.proxy.extraParams.isOwner = true;
                        empStore.load({
                            url: paramsView.urlReadEmployee
                        });
                    } else if (newValue === 4) { // Actual Cost for Project
                        Ext.getCmp('ProjectCode').setDisabled(false);
                        Ext.getCmp('EmployeeID').setDisabled(true);
                        projectStore.proxy.extraParams.isOwner = true;
                        projectStore.load({
                            url: paramsView.urlReadProject
                        });
                    }
                    else if (newValue === 5) { // Actual Cost for All Person

                    }
                    else if (newValue === 6) { // Timesheet Data Recording

                    }
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
        fieldLabel: 'ช่วงวันที่ / Date (วันแรก - วันนี้ ของเดือนปัจจุบัน)',
        name: 'dateBetween',
        layout: 'hbox',
        defaults: {
            allowBlank: false
        },
        items: [{
            xtype: 'datefield',
            id: 'fromStartTimesheet',
            name: 'fromStartTimesheet',
            anchor: '100%',
            fieldLabel: '',
            width: 100,
            format: 'd/m/Y',
            editable: false,
            vtype: 'daterange',
            endDateField: 'toStartTimesheet'
        }, {
            xtype: 'displayfield',
            margin: '0 5 0 5',
            value: '-'
        }, {
            xtype: 'datefield',
            id: 'toStartTimesheet',
            name: 'toStartTimesheet',
            anchor: '100%',
            fieldLabel: '',
            width: 100,
            format: 'd/m/Y',
            padding: "0 250 0 0",
            editable: false,
            vtype: 'daterange',
            startDateField: 'fromStartTimesheet'
        }]
    });

    // set Default
    var setDefault = function () {
        Ext.getCmp('fromStartTimesheet').setValue(paramsView.firstDayOfMonth);
        Ext.getCmp('toStartTimesheet').setValue(paramsView.lastDayOfMonth);

        Ext.getCmp('fromStartTimesheet').setMinValue(paramsView.minDateText);
        Ext.getCmp('toStartTimesheet').setMaxValue(paramsView.maxDateText);
    }

    if (isManagerOrOwner()) {
        searchReportsItems.push(cmbProjectCode);
        searchReportsItems.push(txtEmployeeID);
    }

    var searchReportsFieldset = {
        xtype: 'fieldset',
        title: '<h5>' + TextLabel.searchCriterionLabel + '</h5>',
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

    Ext.create('Ext.panel.Panel', {
        layout: {
            type: 'hbox',
            pack: 'start',
            align: 'stretch'
        },
        renderTo: 'searchReportsPanel',
        //height: WindowHeight.height,
        width: 'auto',
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
                bodyPadding: "0 20 0 20",
                collapsible: false,
                items: [searchReportsFieldset],
                buttonAlign: 'center',
                border: 0,
                buttons: [
                    {
                        text: TextLabel.cmdExportText,
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
                            //console.log(values);

                            var timesheetreport = Ext.create('widget.timesheetreport', {
                                Name: values.Name,
                                Type: values.Type,
                                Data: values.Data,
                                FromDate: values.fromStartTimesheet,
                                ToDate: values.toStartTimesheet,
                                ProjectCode: values.ProjectCode,
                                EmployeeID: values.EmployeeID
                            });

                            //console.log(timesheetreport.data);
                            Ext.MessageBox.wait("กำลังสร้าง Report...", 'กรุณารอ');
                            Ext.Ajax.request({
                                url: paramsView.urlExportReport,
                                timeout: 120000,
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
                        text: TextLabel.cmdClearText,
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

    //timesheetReportStore.load({
    //    url: paramsView.urlGetReport
    //});

    setDefault();
});