using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Support.Web.Models
{
    public class OverviewView
    {
        public string ID { get; set; }
        public string Level { get; set; }
        public DateTime EventDate { get; set; }
        public int TotalRecords { get; set; }
    }
}