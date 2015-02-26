using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class EffortTimesheetItem
    {
        public EffortTimesheetItem()
        {
            Efforts = new List<decimal>();
        }

        public int Group { get; set; }
        public string Date { get; set; }
        public int TotalItems { get; set; }

        public List<decimal> Efforts { get; set; }

        public void InitEfforts(List<string> roles)
        {
            for (int i = 0; i < roles.Count; i++)
            {
                this.Efforts.Add(0);
            }
        }
    }
}