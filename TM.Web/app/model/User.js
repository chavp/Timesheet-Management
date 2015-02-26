Ext.define('TM.model.User', {
    extend: 'Ext.data.Model',
    xtype: 'user',
    fields: [
        { name: 'ID' },
        { name: 'EmployeeID' },
        { name: 'Title' },
        { name: 'FullName' },
        { name: 'Position' },
        { name: 'Division' },
        { name: 'Department' },
        { name: 'Email' },
        { name: 'OldPassword' },
        { name: 'NewPassword' },
        { name: 'NameTH' },
        { name: 'LastTH' },
        { name: 'NameEN' },
        { name: 'LastEN' },
        { name: 'AppRole' }
    ]
});