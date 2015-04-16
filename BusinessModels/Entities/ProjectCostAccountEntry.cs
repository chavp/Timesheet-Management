using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class ProjectCostAccountEntry
        : Entity
    {
        protected ProjectCostAccountEntry() { }

        public ProjectCostAccountEntry(ProjectCostAccount projectCostAccount, DateTime startDate)
        {
            ProjectCostAccount = projectCostAccount;
            StartDate = startDate;
        }

        public virtual ProjectCostAccount ProjectCostAccount { get; protected set; }
        public virtual DateTime StartDate { get; protected set; }

        public virtual decimal EffortHrs { get; set; }
        public virtual decimal Cost { get; set; }
        public virtual int Members { get; set; }
    }
}
