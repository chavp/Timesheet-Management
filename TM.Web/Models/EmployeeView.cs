using Cwn.PM.BusinessModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class EmployeeView
    {
        public long ID { get; set; }
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string Display { get; set; }

        public string Position { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }

        public long PositionID { get; set; }
        public long DivisionID { get; set; }
        public long DepartmentID { get; set; }

        public string Email { get; set; }
        public string AppRole { get; set; }

        public long TitleID { get; set; }
        public string NameTH { get; set; }
        public string LastTH { get; set; }
        public string NameEN { get; set; }
        public string LastEN { get; set; }

        public string Status { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastChangedPassword { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StartDateText { get; set; }
        public string EndDateText { get; set; }

        public int TotalProjectMember { get; set; }
        public int TotalTimesheet { get; set; }

        public static EmployeeView Map(User u)
        {
            return new EmployeeView
                                {
                                    ID = u.ID,
                                    EmployeeID = u.EmployeeID,
                                    FullName = u.FirstNameTH + " " + u.LastNameTH,
                                    Display = u.FirstNameTH + " " + u.LastNameEN,
                                    LastChangedPassword = u.LastPasswordChangedDate,
                                    LastLoginDate = u.LastLoginDate,
                                    Position = u.Position.NameTH,
                                    Division = u.Department.Division.NameTH,
                                    Department = u.Department.NameTH,
                                };
        }
    }
}