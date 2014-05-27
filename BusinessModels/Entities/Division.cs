using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class Division : Entity
    {
        public Division()
        {
            Departments = new List<Department>();
        }

        public virtual string NameTH { get; set; }
        public virtual string NameEN { get; set; }

        public virtual Organization Organization { get; protected set; }

        public virtual IList<Department> Departments { get; protected set; }
    }
}
