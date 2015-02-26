Ext.define('TM.store.InitialMemberStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.initialMemberStore',
    model: 'TM.model.InitialMember',
    autoLoad: false,
    pageSize: 500,

    proxy: {
        type: 'ajax',
        url: paramsView.urlGetAllInitialMember,
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Initial Member '
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