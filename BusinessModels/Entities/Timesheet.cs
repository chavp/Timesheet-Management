using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class Timesheet
        : BigEntity
    {
        protected Timesheet()
        {
            
        }

        public Timesheet(
            Project project, 
            ProjectRole projectRole,
            User user)
        {
            Project = project;
            ProjectRole = projectRole;
            User = user;

            ActualUserPosition = user.Position;
            ActualUserDepartment = user.Department;
        }

        public virtual Project Project { get; set; }
        public virtual ProjectRole ProjectRole { get; set; }
        public virtual User User { get; set; }

        public virtual Phase Phase { get; set; }
        public virtual string MainTask { get; set; }
        public virtual string SubTask { get; set; }
        public virtual TaskType TaskType { get; set; }

        public virtual Position ActualUserPosition { get; set; }
        public virtual Department ActualUserDepartment { get; set; }
        public virtual DateTime? ActualStartDate { get; set; }
        public virtual decimal ActualHourUsed { get; set; }
        public virtual string Remark { get; set; }

        public virtual bool IsOT { get; set; }
    }
}
