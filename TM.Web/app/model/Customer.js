Ext.define('TM.model.Customer', {
    extend: 'Ext.data.Model',
    xtype: 'customer',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'Name' },
        { name: 'ContactChannel' },
        { name: 'IndustryID', type: 'int' },
        { name: 'IndustryDisplay' },
        { name: 'ProjectCount', type: 'int' }
    ]
});