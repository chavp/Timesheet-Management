using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class ProjectThresholdView
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public decimal LimitRatio { get; set; }
    }
}