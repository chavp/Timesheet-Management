using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Loggers
{
    public class ProfileLog : Log
    {
        public virtual string UserID { get; set; }
        public virtual string Action { get; set; }
        public virtual string Controller { get; set; }
        public virtual TimeSpan ElapsedTime { get; set; }
        public virtual string UserHostAddress { get; set; }
        public virtual string QueryString { get; set; }
        public virtual string PostData { get; set; }
    }
}
