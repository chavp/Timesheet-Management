Ext.define('TM.store.CumulativeEffortByWeekStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.cumulativeEffortByWeekStore',
    model: 'TM.model.CumulativeEffortByWeek',
    autoLoad: false,
    
    proxy: {
        type: 'ajax',
        url: paramsView.urlGetCumulativeEffortByWeek,
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล CumulativeEffortByWeek '
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