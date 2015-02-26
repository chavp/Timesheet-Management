using Cwn.PM.BusinessModels.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public static class ModelViewConvertor
    {
        public static DateTime? ToNullableDateTime(this string dtText)
        {
            DateTime? val = null;
            if (!string.IsNullOrEmpty(dtText))
            {
                DateTime date;
                if (DateTime.TryParseExact(dtText, "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out date))
                {
                    val = date;
                }
            }
            return val;
        }

        public static EmployeeStatus ToEmployeeStatus(this string statusText)
        {
            EmployeeStatus result = EmployeeStatus.Work;
            if (!string.IsNullOrEmpty(statusText))
            {
                EmployeeStatus val;
                if (Enum.TryParse < EmployeeStatus>(statusText, out val))
                {
                    result = val;
                }
            }
            return result;
        }
    }
}