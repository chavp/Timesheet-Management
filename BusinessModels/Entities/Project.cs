using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class Project
        : Entity
    {
        public Project()
        {
            Members = new List<ProjectMember>();
            TimeSheets = new List<Timesheet>();
        }

        public virtual string Code { get; set; }
        public virtual string NameTH { get; set; }
        public virtual string NameEN { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual string CustomerName { get; set; }

        public virtual byte[] Logo { get; set; }

        public virtual IList<ProjectMember> Members { get; protected set; }

        public virtual IList<Timesheet> TimeSheets { get; protected set; }

        public virtual ProjectMember AddMemeber(User user, ProjectRole projectRole)
        {
            var member = new ProjectMember
            {
                Project = this,
                User = user,
                ProjectRole = projectRole,
            };
            Members.Add(member);
            return member;
        }

        public virtual bool ContainsMember(long employeeID)
        {
            var count = (from m in Members where m.Project == this && m.User.EmployeeID == employeeID select m).Count();
            return (count > 0);
        }
    }
}
