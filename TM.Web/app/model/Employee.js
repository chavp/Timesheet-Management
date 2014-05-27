Ext.define('TM.model.Employee', {
    extend: 'Ext.data.Model',
    xtype: 'employee',
    fields: [
        { name: 'ID' },
        { name: 'EmployeeID' },
        { name: 'FullName' },
        { name: 'Display' },
        { name: 'Position' },
        { name: 'LastLoginDate', type: 'date', dateFormat: 'MS' },
        { name: 'LastChangedPassword', type: 'date', dateFormat: 'MS' }
    ]
});