using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class Position
        : Entity
    {
        protected Position() { }
        public Position(string name, decimal cost) 
        {
            NameTH = name;
            NameEN = name;
            NameAbbrTH = name;
            NameAbbrEN = name;
            Cost = cost;
        }

        public virtual string NameTH { get; set; }
        public virtual string NameEN { get; set; }
        public virtual string NameAbbrTH { get; set; }
        public virtual string NameAbbrEN { get; set; }

        /// <summary>
        /// Cost Per Day
        /// </summary>
        public virtual decimal Cost { get; set; }

        //public virtual Department Department { get; protected set; }

        public virtual void ChangeNameOrCost(string name, decimal cost)
        {
            NameTH = name;
            NameEN = name;
            NameAbbrTH = name;
            NameAbbrEN = name;
            Cost = cost;
        }
    }
}
