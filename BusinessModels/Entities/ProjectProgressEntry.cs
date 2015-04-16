using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class ProjectProgressEntry : Entity
    {
        protected ProjectProgressEntry() { }
        public ProjectProgressEntry(ProjectProgress projectProgress)
        {
            ProjectProgress = projectProgress;
            StateOfProgress = StateOfProgress.InProgress;
        }

        public virtual ProjectProgress ProjectProgress { get; set; }

        public virtual DateTime? LastUpdateDate { get; set; }

        public virtual int Amount { get; set; }
        public virtual string Note { get; set; }

        public virtual StateOfProgress StateOfProgress { get; set; }
    }
}
