using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class ProjectDeliveryPhase : Entity
    {
        protected ProjectDeliveryPhase() { }
        public ProjectDeliveryPhase(Project project, DateTime deliveryPhaseDate)
        {
            Project = project;
            DeliveryPhaseDate = deliveryPhaseDate;
            StatusOfProjectDeliveryPhase = Entities.StatusOfProjectDeliveryPhase.InProgress;
        }

        public virtual Project Project { get; protected set; }
        public virtual DateTime DeliveryPhaseDate { get; set; }

        public virtual StatusOfProjectDeliveryPhase StatusOfProjectDeliveryPhase { get; set; }
    }

    public enum StatusOfProjectDeliveryPhase
    {
        InProgress,
        Done
    }
}
