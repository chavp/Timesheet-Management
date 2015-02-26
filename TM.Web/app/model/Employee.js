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
        { name: 'LastChangedPassword', type: 'date', dateFormat: 'MS' },
        { name: 'Division' },
        { name: 'Department' },
        { name: 'Email' },
        { name: 'NameTH' },
        { name: 'LastTH' },
        { name: 'NameEN' },
        { name: 'LastEN' },
        { name: 'AppRole' },
        { name: 'Nickname' },

        { name: 'TitleID' },
        { name: 'PositionID' },
        { name: 'DivisionID' },
        { name: 'DepartmentID' },

        { name: 'StartDate', type: 'date', dateFormat: 'MS' },
        { name: 'EndDate', type: 'date', dateFormat: 'MS' },
        { name: 'StartDateText' },
        { name: 'EndDateText' },

        { name: 'Status' },

        { name: 'TotalProjectMember', type: 'int' },
        { name: 'TotalTimesheet', type: 'int' }

    ]
});