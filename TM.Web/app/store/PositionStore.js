Ext.define('TM.store.PositionStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.positionStore',
    model: 'TM.model.Position',
    autoLoad: false,
    pageSize: 1000,

    proxy: {
        type: 'ajax',
        url: paramsView.urlGetPosition,
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Position '
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