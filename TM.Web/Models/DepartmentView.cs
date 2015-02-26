using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class DepartmentView
    {
        public long ID { get; set; }
        public long DivisionID { get; set; }
        public string Name { get; set; }
        public string DivisionName { get; set; }
        public long UnderEmployees { get; set; }
        public long TimesheetCount { get; set; }
    }
}