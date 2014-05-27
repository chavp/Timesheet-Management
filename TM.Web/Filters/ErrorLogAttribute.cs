using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace PJ_CWN019.TM.Web.Filters
{
    using log4net;
    using System.Reflection;
    using System.Web.Mvc;

    public class ErrorLogAttribute : FilterAttribute, IExceptionFilter
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void OnException(ExceptionContext filterContext)
        {
            Logger.Error("OnException", filterContext.Exception);

            // save to error log database

        }
    }
}