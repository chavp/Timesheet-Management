using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PJ_CWN019.TM.Web.Controllers
{
    using Cwn.PM.BusinessModels.Entities;
    using Cwn.PM.BusinessModels.Queries;
    using NHibernate;
    using NHibernate.Linq;
    using PJ_CWN019.TM.Web.Filters;
    using PJ_CWN019.TM.Web.Models;
    using PJ_CWN019.TM.Web.Models.Providers;
    using System.Web.SessionState;

    [ErrorLog]
    [ProfileLog]
    [Authorize(Roles = ConstAppRoles.Support)]
    [FourceToChangeAttribute]
    [SessionState(SessionStateBehavior.Disabled)]
    [ValidateAntiForgeryTokenOnAllPosts]
    public class SupportController : Controller
    {
        ISessionFactory _sessionFactory = null;
        public SupportController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ActionResult Employees()
        {
            return View();
        }

        public JsonResult GetEmployees(int start, int limit, string query = "")
        {
            var viewList = new List<EmployeeView>();
            int count = 0;

            using (var session = _sessionFactory.OpenSession())
            {
                var userList = (from u in session.Query<User>()
                                where u.Status == EmployeeStatus.Work
                                && (u.EmployeeID.ToString().Contains(query)
                                || u.FirstNameEN.Contains(query)
                                || u.FirstNameTH.Contains(query)
                                || u.LastNameEN.Contains(query)
                                || u.LastNameTH.Contains(query))
                                orderby u.EmployeeID, u.FirstNameTH, u.LastNameTH
                                select new EmployeeView
                                {
                                    ID = u.ID,
                                    EmployeeID = u.EmployeeID,
                                    FullName = u.FirstNameTH + " " + u.LastNameTH,
                                    Display = u.FirstNameTH + " " + u.LastNameEN,
                                    LastChangedPassword = u.LastPasswordChangedDate,
                                    LastLoginDate = u.LastLoginDate,
                                    Position = u.Position.NameTH,
                                    Division = u.Department.Division.NameTH,
                                    Department = u.Department.NameTH,
                                });

                count = userList.Count();
                viewList = userList.Skip(start).Take(limit).ToList();
            }

            var result = new
            {
                data = viewList,
                total = count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
