using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PJ_CWN019.TM.Web.Controllers
{
    using Cwn.PM.BusinessModels.Queries;
    using Cwn.PM.BusinessModels.Entities;
    using Cwn.PM.Reports.Values;
    using NHibernate;
    using NHibernate.Linq;
    using PJ_CWN019.TM.Web.Models;
    using System.Globalization;
    using WebMatrix.WebData;
    using PJ_CWN019.TM.Web.Filters;
    using System.Web.Security;
    using System.IO;

    [ErrorLog]
    [ProfileLog]
    [Authorize(Roles = "Executive, Admin")]
    [FourceToChangeAttribute]
    public class ProjectPortfolioController : Controller
    {
        string dateFormat = "dd/MM/yyyy";
        ISessionFactory _sessionFactory = null;
        public ProjectPortfolioController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Report()
        {
            var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            ViewBag.FirstDayOfMonth = firstDayOfMonth.ToString(dateFormat, new CultureInfo("en-US"));
            ViewBag.LastDayOfMonth = lastDayOfMonth.ToString(dateFormat, new CultureInfo("en-US"));

            return View();
        }

        public JsonResult GetReport()
        {
            var viewList = new List<TimesheetReportView>();

            viewList.Add(new TimesheetReportView
            {
                ID = 1,
                Name = "Actual Cost for Person",
            });

            if (Roles.IsUserInRole("Admin"))
            {
                viewList.Add(new TimesheetReportView
                {
                    ID = 5,
                    Name = "Actual Effort for Person",
                });
            }

            viewList.Add(new TimesheetReportView
            {
                ID = 2,
                Name = "Actual Cost for Department",
            });

            if (Roles.IsUserInRole("Admin"))
            {
                viewList.Add(new TimesheetReportView
                {
                    ID = 6,
                    Name = "Actual Effort for Department",
                });
            }

            viewList.Add(new TimesheetReportView
            {
                ID = 3,
                Name = "Actual Cost for Project",
            });

            viewList.Add(new TimesheetReportView
            {
                ID = 4,
                Name = "Actual Cost for All Project",
            });

            var result = new
            {
                data = viewList,
                total = viewList.Count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReadProject(int start, int limit, string query)
        {
            var viewList = new List<ProjectView>();
            int count;

            using (var session = _sessionFactory.OpenSession())
            {
                var prjQuery = (from p in session.Query<Project>()
                                where p.Code.Contains(query)
                                || p.NameTH.Contains(query)
                                select p);

                prjQuery = prjQuery.OrderBy(prj => prj.Code);
                count = prjQuery.Count();
                foreach (var project in prjQuery.Skip(start).Take(limit))
                {
                    viewList.Add(new ProjectView
                    {
                        ID = project.ID,
                        Code = project.Code,
                        Name = project.NameTH,
                        StartDate = project.StartDate,
                        EndDate = project.EndDate
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

        public JsonResult ReadEmployee(int start, int limit, string query)
        {
            var viewList = new List<EmployeeView>();
            int count;

            using (var session = _sessionFactory.OpenSession())
            {
                var userQuery = (from u in session.Query<User>()
                                 where u.EmployeeID.ToString().Contains(query)
                                 || u.FirstNameTH.Contains(query)
                                 || u.LastNameTH.Contains(query)
                                 select u);

                userQuery = userQuery.OrderBy(u => u.EmployeeID);
                count = userQuery.Count();
                foreach (var user in userQuery.Skip(start).Take(limit))
                {
                    viewList.Add(new EmployeeView
                    {
                        ID = user.ID,
                        EmployeeID = user.EmployeeID,
                        FullName = user.FullName,
                        Display = string.Format("{0}: {1}", user.EmployeeID, user.FullName),
                        Position = (user.Position == null) ? string.Empty : user.Position.NameTH,
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

        public JsonResult ReadDepartment(int start, int limit, string query)
        {
            var viewList = new List<DepartmentView>();
            int count;

            using (var session = _sessionFactory.OpenSession())
            {
                var deptQuery = (from dept in session.Query<Department>()
                                 join div in session.Query<Division>() on dept.Division equals div
                                 where dept.NameTH.Contains(query)
                                 || div.NameTH.Contains(query)
                                 orderby div.NameTH, dept.NameTH
                                 select new { 
                                     ID = dept.ID,
                                     DivisionID = div.ID,
                                     Name = div.NameTH + " - " + dept.NameTH,
                                 });

                //deptQuery = deptQuery.OrderBy(u => u.Name);

                count = deptQuery.Count();
                foreach (var dept in deptQuery.Skip(start).Take(limit))
                {
                    viewList.Add(new DepartmentView
                    {
                        ID = dept.ID,
                        DivisionID = dept.DivisionID,
                        Name = dept.Name,
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

        public JsonResult ExportReport(TimesheetReportView timesheetReportView)
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

            if (timesheetReportView.Name == "1") // Actual Cost for Person
            {
                #region Actual Cost for Person
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
            else if (timesheetReportView.Name == "2") // Actual Cost for Department
            {
                #region Actual Cost for Department
                using (var session = _sessionFactory.OpenSession())
                {
                    //get User
                    var dept = (from d in session.Query<Department>()
                                where d.ID == timesheetReportView.DepartmentID
                                select d).Single();

                    var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                    var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                    var report = new ActualCostForDepartment
                    {
                        FromDate = fromDate,
                        ToDate = toDate,
                        Department = dept.NameTH,
                        IsDisplayCost = true
                    };

                    var projectsOfDeptQuery = (from p in session.Query<Project>()
                                               let members = (from u in p.Members
                                                              where u.User.Department == dept
                                                              select u).Count()
                                               where members > 0
                                               select p);


                    int index = 0;
                    foreach (var pro in projectsOfDeptQuery)
                    {
                        var header1 = new ProjectHeader
                        {
                            ProjectCode = pro.Code,
                            ProjectName = pro.NameEN,
                            Members = pro.Members.Count(),
                        };
                        report.DetailHeaders.Add(header1);
                        var timesheetOnlyDepartment = pro.TimeSheets
                            .Where(t => fromDate <= t.ActualStartDate
                                && t.ActualStartDate <= toDate
                                && t.User.Department == dept);

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

                    filename = "actual_cost_for_department_{0}_{1}.xlsx";
                    filename = string.Format(filename, dept.NameTH, DateTime.Now.ToString("yyyyMMdd"));
                    filename = filename.Replace("#", "NO");
                    fullFilepath = fullFilepath + filename;
                    report.WriteExcel(fullFilepath);
                }
                #endregion
            }
            else if (timesheetReportView.Name == "3") // Actual Cost for Project
            {
                #region Actual Cost for Project
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
                #endregion
            }
            else if (timesheetReportView.Name == "4") // Actual Cost for All Project
            {
                #region Actual Cost for All Project
                if (timesheetReportView.Type == "1")// Excel
                {
                    if (timesheetReportView.Data == "1")// ทั้งหมด
                    {
                        using (var session = _sessionFactory.OpenSession())
                        {
                            var allProjects = (from p in session.Query<Project>()
                                               //where fromDate <= p.StartDate 
                                               //&& p.StartDate <= toDate
                                               select p);

                            DateTime? fromDate;
                            DateTime? toDate;
                            if (timesheetReportView.CheckAllTime)
                            {
                                fromDate = session.Query<Timesheet>().Select(t => t.ActualStartDate).Min();
                                toDate = session.Query<Timesheet>().Select(t => t.ActualStartDate).Max();
                            }
                            else
                            {
                                fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                                toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));
                            }

                            var actualCostForAllProject = new ActualCostForAllProject
                            {
                                FromDate = fromDate.Value,
                                ToDate = toDate.Value,
                            };

                            var index = 0;
                            foreach (var project in allProjects)
                            {
                                var totalTimeSheets = project.TimeSheets
                                    .Where(t => fromDate <= t.ActualStartDate
                                        && t.ActualStartDate <= toDate).Count();

                                if (totalTimeSheets > 0)
                                {
                                    var membersCount = project.Members.Count();

                                    var header1 = new ProjectHeader
                                    {
                                        ProjectCode = project.Code,
                                        ProjectName = project.NameTH,
                                        Members = membersCount,
                                    };

                                    actualCostForAllProject.DetailHeaders.Add(header1);

                                    ++index;

                                    var totalDay = project.EndDate - project.StartDate;
                                    var hours = project.TimeSheets.Sum(t => t.ActualHourUsed);
                                    decimal totalCost = 0;

                                    foreach (var timesheet in project.TimeSheets
                                        .Where(t => fromDate <= t.ActualStartDate
                                            && t.ActualStartDate <= toDate))
                                    {
                                        var cost = timesheet.ProjectRole.ProjectRoleRates
                                            .GetEffectiveRatePerHours(timesheet.ActualStartDate)
                                            .FirstOrDefault();

                                        totalCost = timesheet.ActualHourUsed * cost;

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
                            }

                            filename = "actual_cost_for_all_project_{0}.xlsx";
                            filename = string.Format(filename, DateTime.Now.ToString("yyyyMMdd"));
                            fullFilepath = fullFilepath + filename;
                            actualCostForAllProject.WriteExcel(fullFilepath);
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
            else if (timesheetReportView.Name == "5") // Actual Effort for Person
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


                            DateTime fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                            DateTime toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                            var actualCostForPerson = new ActualEffortForPerson
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

                            filename = "actual_effort_for_person_{0}_{1}.xlsx";
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
            else if (timesheetReportView.Name == "6") // Actual Effort for Department
            {
                #region Actual Effort for Department
                if (timesheetReportView.Type == "1")// Excel
                {
                    if (timesheetReportView.Data == "1")// ทั้งหมด
                    {
                        using (var session = _sessionFactory.OpenSession())
                        {
                            //get User
                            var dept = (from d in session.Query<Department>()
                                        where d.ID == timesheetReportView.DepartmentID
                                        select d).Single();

                            var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                            var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                            var report = new ActualCostForDepartment
                            {
                                FromDate = fromDate,
                                ToDate = toDate,
                                Department = dept.NameTH,
                                IsDisplayCost = false
                            };

                            var projectsOfDeptQuery = (from p in session.Query<Project>()
                                                       let members = (from u in p.Members
                                                                      where u.User.Department == dept
                                                                      select u).Count()
                                                       where members > 0
                                                       //&& p.Code == timesheetReportView.ProjectCode
                                                       select p);

                            //for all
                            filename = "all_actual_effort_for_department_{0}_{1}.xlsx";
                            filename = string.Format(filename, dept.NameTH, DateTime.Now.ToString("yyyyMMdd"));

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
                                && t.User.Department == dept);

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
            else
            {
                throw new NotImplementedException();
            }

            var result = new
            {
                exportUrl = exportPath + filename,
                success = success,
                message = message,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
