Ext.define('TM.model.Department', {
    extend: 'Ext.data.Model',
    xtype: 'department',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'DivisionID', type: 'int' },
        { name: 'Name' }
    ]
});