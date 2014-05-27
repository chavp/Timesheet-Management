Ext.define('TM.store.ProjectStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.projectStore',
    model: 'TM.model.Project',
    autoLoad: false,
    pageSize: 100,

    proxy: {
        type: 'ajax',
        url: paramsView.urlReadProject,
        extraParams: {
            includeAll: "All",
            projectCode: "",
            projectName: "",
            fromDateText: "",
            toDateText: "",
            isOwner: false,
            isForDepartment: false
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Project '
                        + response.status + ": "
                        + response.statusText,
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.ERROR
                });
                
                window.location.href = paramsView.urlIndexPage;
            }
        }
    }
});