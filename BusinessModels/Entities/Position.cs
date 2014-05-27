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
        public virtual string NameTH { get; set; }
        public virtual string NameEN { get; set; }
        public virtual string NameAbbrTH { get; set; }
        public virtual string NameAbbrEN { get; set; }
    }
}
