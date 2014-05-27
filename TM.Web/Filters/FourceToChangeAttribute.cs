using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PJ_CWN019.TM.Web.Filters
{
    using Cwn.PM.BusinessModels.Entities;
    using Microsoft.Practices.Unity;
    using NHibernate;
    using NHibernate.Linq;
    using PJ_CWN019.TM.Web.Models;
    using System.Web.Mvc;
    using System.Web.Security;
    using WebMatrix.WebData;

    public class FourceToChangeAttribute : FilterAttribute, IAuthorizationFilter
    {
        ISessionFactory _sessionFactory = null;
        public FourceToChangeAttribute()
        {
            _sessionFactory = DependencyResolver.Current.GetService<ISessionFactory>();
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    var user = (from u in session.Query<User>()
                                where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                select u).Single();

                    if (!user.LastPasswordChangedDate.HasValue)
                    {
                        UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);
                        filterContext.HttpContext.Response.Redirect(urlHelper.Action(
                            "ChangePassword", "Account",
                            new ChangePasswordView
                            {
                                NewPassword = "",
                                OldPassword = user.EmployeeID.ToString(),
                                ConfirmNewPassword = "",
                            }), true);
                    }
                }
            }
        }
    }
}