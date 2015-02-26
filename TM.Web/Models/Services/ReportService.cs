using Cwn.PM.BusinessModels.Entities;
using Cwn.PM.Reports.Values;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models.Services
{
    public class ReportService
    {
        public static ActualEffortOfResourceManager BuildActualEffortOfResourceManager(
            int empID, DateTime fromDate, DateTime toDate,
            ISession session)
        {

            var actualEffortOfResourceManager = new ActualEffortOfResourceManager
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            var leader = (from u in session.Query<User>()
                          where u.EmployeeID == empID
                          select u).Single();

            var childs = (from u in session.Query<User>()
                          where u.Lead == leader
                          select u).ToList();

            var allRMTimesheets = (from t in session.Query<Timesheet>()
                                   where childs.Contains(t.User) &&
                                   fromDate <= t.ActualStartDate &&
                                   t.ActualStartDate <= toDate
                                   select t).ToList();

            var queryTimesheet = from u in childs
                                 join t in allRMTimesheets on u equals t.User into u_t
                                 //from ut in u_t.DefaultIfEmpty()
                                 select new
                                 {
                                     User = u,
                                     Timesheets = u_t.ToList(),
                                 };

            int index = 0;
            foreach (var item in queryTimesheet)
            {
                ++index;
                item.Timesheets.ForEach(timesheet =>
                {
                    actualEffortOfResourceManager.TimesheetDetails.Add(new TimesheetDetail
                    {
                        Index = index,
                        Date = timesheet.ActualStartDate.GetValueOrDefault(),
                        EmployeeID = item.User.EmployeeID,
                        FullName = item.User.FirstNameTH + " " + item.User.LastNameTH,
                        PositionName = item.User.Position.NameTH,
                        DepartmentName = item.User.Department.NameTH,
                        DivisionName = item.User.Department.Division.NameTH,
                        ProjectCode = timesheet.Project.Code,
                        ProjectRole = timesheet.ProjectRole.NameEN,
                        Phase = timesheet.Phase.NameEN,
                        TaskType = timesheet.TaskType.NameEN,
                        MainTask = timesheet.MainTask,
                        SubTask = timesheet.SubTask,
                        Hours = timesheet.ActualHourUsed,
                    });
                });
            }

            return actualEffortOfResourceManager;
        }
    }
}