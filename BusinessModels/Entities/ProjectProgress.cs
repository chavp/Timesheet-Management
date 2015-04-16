using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class ProjectProgress : Entity
    {
        protected ProjectProgress() { }
        public ProjectProgress(Project project)
        {
            Project = project;
            Entries = new List<ProjectProgressEntry>();
        }

        public virtual int PercentageProgress 
        {
            get
            {
                var progress = Entries.OrderByDescending(x => x.LastUpdateDate ).Select(x => x.Amount).FirstOrDefault();
                return progress;
            }
        }

        public virtual StateOfProgress StateOfProgress
        {
            get
            {
                var progress = Entries.OrderByDescending(x => x.LastUpdateDate).Select(x => x.StateOfProgress).FirstOrDefault();
                return progress;
            }
        }

        public virtual Project Project { get; protected set; }

        public virtual IList<ProjectProgressEntry> Entries { get; protected set; }

        public virtual void UpdateProgress(int percentageProgress, string stateOfProgress)
        {
            var currentDate = DateTime.Now.Date;

            var currentEntry = Entries.Where(x => x.LastUpdateDate == currentDate).SingleOrDefault();

            if (currentEntry == null)
            {
                currentEntry = new ProjectProgressEntry(this)
                {
                    LastUpdateDate = currentDate
                };

                Entries.Add(currentEntry);
            }
            StateOfProgress sop;
            if (Enum.TryParse<StateOfProgress>(stateOfProgress, out sop))
            {
                currentEntry.StateOfProgress = sop;
            }

            currentEntry.Amount = percentageProgress;
        }
    }

    public enum StateOfProgress
    {
        InProgress,
        Done
    }
}
