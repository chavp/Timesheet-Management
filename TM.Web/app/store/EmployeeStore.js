Ext.define('TM.store.EmployeeStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.employeeStore',
    model: 'TM.model.Employee',
    autoLoad: false,
    pageSize: 15,

    proxy: {
        type: 'ajax',
        url: paramsView.urlReadEmployee,
        extraParams: {
            divisionID: -1,
            departmentID: -1,
            isOwner: false,
            employeeID: null,
            employeeFirstName: null,
            employeeLastName: null,
            withoutEmpIDList: [],
            projectCode: null
        },
        timeout: 120000,
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success'
        },
        listeners: {
            exception: function (proxy, response, options) {
                Ext.MessageBox.show({
                    title: messagesForm.errorAlertTitle,
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Employee '
                        + response.status + ": "
                        + response.statusText,
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.ERROR
                });
                
                window.location.href = paramsView.urlIndexPage;
            }
        }
    },

    initComponent: function () {
        var self = this;

        this.callParent();
    },

    reset: function () {
        this.currentPage = 1;
        this.proxy.extraParams.divisionID = -1;
        this.proxy.extraParams.departmentID = -1;
        this.proxy.extraParams.isOwner = false;
        this.proxy.extraParams.employeeID = null;
        this.proxy.extraParams.employeeFirstName = null;
        this.proxy.extraParams.employeeLastName = null;
        this.proxy.extraParams.withoutEmpIDList = [];
    }
});