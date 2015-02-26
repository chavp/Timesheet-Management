using Cwn.PM.BusinessModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Values
{
    public class ProjectProgressUpdateLog
         : BigEntity
    {
        protected ProjectProgressUpdateLog()
        {
        }

        public ProjectProgressUpdateLog(Project project, DateTime updatedDate, int updatedValue)
        {
            Project = project;
            UpdatedDate = updatedDate;
            UpdatedValue = updatedValue;
        }

        public virtual Project Project { get; protected set; }
        public virtual DateTime UpdatedDate { get; protected set; }
        public virtual int UpdatedValue { get; set; }

    }
}