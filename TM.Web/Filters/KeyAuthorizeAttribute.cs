using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PJ_CWN019.TM.Web.Filters
{
    public class KeyAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string key = httpContext.Request["Token"];
            return ApiValidatorService.IsValid(key);
        }
    }

    public static class ApiValidatorService
    {
        public static bool IsValid(string key)
        {
            int keyvalue;

            
            if (int.TryParse(key, out keyvalue))
            {
                return keyvalue % 2137 == 7;
            }
            return false;
        }
    }
}