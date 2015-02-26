using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class ProjectProgressUpdateLogView
    {
        public Guid ID { get; set; }
        public long ProjectID { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedValue { get; set; }
    }
}