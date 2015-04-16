using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class ProjectDeliveryPhaseView
    {
        public long ID { get; set; }

        public long ProjectID { get; set; }
        public DateTime DeliveryPhaseDate { get; set; }
        public string StatusOfProjectDeliveryPhase { get; set; }
    }
}