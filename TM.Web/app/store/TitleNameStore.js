Ext.define('TM.store.TitleNameStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.titleNameStore',
    model: 'TM.model.TitleName',
    autoLoad: false,
    pageSize: 100,

    proxy: {
        type: 'ajax',
        url: paramsView.urlGetTitleName,
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Title Name '
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