Ext.define('TM.model.InitialMember', {
    extend: 'Ext.data.Model',
    xtype: 'initialMember',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'EmployeeID', type: 'int' },
        { name: 'FullName' },
        { name: 'ProjectRoleID', type: 'int' },
        { name: 'ProjectRoleName' }
    ]
});