using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class TimesheetView
    {
        public string ID { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public DateTime StartDate { get; set; }
        public string StartDateText { get; set; }
        public string Phase { get; set; }
        public string TaskType { get; set; }
        public string MainTaskDesc { get; set; }
        public string SubTaskDesc { get; set; }
        public decimal HourUsed { get; set; }
        public string Remark { get; set; }

        public Guid? GuidID 
        {
            get
            {
                if (!string.IsNullOrEmpty(ID))
                {
                    return Guid.Parse(ID);
                }
                return null;
            }
            set
            {
            }
        }
    }
}