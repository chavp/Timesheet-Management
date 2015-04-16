using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Extensions
{
    using Cwn.PM.BusinessModels.Entities;
    using Cwn.PM.Reports.Values;
    using PJ_CWN019.TM.Web.Models;

    public static class ViewModelConverter
    {
        public static TimesheetDetail ToViewModel(this Timesheet timesheet, int index,
            Project project,
            decimal totalCost, decimal roleCost)
        {
            return new TimesheetDetail
                    {
                        Index = index,
                        Date = timesheet.ActualStartDate.GetValueOrDefault(),
                        EmployeeID = timesheet.User.EmployeeID,
                        FullName = timesheet.User.FullName,

                        ProjectCode = project.Code,
                        ProjectName = project.NameTH,

                        ProjectStartDate = project.StartDate.ToPresentDateString(),
                        ProjectEndDate = project.EndDate.ToPresentDateString(),

                        ProjectRole = timesheet.ProjectRole.NameTH,
                        ProjectRoleOrder = timesheet.ProjectRole.Order,
                        Phase = timesheet.Phase.NameTH,
                        PhaseOrder = timesheet.Phase.Order,
                        TaskType = timesheet.TaskType.NameTH,
                        MainTask = timesheet.MainTask,
                        SubTask = timesheet.SubTask,
                        Hours = timesheet.ActualHourUsed,

                        Cost = totalCost,
                        RoleCost = roleCost,
                    };

        }

        public static ProjectView ToViewModel(this Project prj, ProjectProgress pg, int totalMembers = 0, int totalTimesheet = 0)
        {
            var projectStatus = (prj.Status != null) ? prj.Status.ID : 0;
            var projectStatusDisplay = (prj.Status != null) ? prj.Status.Name : string.Empty;

            long customerID = 0;
            if (prj.Customer != null)
            {
                customerID = prj.Customer.ID;
            }

            int prjProgress = 0;
            if (pg != null) prjProgress = pg.PercentageProgress;

            string stateOfProgress = string.Empty;
            if (pg != null) stateOfProgress = pg.StateOfProgress.ToString();

            return new ProjectView
                        {
                            ID = prj.ID,
                            Code = prj.Code,
                            Name = prj.NameTH,
                            NameTH = prj.NameTH,
                            NameEN = prj.NameEN,
                            CustomerID = customerID,
                            Members = totalMembers,
                            StartDate = prj.StartDate,
                            EndDate = prj.EndDate,
                            StatusID = projectStatus,
                            StatusDisplay = projectStatusDisplay,

                            ContractStartDate = prj.ContractStartDate,
                            ContractEndDate = prj.ContractEndDate,
                            DeliverDate = prj.DeliverDate,
                            WarrantyStartDate = prj.WarrantyStartDate,
                            WarrantyEndDate = prj.WarrantyEndDate,
                            EstimateProjectValue = prj.EstimateProjectValue,
                            ProjectValue = prj.ProjectValue,

                            Progress = prjProgress,
                            TotalTimesheet = totalTimesheet,
                            StateOfProgress = stateOfProgress,
                        };
        }
        public static List<ProjectDeliveryPhaseView> ToViewModel(this List<ProjectDeliveryPhase> pdhList)
        {
            var results = new List<ProjectDeliveryPhaseView>();
            foreach (var item in pdhList)
            {
                results.Add(new ProjectDeliveryPhaseView
                {
                    ID = item.ID,
                    ProjectID =item.Project.ID,
                    DeliveryPhaseDate = item.DeliveryPhaseDate,
                    StatusOfProjectDeliveryPhase = item.StatusOfProjectDeliveryPhase.ToString()
                });
            }
            return results;
        }
        public static DateTime FirstDayOfWeek(this DateTime date)
        {
            var candidateDate = date;
            while (candidateDate.DayOfWeek != DayOfWeek.Sunday)
            {
                candidateDate = candidateDate.AddDays(-1);
            }
            return candidateDate;
        }
        public static DateTime LastDayOfWeek()
        {
            return LastDayOfWeek(DateTime.Today);
        }
        public static DateTime LastDayOfWeek(this DateTime date)
        {
            var candidateDate = date;
            while (candidateDate.DayOfWeek != DayOfWeek.Saturday)
            {
                candidateDate = candidateDate.AddDays(1);
            }
            return candidateDate;
        }

        public static DateTime FirstDayOfCurrentWeek()
        {
            return FirstDayOfWeek(DateTime.Today);
        } 


    }
}