Ext.define('TM.model.Industry', {
    extend: 'Ext.data.Model',
    xtype: 'industry',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'Code' },
        { name: 'Display' },
        { name: 'Name' },
        { name: 'NameTH' }
    ]
});