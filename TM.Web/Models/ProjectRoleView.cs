using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class ProjectRoleView
    {
        public long ProjectRoleID { get; set; }
        public string ProjectRoleName { get; set; }
        public decimal ProjectRoleCost { get; set; }

        public int Order { get; set; }
        public long ProjectMemberCount { get; set; }
        public long TimesheetCount { get; set; }
    }
}