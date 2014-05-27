Ext.define('TM.model.User', {
    extend: 'Ext.data.Model',
    xtype: 'user',
    fields: [
        { name: 'ID' },
        { name: 'EmployeeID' },
        { name: 'FullName' },
        { name: 'Position' },
        { name: 'Division' },
        { name: 'Department' },
        { name: 'EMail' },
        { name: 'OldPassword' },
        { name: 'NewPassword' }
    ]
});