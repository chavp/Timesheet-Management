Ext.define('TM.model.Position', {
    extend: 'Ext.data.Model',
    xtype: 'position',
    fields: [
        { name: 'ID' },
        { name: 'Display' },
        { name: 'Name' },
        { name: 'Order', type: 'int' },
        { name: 'PositionCost', type: 'float' },
        { name: 'EmployeeCount', type: 'int' },
        { name: 'TimesheetCount', type: 'int' }
    ]
});