using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class DepartmentTimesheetYearItem
    {
        public int Year { get; set; }
        public List<decimal> Costs { get; set; }

        public void InitCosts(List<string> departs)
        {
            this.Costs = new List<decimal>();
            for (int i = 0; i < departs.Count; i++)
            {
                this.Costs.Add(0);
            }
        }
    }
}