using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class EmployeeView
    {
        public long ID { get; set; }
        public long EmployeeID { get; set; }
        public string FullName { get; set; }
        public string Display { get; set; }
        public string Position { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastChangedPassword { get; set; }
    }
}