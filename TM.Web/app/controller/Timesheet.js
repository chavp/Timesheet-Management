Ext.define('TM.controller.Timesheet', {
    extend: 'Ext.app.Controller',
    requires: [
        'Ext.window.Window'
    ],

    models: [
        'Timesheet', 'Project'
    ],

    stores: [
        'ProjectStore'
    ],

    views: [

    ],

    init: function () {
        //this.control({
        //    'viewport': {
        //        afterlayout: 'afterViewportLayout'
        //    },
        //    'contentPanel': {
        //        resize: 'centerContent'
        //    }
        //});
    }
});