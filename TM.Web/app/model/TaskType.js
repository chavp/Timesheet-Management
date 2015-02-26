Ext.define('TM.model.TaskType', {
    extend: 'Ext.data.Model',
    xtype: 'tasktype',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'Name' },
        { name: 'Order', type: 'int' },
        { name: 'TimesheetCount', type: 'int' }
    ]
});