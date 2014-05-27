using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class ProjectView
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Members { get; set; }

        public string Display 
        {
            get
            {
                if (string.IsNullOrEmpty(Code))
                {
                    return Name;
                }
                else
                {
                    return Code + ": " + Name;
                }
            }
            set
            {
            }
        }
    }
}