using Cwn.PM.Loggers;
using Microsoft.AspNet.SignalR;
using NHibernate;
using NHibernate.Linq;
using PJ_CWN019.TM.Support.Web.Models;
using PJ_CWN019.TM.Support.Web.Models.Hubs;
using PJ_CWN019.TM.Support.Web.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PJ_CWN019.TM.Support.Web.Controllers
{
    public class HomeController : Controller
    {
        ISessionFactory _sessionFactory = null;
        IHubContext _logHub;

        public HomeController(ISessionFactory sessionFactory, IHubContext logHub)
        {
            _sessionFactory = sessionFactory;
            _logHub = logHub;
        }

        public ActionResult Index()
        {
            //_logHub.Clients.All.update("1", "2", DateTime.Today, 100);
            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var query = (from e in session.Query<ErrorLog>()
                             where e.EventDate.Date == DateTime.Today
                             select e);

                var log = new OverviewView
                {
                    ID = "error-" + DateTime.Today.ToString("ddMMyyyy"),
                    Level = "Error",
                    EventDate = DateTime.Today,
                    TotalRecords = query.Count()
                };

                _logHub.Clients.All.update(
                    "error-" + DateTime.Today.ToString("ddMMyyyy"),
                    "Error",
                    DateTime.Today,
                    query.Count());

                var query2 = (from p in session.Query<ProfileLog>()
                         where p.EventDate.Date == DateTime.Today
                         select p);

                _logHub.Clients.All.update(
                    "profile-" + DateTime.Today.ToString("ddMMyyyy"),
                    "Profile",
                    DateTime.Today,
                    query2.Count());

            }

            return View();
        }

        public ActionResult Overview()
        {
            ViewBag.DBLog = Settings.Default.DBLog;

            var results = new List<OverviewView>();

            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var query = (from e in session.Query<ErrorLog>()
                             group e by e.EventDate.Date into eGrp
                             orderby eGrp.Key descending
                             select new OverviewView
                             {
                                 ID = "error-" + eGrp.Key.ToString("ddMMyyyy"),
                                 Level = "Error",
                                 EventDate = eGrp.Key,
                                 TotalRecords = eGrp.Count()
                             }
                             );

                results.AddRange(query.ToList().Take(10));

                query = (from e in session.Query<ProfileLog>()
                         group e by e.EventDate.Date into eGrp
                         orderby eGrp.Key descending
                         select new OverviewView
                         {
                             ID = "profile-" + eGrp.Key.ToString("ddMMyyyy"),
                             Level = "Profile",
                             EventDate = eGrp.Key,
                             TotalRecords = eGrp.Count()
                         }
                );

                results.AddRange(query.ToList().Take(10));
            }

            return View(results);
        }

        public ActionResult ProfileLogs(string id, DateTime forDate)
        {
            ViewBag.ForDate = forDate.ToString("dd/MM/yyyy");
            using (var session = _sessionFactory.OpenSession())
            {
                var query = (from e in session.Query<ProfileLog>()
                             where e.EventDate.Date == forDate
                             select e);

                var totals = query.ToList();

                var results = (from t in totals
                               group t by new { t.Action, t.Controller } into eGrp
                               orderby eGrp.Key.Action, eGrp.Key.Controller
                               select new ProfileLogsOverviewView
                               {
                                   Action = eGrp.Key.Action,
                                   Controller = eGrp.Key.Controller,
                                   TotalCall = eGrp.Count(),
                                   AvgElapsedTime = eGrp.Average(l => l.ElapsedTime.TotalSeconds),
                                   LatestEventDate = eGrp.Max(l => l.EventDate),
                               })
                               .OrderByDescending(pl => pl.LatestEventDate)
                               .ToList();

                return View(results);
            }
        }

        public ActionResult ProfileLogDetails(string id, DateTime forDate, string actionName, string controllerName)
        {
            ViewBag.ID = id;
            ViewBag.ForDate = forDate;
            ViewBag.ForAction = actionName;
            ViewBag.ForController = controllerName;
            using (var session = _sessionFactory.OpenSession())
            {
                var query = (from e in session.Query<ProfileLog>()
                             orderby e.EventDate descending
                             where e.EventDate.Date == forDate
                             && e.Action == actionName
                             && e.Controller == controllerName
                             select e);
                var results = query.ToList();
                return View(results);
            }
        }

        public ActionResult ErrorLogs(string id, DateTime forDate)
        {
            ViewBag.ForDate = forDate.ToString("dd/MM/yyyy");
            using (var session = _sessionFactory.OpenSession())
            {
                var query = (from e in session.Query<ErrorLog>()
                             where e.EventDate.Date == forDate
                             orderby e.EventDate descending
                             select e);

                var results = query.ToList();
                return View(results);
            }
        }

        public ActionResult Journeys(string id, DateTime forDate)
        {
            ViewBag.ID = id;
            ViewBag.ForDate = forDate;

            //using (var session = _sessionFactory.OpenSession())
            //{
            //    var query = (from e in session.Query<ProfileLog>()
            //                 where e.EventDate.Date == forDate
            //                 select e);
            //}

            return View();
        }

        public ActionResult Analytics()
        {
            return View();
        }

        public JsonResult GetJourneys(string id, DateTime forDate)
        {
            ViewBag.ID = id;
            ViewBag.ForDate = forDate;

            var results = new List<ProfileLogView>();
            using (var session = _sessionFactory.OpenSession())
            {
                var query = (from e in session.Query<ProfileLog>()
                             where e.EventDate.Date == forDate
                             orderby e.EventDate descending
                             select new ProfileLogView
                             {
                                 UserID = e.UserID,
                                 Action = e.Action,
                                 Controller = e.Controller,
                                 EventTime = e.EventDate.ToString("dd/MM/yyyy HH:mm:ss"),
                                 ElapsedTime = e.ElapsedTime
                             });

                results = query.Take(500).ToList();
            }

            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}
