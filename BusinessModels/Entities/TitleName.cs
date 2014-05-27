using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class TitleName : Entity
    {
        public virtual string NameEN { get; set; }
        public virtual string NameTH { get; set; }
    }
}
