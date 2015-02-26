Ext.onReady(function () {

    var employeeStore = Ext.create('widget.employeeStore', {
        pageSize: 15
    });

    employeeStore.load({
        url: paramsView.urlReadEmployee
    });

    var onResetPasswordClick = function (grid, rowIndex, colIndex, item, event, record, row) {
        var employeeID = record.get('EmployeeID');
        var name = record.get('FullName');

        var dispaly = '<br/><strong>รหัส / Emp ID:</strong> ' + employeeID;
        dispaly += "<br/><strong>ชื่อ-นามสกุล / Full Name:</strong> " + name;

        Ext.MessageBox.show({
            title: 'ยืนยัน / Confirm',
            msg: 'คุณต้องการคืนค่าเริ่มต้นรหัสเข้าระบบผู้ใช้งาน ใช่ หรือ ไม่?<br/>' + dispaly,
            buttons: Ext.MessageBox.YESNO,
            icon: Ext.MessageBox.QUESTION,
            animateTarget: row,
            fn: function (btn) {
                if (btn === "yes") {
                    Ext.MessageBox.wait("กำลังคืนค่าเริ่มต้นรหัสเข้าระบบผู้ใช้งาน...", 'กรุณารอ / Please wait');
                    Ext.Ajax.request({
                        url: paramsView.urlResetPassword,
                        success: function (transport) {
                            Ext.MessageBox.hide();
                            var respose = Ext.decode(transport.responseText);
                            if (respose.success) {
                                Ext.MessageBox.show({
                                    title: messagesForm.successTitle,
                                    msg: respose.message,
                                    //width: 300,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO,
                                    fn: function (btn) {
                                        if (respose.redirectToIndex) {
                                            window.location.href = paramsView.urlIndexPage;
                                        } else {
                                            employeeStore.load();
                                        }
                                    }
                                });
                            } else {
                                Ext.MessageBox.show({
                                    title: messagesForm.errorAlertTitle,
                                    msg: 'เกิดข้อผิดพลาดในการคืนค่าเริ่มต้นรหัสเข้าระบบผู้ใช้งาน<br/>' + respose.message,
                                    //width: 300,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                            }
                        },   // function called on success
                        failure: function (transport) {
                            Ext.MessageBox.hide();
                            Ext.MessageBox.show({
                                title: messagesForm.errorAlertTitle,
                                msg: "เกิดข้อผิดพลาดในขั้นตอนคืนค่าเริ่มต้นรหัสผ่านผู้ใช้งาน<br/>" + transport.responseText,
                                //width: 300,
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                        },
                        jsonData: { empoyeeID: employeeID }  // your json data
                    });
                } else {

                }
            }
        });
    }

    Ext.create('Ext.panel.Panel', {
        layout: 'border',
        renderTo: 'employeesPanel',
        height: 560,
        border: 1,
        defaults: {
            frame: false,
            split: false
        },
        items: [{
            xtype: 'panel',
            id: 'searchEmpForm',
            title: '',
            region: 'north',
            items: [{
                width: 400,
                fieldLabel: 'ค้นหา / Search',
                labelWidth: 100,
                xtype: 'searchfield',
                margin: 10,
                emptyText: 'รหัส / ชื่อ-นามสกุล ',
                store: employeeStore
            }]
        }, {
            xtype: 'grid',
            region: 'center',
            store: employeeStore,
            renderTo: 'employeesPanel',
            columns: [
                {
                    xtype: 'rownumberer',
                    width: 30,
                    sortable: false,
                    locked: true
                },
                {
                    xtype: 'actioncolumn',
                    width: 30,
                    sortable: false,
                    menuDisabled: true,
                    items: [{
                        iconCls: 'reset-password-icon',
                        tooltip: 'Reset Password',
                        scope: this,
                        handler: onResetPasswordClick
                    }]
                },
                { text: 'ID', dataIndex: 'ID', hidden: true, sortable: false },
                { text: 'รหัส<br/>Emp ID', dataIndex: 'EmployeeID', sortable: true, width: 100 },
                { text: 'ชื่อ-นามสกุล<br/>Full Name', dataIndex: 'FullName', sortable: true, width: 200 },
                { text: 'ตำแหน่ง<br/>Position', dataIndex: 'Position', sortable: true, flex: 1 },
                { text: 'เข้าระบบครั้งล่าสุด<br/>Last Login', dataIndex: 'LastLoginDate', sortable: true, width: 170, renderer: Ext.util.Format.dateRenderer('d/m/Y H:i:s') },
                { text: 'เปลี่ยนรหัสผ่านครั้งล่าสุด<br/>Last Changed Password', dataIndex: 'LastChangedPassword', sortable: true, width: 170, renderer: Ext.util.Format.dateRenderer('d/m/Y H:i:s') }
            ],
            // paging bar on the bottom
            bbar: Ext.create('Ext.PagingToolbar', {
                store: employeeStore,
                displayInfo: true,
                displayMsg: 'Employee ที่กำลังแสดงอยู่ {0} - {1} of {2}',
                emptyMsg: "ไม่มี Employee"
            })
        }]
    });
});