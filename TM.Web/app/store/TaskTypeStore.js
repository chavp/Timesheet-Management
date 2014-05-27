Ext.define('TM.store.TaskTypeStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.tasktypeStore',
    model: 'TM.model.TaskType',
    autoLoad: false,
    pageSize: 100,

    proxy: {
        type: 'ajax',
        url: paramsView.urlGetAllTaskType,
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Task Type '
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