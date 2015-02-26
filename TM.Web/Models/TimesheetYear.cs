using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class TimesheetYear
    {
        public int Year { get; set; }

        public decimal NonRecordEfforts { get; set; }
        public decimal NonProjectEfforts { get; set; }
        public decimal ProjectEfforts { get; set; }

    }
}