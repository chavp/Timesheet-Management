using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class Division : Entity
    {
        protected Division()
        {
            Departments = new List<Department>();
        }

        public Division(string name)
            : this()
        {
            NameTH = name;
            NameEN = name;
        }

        public Division(string name, Organization org)
            : this()
        {
            NameTH = name;
            NameEN = name;
            Organization = org;
        }

        public virtual string NameTH { get; set; }
        public virtual string NameEN { get; set; }

        public virtual Organization Organization { get; protected set; }

        public virtual IList<Department> Departments { get; protected set; }

        public virtual void ChangeName(string name)
        {
            NameTH = name;
            NameEN = name;
        }
    }
}
