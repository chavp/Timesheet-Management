Ext.define('TM.model.ProjectMember', {
    extend: 'Ext.data.Model',
    xtype: 'projectMember',
    fields: [
        { name: 'ID' },
        { name: 'EmployeeID' },
        { name: 'FullName' },
        { name: 'Position' },
        { name: 'ProjectRoleID' },
        { name: 'ProjectRoleName' },
        { name: 'ProjectID' },
        { name: 'CanEditProjectRole', type: 'boolean' },
        { name: 'CanRemove', type: 'boolean' }
    ]
});