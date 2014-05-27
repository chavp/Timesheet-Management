using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class ProjectRoleRate
        : Entity
    {
        public ProjectRoleRate()
        {
            EffectiveStart = new DateTime(2014, 1, 1);
            EffectiveEnd = null;
        }

        public virtual ProjectRole ProjectRole { get; set; }
        /// <summary>
        /// Cost Per Day
        /// </summary>
        public virtual decimal Cost { get; set; }

        public virtual DateTime? EffectiveStart { get; set; }
        public virtual DateTime? EffectiveEnd { get; set; }
    }
}
