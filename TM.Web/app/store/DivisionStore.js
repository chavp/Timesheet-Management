Ext.define('TM.store.DivisionStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.divisionStore',
    model: 'TM.model.Division',
    autoLoad: false,
    pageSize: 100,

    proxy: {
        type: 'ajax',
        url: paramsView.urlReadDivision,
        extraParams: {
            includeAll: "All"
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Division '
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