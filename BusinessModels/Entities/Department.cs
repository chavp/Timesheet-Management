using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class Department : Entity
    {
        protected Department()
        {
            //Positions = new List<Position>();
        }

        public Department(string name)
            : this()
        {
            //Positions = new List<Position>();
            NameTH = name;
            NameEN = name;
        }

        public Department(string name, Division div)
            : this()
        {
            //Positions = new List<Position>();
            NameTH = name;
            NameEN = name;
            Division = div;
        }

        public virtual string NameTH { get; set; }
        public virtual string NameEN { get; set; }

        public virtual Division Division { get; protected set; }

        public virtual void ChangeNameOrDivision(string name, Division div)
        {
            NameTH = name;
            NameEN = name;
            Division = div;
        }

        public virtual void ChangeName(string name)
        {
            NameTH = name;
            NameEN = name;
        }

        //public virtual IList<Position> Positions { get; protected set; }
    }
}
