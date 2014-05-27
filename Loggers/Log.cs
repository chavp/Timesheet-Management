using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Loggers
{
    public class Log
    {
        public virtual Guid ID { get; protected set; }
        public virtual DateTime Version { get; protected set; }
        public virtual DateTime EventDate { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Name { get; set; }
        public virtual string Message { get; set; }
        public virtual string Level { get; set; }
    }
}
