using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

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
    using PJ_CWN019.TM.Web.Extensions;
    using PJ_CWN019.TM.Web.Models.Services;
using PJ_CWN019.TM.Web.Models.Providers;

    [ErrorLog]
    [ProfileLog]
    [Authorize(Roles = ConstAppRoles.Executive + ", " + ConstAppRoles.Admin)]
    [FourceToChangeAttribute]
    [SessionState(SessionStateBehavior.Disabled)]
    [ValidateAntiForgeryTokenOnAllPosts]
    public class ProjectPortfolioController : Controller
    {
        string dateFormat = "dd/MM/yyyy";
        ISessionFactory _sessionFactory = null;
        bool _addChartExportReport = false;
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

            ViewBag.MinDate = DateTime.Now.AddYears(-1).ToString(dateFormat, new CultureInfo("en-US"));
            ViewBag.MaxDate = DateTime.Now.ToString(dateFormat, new CultureInfo("en-US"));

            ViewBag.FirstDayOfMonth = firstDayOfMonth.ToString(dateFormat, new CultureInfo("en-US"));
            ViewBag.LastDayOfMonth = ViewBag.MaxDate;

            return View();
        }

        [OutputCache(Duration = 60 * 60 * 24)]
        public JsonResult GetReport()
        {
            var viewList = new List<TimesheetReportView>();

            viewList.Add(new TimesheetReportView
            {
                ID = 1,
                Name = "Actual Cost for Person",
            });

            if (Roles.IsUserInRole(ConstAppRoles.Admin))
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

            viewList.Add(new TimesheetReportView
            {
                ID = 7,
                Name = "Timesheet Data Recording",
            });

            if (Roles.IsUserInRole(ConstAppRoles.TopManager)
                || Roles.IsUserInRole(ConstAppRoles.MiddleManager))
            {
                viewList.Add(new TimesheetReportView
                {
                    ID = 8,
                    Name = "Actual Effort for Manager",
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
                                 where u.Status == EmployeeStatus.Work
                                 && (u.EmployeeID.ToString().Contains(query)
                                 || u.FirstNameTH.Contains(query)
                                 || u.LastNameTH.Contains(query))
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
                            var empTarget = session.Query<User>().QueryByEmployeeID(timesheetReportView.EmployeeID).Single();

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

                                //fill only memebr timesheet
                                var timesheets = from t in session.Query<Timesheet>()
                                                 where t.Project == pro.Project
                                                 && t.User == empTarget
                                                 && fromDate <= t.ActualStartDate 
                                                 && t.ActualStartDate <= toDate
                                                 select t;

                                //var timesheets = from t in pro.Project.TimeSheets
                                //                where t.User == empTarget
                                //                && fromDate <= t.ActualStartDate && t.ActualStartDate <= toDate
                                //                select t;

                                foreach (var item in timesheets)
                                {
                                    ++index;
                                    var roleCost = item.ProjectRole.ProjectRoleRates
                                        .GetEffectiveRatePerHours(item.ActualStartDate)
                                        .FirstOrDefault();

                                    var totalCost = item.ActualHourUsed * roleCost;
                                    var detail = item.ToViewModel(index, pro.Project, totalCost, roleCost);

                                    header1.Details.Add(detail);
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

                        var timesheetOnlyDepartment = from tm in session.Query<Timesheet>()
                                                          where tm.Project == pro
                                                          &&  fromDate <= tm.ActualStartDate
                                                          && tm.ActualStartDate <= toDate
                                                          && tm.User.Department == dept
                                                          select tm;

                        foreach (var timesheet in timesheetOnlyDepartment)
                        {
                            ++index;
                            var cost = timesheet.ProjectRole.ProjectRoleRates
                                .GetEffectiveRatePerHours(timesheet.ActualStartDate)
                                .FirstOrDefault();

                            var totalCost = timesheet.ActualHourUsed * cost;
                            var detail = timesheet.ToViewModel(index, pro, totalCost, cost);
                            header1.Details.Add(detail);
                        }
                    }

                    filename = "actual_cost_for_department_{0}_{1}.xlsx";
                    filename = string.Format(filename, dept.NameTH, DateTime.Now.ToString("yyyyMMdd"));
                    filename = filename.Replace("#", "NO");
                    fullFilepath = fullFilepath + filename;
                    report.WriteExcel(fullFilepath, _addChartExportReport);
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
                            ProjectStartDate = project.StartDate.ToPresentDateString(),
                            ProjectEndDate = project.EndDate.ToPresentDateString(),
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

                        var timesheets = (from t in session.Query<Timesheet>()
                                         where t.Project == project
                                         && fromDate <= t.ActualStartDate
                                         && t.ActualStartDate <= toDate
                                         select t).ToList();

                        foreach (var timesheet in timesheets)
                        {
                            ++index;
                            var cost = timesheet.ProjectRole.ProjectRoleRates
                                .GetEffectiveRatePerHours(timesheet.ActualStartDate)
                                .FirstOrDefault();
                            var totalCost = timesheet.ActualHourUsed * cost;
                            var detail = timesheet.ToViewModel(index, project, totalCost, cost);
                            header1.Details.Add(detail);
                        }
                    }

                    filename = "actual_cost_for_project_{0}_{1}.xlsx";
                    filename = string.Format(filename, timesheetReportView.ProjectCode, DateTime.Now.ToString("yyyyMMdd"));
                    fullFilepath = fullFilepath + filename;
                    actualCostForProjects.WriteExcel(fullFilepath, _addChartExportReport);
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
                                var totalTimeSheets = (from tm in session.Query<Timesheet>()
                                                      where tm.Project == project
                                                      && fromDate <= tm.ActualStartDate
                                                      && tm.ActualStartDate <= toDate
                                                       select tm);

                                //var totalTimeSheets = project.TimeSheets
                                //    .Where(t => fromDate <= t.ActualStartDate
                                //        && t.ActualStartDate <= toDate).Count();

                                if (totalTimeSheets.Count() > 0)
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
                                    //var hours = project.TimeSheets.Sum(t => t.ActualHourUsed);
                                    decimal totalCost = 0;

                                    foreach (var timesheet in totalTimeSheets)
                                    {
                                        var cost = timesheet.ProjectRole.ProjectRoleRates
                                            .GetEffectiveRatePerHours(timesheet.ActualStartDate)
                                            .FirstOrDefault();

                                        totalCost = timesheet.ActualHourUsed * cost;

                                        var detail = timesheet.ToViewModel(index, project, totalCost, cost);
                                        header1.Details.Add(detail);
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
                            var empTarget = session.Query<User>().QueryByEmployeeID(timesheetReportView.EmployeeID).Single();

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

                                //fill only memebr timesheet
                                var timesheet = from t in session.Query<Timesheet>()
                                                where t.Project == pro.Project
                                                && t.User == empTarget
                                                && fromDate <= t.ActualStartDate 
                                                && t.ActualStartDate <= toDate
                                                select t;

                                //var timesheet = from t in pro.Project.TimeSheets
                                //                where t.User == empTarget
                                //                && fromDate <= t.ActualStartDate && t.ActualStartDate <= toDate
                                //                select t;

                                foreach (var item in timesheet)
                                {
                                    ++index;
                                    var roleCost = item.ProjectRole.ProjectRoleRates
                                        .GetEffectiveRatePerHours(item.ActualStartDate)
                                        .FirstOrDefault();

                                    var totalCost = item.ActualHourUsed * roleCost;

                                    var detail = item.ToViewModel(index, pro.Project, totalCost, roleCost);
                                    header1.Details.Add(detail);
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
                                IsDisplayCost = false,
                                Title = "Actual Effort for Department"
                            };

                            var projectsOfDeptQuery = (from p in session.Query<Project>()
                                                       let members = (from u in p.Members
                                                                      where u.User.Department == dept
                                                                      select u).Count()
                                                       where members > 0
                                                       //&& p.Code == timesheetReportView.ProjectCode
                                                       select p);

                            //for all
                            filename = "actual_effort_for_department_{0}_{1}.xlsx";
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

                                var timesheetOnlyDepartment = from t in session.Query<Timesheet>()
                                                              where t.Project == pro
                                                              && fromDate <= t.ActualStartDate
                                                              && t.ActualStartDate <= toDate
                                                              && t.User.Department == dept
                                                              select t;

                            //    var timesheetOnlyDepartment = pro.TimeSheets
                            //.Where(t => fromDate <= t.ActualStartDate
                            //    && t.ActualStartDate <= toDate
                            //    && t.User.Department == dept);

                                foreach (var timesheet in timesheetOnlyDepartment)
                                {
                                    ++index;
                                    var cost = timesheet.ProjectRole.ProjectRoleRates
                                        .GetEffectiveRatePerHours(timesheet.ActualStartDate)
                                        .FirstOrDefault();

                                    var totalCost = timesheet.ActualHourUsed * cost;

                                    var detail = timesheet.ToViewModel(index, pro, totalCost, cost);
                                    header1.Details.Add(detail);
                                }
                            }

                            fullFilepath = fullFilepath + filename;
                            report.WriteExcel(fullFilepath, _addChartExportReport);
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
            else if (timesheetReportView.Name == "7") // Timesheet Data Recording
            {
                #region Timesheet Data Recording
                if (timesheetReportView.Type == "1")// Excel
                {
                    if (timesheetReportView.Data == "1")// ทั้งหมด
                    {
                        var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                        var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                        var timesheetDataRecording = new TimesheetDataRecording(fromDate, toDate);

                        using (var session = _sessionFactory.OpenSession())
                        {
                            var allUser = (from u in session.Query<User>()
                                           where u.Status == EmployeeStatus.Work
                                           orderby u.Department.Division.NameTH, u.Department.NameTH, u.FirstNameTH, u.LastNameTH
                                           select u).ToList();

                            var allTimesheet = (from t in session.Query<Timesheet>()
                                               orderby t.User.Department.Division.NameTH, t.User.Department.NameTH, t.User.FirstNameTH, t.User.LastNameTH
                                                     where fromDate <= t.ActualStartDate
                                                && t.ActualStartDate <= toDate select t).ToList();


                            var queryTimesheet = (from u in allUser
                                                 join t in allTimesheet on u equals t.User into u_t
                                                 from ut in u_t.DefaultIfEmpty()
                                                 select new
                                                 {
                                                     User = u,
                                                     Timesheet = ut,
                                                 }).ToList();

                            foreach (var t in queryTimesheet)
                            {
                                //var queryTimesheet = from t in session.Query<Timesheet>()
                                //                     orderby t.User.Department.Division.NameTH, t.User.Department.NameTH, t.User.FirstNameTH, t.User.LastNameTH
                                //                     where fromDate <= t.ActualStartDate
                                //                && t.ActualStartDate <= toDate && t.User == user
                                //                     select new TimesheetDetail
                                //                     {
                                //                         EmployeeID = t.User.EmployeeID,
                                //                         FullName = string.Format("{0} {1}", t.User.FirstNameTH, t.User.LastNameTH),
                                //                         Email = t.User.Email,
                                //                         PositionName = t.User.Position.NameTH,
                                //                         DivisionName = t.User.Department.Division.NameTH,
                                //                         DepartmentName = t.User.Department.NameTH,

                                //                         Date = t.ActualStartDate.GetValueOrDefault(),
                                //                         Hours = t.ActualHourUsed,
                                //                     };

                                string positionName = (t.User.Position != null) ? t.User.Position.NameTH : string.Empty;
                                if (t.Timesheet != null)
                                {
                                    var td = new TimesheetDetail
                                                     {
                                                         EmployeeID = t.User.EmployeeID,
                                                         FullName = string.Format("{0} {1}", t.User.FirstNameTH, t.User.LastNameTH),
                                                         Email = t.User.Email,
                                                         PositionName = positionName,
                                                         DivisionName = t.User.Department.Division.NameTH,
                                                         DepartmentName = t.User.Department.NameTH,
                                                         Date = t.Timesheet.ActualStartDate.GetValueOrDefault(),
                                                         Hours = t.Timesheet.ActualHourUsed,
                                                     };
                                    timesheetDataRecording.Details.Add(td);
                                }
                                else
                                {
                                    var td = new TimesheetDetail
                                    {
                                        EmployeeID = t.User.EmployeeID,
                                        FullName = string.Format("{0} {1}", t.User.FirstNameTH, t.User.LastNameTH),
                                        Email = t.User.Email,
                                        PositionName = positionName,
                                        DivisionName = t.User.Department.Division.NameTH,
                                        DepartmentName = t.User.Department.NameTH,
                                        Date = DateTime.MinValue,
                                        Hours = 0,
                                    };

                                    timesheetDataRecording.Details.Add(td);
                                }
                            }
                        }

                        filename = "timesheet_data_recording_{0}.xlsx";
                        filename = string.Format(filename, DateTime.Now.ToString("yyyyMMdd"));
                        fullFilepath = fullFilepath + filename;
                        timesheetDataRecording.WriteExcel(fullFilepath);
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
            else if (timesheetReportView.Name == "8") // Actual Effort for Manager
            {
                #region Actual Effort for Manager
                if (timesheetReportView.Type == "1")// Excel
                {
                    if (timesheetReportView.Data == "1")// ทั้งหมด
                    {
                        using (var session = _sessionFactory.OpenSession())
                        {
                            int empID;
                            int.TryParse(WebSecurity.CurrentUserName, out empID);

                            var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, dateFormat, new CultureInfo("en-US"));
                            var toDate = DateTime.ParseExact(timesheetReportView.ToDate, dateFormat, new CultureInfo("en-US"));

                            var actualEffortOfResourceManager = ReportService.BuildActualEffortOfResourceManager(empID, fromDate, toDate, session);
                            filename = "actual_effort_for_manager_{0}.xlsx";
                            filename = string.Format(filename, DateTime.Now.ToString("yyyyMMdd"));
                            fullFilepath = fullFilepath + filename;
                            actualEffortOfResourceManager.WriteExcel(fullFilepath);
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

        public ActionResult ProjectDashboard()
        {
            return View();
        }
    }
}
