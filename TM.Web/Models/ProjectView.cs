using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class ProjectView
    {
        public ProjectView()
        {
            InitMembers = new List<long>();

        }

        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal TotalEffort { get; set; }
        public decimal TotalCost { get; set; }
        public int Members { get; set; }

        public string NameTH { get; set; }
        public string NameEN { get; set; }
        public long CustomerID { get; set; }

        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public DateTime? DeliverDate { get; set; }
        public DateTime? WarrantyStartDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
        public decimal EstimateProjectValue { get; set; }
        public decimal ProjectValue { get; set; }
        public List<long> InitMembers { get; set; }

        public DateTime? LatestUpdateProgress { get; set; }

        public int TotalTimesheet { get; set; }

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

        public string StatusDisplay { get; set; }
        public long StatusID { get; set; }

        public int Progress { get; set; }
        public string StateOfProgress { get; set; }
        public bool IsOwner { get; set; }

        public int ProjectDeliveryPhaseCount { get; set; }

        public List<ProjectDeliveryPhaseView> ProjectDeliveryPhases { get; set; }

        public bool IsProjectAccept { get; set; }
        public int TotalAcceptedProject { get; set; }


    }
}