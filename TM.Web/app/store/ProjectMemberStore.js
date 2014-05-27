Ext.define('TM.store.ProjectMemberStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.projectMemberStore',
    model: 'TM.model.ProjectMember',
    autoLoad: false,
    pageSize: 100,

    proxy: {
        type: 'ajax',
        url: paramsView.urlReadProjectMember,
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Project Member '
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