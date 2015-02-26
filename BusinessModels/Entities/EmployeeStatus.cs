using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public enum EmployeeStatus
    {
        Work, // การทํางาน (doing jobs)
        Resign, // ลาออก (give up one's job) (n) = resignation 
        Retirement // ปลดเกษียณ (n) = retirement (stop working because of old age or illness) 
    }
}
