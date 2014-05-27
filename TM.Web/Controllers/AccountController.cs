﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PJ_CWN019.TM.Web.Controllers
{
    using Cwn.PM.BusinessModels.Entities;
    using NHibernate;
    using NHibernate.Linq;
    using PJ_CWN019.TM.Web.Filters;
    using PJ_CWN019.TM.Web.Models;
    using System.Web.Security;
    using WebMatrix.WebData;

    [ErrorLog]
    [ProfileLog]
    [Authorize]
    public class AccountController : Controller
    {
        ISessionFactory _sessionFactory = null;
        public AccountController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid &&
                WebSecurity.Login(model.EmployeeID, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "กรุณา ระบุรหัสพนักงาน หรือ รหัสเข้าระบบ ให้ถูกต้อง");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            using (var session = _sessionFactory.OpenSession())
            {
                var user = (from u in session.Query<User>()
                            where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                            select u).FirstOrDefault();

                if (user != null)
                {

                    user.LastLockoutIP = HttpContext.Request.UserHostAddress;
                    user.LastLockoutDate = DateTime.Now;
                    user.IsLockedOut = true;

                    session.Flush();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmNewPassword, bool isFourceToChange = false)
        {
            var oldPasswordNotValid = "กรุณาระบุ รหัสเข้าระบบเดิม ให้ถูกต้อง";
            var strong8PasswordNotValid = "กรุณาระบุ รหัสเข้าระบบใหม่ ให้มีความยาวมากกว่า 8 ตัวอักษร";
            var newPasswordNotValid = "กรุณาระบุ รหัสเข้าระบบใหม่ ไม่ให้ตรงกับ รหัสพนักงาน";

            if (!isFourceToChange)
            {
                var success = false;
                var msg = string.Empty;

                var isValidOldPassword = WebSecurity.Login(WebSecurity.CurrentUserName, oldPassword);
                if (!isValidOldPassword)
                {
                    msg = oldPasswordNotValid;
                }
                else if (newPassword.Length < 8)
                {
                    // http://windows.microsoft.com/en-us/windows-vista/tips-for-creating-a-strong-password
                    msg = strong8PasswordNotValid;
                }
                else
                {
                    using (var session = _sessionFactory.OpenSession())
                    {
                        var user = (from u in session.Query<User>()
                                    where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                    select u).First();

                        user.SetPassword(newPassword);
                        user.LastPasswordChangedDate = DateTime.Now;
                        session.Flush();
                    }
                    msg = "แก้ไข รหัสเข้าระบบ เสร็จสบบูรณ์";
                    success = true;
                }
                return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (string.IsNullOrEmpty(newPassword))
                {
                    ModelState.AddModelError("", "กรุณาระบุ รหัสเข้าระบบใหม่ ให้ถูกต้อง");
                }
                else if (newPassword != confirmNewPassword)
                {
                    ModelState.AddModelError("", "กรุณาระบุ รหัสเข้าระบบใหม่ ให้ตรงกับ ยืนยันรหัสเข้าระบบใหม่");
                }
                else if (newPassword.Length < 8)
                {
                    // http://windows.microsoft.com/en-us/windows-vista/tips-for-creating-a-strong-password
                    ModelState.AddModelError("", strong8PasswordNotValid);
                }
                else
                {
                    using (var session = _sessionFactory.OpenSession())
                    {
                        var user = (from u in session.Query<User>()
                                    where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                    select u).First();

                        if (newPassword == user.EmployeeID.ToString())
                        {
                            ModelState.AddModelError("", newPasswordNotValid);
                        }
                        else
                        {
                            user.SetPassword(newPassword);
                            user.LastPasswordChangedDate = DateTime.Now;
                            session.Flush();

                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                return View("ChangePassword", new ChangePasswordView
                {
                    NewPassword = newPassword,
                    ConfirmNewPassword = confirmNewPassword
                });
            }
        }

        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordView());
        }

        [HttpPost]
        [Authorize(Roles = "Support")]
        public JsonResult ResetPassword(int empoyeeID)
        {
            var success = false;
            var msg = string.Empty;
            var redirectToIndex = false;
            using (var session = _sessionFactory.OpenSession())
            {
                var user = (from u in session.Query<User>()
                            where u.EmployeeID == empoyeeID
                            select u).First();

                user.SetPassword(empoyeeID.ToString());
                user.LastPasswordChangedDate = null;
                session.Flush();

                if (empoyeeID.ToString() == WebSecurity.CurrentUserName)
                {
                    redirectToIndex = true;
                }
            }
            msg = "คืนค่าเริ่มต้นของ รหัสเข้าระบบ ผู้ใช้งานเสร็จสบบูรณ์";
            success = true;

            return Json(new { success = success, message = msg, redirectToIndex = redirectToIndex }, JsonRequestBehavior.AllowGet);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
