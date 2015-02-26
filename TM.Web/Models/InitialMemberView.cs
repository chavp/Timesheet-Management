using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class InitialMemberView
    {
        public long ID { get; set; }
        public long EmployeeID { get; set; }
        public string FullName { get; set; }
        public long ProjectRoleID { get; set; }
        public string ProjectRoleName { get; set; }
    }
}