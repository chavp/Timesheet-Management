Ext.define('TM.store.CustomerStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.customerstore',
    model: 'TM.model.Customer',
    autoLoad: false,
    pageSize: 99999,

    proxy: {
        type: 'ajax',
        url: paramsView.urlGetCustomer,
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Customer'
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