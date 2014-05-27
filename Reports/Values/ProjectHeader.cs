using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Reports.Values
{
    public class ProjectHeader
    {
        public ProjectHeader()
        {
            Details = new List<TimesheetDetail>();
        }

        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string CurrentProjectRole { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Members { get; set; }

        public decimal Hours 
        {
            get
            {
                return Details.Sum(c => c.Hours);
            }
        }
        public IList<TimesheetDetail> Details { get; set; }
    }
}
