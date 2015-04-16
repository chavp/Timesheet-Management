using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PJ_CWN019.TM.Web.Filters
{
    using Cwn.PM.BusinessServices.ViewModels;
    using log4net;
    using Newtonsoft.Json;
    using PJ_CWN019.TM.Web.Models;

    public class ProfileLogAttribute : FilterAttribute, IActionFilter
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Stopwatch _timer;
        private string _postDataText;

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _timer = Stopwatch.StartNew();
            _postDataText = string.Empty;

            object postData = null;

            if (filterContext.ActionParameters.ContainsKey("timesheetView"))
            {
                postData = filterContext.ActionParameters["timesheetView"] as TimesheetView;
            }
            else if (filterContext.ActionParameters.ContainsKey("listOfDelete"))
            {
                postData = filterContext.ActionParameters["listOfDelete"] as List<string>;
            }
            else if (filterContext.ActionParameters.ContainsKey("timesheetReportView"))
            {
                postData = filterContext.ActionParameters["timesheetReportView"] as TimesheetReportView;
            }
            else if (filterContext.ActionParameters.ContainsKey("projectMemberViewList")
                && filterContext.ActionParameters.ContainsKey("projectCode"))
            {
                var para = new
                {
                    projectMemberViewList = filterContext.ActionParameters["projectMemberViewList"] as List<ProjectMemberView>,
                    projectCode = filterContext.ActionParameters["projectCode"] as string
                };
                _postDataText = JsonConvert.SerializeObject(para);
            }
            else if (filterContext.ActionParameters.ContainsKey("employeeIDList")
                && filterContext.ActionParameters.ContainsKey("projectCode"))
            {
                postData = new
                {
                    employeeIDList = filterContext.ActionParameters["employeeIDList"] as List<long>,
                    projectCode = filterContext.ActionParameters["projectCode"] as string
                };
                //_postDataText = JsonConvert.SerializeObject(para);
            }
            else if (filterContext.ActionParameters.ContainsKey("projectMemberIDList")
                && filterContext.ActionParameters.ContainsKey("projectCode"))
            {
                postData = new
                {
                    projectMemberIDList = filterContext.ActionParameters["projectMemberIDList"] as List<long>,
                    projectCode = filterContext.ActionParameters["projectCode"] as string
                };
                //_postDataText = JsonConvert.SerializeObject(para);
            }
            else if (filterContext.ActionParameters.ContainsKey("model"))
            {
                var loginModel = filterContext.ActionParameters["model"] as LoginModel;
                if (loginModel != null)
                {
                    //loginModel.Password = "xxxxxx";
                    var copyModel = new LoginModel
                    {
                        EmployeeID = loginModel.EmployeeID,
                        Password = "*******",
                        RememberMe = loginModel.RememberMe,
                    };

                    postData = new
                    {
                        loginModel = copyModel
                    };
                }
                //_postDataText = JsonConvert.SerializeObject(para);
            }

            if (postData != null)
            {
                _postDataText = JsonConvert.SerializeObject(postData);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception == null)
            {
                string ip = HttpContext.Current.Request.Params["HTTP_CLIENT_IP"] ?? HttpContext.Current.Request.UserHostAddress;

                var profile = new 
                {
                    UserID = filterContext.HttpContext.User.Identity.Name,
                    Action = filterContext.ActionDescriptor.ActionName,
                    Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    ElapsedTime = _timer.Elapsed,
                    UserHostAddress = filterContext.HttpContext.Request.UserHostAddress,
                    QueryString = filterContext.HttpContext.Request.QueryString.ToString(),
                    PostData = _postDataText
                };

                string profileMsg = JsonConvert.SerializeObject(profile);

                Logger.Info(profileMsg);
            }
        }
    }
}