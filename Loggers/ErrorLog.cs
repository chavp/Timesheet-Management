using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Loggers
{
    public class ErrorLog : Log
    {
        public virtual string ErrorMessage { get; set; }
        public virtual string StackTrace { get; set; }
    }
}
