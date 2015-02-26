Ext.define('TM.model.EmployeeStatus', {
    extend: 'Ext.data.Model',
    xtype: 'employeestatus',
    idProperty: 'ID',
    fields: [
        { name: 'ID' },
        { name: 'Name', type: 'string' }
    ]
});