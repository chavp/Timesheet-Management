using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class ProjectRole : Entity
    {
        protected ProjectRole()
        {
            ProjectRoleRates = new List<ProjectRoleRate>();
        }

        public ProjectRole(string name, int order)
            : this()
        {
            NameTH = name;
            NameEN = name;
            Order = order;
        }

        public virtual string NameTH { get; set; }
        public virtual string NameEN { get; set; }
        public virtual int Order { get; set; }
        public virtual bool IsOwner { get; set; }
        public virtual bool IsNonRole { get; set; }

        public virtual void ChangeNameOrOrder(string name, int order)
        {
            NameTH = name;
            NameEN = name;
            Order = order;
        }

        public virtual IList<ProjectRoleRate> ProjectRoleRates { get; set; }
    }
}
