Ext.define('TM.model.Timesheet', {
    extend: 'Ext.data.Model',
    xtype: 'timesheet',
    fields: [
        { name: 'ID' },
        { name: 'ProjectCode' },
        { name: 'ProjectName' },
        { name: 'StartDate', type: 'date', dateFormat: 'MS' },
        { name: 'StartDateText' },
        { name: 'Phase' },
        { name: 'TaskType' },
        { name: 'MainTaskDesc' },
        { name: 'SubTaskDesc' },
        { name: 'HourUsed', type: 'number' },
        { name: 'Remark' }
    ]
});