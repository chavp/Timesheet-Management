Ext.Loader.setConfig(
    {
        enabled: true
    });
//Ext.Loader.setPath('Ext.ux', '../ux');

Ext.Ajax.defaultHeaders = {
    '__RequestVerificationToken': $("input[name='__RequestVerificationToken']").val()
};

WindowHeight = (function () {
    var margin = { top: 200 },
        height = Ext.getBody().getViewSize().height - margin.top;

    return {
        height: 650
    }
})();

TextLabel = (function () {
    var heartIcon = "<i class='glyphicon glyphicon-heart'></i>",

        add = "Add",
        addTH = "เพิ่ม",
        edit = "Edit",
        editTH = "แก้ไข",

        projectPhaseTH = "ช่วงโครงการ",
        projectPhaseEN = "Project Phase",

        taskTypeTH = "ประเภทงาน",
        taskTypeEN = "Task Type",

        mainTaskTH = "งานหลัก",
        mainTaskEN = "Main Task",

        projectRoleTH = "หน้าที่ในโครงการ",
        projectRoleEN = "Project Role",

        orderTH = "ลำดับ",
        orderEN = "Order",
        customer = "Customer",
        customerTH = "ลูกค้า",

        industry = "Industry",
        industryTH = "อุตสาหกรรม",

        type = "Type",
        typeTH = "ประเภท"
    ;

    return {
        searchCriterionLabel: 'เงื่อนไขการค้นหา / Search Criteria',
        projectLabel: 'โครงการ / Project',
        codeLabel: 'รหัส / Code',
        nameLabel: 'ชื่อ / Name',

        projectCodeColumnText: 'รหัสโครงการ<br/>Project Code',
        projectNameColumnText: 'ชื่อโครงการ<br/>Project Name',
        projectProgressColumnText: 'โปรเกรสของโครงการ<br/>Project Progress',

        allLabel: 'ทั้งหมด',

        cmdSearchText: '<i class="glyphicon glyphicon-search"></i> ค้นหา / Search',
        cmdClearText: '<i class="glyphicon glyphicon-trash"></i> ล้างข้อมูล / Clear',
        cmdExportText: '<i class="glyphicon glyphicon-print"></i> ส่งออกข้อมูล / Export',

        okActionText: '<i class="glyphicon glyphicon-ok"></i> ตกลง / OK',
        saveActionText: '<i class="glyphicon glyphicon-floppy-disk"></i> บันทึก / Save',
        cancleActionText: '<i class="glyphicon glyphicon-remove"></i> ยกเลิก / Cancel',
        closeActionText: '<i class="glyphicon glyphicon-remove"></i> ปิด / Close',

        cmdDeleteText: 'ลบข้อมูล',

        departmentColumnText: "แผนก<br/>Department",
        divisionColumnText: "ฝ่าย<br/>Division",
        divisionLabelText: "ฝ่าย / Division",
        departmentLabelText: "แผนก / Department",
        positionLabelText: "ตำแหน่ง / Position",
        positionProjectRoleRateCostLabelText: "ต้นทุนตำแหน่งต่อวัน / Position Cost per day",

        positionNameColumnText: "ตำแหน่ง<br/>Position",
        positionCostColumnText: "ต้นทุนตำแหน่งต่อวัน<br/>Position Cost / Day",

        validationTitle: 'ผลการตรวจสอบข้อมูล / Validation Fields',
        validationAddProjectMember: 'กรุณากำหนด หน้าที่ในโครงการ ให้สมบูรณ์',
        errorAlertTitle: 'ข้อผิดพลาด / Error',
        validationWarning: 'กรุณากรอกข้อมูลให้สมบูรณ์',
        successTitle: 'สำเร็จ / Success',
        successMsg: 'บันทึกข้อมูลเสร็จสมบูรณ์',

        requireInputEmptyText: 'กรุณาระบุข้อมูล',
        requireSelectEmptyText: 'กรุณาเลือก',

        addDivisionTitle: '<i class="glyphicon glyphicon-plus"></i> เพิ่มฝ่าย / Add Division',
        editDivisionTitle: 'แก้ไขข้อมูลฝ่าย / Edit Division',

        addDepartmentTitle: '<i class="glyphicon glyphicon-plus"></i> เพิ่มแผนก / Add Department',
        editDepartmentTitle: 'แก้ไขข้อมูลแผนก / Edit Department',

        addPositionTitle: '<i class="glyphicon glyphicon-plus"></i> เพิ่มตำแหน่ง / Add Position',
        editPositionTitle: 'แก้ไขข้อมูลตำแหน่ง / Edit Position',

        projectPhaseTitle: projectPhaseTH + " / " + projectPhaseEN,
        projectTaskTypeTitle: taskTypeTH + " / " + taskTypeEN,
        projectMainTaskTitle: mainTaskTH + " / " + mainTaskEN,

        projectPhaseTabTitle: '<i class="glyphicon glyphicon-road"></i> ' + projectPhaseTH + " / " + projectPhaseEN,
        projectTaskTypeTabTitle: '<i class="glyphicon glyphicon-flag"></i> ' + taskTypeTH + " / " + taskTypeEN,
        projectMainTaskTabTitle: '<i class="glyphicon glyphicon-tasks"></i> ' + mainTaskTH + " / " + mainTaskEN,
        projectProjectRoleTabTitle: '<i class="glyphicon glyphicon-hand-right"></i> ' + projectRoleTH + " / " + projectRoleEN,

        customersTabTitle: heartIcon + ' ' + customer + 's',

        orderColumnText: orderTH + "<br/>" + orderEN,
        orderLabel: orderTH + " / " + orderEN,

        projectPhaseColumnText: projectPhaseTH + '<br/>' + projectPhaseEN,
        editProjectPhaseText: "แก้ไขข้อมูล" + projectPhaseTH + " / Edit " + projectPhaseEN,
        addProjectPhaseText: "เพิ่ม" + projectPhaseTH + " / Add " + projectPhaseEN,
        deleteProjectPhaseText: "ลบ" + projectPhaseTH + " / Delete " + projectPhaseEN,
        addProjectPhaseTitle: '<i class="glyphicon glyphicon-plus"></i> เพิ่ม' + projectPhaseTH + ' / Add ' + projectPhaseEN,
        editProjectPhaseTitle: 'แก้ไข' + projectPhaseTH + ' / Edit ' + projectPhaseEN,
        projectPhaseLabelText: projectPhaseTH + " / " + projectPhaseEN,

        taskTypeColumnText: taskTypeTH + '<br/>' + taskTypeEN,
        editTaskTypeText: "แก้ไขข้อมูล" + taskTypeTH + " / Edit " + taskTypeEN,
        addTaskTypeText: "เพิ่ม" + taskTypeTH + " / Add " + taskTypeEN,
        deleteTaskTypeText: "ลบ" + taskTypeTH + " / Delete Task Type",
        addTaskTypeTitle: '<i class="glyphicon glyphicon-plus"></i> ' + addTH + taskTypeTH + ' / ' + add + ' ' + taskTypeEN,
        editTaskTypeTitle: 'แก้ไข' + taskTypeTH + ' / Edit ' + taskTypeEN,
        taskTypeLabelText: taskTypeTH + " / " + taskTypeEN,

        mainTaskColumnText: mainTaskTH + '<br/>' + mainTaskEN,
        editMainTaskText: "แก้ไขข้อมูล" + mainTaskTH + " / Edit " + mainTaskEN,
        addMainTaskText: "เพิ่ม" + mainTaskTH + " / Add " + mainTaskEN,
        deleteMainTaskText: "ลบ" + mainTaskTH + " / Delete " + mainTaskEN,
        addMainTaskTitle: '<i class="glyphicon glyphicon-plus"></i> เพิ่ม' + mainTaskTH + ' / Add ' + mainTaskEN,
        editMainTaskTitle: 'แก้ไข' + mainTaskTH + ' / Edit ' + mainTaskEN,
        mainTaskLabelText: mainTaskTH + " / " + mainTaskEN,

        projectRoleColumnText: projectRoleTH + '<br/>' + projectRoleEN,
        projectRoleCostColumnText: 'ต้นทุนต่อวัน<br/>Cost / Day',
        projectRoleCostText: 'ต้นทุนต่อวัน / Cost Per day',
        editProjectRoleText: "แก้ไข" + projectRoleTH + " / Edit " + projectRoleEN,
        addProjectRoleText: "เพิ่ม" + projectRoleTH + " / Add " + projectRoleEN,
        deleteProjectRoleText: "ลบ" + projectRoleTH + " / Delete " + projectRoleEN,
        addProjectRoleTitle: '<i class="glyphicon glyphicon-plus"></i> เพิ่ม' + projectRoleTH + ' / Add ' + projectRoleEN,
        editProjectRoleTitle: 'แก้ไข' + projectRoleTH + ' / Edit ' + projectRoleEN,
        projectRoleLabelText: projectRoleTH + " / " + projectRoleEN,

        customer: customer,
        customerColumtextText: customerTH + "<br/>" + customer,
        customerAddText: addTH + customerTH + " / " + add + " " + customer,
        customerEditTitle: editTH + customerTH + " / " + edit + " " + customer,
        customerFieldLabel: customerTH + " / " + customer,
        customerContactFieldLabel: "ช่องทางติดต่อ / Contact Channel",
        customerContactEmpaty: "ตัวอย่าง: ที่อยู่, เบอร์โทรติดต่อ, email, fax, facebook, website",
        
        industryTypeColumtextText: typeTH + industryTH + "<br/>" + industry + " " + type,
        industryTypeFieldLabel: typeTH + industryTH + " / " + industry + " " + type
    }
})();

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
var paramsConst = {
    limitMember: 99999
};

