Ext.define('TM.model.CumulativeEffortByWeek', {
    extend: 'Ext.data.Model',
    xtype: 'cumulativeEffortByWeek',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'WeekOfTheYear' },
        { name: 'PreSale', type: 'float' },
        { name: 'ImplementAndDev', type: 'float' },
        { name: 'Warranty', type: 'float' },
        { name: 'Operation', type: 'float' }
    ]
});