using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class PositionView
    {
        public long ID { get; set; }
        public string Display { get; set; }

        public string Name { get; set; }

        public long EmployeeCount { get; set; }
        public long TimesheetCount { get; set; }

        public decimal? PositionCost { get; set; }
    }
}