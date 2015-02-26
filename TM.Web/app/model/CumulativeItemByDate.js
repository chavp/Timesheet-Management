Ext.define('TM.model.CumulativeItemByDate', {
    extend: 'Ext.data.Model',
    xtype: 'cumulativeItemByDate',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'Date' },
        { name: 'PreSale', type: 'float' },
        { name: 'ImplementAndDev', type: 'float' },
        { name: 'Warranty', type: 'float' },
        { name: 'Operation', type: 'float' }
    ]
});