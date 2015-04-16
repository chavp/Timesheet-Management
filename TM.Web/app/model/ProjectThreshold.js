Ext.define('TM.model.ProjectThreshold', {
    extend: 'Ext.data.Model',
    xtype: 'projectthreshold',
    fields: [
        { name: 'ID' },
        { name: 'Name' },
        { name: 'LimitRatio', type: 'int' }
    ]
});