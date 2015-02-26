Ext.define('TM.model.ProjectRole', {
    extend: 'Ext.data.Model',
    xtype: 'projectRole',
    fields: [
        { name: 'ProjectRoleID', type: 'int' },
        { name: 'ProjectRoleName' },
        { name: 'Order', type: 'int' },
        { name: 'ProjectRoleCost', type: 'int' },
        { name: 'ProjectMemberCount', type: 'int' },
        { name: 'TimesheetCount', type: 'int' }
    ]
});