Ext.define('TM.store.TimesheetStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.timesheetStore',
    model: 'TM.model.Timesheet',
    autoLoad: false,
    pageSize: 100,

    proxy: {
        type: 'rest',
        url: paramsView.urlGetTimesheet,
        extraParams: {
            projectID: -1,
            fromDateText: null,
            toDateText: null
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Timesheet '
                        + response.status  + ": " 
                        + response.statusText,
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.ERROR
                });
                
                //window.location.href = paramsView.urlIndexPage;
            }
        }
    }
});