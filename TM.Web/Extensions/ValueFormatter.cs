using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Extensions
{
    public static class ValueFormatter
    {
        public static string ToPresentDateString(this DateTime? row)
        {
            return (row.HasValue) ? row.Value.ToString("dd/MM/yyyy") : "N/A";
        }
    }
}