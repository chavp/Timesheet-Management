Ext.define('TM.store.IndustryStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.industrystore',
    model: 'TM.model.Industry',
    autoLoad: false,
    pageSize: 100,

    proxy: {
        type: 'ajax',
        url: paramsView.urlReadIndustry,
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Industry '
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