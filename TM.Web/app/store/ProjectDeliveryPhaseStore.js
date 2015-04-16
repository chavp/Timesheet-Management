Ext.define('TM.store.ProjectDeliveryPhaseStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.projectdeliveryphasestore',
    model: 'TM.model.ProjectDeliveryPhase',
    autoLoad: false,
    pageSize: 999,

    proxy: {
        type: 'ajax',
        url: paramsView.urlGetProjectDeliveryPhases,
        extraParams: {
            projectID: null
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Project Delivery Phase '
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