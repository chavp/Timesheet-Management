Ext.Loader.setConfig(
    {
        enabled: true
    });
//Ext.Loader.setPath('Ext.ux', '../ux');

Ext.override(Ext.form.action.Submit, {
    onSuccess: function (response) {
        var form = this.form,
            success = true,
            result = response;
        response.responseText = '{"success": true}';
        form.afterAction(this, success);
    }
});

//Default
var paramsView = {};
var messagesForm = {
    validationTitle: 'ผลการตรวจสอบข้อมูล / Validation Fields',
    validationAddProjectMember: 'กรุณากำหนด หน้าที่ในโครงการ ให้สมบูรณ์',
    errorAlertTitle: 'ข้อผิดพลาด / Error',
    validationWarning: 'กรุณากรอกข้อมูลให้สมบูรณ์',
    successTitle: 'สำเร็จ / Success',
    successMsg: 'บันทึกข้อมูลเสร็จสมบูรณ์',

    requireInputEmptyText: 'กรุณาระบุข้อมูล',
    requireSelectEmptyText: 'กรุณาเลือก',

    okActionText: '<i class="glyphicon glyphicon-ok"></i> ตกลง / OK',
    saveActionText: '<i class="glyphicon glyphicon-floppy-disk"></i> บันทึก / Save',
    cancleActionText: '<i class="glyphicon glyphicon-remove"></i> ยกเลิก / Cancel'
};

Ext.application({
    name: 'TM',
    appFolder: document.appFolderPath + 'app',
    autoCreateViewport: false,

    requires: [
        'Ext.form.field.ComboBox',
        'Ext.state.CookieProvider',
        'Ext.window.MessageBox',
        'Ext.form.*',
        'Ext.tip.QuickTipManager',
        'Ext.data.*',
        'Ext.ux.CheckColumn',
        'Ext.window.MessageBox',
        'Ext.tip.*'
    ],

    models: [
        'Timesheet',
        'Project',
        'Phase',
        'TaskType',
        'MainTask',
        'TimesheetReport',
        'Division',
        'Department',
        'Employee',
        'ProjectMember',
        'ProjectRole',
        'User'
    ],

    stores: [
        'ProjectStore',
        'TimesheetStore',
        'PhaseStore',
        'TaskTypeStore',
        'MainTaskStore',
        'TimesheetReportStore',
        'DivisionStore',
        'DepartmentStore',
        'EmployeeStore',
        'ProjectMemberStore',
        'ProjectRoleStore',
        'UserStore'
    ],

    views: [
        'TimesheetWindow',
        'ProjectWindow',
        'EditProjectMemberWindow',
        'UserWindow',
        'UserChangePasswordWindow',
        'FeedbackWindow'
    ],

    //controllers: [
    //    'Timesheet'
    //],

    init: function () {
        //Ext.setGlyphFontFamily('Pictos');
        Ext.tip.QuickTipManager.init();
        Ext.state.Manager.setProvider(Ext.create('Ext.state.CookieProvider'));
    }
});