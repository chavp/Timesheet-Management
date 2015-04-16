using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class AcceptedProject
        : Entity
    {
        protected AcceptedProject() { }

        public AcceptedProject(Project project, User acceptBy)
        {
            AcceptBy = acceptBy;
            Project = project;
        }

        public virtual User AcceptBy { get; set; }
        public virtual Project Project { get; set; }
    }
}
