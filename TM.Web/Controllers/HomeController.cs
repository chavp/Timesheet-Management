using Cwn.PM.BusinessModels.Entities;
using Cwn.PM.Loggers;
using log4net;

using PJ_CWN019.TM.Web.Filters;
using PJ_CWN019.TM.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using WebMatrix.WebData;

namespace PJ_CWN019.TM.Web.Controllers
{
    using NHibernate;
    using NHibernate.Linq;
    using PJ_CWN019.TM.Web.Models.Providers;
    using System.Web.Security;

    [ErrorLog]
    [ProfileLog]
    [Authorize]
    [FourceToChangeAttribute]
    [SessionState(SessionStateBehavior.Disabled)]
    public class HomeController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISessionFactory _sessionFactory = null;
        public HomeController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        string format = "yyyy-MM-dd";
        CultureInfo forParseDate = new CultureInfo("en-US");
        public ActionResult Index()
        {
            ViewBag.ToDay = DateTime.Now.ToString(this.format, forParseDate);
            ViewBag.FromDate = DateTime.Now.AddMonths(-3).ToString(this.format, forParseDate);
            ViewBag.ToDate = DateTime.Now.AddMonths(9).ToString(this.format, forParseDate);
            return View();
        }

        [HttpPost]
        public ActionResult SaveFeedback(string message, double rating)
        {
            var success = false;
            var msg = string.Empty;

            if (string.IsNullOrEmpty(message) ||
                string.IsNullOrEmpty(message.Trim()))
            {
                msg = "กรุณาระบุข้อมูลให้สมบูรณ์";
            }
            else
            {

                ISessionFactory sessionFactory = MvcApplication.CreateLogTimesheetSessionFactory();
                using (var session = sessionFactory.OpenSession())
                {
                    var newErroLog = new Feedback
                    {
                        UserName = WebSecurity.CurrentUserName,
                        Message = message,
                        Rating = rating,
                        EventDate = DateTime.Now,
                    };

                    session.Save(newErroLog);
                    session.Flush();
                }
                msg = "บันทึก feedback เสร็จสมบูรณ์";
                success = true;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectTimeline(string from, string to)
        {
            var projects = new List<ProjectTimeline>();

            var fromDate = DateTime.ParseExact(from, format, forParseDate).AddDays(-1);
            var toDate = DateTime.ParseExact(to, format, forParseDate).AddDays(1);

            using (var session = _sessionFactory.OpenSession())
            {
                var currentUser = GetCurrentUser(session);

                var prjQuery = (from p in session.Query<Project>()
                                join m in session.Query<ProjectMember>() on p equals m.Project
                                let pg = (from pg in session.Query<ProjectProgress>() 
                                          where p == pg.Project select pg).SingleOrDefault()
                                where 
                                m.ProjectRole != null
                                && m.User == currentUser
                                && p.Status.Name == "Open"
                                && p.Code != "PJ-CWN000"
                                && p.StartDate != null
                                //where p.StartDate.HasValue
                                //&& fromDate <= p.StartDate.Value
                                //&& p.StartDate.Value <= toDate
                                orderby p.StartDate descending
                                select new { 
                                    Prj = p, 
                                    Prg = pg 
                                });

                if (Roles.IsUserInRole(ConstAppRoles.Executive))
                {
                    prjQuery = (from p in session.Query<Project>()
                                let pg = (from pg in session.Query<ProjectProgress>() where p == pg.Project select pg).SingleOrDefault()
                                where p.Status.Name == "Open"
                                && p.Code != "PJ-CWN000"
                                && p.StartDate != null
                                //where p.StartDate.HasValue
                                //&& fromDate <= p.StartDate.Value
                                //&& p.StartDate.Value <= toDate
                                orderby p.StartDate descending
                                select new { Prj = p, Prg = pg });
                }
                //var prjQuery = (from p in session.Query<Project>()
                //                let pg = (from pg in session.Query<ProjectProgress>() where p == pg.Project select pg).SingleOrDefault()
                //                where p.Status.Name == "Open"
                //                && p.Code != "PJ-CWN000"
                //                && p.StartDate != null
                //                //where p.StartDate.HasValue
                //                //&& fromDate <= p.StartDate.Value
                //                //&& p.StartDate.Value <= toDate
                //                orderby p.StartDate descending
                //                select new { Prj = p, Prg = pg });

                var prjs = prjQuery.ToList();
                prjs.ForEach(p =>
                {

                    var prj = new ProjectTimeline
                    {
                        Name = p.Prj.Code,
                        StartDate = p.Prj.StartDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US")),
                        
                    };

                    if (p.Prg != null) prj.Progress = p.Prg.PercentageProgress;
                    if (p.Prj.ContractStartDate.HasValue) prj.ContractStartDate = p.Prj.ContractStartDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    if (p.Prj.ContractEndDate.HasValue) prj.ContractEndDate = p.Prj.ContractEndDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    if (p.Prj.WarrantyEndDate.HasValue) prj.WarrantyEndDate = p.Prj.WarrantyEndDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"));

                    //if (p.Code == "K4-PEB007")
                    //{
                    //    prj.ContractStartDate = new DateTime(2015, 3, 1).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //}

                    //if (p.Code == "PJ-CUS007")
                    //{
                    //    prj.ContractStartDate = new DateTime(2015, 2, 25).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.ContractEndDate = new DateTime(2015, 7, 3).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.Progress = 20;
                    //}

                    //if (p.Code == "PJ-KBK007")
                    //{
                    //    prj.ContractStartDate = new DateTime(2014, 12, 19).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.ContractEndDate = new DateTime(2015, 2, 10).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.WarrantyEndDate = new DateTime(2015, 3, 10).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.Progress = 100;
                    //}


                    //if (p.Code == "PJ-DOE012")
                    //{
                    //    prj.ContractStartDate = p.StartDate.Value.AddDays(20).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.ContractEndDate = p.StartDate.Value.AddMonths(4).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    //prj.WarrantyEndDate = new DateTime(2015, 3, 10).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.Progress = 0;
                    //}
                    //if (p.Code == "PJ-BKK008")
                    //{
                    //    prj.ContractStartDate = p.StartDate.Value.AddDays(30).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.ContractEndDate = p.StartDate.Value.AddMonths(5).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.Progress = 10;
                    //}
                    //if (p.Code == "K4-PEB006")
                    //{
                    //    prj.ContractStartDate = p.StartDate.Value.AddDays(10).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.ContractEndDate = p.StartDate.Value.AddMonths(8).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.Progress = 13;
                    //}
                    //if (p.Code == "PJ-AOT003")
                    //{
                    //    prj.ContractStartDate = p.StartDate.Value.AddDays(15).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.ContractEndDate = p.StartDate.Value.AddMonths(10).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    //    prj.Progress = 18;
                    //}
                    //prj.ContractStartDate = new DateTime(2015, 4, 10).ToString("yyyy-MM-dd", new CultureInfo("en-US"));

                    projects.Add(prj);
                });
            }

            return Json(projects, JsonRequestBehavior.AllowGet);
        }

        private User GetCurrentUser(ISession session)
        {
            return (from u in session.Query<User>()
                    where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                    select u).Single();
        }
    }
}
