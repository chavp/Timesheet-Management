using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Reports.Values
{
    public class TimesheetDetail
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectRole { get; set; }
        public int ProjectRoleOrder { get; set; }
        public string Phase { get; set; }
        public int PhaseOrder { get; set; }
        public string TaskType { get; set; }
        public string MainTask { get; set; }
        public string SubTask { get; set; }
        public decimal Hours { get; set; }
        public decimal Cost { get; set; }
        public decimal RoleCost { get; set; }
    }
}
