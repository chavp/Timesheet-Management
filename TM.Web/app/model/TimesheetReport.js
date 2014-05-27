Ext.define('TM.model.TimesheetReport', {
    extend: 'Ext.data.Model',
    xtype: 'timesheetreport',
    fields: [
        { name: 'ID' },
        { name: 'Name' },
        { name: 'Type' },
        { name: 'Data' },
        { name: 'FromDate' },
        { name: 'ToDate' },
        { name: 'CheckAllTime', type: 'boolean' },
        { name: 'ProjectCode' },
        { name: 'EmployeeID' },
        { name: 'DepartmentID' }
    ]
});