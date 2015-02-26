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
                var prjQuery = (from p in session.Query<Project>()
                                where p.StartDate.HasValue
                                && fromDate <= p.StartDate.Value
                                && p.StartDate.Value <= toDate
                                orderby p.StartDate descending
                                select p);

                var prjs = prjQuery.ToList();
                prjQuery.ForEach(p =>
                {
                    var prj = new ProjectTimeline
                    {
                        Name = p.Code,
                        StartDate = p.StartDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"))
                    };

                    if (p.ContractStartDate.HasValue) prj.ContractStartDate = p.ContractStartDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    if (p.ContractEndDate.HasValue) prj.ContractEndDate = p.ContractEndDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    if (p.WarrantyEndDate.HasValue) prj.WarrantyEndDate = p.WarrantyEndDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US"));

                    if (p.Code == "K4-PEB007")
                    {
                        prj.ContractStartDate = new DateTime(2015, 3, 1).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                    }

                    if (p.Code == "PJ-CUS007")
                    {
                        prj.ContractStartDate = new DateTime(2015, 2, 25).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.ContractEndDate = new DateTime(2015, 7, 3).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.Progress = 20;
                    }

                    if (p.Code == "PJ-KBK007")
                    {
                        prj.ContractStartDate = new DateTime(2014, 12, 19).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.ContractEndDate = new DateTime(2015, 2, 10).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.WarrantyEndDate = new DateTime(2015, 3, 10).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.Progress = 100;
                    }


                    if (p.Code == "PJ-DOE012")
                    {
                        prj.ContractStartDate = p.StartDate.Value.AddDays(20).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.ContractEndDate = p.StartDate.Value.AddMonths(4).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        //prj.WarrantyEndDate = new DateTime(2015, 3, 10).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.Progress = 0;
                    }
                    if (p.Code == "PJ-BKK008")
                    {
                        prj.ContractStartDate = p.StartDate.Value.AddDays(30).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.ContractEndDate = p.StartDate.Value.AddMonths(5).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.Progress = 10;
                    }
                    if (p.Code == "K4-PEB006")
                    {
                        prj.ContractStartDate = p.StartDate.Value.AddDays(10).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.ContractEndDate = p.StartDate.Value.AddMonths(8).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.Progress = 13;
                    }
                    if (p.Code == "PJ-AOT003")
                    {
                        prj.ContractStartDate = p.StartDate.Value.AddDays(15).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.ContractEndDate = p.StartDate.Value.AddMonths(10).ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                        prj.Progress = 18;
                    }
                    //prj.ContractStartDate = new DateTime(2015, 4, 10).ToString("yyyy-MM-dd", new CultureInfo("en-US"));

                    projects.Add(prj);
                });
            }

            return Json(projects, JsonRequestBehavior.AllowGet);
        }

    }
}
