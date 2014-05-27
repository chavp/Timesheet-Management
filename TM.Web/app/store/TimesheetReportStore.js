Ext.define('TM.store.TimesheetReportStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.timesheetreportStore',
    model: 'TM.model.TimesheetReport',
    autoLoad: false,
    pageSize: 10,

    proxy: {
        type: 'ajax',
        url: paramsView.urlGetReport,
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Timesheet Report '
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