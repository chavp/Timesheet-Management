using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class ProjectTimeline
    {
        public ProjectTimeline()
        {
            ContractStartDate = string.Empty;
            ContractEndDate = string.Empty;
            DeliverDate = string.Empty;
            WarrantyStartDate = string.Empty;
            WarrantyEndDate = string.Empty;
        }

        public string Name { get; set; }
        public string StartDate { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }
        public string DeliverDate { get; set; }
        public string WarrantyStartDate { get; set; }
        public string WarrantyEndDate { get; set; }
        public int Progress { get; set; }
    }
}