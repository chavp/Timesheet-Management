﻿Ext.define('TM.store.ProjectProgressUpdateLogStore', {
    extend: 'Ext.data.Store',
    alias: 'widget.projectProgressUpdateLogStore',
    model: 'TM.model.ProjectProgressUpdateLog',
    autoLoad: false,
    pageSize: 100,

    proxy: {
        type: 'ajax',
        url: paramsView.urlFindProjectProgressUpdateLog,
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
                    msg: 'เกิดข้อผิดพลาดในการค้นข้อมูล Phase '
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