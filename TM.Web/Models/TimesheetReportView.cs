using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class TimesheetReportView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public bool CheckAllTime { get; set; }

        public string ProjectCode { get; set; }
        public string EmployeeID { get; set; }
        public long DepartmentID { get; set; }
    }
}