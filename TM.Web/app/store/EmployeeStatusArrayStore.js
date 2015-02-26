Ext.define('TM.store.EmployeeStatusArrayStore', {
    extend: 'Ext.data.ArrayStore',
    xtype: 'employeestatusarraystore',
    model: 'TM.model.EmployeeStatus',
    data: [
        Ext.create('widget.employeestatus', { ID: 'Work', Name: 'ทำงานอยู่ / Work' }),
        Ext.create('widget.employeestatus', { ID: 'Resign', Name: 'ลาออก / Resign' })
    ]
});