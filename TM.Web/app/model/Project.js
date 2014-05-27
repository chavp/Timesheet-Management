Ext.define('TM.model.Project', {
    extend: 'Ext.data.Model',
    xtype: 'project',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'Code' },
        { name: 'Name' },
        { name: 'Display' },
        { name: 'StartDate', type: 'date', dateFormat: 'MS' },
        { name: 'EndDate', type: 'date', dateFormat: 'MS' },
        { name: 'Members', type: 'int' }
    ]
});