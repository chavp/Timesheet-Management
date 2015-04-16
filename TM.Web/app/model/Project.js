Ext.define('TM.model.Project', {
    extend: 'Ext.data.Model',
    xtype: 'project',
    idProperty: 'ID',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'Code' },
        { name: 'Name' },
        { name: 'Display' },
        { name: 'StartDate', type: 'date', dateFormat: 'MS' },
        { name: 'EndDate', type: 'date', dateFormat: 'MS' },
        { name: 'TotalEffort', type: 'float' },
        { name: 'TotalCost', type: 'float' },
        { name: 'Members', type: 'int' },
        { name: 'StatusDisplay' },
        { name: 'StatusID', type: 'int' },
        { name: 'NameTH' },
        { name: 'NameEN' },
        { name: 'CustomerID', type: 'int' },

        { name: 'ContractStartDate', type: 'date', dateFormat: 'MS' },
        { name: 'ContractEndDate', type: 'date', dateFormat: 'MS' },
        { name: 'DeliverDate', type: 'date', dateFormat: 'MS' },
        { name: 'WarrantyStartDate', type: 'date', dateFormat: 'MS' },
        { name: 'WarrantyEndDate', type: 'date', dateFormat: 'MS' },

        { name: 'EstimateProjectValue', type: 'float' },
        { name: 'ProjectValue', type: 'float' },
        { name: 'Progress', type: 'int' },
        { name: 'StateOfProgress' },
        { name: 'LatestUpdateProgress', type: 'date', dateFormat: 'MS' },
        { name: 'TotalTimesheet', type: 'int' },
        { name: 'IsOwner', type: 'boolean' },
        { name: 'ProjectDeliveryPhaseCount', type: 'int' },
        { name: 'IsProjectAccept', type: 'boolean' },
        { name: 'TotalAcceptedProject', type: 'int' }
    ],
    associations: [
        { type: 'hasMany', model: 'ProjectDeliveryPhase', name: 'ProjectDeliveryPhases' }
    ],
    proxy: {
        type: 'rest',
        api: {
            read: document.urlProjectApi + '/ReadProject',
            create: document.urlProjectApi + '/SaveProject',
            update: document.urlProjectApi + '/UpdateProject',
            destroy: document.urlProjectApi + '/DeleteProject'
        },
        writer: {
            type: 'json',
            writeAllFields: true
        },
        listeners: {
            exception: function (proxy, response, operation) {
                var json = Ext.decode(response.responseText);
                if (json) {
                    Ext.MessageBox.show({
                        title: 'เกิดข้อผิดพลาดจากการดำเนินการ',
                        msg: json.message,
                        icon: Ext.MessageBox.ERROR,
                        buttons: Ext.Msg.OK
                    });
                } else {
                    Ext.MessageBox.show({
                        title: 'REMOTE EXCEPTION',
                        msg: operation.getError(),
                        icon: Ext.MessageBox.ERROR,
                        buttons: Ext.Msg.OK
                    });
                }
            }
        }
    }
});