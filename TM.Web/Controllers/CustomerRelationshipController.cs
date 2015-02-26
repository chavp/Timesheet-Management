using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace PJ_CWN019.TM.Web.Controllers
{
    using Cwn.PM.BusinessModels.Entities;
    using NHibernate;
    using NHibernate.Linq;
    using PJ_CWN019.TM.Web.Filters;
    using PJ_CWN019.TM.Web.Models;
    using PJ_CWN019.TM.Web.Models.Providers;

    [ErrorLog]
    [ProfileLog]
    [Authorize(Roles = ConstAppRoles.Admin)]
    [FourceToChangeAttribute]
    [SessionState(SessionStateBehavior.Disabled)]
    [ValidateAntiForgeryTokenOnAllPosts]
    public class CustomerRelationshipController : Controller
    {
        //
        // GET: /CustomerRelationship/
        ISessionFactory _sessionFactory = null;
        public CustomerRelationshipController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ActionResult Management()
        {
            return View();
        }

        // WEB API
        public JsonResult GetAllIndustryForCombobox()
        {
            var viewList = new List<IndustryView>();
            viewList.Add(new IndustryView
            {
                ID = 0,
                Display = "ไม่ระบุ"
            });

            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var query = (from x in session.Query<Industry>() select x);
                foreach (var item in query)
                {
                    var indust = new IndustryView
                    {
                        ID = item.ID,
                        Code = item.Code,
                        Name = item.Name,
                        NameTH = item.NameTH,
                    };

                    viewList.Add(indust);
                }
            }
            return Json(viewList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllCustomer()
        {
            var viewList = new List<CustomerView>();
            using (var session = _sessionFactory.OpenSession())
            {
                var qCusts = (from x in session.Query<Customer>()
                              orderby x.Name
                              let projectCount = (from p in session.Query<Project>()
                                                  where p.Customer == x
                                                  select p).Count()
                              select new { Cust = x, ProjectCount = projectCount });
                foreach (var cus in qCusts)
                {
                    var cusView = new CustomerView
                    {
                        ID = cus.Cust.ID,
                        Name = cus.Cust.Name,
                        ContactChannel = cus.Cust.ContactChannel,
                        ProjectCount = cus.ProjectCount,
                    };

                    if (cus.Cust.Industry != null)
                    {
                        cusView.IndustryID = cus.Cust.Industry.ID;
                        cusView.IndustryDisplay = IndustryView.FormatDisplay(
                            cus.Cust.Industry.Code, 
                            cus.Cust.Industry.NameTH, 
                            cus.Cust.Industry.Name);
                    }
                    viewList.Add(cusView);
                }
            }
            return Json(viewList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveCustomer(CustomerView customerView)
        {
            var success = false;
            var msg = string.Empty;
            var targetMsg = "ลูกค้า";
            var dupMessage = string.Format("พบชื่อ{0}นี้อยู่ในระบบแล้ว กรุณาระบุชื่อใหม่", targetMsg);
            var successMessage = string.Format("การบันทึก{0}การเสร็จสมบูรณ์", targetMsg);

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // Save
                if (customerView.ID <= 0)
                {
                    // check Dup
                    var y = (from x in session.Query<Customer>()
                             where x.Name == customerView.Name
                             select x).FirstOrDefault();

                    if (y != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var newY = new Customer(customerView.Name)
                        {
                            ContactChannel = customerView.ContactChannel
                        };

                        if (customerView.IndustryID > 0)
                        {
                            var indus = (from x in session.Query<Industry>()
                                         where x.ID == customerView.IndustryID
                                         select x).Single();

                            newY.Industry = indus;
                        }

                        session.Save(newY);
                    }
                }
                // Update
                else
                {
                    // check Dup
                    var y = (from x in session.Query<Customer>()
                             where x.Name == customerView.Name
                               && x.ID != customerView.ID
                             select x).FirstOrDefault();

                    if (y != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var updateY = (from d in session.Query<Customer>()
                                       where d.ID == customerView.ID
                                       select d).Single();

                        updateY.Name = customerView.Name;
                        updateY.ContactChannel = customerView.ContactChannel;

                        var indus = (from x in session.Query<Industry>()
                                     where x.ID == customerView.IndustryID
                                     select x).SingleOrDefault();

                        updateY.Industry = indus;

                        session.Update(updateY);
                    }
                }

                transaction.Commit();
                success = true;
                msg = successMessage;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteCustomer(int id)
        {
            var success = false;
            var msg = string.Empty;
            var targetMsg = "ลูกค้า";

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var y = (from x in session.Query<Customer>()
                         where x.ID == id
                         select x).Single();

                var projectCount = (from t in session.Query<Project>() where t.Customer == y select t).Count();
                if (projectCount > 0)
                {
                    msg = string.Format(
                        "ไม่สามารถลบ{0}นี้ได้ กรุณาลบข้อโครงการของ{0}นี้ทั้งหมดก่อน", targetMsg);
                    return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                }

                session.Delete(y);

                transaction.Commit();
                success = true;
                msg = string.Format("ลบ{0}เสร็จสมบูรณ์", targetMsg);
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        //END
    }
}
