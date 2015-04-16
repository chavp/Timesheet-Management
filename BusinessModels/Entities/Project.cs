using Cwn.PM.BusinessModels.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class Project
        : Entity
    {
        public Project()
        {
            Members = new List<ProjectMember>();
            //TimeSheets = new List<Timesheet>();
        }

        public virtual string Code { get; set; }
        public virtual string NameTH { get; set; }
        public virtual string NameEN { get; set; }

        /// <summary>
        /// วันที่บันทึกเข้าระบบ ตาม SharPoint
        /// </summary>
        public virtual DateTime? StartDate { get; set; }

        /// <summary>
        /// วันที่บันทึกเข้าระบบ ตาม SharPoint
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        public virtual string CustomerName { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual ProjectStatus Status { get; set; }

        public virtual byte[] Logo { get; set; }

        /// <summary>
        /// วันที่เริ่มโครงการตามสัญญา
        /// </summary>
        public virtual DateTime? ContractStartDate { get; set; }

        /// <summary>
        /// วันที่สิ้นสุดโครงการตามสัญญา
        /// </summary>
        public virtual DateTime? ContractEndDate { get; set; }

        /// <summary>
        /// วันที่ส่งมอบงานจริง                   
        /// </summary>
        public virtual DateTime? DeliverDate { get; set; }

        /// <summary>
        /// วันที่เริ่มต้น ของ warranty หรือ วันที่รับมอบงาน               
        /// </summary>
        public virtual DateTime? WarrantyStartDate { get; set; }

        /// <summary>
        /// วันที่สิ้นสุด ของ warranty                      
        /// </summary>
        public virtual DateTime? WarrantyEndDate { get; set; }

        /// <summary>
        /// มูลค่าโครงการที่ประมาณการไว้                                      
        /// </summary>
        public virtual decimal EstimateProjectValue { get; set; }

        /// <summary>
        /// มูลค่าโครงการ                                
        /// </summary>
        public virtual decimal ProjectValue { get; set; }

        public virtual bool IsNonProject { get; set; }

        public virtual IList<ProjectMember> Members { get; protected set; }

        public virtual IList<Timesheet> TimeSheets { get; protected set; }

        public virtual ProjectMember AddMemeber(User user, ProjectRole projectRole)
        {
            var member = new ProjectMember
            {
                Project = this,
                User = user,
                ProjectRole = projectRole,
            };
            Members.Add(member);
            return member;
        }

        //public virtual ProjectProgress Progress { get; set; }

        //public virtual ProjectProgressUpdateLog UpdateProgress(int newProgress, DateTime updateDate)
        //{
        //    Progress = newProgress;
        //    var newLog = new ProjectProgressUpdateLog(this, updateDate, newProgress);
        //    return newLog;
        //}

        public virtual bool ContainsMember(long employeeID)
        {
            var count = (from m in Members where m.Project == this && m.User.EmployeeID == employeeID select m).Count();
            return (count > 0);
        }

    }
}
