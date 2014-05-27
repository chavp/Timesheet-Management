using Cwn.PM.Loggers;
using NHibernate;
using PJ_CWN019.TM.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace PJ_CWN019.TM.Web.Controllers
{
    [ErrorLog]
    [ProfileLog]
    [Authorize]
    [FourceToChangeAttribute]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
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
                msg = "บันทึก feedback เสร็จสบบูรณ์";
                success = true;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }
    }
}
