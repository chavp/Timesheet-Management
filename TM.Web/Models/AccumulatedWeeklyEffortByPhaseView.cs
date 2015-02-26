using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class AccumulatedWeeklyEffortByPhaseView
    {
        public string Key { get; set; }

        public List<dynamic> Values { get; set; }
    }
}