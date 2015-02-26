using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class AppUser
        : Entity
    {
        public virtual string LoginName { get; set; }

        public virtual User RefUser { get; set; }
    }
}