// obsolete -> TextLabel
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
    cancleActionText: '<i class="glyphicon glyphicon-remove"></i> ยกเลิก / Cancel',
    closeActionText: '<i class="glyphicon glyphicon-remove"></i> ปิด / Close'
};

CommandActionBuilder = (function () {

    function deleteData(id, url, store, store2) {
        Ext.MessageBox.confirm('ยืนยัน', 'คุณต้องการลบข้อมูลนี้ใช่ หรือ ไม่?',
                function (btn) {
                    if (btn === "yes") {
                        Ext.MessageBox.wait("กำลังลบข้อมูล...", 'กรุณารอ');

                        Ext.Ajax.request({
                            url: url,
                            success: function (transport) {
                                Ext.MessageBox.hide();
                                var respose = Ext.decode(transport.responseText);
                                if (respose.success) {
                                    Ext.MessageBox.show({
                                        title: messagesForm.successTitle,
                                        msg: respose.message,
                                        //width: 300,
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.INFO,
                                        fn: function (btn) {
                                            if (store) store.load();
                                            if (store2) store2.load();
                                        }
                                    });

                                } else {
                                    Ext.MessageBox.show({
                                        title: messagesForm.errorAlertTitle,
                                        msg: respose.message,
                                        //width: 300,
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.ERROR
                                    });
                                }
                            },
                            failure: function (transport) {
                                Ext.MessageBox.show({
                                    title: messagesForm.errorAlertTitle,
                                    msg: transport.responseText,
                                    //width: 300,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                            },
                            jsonData: { id: id }
                        });
                    }
                },
                this);
    }

    return {
        cancleAction: function (from) {
            var text = TextLabel.cancleActionText;
            if (from.getReadOnly) {
                if (from.getReadOnly()) {
                    text = TextLabel.closeActionText;
                }
            }
            return Ext.create('Ext.Action', {
                //iconCls: 'add-button',
                text: text,
                disabled: false,
                handler: function (widget, event) {
                    from._commandClose = true;
                    from.close();
                }
            });
        },

        deleteData: deleteData
    }
})();

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
        'User',
        'CumulativeEffortByWeek',
        'CumulativeItemByDate',
        'ProjectStatus',
        'TitleName',
        'Position',
        'InitialMember',
        'ProjectProgressUpdateLog',
        'EmployeeStatus',
        'Customer',
        'Industry'
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
        'UserStore',
        'CumulativeEffortByWeekStore',
        'CumulativeEffortByDateStore',
        'ProjectStatusStore',
        'PositionStore',
        'TitleNameStore',
        'InitialMemberStore',
        'ProjectProgressUpdateLogStore',
        'EmployeeStatusArrayStore',
        'CustomerStore',
        'IndustryStore'
    ],

    views: [
        'TimesheetWindow',
        'ProjectWindow',
        'EditProjectMemberWindow',
        'UserWindow',
        'UserChangePasswordWindow',
        'FeedbackWindow',
        'ChartWindow',
        'ProjectSaveWindow',
        'ProjectUpdateProgressWindow',
        'ProjectUpdateProgressSaveWindow',
        'ProjectActivitiesBarWindow',
        'DivisionWindow',
        'DepartmentWindow',
        'PositionWindow',
        'ProjectPhaseWindow',
        'TaskTypeWindow',
        'MainTaskWindow',
        'ProjectRoleWindow',
        'CustomerWindow'
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