using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class ProjectRole : Entity
    {
        public ProjectRole()
        {
            ProjectRoleRates = new List<ProjectRoleRate>();
        }

        public virtual string NameTH { get; set; }
        public virtual string NameEN { get; set; }
        public virtual int Order { get; set; }
        public virtual bool IsOwner { get; set; }

        public virtual IList<ProjectRoleRate> ProjectRoleRates { get; set; }


    }
}
