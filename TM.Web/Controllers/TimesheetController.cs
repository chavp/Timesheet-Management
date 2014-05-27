﻿
using PJ_CWN019.TM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PJ_CWN019.TM.Web.Controllers
{
    using Cwn.PM.BusinessModels.Entities;
    using Cwn.PM.BusinessModels.Queries;
    using Cwn.PM.Reports.Values;
    using log4net;
    using NHibernate;
    using NHibernate.Linq;
    using PJ_CWN019.TM.Web.Filters;
    using PJ_CWN019.TM.Web.Models;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Reflection;
    using System.Web.Security;
    using WebMatrix.WebData;

    [ErrorLog]
    [ProfileLog]
    [Authorize(Roles = "Member, Manager, Admin, ProjectOwner")]
    [FourceToChangeAttribute]
    public class TimesheetController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISessionFactory _sessionFactory = null;
        public TimesheetController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ActionResult Index()
        {
            throw new Exception("5555");
            return View();
        }

        string dateFormat = "dd/MM/yyyy";
        public ActionResult Timesheet()
        {
            ViewBag.SearchFromDate = FirstDayOfCurrentWeek().ToString(dateFormat, new CultureInfo("en-US"));
            ViewBag.SearchToDate = LastDayOfWeek().ToString(dateFormat, new CultureInfo("en-US"));
            ViewBag.MaxDate = DateTime.Now.ToString(dateFormat, new CultureInfo("en-US"));
            return View();
        }

        public JsonResult GetAllProject(string query, int start, int limit, string includeAll = null)
        {
            var viewList = new List<ProjectView>();
            if (!string.IsNullOrEmpty(includeAll))
            {
                viewList.Add(new ProjectView
                {
                    ID = -1,
                    Code = "",
                    Name = "ทั้งหมด",
                });
            }

            using (var session = _sessionFactory.OpenSession())
            {
                var user = (from u in session.Query<User>()
                            where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                            select u).First();

                //var q = from p in session.Query<Project>()
                //        where p.NameEN.Contains(query)
                //        || p.NameTH.Contains(query)
                //        || p.Code.Contains(query)
                //        select p;

                var memberProjects = (from p in session.Query<Project>()
                                      where p.NameEN.Contains(query)
                        || p.NameTH.Contains(query)
                        || p.Code.Contains(query)
                                      join m in session.Query<ProjectMember>() on p equals m.Project
                                      where m.User == user
                                      && m.ProjectRole != null
                                      select new { Project = p, Member = m });

                memberProjects = memberProjects.OrderBy(prj => prj.Project.Code);
                foreach (var p in memberProjects)
                {
                    viewList.Add(new ProjectView
                    {
                        ID = p.Project.ID,
                        Code = p.Project.Code,
                        Name = p.Project.NameTH,
                    });
                }
            }

            var result = new
            {
                data = viewList,
                total = viewList.Count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTimesheet(int projectID, string fromDateText, string toDateText,
            int start, int limit)
        {
            var viewList = new List<TimesheetView>();
            int count;

            var fromDate = DateTime.ParseExact(fromDateText, dateFormat, new CultureInfo("en-US"));
            var toDate = DateTime.ParseExact(toDateText, dateFormat, new CultureInfo("en-US"));

            using (var session = _sessionFactory.OpenSession())
            {
                var user = (from u in session.Query<User>()
                            where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                            select u).First();

                var q = from t in session.Query<Timesheet>()
                        where fromDate <= t.ActualStartDate && t.ActualStartDate <= toDate
                        && t.User == user
                        select t;

                if (projectID > 0)
                {
                    q = q.Where(t => t.Project.ID == projectID);
                }

                q = q.OrderByDescending(t => t.ActualStartDate);
                q = q.OrderBy(t => t.Project.Code);

                count = q.Count();

                //var order = q.OrderByDescending(t => new { t.ActualStartDate, t.CreatedAt });
                
                foreach (var timesheet in q.Skip(start).Take(limit))
                {
                    viewList.Add(new TimesheetView
                    {
                        ID = timesheet.ID.ToString(),
                        GuidID = timesheet.ID,
                        ProjectCode = timesheet.Project.Code,
                        ProjectName = timesheet.Project.NameTH,
                        StartDate = timesheet.ActualStartDate.GetValueOrDefault(),
                        Phase = timesheet.Phase.NameTH,
                        TaskType = timesheet.TaskType.NameTH,
                        MainTaskDesc = timesheet.MainTask,
                        SubTaskDesc = timesheet.SubTask,
                        HourUsed = timesheet.ActualHourUsed,
                        Remark = timesheet.Remark
                    });
                }
            }

            var result = new
            {
                data = viewList,
                total = count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllPhase()
        {
            var viewList = new List<PhaseView>();

            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var q = from p in session.Query<Phase>()
                        select p;

                q = q.OrderBy(p => p.Order);
                foreach (var p in q)
                {
                    viewList.Add(new PhaseView
                    {
                        ID = p.ID,
                        Name = p.NameTH,
                    });
                }
            }

            var result = new
            {
                data = viewList,
                total = viewList.Count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllTaskType()
        {
            var viewList = new List<TaskTypeView>();

            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var q = from p in session.Query<TaskType>()
                        select p;

                q = q.OrderBy(p => p.Order);
                foreach (var p in q)
                {
                    viewList.Add(new TaskTypeView
                    {
                        ID = p.ID,
                        Name = p.NameTH,
                    });
                }
            }

            var result = new
            {
                data = viewList,
                total = viewList.Count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMainTask(string query)
        {
            var viewList = new List<MainTaskView>();

            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var q = from p in session.Query<MainTask>()
                        where p.Desc.Contains(query)
                        select p;

                q = q.OrderBy(mt => mt.Desc);
                foreach (var p in q)
                {
                    viewList.Add(new MainTaskView
                    {
                        ID = p.ID,
                        Name = p.Desc,
                    });
                }
            }

            var result = new
            {
                data = viewList,
                total = viewList.Count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddTimesheet(TimesheetView timesheetView)
        {
            var success = false;
            var msg = string.Empty;
            //throw new Exception("55555");
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                //get User
                var me = (from u in session.Query<User>()
                          where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                          select u).Single();

                var startDate = DateTime.ParseExact(timesheetView.StartDateText, dateFormat, new CultureInfo("en-US"));

                // ActualHourUsed guard
                var totalHourUsed = (from t in session.Query<Timesheet>()
                                     where t.User == me
                                     && t.ActualStartDate == startDate
                                     select t.ActualHourUsed).ToList().Sum();
                if (totalHourUsed + timesheetView.HourUsed > 24)
                {
                    return Json(new { success = false, message = "ท่านเพิ่มเวลาที่ใช้ใน Timesheet เกิน 24 ชั่วโมง/วัน แล้ว" }, JsonRequestBehavior.AllowGet);
                }
                // end

                var project = (from p in session.Query<Project>()
                               where p.ID == long.Parse(timesheetView.ProjectCode)
                               select p).Single();

                var member = (from m in project.Members
                              where m.User == me
                              && m.Project == project
                              select m).Single();

                var phase = (from ph in session.Query<Phase>()
                             where ph.ID == long.Parse(timesheetView.Phase)
                             select ph).Single();

                var taskType = (from tt in session.Query<TaskType>()
                                where tt.ID == long.Parse(timesheetView.TaskType)
                                select tt).Single();

                var newTimesheet = new Timesheet
                (member.Project, member.ProjectRole, member.User)
                {
                    ActualStartDate = startDate,
                    Phase = phase,
                    TaskType = taskType,
                    SubTask = timesheetView.SubTaskDesc,
                    MainTask = timesheetView.MainTaskDesc,
                    Remark = timesheetView.Remark,
                    ActualHourUsed = timesheetView.HourUsed,
                };

                project.TimeSheets.Add(newTimesheet);

                transaction.Commit();

                msg = "บันทึก Timesheet เสร็จสมบูรณ์";
                success = true;
            }

            //What is important is to return the data as JSON.
            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveTimesheet(TimesheetView timesheetView)
        {
            var success = false;
            var msg = string.Empty;

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                //get User
                var me = (from u in session.Query<User>()
                          where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                          select u).Single();
                var startDate = DateTime.ParseExact(timesheetView.StartDateText, dateFormat, new CultureInfo("en-US"));

                // ActualHourUsed guard
                var totalHourUsed = (from t in session.Query<Timesheet>()
                                     where t.User == me
                                     && t.ActualStartDate == startDate
                                     && t.ID != timesheetView.GuidID
                                     select t.ActualHourUsed).ToList().Sum();

                if (totalHourUsed + timesheetView.HourUsed > 24)
                {
                    return Json(new { success = false, message = "ท่านได้แก้ไขเวลาที่ใช้ใน Timesheet เกิน 24 ชั่วโมง/วัน" }, JsonRequestBehavior.AllowGet);
                }
                // end

                var oldTimesheetView = (from t in session.Query<Timesheet>()
                                        where t.ID == timesheetView.GuidID
                                        select t).Single();

                //var project = (from p in session.Query<Project>()
                //               where p.Code == timesheetView.ProjectCode
                //               select p).Single();

                var phase = (from ph in session.Query<Phase>()
                             where ph.NameEN == timesheetView.Phase
                             select ph).Single();

                var taskType = (from tt in session.Query<TaskType>()
                                where tt.NameEN == timesheetView.TaskType
                                select tt).Single();

                //oldTimesheetView.Member.Project = project;
                oldTimesheetView.ActualStartDate = startDate;
                oldTimesheetView.Phase = phase;
                oldTimesheetView.TaskType = taskType;
                oldTimesheetView.MainTask = timesheetView.MainTaskDesc;
                oldTimesheetView.SubTask = timesheetView.SubTaskDesc;
                oldTimesheetView.ActualHourUsed = timesheetView.HourUsed;
                oldTimesheetView.Remark = timesheetView.Remark;

                transaction.Commit();

                msg = "บันทึก Timesheet เสร็จสมบูรณ์";
                success = true;
            }
            //What is important is to return the data as JSON.
            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteTimesheet(List<string> listOfDelete)
        {
            var success = false;
            var msg = string.Empty;

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                foreach (var item in listOfDelete)
                {
                    var timesheet = (from t in session.Query<Timesheet>()
                                     where t.ID == Guid.Parse(item)
                                     select t).Single();

                    session.Delete(timesheet);
                }
                transaction.Commit();

                msg = "ลบ Timesheet เสร็จสมบูรณ์";
                success = true;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Report()
        {
            var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            ViewBag.FirstDayOfMonth = firstDayOfMonth.ToString(dateFormat, new CultureInfo("en-US"));
            ViewBag.LastDayOfMonth = lastDayOfMonth.ToString(dateFormat, new CultureInfo("en-US"));

            //check PM
            ViewBag.IsOwner = false;
            if (Roles.IsUserInRole("ProjectOwner"))
            {
                ViewBag.IsOwner = true;
            }
            return View();
        }

        public JsonResult GetReport()
        {
            var viewList = new List<TimesheetReportView>();

            viewList.Add(new TimesheetReportView
            {
                ID = 1,
                Name = "Actual Effort for Person",
            });

            if (Roles.IsUserInRole("Manager"))
            {
                viewList.Add(new TimesheetReportView
                {
                    ID = 2,
                    Name = "Actual Effort for Department",
                });
            }

            if (Roles.IsUserInRole("ProjectOwner"))
            {
                viewList.Add(new TimesheetReportView
                {
                    ID = 3,
                    Name = "Actual Cost for Person",
                });

                viewList.Add(new TimesheetReportView
                {
                    ID = 5,
                    Name = "Actual Cost for All Person",
                });

                viewList.Add(new TimesheetReportView
                {
                    ID = 4,
                    Name = "Actual Cost for Project",
                });
            }

            var result = new
            {
                data = viewList,
                total = viewList.Count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Member, ProjectOwner")]
        public JsonResult ExportReport(TimesheetReportView timesheetReportView)
        {
            //byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Reports/Invoices/" + Table.First(x => x.ID == id).ID + ".pdf"));
            //string fileName = Table.First(x => x.ID == id).ID.ToString();
            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);

            string filename = "actual_effort_for_person_{0}_{1}.xlsx";
            int randomFolder = new Random(DateTime.Now.Second).Next(1, int.MaxValue);
            string fullFilepath = Server.MapPath(@"~\Export\" + randomFolder + "\\");
            string exportPath = Url.Content(@"~\Export\" + randomFolder + "\\");
            if (!Directory.Exists(fullFilepath))
            {
                Directory.CreateDirectory(fullFilepath);
            }

            string message = string.Empty;
            bool success = true;

            //D:\projects\PJ_CWN019\source code\pjcwn019\pjcwn019_Solution\PJ-CWN019.TM.Web\Export
            if (timesheetReportView.Name == "1") // Actual Effort for Person
            {
                #region Actual Effort for Person
                if (timesheetReportView.Type == "1")// Excel
                {
                    if (timesheetReportView.Data == "1")// ทั้งหมด
                    {
                        using (var session = _sessionFactory.OpenSession())
                        {
                            //get User
                            var me = (from u in session.Query<User>()
                                      where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                      select u).Single();

                            var memberProjects = (from p in session.Query<Project>()
                                                  join m in session.Query<ProjectMember>() on p equals m.Project
                                                  where m.User == me
                                                  select new { Project = p, Member = m });

                            if (memberProjects.Count() == 0)
                            {
                                return Json(new
                                {
                                    success = false,
                                    message = "ไม่พบโครงการของพนักงานท่านนี้",
                                }, JsonRequestBehavior.AllowGet);
                            }

                            var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                            var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                            var actualEffortForPerson = new ActualEffortForPerson
                            {
                                FromDate = fromDate,
                                ToDate = toDate,
                            };

                            actualEffortForPerson.EmployeeID = me.EmployeeID;
                            actualEffortForPerson.FullName = me.FullName;
                            actualEffortForPerson.Position = me.Position.NameTH;
                            actualEffortForPerson.Department = me.Department.NameTH;

                            int index = 0;
                            foreach (var pro in memberProjects)
                            {
                                var header1 = new ProjectHeader
                                {
                                    ProjectCode = pro.Project.Code,
                                    ProjectName = pro.Project.NameTH,

                                    CurrentProjectRole = pro.Member.ProjectRole.NameTH, // ตำแหน่งปัจจุบันในโครงการ
                                };
                                actualEffortForPerson.DetailHeaders.Add(header1);

                                //fill only memebr timesheet
                                var timesheet = from t in pro.Project.TimeSheets
                                                where t.User == me
                                                && fromDate <= t.ActualStartDate && t.ActualStartDate <= toDate
                                                select t;

                                foreach (var item in timesheet)
                                {
                                    ++index;
                                    header1.Details.Add(new TimesheetDetail
                                    {
                                        Index = index,
                                        Date = item.ActualStartDate.GetValueOrDefault(),
                                        ProjectCode = pro.Project.Code,
                                        ProjectRole = item.ProjectRole.NameTH,
                                        ProjectRoleOrder = item.ProjectRole.Order,
                                        Phase = item.Phase.NameTH,
                                        PhaseOrder = item.Phase.Order,
                                        TaskType = item.TaskType.NameTH,
                                        MainTask = item.MainTask,
                                        SubTask = item.SubTask,
                                        Hours = item.ActualHourUsed,
                                    });
                                }
                            }

                            filename = "actual_effort_for_person_{0}_{1}.xlsx";

                            filename = string.Format(filename, actualEffortForPerson.EmployeeID, DateTime.Now.ToString("yyyyMMdd"));

                            fullFilepath = fullFilepath + filename;

                            actualEffortForPerson.WriteExcel(fullFilepath);
                        }
                    }
                    else if (timesheetReportView.Data == "2")// Summary
                    {
                        throw new NotImplementedException();
                    }
                    else if (timesheetReportView.Data == "3")// Detail
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                #endregion
            }
            else if (Roles.IsUserInRole("ProjectOwner"))
            {
                if (timesheetReportView.Name == "3") // Actual Cost for Person
                {
                    #region Actual Cost for Person
                    if (timesheetReportView.Type == "1")// Excel
                    {
                        if (timesheetReportView.Data == "1")// ทั้งหมด
                        {
                            using (var session = _sessionFactory.OpenSession())
                            {
                                var owner = (from u in session.Query<User>()
                                             where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                             select u).Single();

                                //get User
                                var empTarget = (from u in session.Query<User>()
                                                 where u.EmployeeID.ToString() == timesheetReportView.EmployeeID
                                                 select u).Single();

                                var memberProjects = (from p in session.Query<Project>()
                                                      join m in session.Query<ProjectMember>() on p equals m.Project
                                                      where m.User == empTarget
                                                      select new { Project = p, Member = m });

                                if (memberProjects.Count() == 0)
                                {
                                    return Json(new
                                    {
                                        success = false,
                                        message = "ไม่พบโครงการของพนักงานท่านนี้",
                                    }, JsonRequestBehavior.AllowGet);
                                }

                                var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                                var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                                var actualCostForPerson = new ActualCostForPerson
                                {
                                    FromDate = fromDate,
                                    ToDate = toDate,
                                };

                                actualCostForPerson.EmployeeID = empTarget.EmployeeID;
                                actualCostForPerson.FullName = empTarget.FullName;
                                actualCostForPerson.Position = empTarget.Position.NameTH;
                                actualCostForPerson.Department = empTarget.Department.NameTH;

                                int index = 0;
                                foreach (var pro in memberProjects)
                                {
                                    var isOwner = (from pm in pro.Project.Members where pm.User == owner select pm.ProjectRole.IsOwner).FirstOrDefault();
                                    if (isOwner)
                                    {
                                        var header1 = new ProjectHeader
                                        {
                                            ProjectCode = pro.Project.Code,
                                            ProjectName = pro.Project.NameTH,
                                            CurrentProjectRole = pro.Member.ProjectRole.NameTH
                                        };
                                        actualCostForPerson.DetailHeaders.Add(header1);

                                        //fill only memebr timesheet
                                        foreach (var item in pro.Project.TimeSheets.Where(m => m.User == empTarget))
                                        {
                                            ++index;
                                            var roleCost = item.ProjectRole.ProjectRoleRates
                                                .GetEffectiveRatePerHours(item.ActualStartDate)
                                                .FirstOrDefault();

                                            var totalCost = item.ActualHourUsed * roleCost;

                                            header1.Details.Add(new TimesheetDetail
                                            {
                                                Index = index,
                                                Date = item.ActualStartDate.GetValueOrDefault(),
                                                ProjectCode = pro.Project.Code,
                                                ProjectRole = item.ProjectRole.NameTH,
                                                ProjectRoleOrder = item.ProjectRole.Order,
                                                Phase = item.Phase.NameTH,
                                                PhaseOrder = item.Phase.Order,
                                                TaskType = item.TaskType.NameTH,
                                                MainTask = item.MainTask,
                                                SubTask = item.SubTask,
                                                Hours = item.ActualHourUsed,
                                                Cost = totalCost,
                                                RoleCost = roleCost,
                                            });
                                        }
                                    }
                                }

                                filename = "actual_cost_for_person_{0}_{1}.xlsx";
                                filename = string.Format(filename, actualCostForPerson.EmployeeID, DateTime.Now.ToString("yyyyMMdd"));
                                fullFilepath = fullFilepath + filename;
                                actualCostForPerson.WriteExcel(fullFilepath);
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    #endregion
                }
                else if (timesheetReportView.Name == "4") // Actual Cost for Project
                {
                    #region Actual Cost for Project
                    if (timesheetReportView.Type == "1")// Excel
                    {
                        if (timesheetReportView.Data == "1")// ทั้งหมด
                        {
                            using (var session = _sessionFactory.OpenSession())
                            {
                                var projects = (from p in session.Query<Project>()
                                                where p.Code == timesheetReportView.ProjectCode
                                                select p).ToList();

                                var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                                var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                                var actualCostForProjects = new ActualCostForProjects
                                {
                                    FromDate = fromDate,
                                    ToDate = toDate,
                                };

                                foreach (var project in projects)
                                {
                                    var actualCostForProject = new ActualCostForProject
                                    {
                                        FromDate = fromDate,
                                        ToDate = toDate,
                                    };

                                    actualCostForProjects.ActualCostForProjectList.Add(actualCostForProject);
                                    var header1 = new ProjectHeader
                                    {
                                        ProjectCode = project.Code,
                                        ProjectName = project.NameTH,
                                    };
                                    actualCostForProject.DetailHeaders.Add(header1);
                                    actualCostForProject.ProjectCode = project.Code;
                                    actualCostForProject.ProjectName = project.NameTH;
                                    var index = 0;
                                    foreach (var timesheet in project.TimeSheets.Where(t =>
                                        fromDate <= t.ActualStartDate && t.ActualStartDate <= toDate))
                                    {
                                        ++index;
                                        var cost = timesheet.ProjectRole.ProjectRoleRates
                                            .GetEffectiveRatePerHours(timesheet.ActualStartDate)
                                            .FirstOrDefault();
                                        var totalCost = timesheet.ActualHourUsed * cost;
                                        header1.Details.Add(new TimesheetDetail
                                        {
                                            Index = index,
                                            Date = timesheet.ActualStartDate.GetValueOrDefault(),
                                            EmployeeID = timesheet.User.EmployeeID,
                                            FullName = timesheet.User.FullName,
                                            ProjectCode = project.Code,
                                            ProjectRole = timesheet.ProjectRole.NameTH,
                                            ProjectRoleOrder = timesheet.ProjectRole.Order,
                                            Phase = timesheet.Phase.NameEN,
                                            PhaseOrder = timesheet.Phase.Order,
                                            TaskType = timesheet.TaskType.NameTH,
                                            MainTask = timesheet.MainTask,
                                            SubTask = timesheet.SubTask,
                                            Hours = timesheet.ActualHourUsed,
                                            Cost = totalCost,
                                            RoleCost = cost,
                                        });
                                    }
                                }

                                filename = "actual_cost_for_project_{0}_{1}.xlsx";
                                filename = string.Format(filename, timesheetReportView.ProjectCode, DateTime.Now.ToString("yyyyMMdd"));
                                fullFilepath = fullFilepath + filename;
                                actualCostForProjects.WriteExcel(fullFilepath);
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    #endregion
                }
                else if (timesheetReportView.Name == "5") // Actual Cost for All Person
                {
                    #region Actual Cost for Person
                    if (timesheetReportView.Type == "1")// Excel
                    {
                        if (timesheetReportView.Data == "1")// ทั้งหมด
                        {
                            using (var session = _sessionFactory.OpenSession())
                            {
                                var owner = (from u in session.Query<User>()
                                             where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                             select u).Single();

                                string folderZip = string.Format("actual_cost_for_all_person_{0}_{1}\\", owner.EmployeeID, DateTime.Now.ToString("yyyyMMdd"));
                                folderZip = fullFilepath + folderZip;

                                if (Directory.Exists(folderZip))
                                {
                                    Directory.Delete(folderZip, true);
                                }
                                Directory.CreateDirectory(folderZip);

                                var memberProjects = (from p in session.Query<Project>()
                                                      join m in session.Query<ProjectMember>() on p equals m.Project
                                                      where m.User == owner
                                                      && m.ProjectRole.IsOwner
                                                      select new { Project = p, Member = m });

                                if (memberProjects.Count() == 0)
                                {
                                    return Json(new
                                    {
                                        success = false,
                                        message = "ไม่พบโครงการของพนักงานท่านนี้",
                                    }, JsonRequestBehavior.AllowGet);
                                }

                                var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                                var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                                var packOfFiles = new List<string>();

                                memberProjects.ForEach(yourProjectOwner =>
                                {
                                    yourProjectOwner.Project.Members.ForEach(member =>
                                    {
                                        var empTarget = member.User;
                                        var actualCostForPerson = new ActualCostForPerson
                                        {
                                            FromDate = fromDate,
                                            ToDate = toDate,
                                            EmployeeID = empTarget.EmployeeID,
                                            FullName = empTarget.FullName,
                                            Position = empTarget.Position.NameTH,
                                            Department = empTarget.Department.NameTH,
                                        };
                                        foreach (var pro in memberProjects)
                                        {
                                            var header1 = new ProjectHeader
                                            {
                                                ProjectCode = pro.Project.Code,
                                                ProjectName = pro.Project.NameTH,
                                                CurrentProjectRole = pro.Member.ProjectRole.NameTH
                                            };
                                            actualCostForPerson.DetailHeaders.Add(header1);

                                            int index = 0;
                                            foreach (var item in pro.Project.TimeSheets.Where(m => m.User == empTarget))
                                            {
                                                ++index;
                                                var roleCost = item.ProjectRole.ProjectRoleRates
                                                    .GetEffectiveRatePerHours(item.ActualStartDate)
                                                    .FirstOrDefault();

                                                var totalCost = item.ActualHourUsed * roleCost;

                                                header1.Details.Add(new TimesheetDetail
                                                {
                                                    Index = index,
                                                    Date = item.ActualStartDate.GetValueOrDefault(),
                                                    ProjectCode = pro.Project.Code,
                                                    ProjectRole = item.ProjectRole.NameTH,
                                                    ProjectRoleOrder = item.ProjectRole.Order,
                                                    Phase = item.Phase.NameTH,
                                                    PhaseOrder = item.Phase.Order,
                                                    TaskType = item.TaskType.NameTH,
                                                    MainTask = item.MainTask,
                                                    SubTask = item.SubTask,
                                                    Hours = item.ActualHourUsed,
                                                    Cost = totalCost,
                                                    RoleCost = roleCost,
                                                });
                                            }
                                        }

                                        filename = "actual_cost_for_person_{0}.xlsx";
                                        filename = string.Format(filename, empTarget.EmployeeID);
                                        packOfFiles.Add(filename);
                                        actualCostForPerson.WriteExcel(folderZip + filename);
                                    });
                                });

                                filename = "actual_cost_for_all_person_{0}_{1}.zip";
                                filename = string.Format(filename, owner.EmployeeID, DateTime.Now.ToString("yyyyMMdd"));
                                //filename = fullFilepath + filename;

                                System.IO.File.Delete(fullFilepath + filename);
                                ZipFile.CreateFromDirectory(folderZip, fullFilepath + filename);
                            }


                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    #endregion
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException("Support Only ProjectOwner Role");
            }


            var result = new
            {
                exportUrl = exportPath + filename,
                success = success,
                message = message,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Manager, ProjectOwner")]
        public JsonResult ExportReportForManager(TimesheetReportView timesheetReportView)
        {
            string filename = "actual_effort_for_person_{0}_{1}.xlsx";
            int randomFolder = new Random(DateTime.Now.Second).Next(1, int.MaxValue);
            string fullFilepath = Server.MapPath(@"~\Export\" + randomFolder + "\\");
            string exportPath = Url.Content(@"~\Export\" + randomFolder + "\\");
            if (!Directory.Exists(fullFilepath))
            {
                Directory.CreateDirectory(fullFilepath);
            }

            string message = string.Empty;
            bool success = true;
            try
            {
                if (timesheetReportView.Name == "1")
                {
                    #region Actual Effort for Person
                    if (timesheetReportView.Type == "1")// Excel
                    {
                        if (timesheetReportView.Data == "1")// ทั้งหมด
                        {
                            using (var session = _sessionFactory.OpenSession())
                            {
                                //get User
                                var empTarget = (from u in session.Query<User>()
                                                 where u.EmployeeID.ToString() == timesheetReportView.EmployeeID
                                                 select u).Single();

                                var memberProjects = (from p in session.Query<Project>()
                                                      join m in session.Query<ProjectMember>() on p equals m.Project
                                                      where m.User == empTarget
                                                      select new { Project = p, Member = m });

                                if (memberProjects.Count() == 0)
                                {
                                    return Json(new
                                    {
                                        success = false,
                                        message = "ไม่พบโครงการของพนักงานท่านนี้",
                                    }, JsonRequestBehavior.AllowGet);
                                }

                                var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                                var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                                var actualEffortForPerson = new ActualEffortForPerson
                                {
                                    FromDate = fromDate,
                                    ToDate = toDate,
                                };

                                actualEffortForPerson.EmployeeID = empTarget.EmployeeID;
                                actualEffortForPerson.FullName = empTarget.FullName;
                                actualEffortForPerson.Position = empTarget.Position.NameTH;
                                actualEffortForPerson.Department = empTarget.Department.NameTH;

                                int index = 0;
                                foreach (var pro in memberProjects)
                                {
                                    var header1 = new ProjectHeader
                                    {
                                        ProjectCode = pro.Project.Code,
                                        ProjectName = pro.Project.NameTH,
                                        CurrentProjectRole = pro.Member.ProjectRole.NameTH
                                    };
                                    actualEffortForPerson.DetailHeaders.Add(header1);

                                    //fill only memebr timesheet
                                    var timesheet = from t in pro.Project.TimeSheets
                                                    where t.User == empTarget
                                                    && fromDate <= t.ActualStartDate && t.ActualStartDate <= toDate
                                                    select t;

                                    foreach (var item in timesheet)
                                    {
                                        ++index;
                                        header1.Details.Add(new TimesheetDetail
                                        {
                                            Index = index,
                                            Date = item.ActualStartDate.GetValueOrDefault(),
                                            ProjectCode = pro.Project.Code,
                                            ProjectRole = item.ProjectRole.NameTH,
                                            ProjectRoleOrder = item.ProjectRole.Order,
                                            Phase = item.Phase.NameTH,
                                            PhaseOrder = item.Phase.Order,
                                            TaskType = item.TaskType.NameTH,
                                            MainTask = item.MainTask,
                                            SubTask = item.SubTask,
                                            Hours = item.ActualHourUsed,
                                        });
                                    }
                                }

                                filename = "actual_effort_for_person_{0}_{1}.xlsx";
                                filename = string.Format(filename, actualEffortForPerson.EmployeeID, DateTime.Now.ToString("yyyyMMdd"));
                                fullFilepath = fullFilepath + filename;
                                actualEffortForPerson.WriteExcel(fullFilepath);
                            }
                        }
                        else if (timesheetReportView.Data == "2")// Summary
                        {
                            throw new NotImplementedException();
                        }
                        else if (timesheetReportView.Data == "3")// Detail
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    #endregion
                }
                else if (timesheetReportView.Name == "2") // Actual Effort for Department
                {
                    #region Actual Effort for Department
                    if (timesheetReportView.Type == "1")// Excel
                    {
                        if (timesheetReportView.Data == "1")// ทั้งหมด
                        {
                            using (var session = _sessionFactory.OpenSession())
                            {
                                //get User
                                var empTarget = (from u in session.Query<User>()
                                                 where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                                 select u).Single();

                                var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                                var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                                var report = new ActualCostForDepartment
                                {
                                    FromDate = fromDate,
                                    ToDate = toDate,
                                    Department = empTarget.Department.NameTH,
                                    IsDisplayCost = false
                                };

                                var projectsOfDeptQuery = (from p in session.Query<Project>()
                                                           let members = (from u in p.Members
                                                                    where u.User.Department == empTarget.Department
                                                                    select u).Count()
                                                            where members > 0
                                                            //&& p.Code == timesheetReportView.ProjectCode
                                                           select p);

                                if (!string.IsNullOrEmpty(timesheetReportView.ProjectCode))
                                {
                                    projectsOfDeptQuery = projectsOfDeptQuery
                                        .Where(p => p.Code == timesheetReportView.ProjectCode);

                                    filename = "actual_effort_for_department_{0}_{1}_{2}.xlsx";
                                    filename = string.Format(filename, 
                                        empTarget.Department.NameTH,
                                        timesheetReportView.ProjectCode, 
                                        DateTime.Now.ToString("yyyyMMdd"));
                                }
                                else
                                {
                                    //for all
                                    filename = "all_actual_effort_for_department_{0}_{1}.xlsx";
                                    filename = string.Format(filename, empTarget.Department.NameTH, DateTime.Now.ToString("yyyyMMdd"));

                                    report.Title = "All Actual Effort For Department";
                                }

                                int index = 0;
                                foreach (var pro in projectsOfDeptQuery)
                                {
                                    var header1 = new ProjectHeader
                                    {
                                        ProjectCode = pro.Code,
                                        ProjectName = pro.NameTH,
                                        Members = pro.Members.Count(),
                                    };
                                    report.DetailHeaders.Add(header1);
                                    //var timesheetOnlyDepartment = pro.TimeSheets
                                    //    .Where(t => t.User.Department == empTarget.Department)
                                    //    .ToList();

                                    var timesheetOnlyDepartment = pro.TimeSheets
                                .Where(t => fromDate <= t.ActualStartDate
                                    && t.ActualStartDate <= toDate
                                    && t.User.Department == empTarget.Department);

                                    foreach (var timesheet in timesheetOnlyDepartment)
                                    {
                                        ++index;
                                        var cost = timesheet.ProjectRole.ProjectRoleRates
                                            .GetEffectiveRatePerHours(timesheet.ActualStartDate)
                                            .FirstOrDefault();

                                        var totalCost = timesheet.ActualHourUsed * cost;

                                        header1.Details.Add(new TimesheetDetail
                                        {
                                            Index = index,
                                            Date = timesheet.ActualStartDate.GetValueOrDefault(),
                                            EmployeeID = timesheet.User.EmployeeID,
                                            FullName = timesheet.User.FullName,
                                            ProjectCode = pro.Code,
                                            ProjectRole = timesheet.ProjectRole.NameTH,
                                            ProjectRoleOrder = timesheet.ProjectRole.Order,
                                            Phase = timesheet.Phase.NameTH,
                                            PhaseOrder = timesheet.Phase.Order,
                                            TaskType = timesheet.TaskType.NameTH,
                                            MainTask = timesheet.MainTask,
                                            SubTask = timesheet.SubTask,
                                            Hours = timesheet.ActualHourUsed,
                                            Cost = totalCost,
                                            RoleCost = cost,
                                        });
                                    }
                                }

                                fullFilepath = fullFilepath + filename;
                                report.WriteExcel(fullFilepath);
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    #endregion
                }
                else if (Roles.IsUserInRole("ProjectOwner"))
                {
                    if (timesheetReportView.Name == "3") // Actual Cost for Person
                    {
                        #region Actual Cost for Person
                        if (timesheetReportView.Type == "1")// Excel
                        {
                            if (timesheetReportView.Data == "1")// ทั้งหมด
                            {
                                using (var session = _sessionFactory.OpenSession())
                                {
                                    var owner = (from u in session.Query<User>()
                                                 where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                                 select u).Single();

                                    //get User
                                    var empTarget = (from u in session.Query<User>()
                                                     where u.EmployeeID.ToString() == timesheetReportView.EmployeeID
                                                     select u).Single();

                                    var memberProjects = (from p in session.Query<Project>()
                                                          join m in session.Query<ProjectMember>() on p equals m.Project
                                                          where m.User == empTarget
                                                          select new { Project = p, Member = m });

                                    if (memberProjects.Count() == 0)
                                    {
                                        return Json(new
                                        {
                                            success = false,
                                            message = "ไม่พบโครงการของพนักงานท่านนี้",
                                        }, JsonRequestBehavior.AllowGet);
                                    }

                                    var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                                    var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                                    var actualCostForPerson = new ActualCostForPerson
                                    {
                                        FromDate = fromDate,
                                        ToDate = toDate,
                                    };

                                    actualCostForPerson.EmployeeID = empTarget.EmployeeID;
                                    actualCostForPerson.FullName = empTarget.FullName;
                                    actualCostForPerson.Position = empTarget.Position.NameTH;
                                    actualCostForPerson.Department = empTarget.Department.NameTH;

                                    int index = 0;
                                    foreach (var pro in memberProjects)
                                    {
                                        //fill only memebr timesheet
                                        var isOwner = (from pm in pro.Project.Members where pm.User == owner select pm.ProjectRole.IsOwner).FirstOrDefault();
                                        if (isOwner)
                                        {
                                            var header1 = new ProjectHeader
                                            {
                                                ProjectCode = pro.Project.Code,
                                                ProjectName = pro.Project.NameTH,
                                                CurrentProjectRole = pro.Member.ProjectRole.NameTH
                                            };
                                            actualCostForPerson.DetailHeaders.Add(header1);
                                            foreach (var item in pro.Project.TimeSheets.Where(m => m.User == empTarget))
                                            {
                                                ++index;
                                                var roleCost = item.ProjectRole.ProjectRoleRates
                                                    .GetEffectiveRatePerHours(item.ActualStartDate)
                                                    .FirstOrDefault();

                                                var totalCost = item.ActualHourUsed * roleCost;

                                                header1.Details.Add(new TimesheetDetail
                                                {
                                                    Index = index,
                                                    Date = item.ActualStartDate.GetValueOrDefault(),
                                                    ProjectCode = pro.Project.Code,
                                                    ProjectRole = item.ProjectRole.NameTH,
                                                    ProjectRoleOrder = item.ProjectRole.Order,
                                                    Phase = item.Phase.NameTH,
                                                    PhaseOrder = item.Phase.Order,
                                                    TaskType = item.TaskType.NameTH,
                                                    MainTask = item.MainTask,
                                                    SubTask = item.SubTask,
                                                    Hours = item.ActualHourUsed,
                                                    Cost = totalCost,
                                                    RoleCost = roleCost,
                                                });
                                            }
                                        }
                                    }

                                    filename = "actual_cost_for_person_{0}_{1}.xlsx";
                                    filename = string.Format(filename, actualCostForPerson.EmployeeID, DateTime.Now.ToString("yyyyMMdd"));
                                    fullFilepath = fullFilepath + filename;
                                    actualCostForPerson.WriteExcel(fullFilepath);
                                }
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        #endregion
                    }
                    else if (timesheetReportView.Name == "4") // Actual Cost for Project
                    {
                        #region Actual Cost for Project
                        if (timesheetReportView.Type == "1")// Excel
                        {
                            if (timesheetReportView.Data == "1")// ทั้งหมด
                            {
                                using (var session = _sessionFactory.OpenSession())
                                {
                                    var projects = (from p in session.Query<Project>()
                                                    where p.Code == timesheetReportView.ProjectCode
                                                    select p).ToList();

                                    var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                                    var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                                    var actualCostForProjects = new ActualCostForProjects
                                    {
                                        FromDate = fromDate,
                                        ToDate = toDate,
                                    };

                                    foreach (var project in projects)
                                    {
                                        var actualCostForProject = new ActualCostForProject
                                        {
                                            FromDate = fromDate,
                                            ToDate = toDate,
                                        };

                                        actualCostForProjects.ActualCostForProjectList.Add(actualCostForProject);
                                        var header1 = new ProjectHeader
                                        {
                                            ProjectCode = project.Code,
                                            ProjectName = project.NameTH,
                                            Members = project.Members.Count(),
                                        };
                                        actualCostForProject.DetailHeaders.Add(header1);
                                        actualCostForProject.ProjectCode = project.Code;
                                        actualCostForProject.ProjectName = project.NameTH;
                                        var index = 0;
                                        foreach (var timesheet in project.TimeSheets.Where(t =>
                                            fromDate <= t.ActualStartDate && t.ActualStartDate <= toDate))
                                        {
                                            ++index;
                                            var cost = timesheet.ProjectRole.ProjectRoleRates
                                                .GetEffectiveRatePerHours(timesheet.ActualStartDate)
                                                .FirstOrDefault();
                                            var totalCost = timesheet.ActualHourUsed * cost;
                                            header1.Details.Add(new TimesheetDetail
                                            {
                                                Index = index,
                                                Date = timesheet.ActualStartDate.GetValueOrDefault(),
                                                EmployeeID = timesheet.User.EmployeeID,
                                                FullName = timesheet.User.FullName,
                                                ProjectCode = project.Code,
                                                ProjectRole = timesheet.ProjectRole.NameTH,
                                                ProjectRoleOrder = timesheet.ProjectRole.Order,
                                                Phase = timesheet.Phase.NameTH,
                                                PhaseOrder = timesheet.Phase.Order,
                                                TaskType = timesheet.TaskType.NameTH,
                                                MainTask = timesheet.MainTask,
                                                SubTask = timesheet.SubTask,
                                                Hours = timesheet.ActualHourUsed,
                                                Cost = totalCost,
                                                RoleCost = cost,
                                            });
                                        }
                                    }

                                    filename = "actual_cost_for_project_{0}_{1}.xlsx";
                                    filename = string.Format(filename, timesheetReportView.ProjectCode, DateTime.Now.ToString("yyyyMMdd"));
                                    fullFilepath = fullFilepath + filename;
                                    actualCostForProjects.WriteExcel(fullFilepath);
                                }
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        #endregion
                    }
                    else if (timesheetReportView.Name == "5") // Actual Cost for All Person
                    {
                        #region Actual Cost for All Person
                        if (timesheetReportView.Type == "1")// Excel
                        {
                            if (timesheetReportView.Data == "1")// ทั้งหมด
                            {
                                using (var session = _sessionFactory.OpenSession())
                                {
                                    var owner = (from u in session.Query<User>()
                                                 where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                                 select u).Single();

                                    string folderZip = string.Format("actual_cost_for_all_person_{0}_{1}\\", owner.EmployeeID, DateTime.Now.ToString("yyyyMMdd"));
                                    folderZip = fullFilepath + folderZip;
                                    if (Directory.Exists(folderZip))
                                    {
                                        Directory.Delete(folderZip, true);
                                    }
                                    Directory.CreateDirectory(folderZip);

                                    var memberProjects = (from p in session.Query<Project>()
                                                          join m in session.Query<ProjectMember>() on p equals m.Project
                                                          where m.User == owner
                                                          && m.ProjectRole.IsOwner
                                                          select new { Project = p, Member = m });

                                    if (memberProjects.Count() == 0)
                                    {
                                        return Json(new
                                        {
                                            success = false,
                                            message = "ไม่พบโครงการของพนักงานท่านนี้",
                                        }, JsonRequestBehavior.AllowGet);
                                    }

                                    var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                                    var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                                    var packOfFiles = new List<string>();

                                    memberProjects.ForEach(yourProjectOwner =>
                                    {
                                        yourProjectOwner.Project.Members.ForEach(member =>
                                        {
                                            var empTarget = member.User;
                                            var actualCostForPerson = new ActualCostForPerson
                                            {
                                                FromDate = fromDate,
                                                ToDate = toDate,
                                                EmployeeID = empTarget.EmployeeID,
                                                FullName = empTarget.FullName,
                                                Position = empTarget.Position.NameTH,
                                                Department = empTarget.Department.NameTH,
                                            };
                                            foreach (var pro in memberProjects)
                                            {
                                                var header1 = new ProjectHeader
                                                {
                                                    ProjectCode = pro.Project.Code,
                                                    ProjectName = pro.Project.NameTH,
                                                    CurrentProjectRole = pro.Member.ProjectRole.NameTH
                                                };
                                                actualCostForPerson.DetailHeaders.Add(header1);

                                                int index = 0;
                                                foreach (var item in pro.Project.TimeSheets.Where(m => m.User == empTarget))
                                                {
                                                    ++index;
                                                    var roleCost = item.ProjectRole.ProjectRoleRates
                                                        .GetEffectiveRatePerHours(item.ActualStartDate)
                                                        .FirstOrDefault();

                                                    var totalCost = item.ActualHourUsed * roleCost;

                                                    header1.Details.Add(new TimesheetDetail
                                                    {
                                                        Index = index,
                                                        Date = item.ActualStartDate.GetValueOrDefault(),
                                                        ProjectCode = pro.Project.Code,
                                                        ProjectRole = item.ProjectRole.NameTH,
                                                        ProjectRoleOrder = item.ProjectRole.Order,
                                                        Phase = item.Phase.NameTH,
                                                        PhaseOrder = item.Phase.Order,
                                                        TaskType = item.TaskType.NameTH,
                                                        MainTask = item.MainTask,
                                                        SubTask = item.SubTask,
                                                        Hours = item.ActualHourUsed,
                                                        Cost = totalCost,
                                                        RoleCost = roleCost,
                                                    });
                                                }
                                            }

                                            filename = "actual_cost_for_person_{0}.xlsx";
                                            filename = string.Format(filename, empTarget.EmployeeID);
                                            packOfFiles.Add(filename);
                                            actualCostForPerson.WriteExcel(folderZip + filename);
                                        });
                                    });

                                    filename = "actual_cost_for_all_person_{0}_{1}.zip";
                                    filename = string.Format(filename, owner.EmployeeID, DateTime.Now.ToString("yyyyMMdd"));
                                    //filename = fullFilepath + filename;
                                    System.IO.File.Delete(fullFilepath + filename);
                                    ZipFile.CreateFromDirectory(folderZip, fullFilepath + filename);
                                }
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        #endregion
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    throw new NotImplementedException("Support Only ProjectOwner Role");
                }

            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            var result = new
            {
                exportUrl = exportPath + filename,
                success = success,
                message = message,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected DateTime FirstDayOfWeek(DateTime date)
        {
            var candidateDate = date;
            while (candidateDate.DayOfWeek != DayOfWeek.Sunday)
            {
                candidateDate = candidateDate.AddDays(-1);
            }
            return candidateDate;
        }
        protected DateTime FirstDayOfCurrentWeek()
        {
            return FirstDayOfWeek(DateTime.Today);
        }
        protected DateTime LastDayOfWeek(DateTime date)
        {
            var candidateDate = date;
            while (candidateDate.DayOfWeek != DayOfWeek.Saturday)
            {
                candidateDate = candidateDate.AddDays(1);
            }
            return candidateDate;
        }
        protected DateTime LastDayOfWeek()
        {
            return LastDayOfWeek(DateTime.Today);
        }
    }
}