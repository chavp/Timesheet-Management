using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class ProjectMemberView
    {
        public long ID { get; set; }
        public long EmployeeID { get; set; }
        public long DepartmentID { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public long ProjectRoleID { get; set; }
        public string ProjectRoleName { get; set; }
        public string ProjectCode { get; set; }

        public bool CanEditProjectRole { get; set; }
        public bool CanRemove { get; set; }
    }
}