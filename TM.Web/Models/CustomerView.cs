using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class CustomerView
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string ContactChannel { get; set; }
        public long IndustryID { get; set; }
        public string IndustryDisplay { get; set; }
        public int ProjectCount { get; set; }
    }
}