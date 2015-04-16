using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    using Cwn.PM.BusinessModels.Queries;

    public class ProjectCostAccount
        : Entity
    {
        protected ProjectCostAccount() { }

        public ProjectCostAccount(Project project)
        {
            Project = project;
            Entries = new List<ProjectCostAccountEntry>();
        }

        public virtual Project Project { get; protected set; }

        public virtual IList<ProjectCostAccountEntry> Entries { get; protected set; }

        //public virtual decimal EffortBalanceHrs 
        //{
        //    get
        //    {
        //        return Entries.Select(x => x.EffortHrs).Sum();
        //    }
        //}
        //public virtual decimal CostBalance 
        //{
        //    get
        //    {
        //        return Entries.Select(x => x.Cost).Sum();
        //    } 
        //}
        //public virtual int MembersBalance 
        //{
        //    get
        //    {
                //return Entries
                //    .OrderByDescending( x => x.StartDate )
                //    .Select(x => x.Members)
                //    .FirstOrDefault();
        //    }  
        //}

        public virtual decimal EffortBalanceHrs { get; protected set; }
        public virtual decimal CostBalance { get; protected set; }
        public virtual int MembersBalance { get; protected set; }

        public virtual void UpdateEntries()
        {
            var timesheets = Project.TimeSheets;
            var grpByDates = (from t in timesheets group t by 
                             new {
                                 Date = t.ActualStartDate.Value.Date,
                             } 
                             into dateGrp
                             select new
                                         {
                                             Date = dateGrp.Key.Date,
                                             Timesheets = dateGrp.ToList()
                                         });

            
            foreach (var grpByDate in grpByDates)
            {
                var grpTimesheets = grpByDate.Timesheets;
                var entry = (from ent in Entries
                             where ent.StartDate.Date == grpByDate.Date
                             select ent).FirstOrDefault();

                decimal totalEffortHrs = grpTimesheets.Sum(t => t.ActualHourUsed);
                decimal totalCost = 0;

                int totalMembers = timesheets.Where(t => t.ActualStartDate <= grpByDate.Date).Select(t => t.User).Distinct().Count();

                foreach (var item in grpTimesheets)
                {
                    var roleCost = item.ProjectRole.ProjectRoleRates
                        .GetEffectiveRatePerHours(item.ActualStartDate)
                        .FirstOrDefault();

                    totalCost += item.ActualHourUsed * roleCost;
                }

                if (entry != null)
                {
                    entry.Members = totalMembers;
                    entry.EffortHrs = totalEffortHrs;
                    entry.Cost = totalCost;
                }
                else
                {
                    entry = new ProjectCostAccountEntry(this, grpByDate.Date)
                    {
                        Members = totalMembers,
                        EffortHrs = totalEffortHrs,
                        Cost = totalCost,
                    };

                    Entries.Add(entry);
                }

            }
        }

        public virtual void UpdateEntry(IList<Timesheet> timesheets, DateTime startDate, int totalMembers)
        {
            var entry = (from ent in Entries
                         where ent.StartDate.Date == startDate.Date
                         select ent).FirstOrDefault();

            decimal totalEffortHrs = timesheets.Sum(t => t.ActualHourUsed);
            decimal totalCost = 0;

            foreach (var item in timesheets)
            {
                var roleCost = item.ProjectRole.ProjectRoleRates
                    .GetEffectiveRatePerHours(item.ActualStartDate)
                    .FirstOrDefault();

                totalCost += item.ActualHourUsed * roleCost;
            }

            if (entry != null)
            {
                entry.Members = totalMembers;
                entry.EffortHrs = totalEffortHrs;
                entry.Cost = totalCost;
            }
            else
            {
                entry = new ProjectCostAccountEntry(this, startDate.Date)
                {
                    Members = totalMembers,
                    EffortHrs = totalEffortHrs,
                    Cost = totalCost,
                };

                Entries.Add(entry);
            }
        }

        public virtual void UpdateBalance()
        {
            decimal effortBalanceHrs = 0, costBalance = 0;
            int membersBalance = 0;

            effortBalanceHrs = Entries.Select(x => x.EffortHrs).Sum();
            costBalance = Entries.Select(x => x.Cost).Sum();
            membersBalance = Entries
                    .OrderByDescending(x => x.StartDate)
                    .Select(x => x.Members)
                    .FirstOrDefault();

            EffortBalanceHrs = effortBalanceHrs;
            CostBalance = costBalance;
            MembersBalance = membersBalance;
        }
    }
}
