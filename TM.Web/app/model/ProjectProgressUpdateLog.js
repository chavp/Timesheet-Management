Ext.define('TM.model.ProjectProgressUpdateLog', {
    extend: 'Ext.data.Model',
    xtype: 'projectProgressUpdateLog',
    fields: [
        { name: 'ID' },
        { name: 'ProjectID', type: 'int' },
        { name: 'UpdatedDate', type: 'date', dateFormat: 'MS' },
        { name: 'UpdatedValue', type: 'int' }
    ]
});