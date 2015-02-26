using Cwn.PM.BusinessModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class DepartmentTimesheetYear
    {
        public int Year { get; set; }
        public string Department { get; set; }
        public string ProjectRole { get; set; }
        public long ProjectRoleID { get; set; }
        public decimal TotalEfforts { get; set; }
        public decimal TotalCost { get; set; }
    }
}