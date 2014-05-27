Ext.define('TM.view.UserWindow', {
    extend: 'Ext.window.Window',
    xtype: 'userWindow',
    width: 1000,
    title: 'ข้อมูลผู้ใช้ / User Profile',
    resizable: false,
    closable: false,

    config: {
    },

    initComponent: function () {
        var self = this;

        this.items = [{
            xtype: 'panel',
            layout: {
                type: 'hbox',
                pack: 'start',
                align: 'stretch'
            }
        }];

    }
});