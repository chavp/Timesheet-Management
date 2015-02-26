Ext.define('TM.model.Phase', {
    extend: 'Ext.data.Model',
    xtype: 'phase',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'Name' },
        { name: 'Order', type: 'int' },
        { name: 'TimesheetCount', type: 'int' }
    ]
});