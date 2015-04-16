Ext.define('TM.model.ProjectDeliveryPhase', {
    extend: 'Ext.data.Model',
    xtype: 'projectdeliveryphase',
    idProperty: 'ID',
    fields: [
        { name: 'ID' },
        { name: 'ProjectID' },
        { name: 'DeliveryPhaseDate', type: 'date', dateFormat: 'MS' },
        { name: 'StatusOfProjectDeliveryPhase' }
    ]
});