Ext.define('TM.store.CumulativeEffortByDateStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.cumulativeEffortByDateStore',
    model: 'TM.model.CumulativeItemByDate',
    autoLoad: false,

    proxy: {
        type: 'ajax',
        url: paramsView.urlGetCumulativeEffortByDate,
        extraParams: {
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล CumulativeItemByDate '
                        + response.status + ": "
                        + response.statusText,
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.ERROR
                });

                //window.location.href = paramsView.urlIndexPage;
            }
        }
    }
});