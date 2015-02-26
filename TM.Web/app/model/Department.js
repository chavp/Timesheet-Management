Ext.define('TM.model.Department', {
    extend: 'Ext.data.Model',
    xtype: 'department',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'Name' },
        { name: 'DivisionID', type: 'int' },
        { name: 'DivisionName' },
        { name: 'UnderEmployees', type: 'int' },
        { name: 'TimesheetCount', type: 'int' }
    ]
});