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
    [Authorize(Roles = ConstAppRoles.Executive 
        + ", " + ConstAppRoles.Admin
        + ", " + ConstAppRoles.Manager
        + ", " + ConstAppRoles.ProjectOwner
        + ", " + ConstAppRoles.HR)]
    [FourceToChangeAttribute]
    [SessionState(SessionStateBehavior.Disabled)]
    [ValidateAntiForgeryTokenOnAllPosts]
    public class ProjectPortfolioController : Controller
    {
        ISessionFactory _sessionFactory = null;
        bool _addChartExportReport = false;

        Func<DateTime, int> _weekProjector = ConstPage.WeekProjector;
        string format = ConstPage.FormatDefault;
        string format2 = ConstPage.Format2;
        CultureInfo forParseDate = new CultureInfo("en-US");

        public ProjectPortfolioController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ActionResult Home()
        {
            return View();
        }

        string PRESALE_LV_1 = "PRESALE_LV_1";
        string PRESALE_LV_2 = "PRESALE_LV_2";
        public ActionResult StrategyConfiguration()
        {

            using (var session = _sessionFactory.OpenSession())
            using (var tran = session.BeginTransaction())
            {
                var projectThresholdPresaleLv1 = (from pt in session.Query<ProjectThreshold>()
                                                  where pt.Name == PRESALE_LV_1
                                                  select pt).FirstOrDefault();
                if (projectThresholdPresaleLv1 == null)
                {
                    projectThresholdPresaleLv1 = new ProjectThreshold
                    {
                        Name = PRESALE_LV_1
                    };

                    session.Save(projectThresholdPresaleLv1);
                }

                var projectThresholdPresaleLv2 = (from pt in session.Query<ProjectThreshold>()
                                                  where pt.Name == PRESALE_LV_2
                                                  select pt).FirstOrDefault();
                if (projectThresholdPresaleLv2 == null)
                {
                    projectThresholdPresaleLv2 = new ProjectThreshold
                    {
                        Name = PRESALE_LV_2
                    };

                    session.Save(projectThresholdPresaleLv2);
                }

                ViewBag.PRESALE_LV_1_NAME = projectThresholdPresaleLv1.Name;
                ViewBag.PRESALE_LV_1_ID = projectThresholdPresaleLv1.ID;
                ViewBag.PRESALE_LV_1 = projectThresholdPresaleLv1.LimitRatio;

                ViewBag.PRESALE_LV_2_NAME = projectThresholdPresaleLv2.Name;
                ViewBag.PRESALE_LV_2_ID = projectThresholdPresaleLv2.ID;
                ViewBag.PRESALE_LV_2 = projectThresholdPresaleLv2.LimitRatio;

                tran.Commit();
            }
            return View();
        }

        [HttpPost]
        public JsonResult SaveStrategyConfiguration(ProjectThresholdView presaleLv1, ProjectThresholdView presaleLv2)
        {
            var success = false;
            var msg = string.Empty;
            using (var session = _sessionFactory.OpenSession())
            using (var tran = session.BeginTransaction())
            {
                var projectThresholdPresaleLv1 = (from pt in session.Query<ProjectThreshold>()
                                                  where pt.ID == presaleLv1.ID
                                                  select pt).FirstOrDefault();
                if (projectThresholdPresaleLv1 == null)
                {
                    projectThresholdPresaleLv1 = new ProjectThreshold
                    {
                        Name = PRESALE_LV_1,
                        LimitRatio = presaleLv1.LimitRatio
                    };

                    session.Save(projectThresholdPresaleLv1);
                }
                else
                {
                    projectThresholdPresaleLv1.LimitRatio = presaleLv1.LimitRatio;
                }

                var projectThresholdPresaleLv2 = (from pt in session.Query<ProjectThreshold>()
                                                  where pt.ID == presaleLv2.ID
                                                  select pt).FirstOrDefault();
                if (projectThresholdPresaleLv2 == null)
                {
                    projectThresholdPresaleLv2 = new ProjectThreshold
                    {
                        Name = PRESALE_LV_2,
                        LimitRatio = presaleLv2.LimitRatio
                    };

                    session.Save(projectThresholdPresaleLv2);
                }
                else
                {
                    projectThresholdPresaleLv2.LimitRatio = presaleLv2.LimitRatio;
                }

                tran.Commit();
                success = true;
            }
            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Report()
        {
            var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            ViewBag.MinDate = DateTime.Now.AddYears(-1).ToString(format, new CultureInfo("en-US"));
            ViewBag.MaxDate = DateTime.Now.ToString(format, new CultureInfo("en-US"));

            ViewBag.FirstDayOfMonth = firstDayOfMonth.ToString(format, new CultureInfo("en-US"));
            ViewBag.LastDayOfMonth = ViewBag.MaxDate;

            return View();
        }

        [OutputCache(Duration = 60 * 60 * 24)]
        public JsonResult GetReport()
        {
            var viewList = new List<TimesheetReportView>();

            if (!Roles.IsUserInRole(ConstAppRoles.HR))
            {
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
            }
            else
            {
                viewList.Add(new TimesheetReportView
                {
                    ID = 5,
                    Name = "Actual Effort for Person",
                });
            }

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

                            var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, format, new CultureInfo("en-US"));
                            var toDate = DateTime.ParseExact(timesheetReportView.ToDate, format, new CultureInfo("en-US"));

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

                    var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, format, new CultureInfo("en-US"));
                    var toDate = DateTime.ParseExact(timesheetReportView.ToDate, format, new CultureInfo("en-US"));

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

                    var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, format, new CultureInfo("en-US"));
                    var toDate = DateTime.ParseExact(timesheetReportView.ToDate, format, new CultureInfo("en-US"));

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
                                fromDate = DateTime.ParseExact(timesheetReportView.FromDate, format, new CultureInfo("en-US"));
                                toDate = DateTime.ParseExact(timesheetReportView.ToDate, format, new CultureInfo("en-US"));
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


                            DateTime fromDate = DateTime.ParseExact(timesheetReportView.FromDate, format, new CultureInfo("en-US"));
                            DateTime toDate = DateTime.ParseExact(timesheetReportView.ToDate, format, new CultureInfo("en-US"));

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

                            var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, format, new CultureInfo("en-US"));
                            var toDate = DateTime.ParseExact(timesheetReportView.ToDate, format, new CultureInfo("en-US"));

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
                        var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, format, new CultureInfo("en-US"));
                        var toDate = DateTime.ParseExact(timesheetReportView.ToDate, format, new CultureInfo("en-US"));

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

                            var fromDate = DateTime.ParseExact(timesheetReportView.FromDate, format, new CultureInfo("en-US"));
                            var toDate = DateTime.ParseExact(timesheetReportView.ToDate, format, new CultureInfo("en-US"));

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

        public JsonResult GetProject(int start, int limit, string sort, string query = "")
        {
            var viewList = new List<ProjectView>();
            int count = 0;

            using (var session = _sessionFactory.OpenSession())
            {
                var prjOpenStatusName = "Open";

                var prjQuery = (from p in session.Query<Project>()
                                join accP in session.Query<ProjectCostAccount>() 
                                on p equals accP.Project
                                where p.Status.Name == prjOpenStatusName
                                orderby p.StartDate descending
                                select new
                                {
                                    Prj = p,
                                    PrjCostAcc = accP,
                                    Members = p.Members.Count
                                });

                //if (!string.IsNullOrEmpty(sort))
                //{
                //    sort = sort.Replace("[", "").Replace("]", "");
                //    var sorting = JsonConvert.DeserializeObject<Sorting>(sort);
                //    if (sorting.direction == Sorting.Direction.ASC)
                //    {
                //        if (sorting.property == "Code") prjQuery = prjQuery.OrderBy(p => p.Prj.Code);
                //        if (sorting.property == "Name") prjQuery = prjQuery.OrderBy(p => p.Prj.NameTH);
                //        if (sorting.property == "StartDate") prjQuery = prjQuery.OrderBy(p => p.Prj.StartDate);
                //        if (sorting.property == "EndDate") prjQuery = prjQuery.OrderBy(p => p.Prj.EndDate);
                //        if (sorting.property == "TotalEffort") prjQuery = prjQuery.OrderBy(p => p.Prj.TotalEffort);
                //        if (sorting.property == "EndDate") prjQuery = prjQuery.OrderBy(p => p.Prj.EndDate);
                //    }
                //    else
                //    {
                //        if (sorting.property == "Code") prjQuery = prjQuery.OrderByDescending(p => p.Prj.Code);
                //    }
                //}
                //else
                //{
                //    prjQuery = prjQuery.OrderByDescending(p => p.Prj.StartDate);
                //}

                count = prjQuery.Count();
                var projects = prjQuery.Skip(start).Take(limit).ToList();
                foreach (var project in projects)
                {
                    var membersBalance = project.PrjCostAcc.MembersBalance;

                    var effortBalanceHrs = project.PrjCostAcc.EffortBalanceHrs;

                    var costBalance = project.PrjCostAcc.CostBalance;

                    //var timeSheets = (from t in session.Query<Timesheet>() 
                    //                  where t.Project == project.Prj
                    //                  select new { 
                    //                      ActualStartDate = t.ActualStartDate,
                    //                      ActualHourUsed = t.ActualHourUsed,
                    //                      ProjectRole = t.ProjectRole,
                    //                      UserID = t.User.ID
                    //                  });
                    //var timeSheets = project.Prj.TimeSheets.Select( t => new { 
                    //                      ActualStartDate = t.ActualStartDate,
                    //                      ActualHourUsed = t.ActualHourUsed,
                    //                      ProjectRole = t.ProjectRole,
                    //                      UserID = t.User.ID
                    //                  }).ToList();
                    //var totalEffort = timeSheets.Sum(t => t.ActualHourUsed);
                    //decimal totalCost = 0;
                    //foreach (var item in timeSheets)
                    //{
                    //    var roleCost = item.ProjectRole.ProjectRoleRates
                    //        .GetEffectiveRatePerHours(item.ActualStartDate)
                    //        .FirstOrDefault();

                    //    totalCost += item.ActualHourUsed * roleCost;
                    //}

                    //long prjStatus = (project.Prj.Status != null) ? project.Prj.Status.ID : 0;
                    //var memberCount = (from tm in timeSheets select tm.UserID).Distinct().Count();

                    var prgPG = (from pg in session.Query<ProjectProgress>() where 
                                     project.Prj == pg.Project select pg).SingleOrDefault();

                    int progress = 0;
                    if (prgPG != null) progress = prgPG.PercentageProgress;
                    viewList.Add(new ProjectView
                    {
                        ID = project.Prj.ID,
                        Code = project.Prj.Code,
                        Name = project.Prj.NameTH,
                        NameTH = project.Prj.NameTH,
                        NameEN = project.Prj.NameEN,
                        //CustomerName = project.Prj.CustomerName,
                        StartDate = project.Prj.StartDate,
                        EndDate = project.Prj.EndDate,
                        //StatusID = prjStatus,
                        Members = membersBalance,
                        TotalEffort = effortBalanceHrs,
                        TotalCost = costBalance,
                        Progress = progress,
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

        public JsonResult GetEffortGroupByProjectRole(long projectID)
        {
            var data = new Dictionary<string, decimal>();

            using (var session = _sessionFactory.OpenSession())
            {
                var thePrj = (from x in session.Query<Project>()
                              where x.ID == projectID
                              select x).First();

                thePrj.TimeSheets.ForEach(t =>
                {
                    var key = t.ProjectRole.NameTH;
                    if (!data.ContainsKey(key))
                    {
                        data.Add(key, 0);
                    }

                    data[key] += t.ActualHourUsed;
                });
            }

            return Json(new
            {
                data = data.ToList(),
                success = true,
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCostGroupByProjectRole(long projectID)
        {
            var data = new Dictionary<string, decimal>();

            using (var session = _sessionFactory.OpenSession())
            {
                var thePrj = (from x in session.Query<Project>()
                              where x.ID == projectID
                              select x).First();

                thePrj.TimeSheets.ForEach(t =>
                {
                    var roleCost = t.ProjectRole.ProjectRoleRates
                            .GetEffectiveRatePerHours(t.ActualStartDate)
                            .FirstOrDefault();

                    var key = t.ProjectRole.NameTH;
                    if (!data.ContainsKey(key))
                    {
                        data.Add(key, 0);
                    }

                    data[key] += t.ActualHourUsed * roleCost;
                });
            }

            return Json(new
            {
                data = data.ToList(),
                success = true,
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCumulativeCostByDate(string projectCode)
        {
            var viewList = new List<CumulativeItemByDateView>();

            string startDate = "",
                endDate = "",
                contractStartDate = "",
                contractEndDate = "",
                endWarrantyDate = "";

            List<string> deliveryPhases = new List<string>();
            decimal total = 0,
                presalePhaseLV1 = 0,
                presalePhaseLV2 = 0,
                projectValue = 0,
                estimatedProjectValue = 0;

            int totalItems = 0;
            int totalGroup = 0;
            string theFormat = format2;

            using (var session = _sessionFactory.OpenSession())
            {
                var prj = (from p in session.Query<Project>()
                           where p.Code == projectCode
                           select p).Single();

                projectValue = prj.ProjectValue;
                estimatedProjectValue = prj.EstimateProjectValue;

                if (prj.StartDate.HasValue)
                {
                    startDate = prj.StartDate.Value.ToString(theFormat, forParseDate);
                    if (prj.EndDate.HasValue)
                    {
                        endDate = prj.EndDate.Value.ToString(theFormat, forParseDate);
                    }
                }

                if (prj.ContractStartDate.HasValue)
                {
                    contractStartDate = prj.ContractStartDate.Value.ToString(theFormat, forParseDate);
                    if (prj.ContractEndDate.HasValue)
                    {
                        contractEndDate = prj.ContractEndDate.Value.ToString(theFormat, forParseDate);
                    }
                }

                if (prj.WarrantyEndDate.HasValue)
                {
                    endWarrantyDate = prj.WarrantyEndDate.Value.ToString(theFormat, forParseDate);
                }
                var query = from tm in session.Query<Timesheet>()
                            where tm.Project == prj
                            orderby tm.ActualStartDate
                            group tm by new
                            {
                                Date = tm.ActualStartDate.Value.Date,
                            } into weekGrp
                            select new
                            {
                                Date = weekGrp.Key.Date,
                                Timesheets = weekGrp.ToList()
                            };

                foreach (var item in query)
                {
                    var timesheetDetails = new List<TimesheetDetail>();
                    foreach (var t in item.Timesheets)
                    {
                        var cost = t.ProjectRole.ProjectRoleRates
                                .GetEffectiveRatePerHours(t.ActualStartDate)
                                .FirstOrDefault();
                        var totalCost = t.ActualHourUsed * cost;
                        var tDetail = new TimesheetDetail
                        {
                            Phase = t.Phase.ID.ToString(),
                            Hours = t.ActualHourUsed,
                            Cost = totalCost,
                        };
                        timesheetDetails.Add(tDetail);
                    }

                    var c = new CumulativeItemByDateView
                    {
                        ID = item.Date.Ticks,
                        Date = item.Date.ToString(theFormat, forParseDate),

                        PreSale = timesheetDetails.Where(t => t.Phase == "1").Select(t => t.Cost).Sum(),
                        Implement = timesheetDetails.Where(t => t.Phase == "2").Select(t => t.Cost).Sum(),
                        Warranty = timesheetDetails.Where(t => t.Phase == "3").Select(t => t.Cost).Sum(),
                        Operation = timesheetDetails.Where(t => t.Phase == "4").Select(t => t.Cost).Sum(),
                    };

                    total += c.PreSale + c.Implement + c.Warranty + c.Operation;

                    viewList.Add(c);
                }

                viewList = viewList.ToList();
                totalItems = viewList.Count;
                int maxItems = 50;
                if (viewList.Count > maxItems)
                {
                    int groupSize = viewList.Count / maxItems;
                    int groupID = 1;
                    for (int i = 0; i < viewList.Count; i++)
                    {
                        viewList[i].Group = groupID;
                        if ((i + 1) % groupSize == 0) ++groupID;
                    }

                    var groupBy = from r in viewList
                                  group r by r.Group into grp
                                  select new
                                  {
                                      key = grp.Key,
                                      Count = grp.Count(),
                                      Date = grp.Last().Date,
                                      PreSale = grp.Sum(d => d.PreSale),
                                      Implement = grp.Sum(d => d.Implement),
                                      Warranty = grp.Sum(d => d.Warranty),
                                      Operation = grp.Sum(d => d.Operation),
                                  };

                    totalGroup = groupBy.Count();

                    var newViewList = new List<CumulativeItemByDateView>();
                    foreach (var item in groupBy)
                    {
                        var cumItemDateView = new CumulativeItemByDateView
                        {
                            Date = item.Date,
                            Group = item.key,
                            Count = item.Count,
                            PreSale = item.PreSale,
                            Implement = item.Implement,
                            Warranty = item.Warranty,
                            Operation = item.Operation,
                        };

                        newViewList.Add(cumItemDateView);
                    }

                    viewList = newViewList;
                }

                decimal sumPreSale = 0;
                decimal sumImplementAndDev = 0;
                decimal sumWarranty = 0;
                decimal sumOperation = 0;
                foreach (var item in viewList)
                {
                    sumPreSale = sumPreSale + item.PreSale;
                    item.PreSale = sumPreSale;

                    sumImplementAndDev = sumImplementAndDev + item.Implement;
                    item.Implement = sumImplementAndDev;

                    sumWarranty = sumWarranty + item.Warranty;
                    item.Warranty = sumWarranty;

                    sumOperation = sumOperation + item.Operation;
                    item.Operation = sumOperation;
                }

                var projectThresholdPresaleLv1 = (from pt in session.Query<ProjectThreshold>()
                                                  where pt.Name == PRESALE_LV_1
                                                  select pt).FirstOrDefault();
                if (projectThresholdPresaleLv1 != null)
                {
                    presalePhaseLV1 = projectThresholdPresaleLv1.LimitRatio;
                }

                var projectThresholdPresaleLv2 = (from pt in session.Query<ProjectThreshold>()
                                                  where pt.Name == PRESALE_LV_2
                                                  select pt).FirstOrDefault();
                if (projectThresholdPresaleLv2 != null)
                {
                    presalePhaseLV2 = projectThresholdPresaleLv2.LimitRatio;
                }

                var projectDeliveryPhases = (from pd in session.Query<ProjectDeliveryPhase>()
                                             where pd.Project == prj
                                             select pd).ToList();

                foreach (var projectDeliveryPhase in projectDeliveryPhases)
                {
                    var deliveryPhase = projectDeliveryPhase.DeliveryPhaseDate.ToString(theFormat, forParseDate);
                    deliveryPhases.Add(deliveryPhase);
                }
            }

            var result = new
            {
                startDate = startDate,

                contractStartDate = contractStartDate,
                contractEndDate = contractEndDate,

                endWarrantyDate = endWarrantyDate,

                endDate = endDate,

                deliveryPhases = deliveryPhases,

                total = total,
                totalItems = totalItems,
                totalGroup = totalGroup,
                data = viewList,
                success = true,

                projectValue = projectValue,
                estimatedProjectValue = estimatedProjectValue,

                presalePhaseLV1 = presalePhaseLV1 / 100,
                presalePhaseLV2 = presalePhaseLV2 / 100,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPhaseEffortByWeekly(string projectCode)
        {
            var viewList = new Dictionary<string, KeyValues>();//{ Key: 'xxxx', Values: [] }

            using (var session = _sessionFactory.OpenSession())
            {
                var query = from tm in session.Query<Timesheet>()
                            where tm.Project.Code == projectCode
                            orderby tm.ActualStartDate
                            group tm by new { Week = _weekProjector(tm.ActualStartDate.Value) } into weekGrp
                            select new
                            {
                                Week = weekGrp.Key.Week,
                                Timesheets = weekGrp.ToList()
                            };

                foreach (var week in query)
                {
                    var firstDayOfWeek = MemberDetail.FirstDayOfWeek(week.Timesheets.FirstOrDefault().ActualStartDate.Value);
                    var lastDayOfWeek = MemberDetail.LastDayOfWeek(week.Timesheets.FirstOrDefault().ActualStartDate.Value);
                    var period = string.Format("{0} - {1}", firstDayOfWeek.ToString(format), lastDayOfWeek.ToString(format));

                    var byPhases = from tm in week.Timesheets
                                   orderby tm.Phase.Order
                                   group tm by new { Phase = tm.Phase.NameTH } into phaseGrp
                                   select new
                                   {
                                       Phase = phaseGrp.Key.Phase,
                                       Timesheets = phaseGrp.ToList()
                                   };

                    foreach (var byPhase in byPhases)
                    {
                        string key = byPhase.Phase;
                        if (!viewList.ContainsKey(key))
                        {
                            viewList.Add(key, new KeyValues
                            {
                                Key = key,
                            });
                        }

                        var actualHourUsed = byPhase.Timesheets.Sum(t => t.ActualHourUsed);
                        viewList[key].Values.Add(
                            new KeyValue
                            {
                                Key = period,
                                Value = actualHourUsed,
                            });
                    }
                }


                foreach (var key in viewList.Keys)
                {
                    decimal accum = 0;
                    foreach (var v in viewList[key].Values)
                    {
                        accum = accum + v.Value;
                        v.Value = accum;
                    }
                }


            }

            var result = new
            {
                data = viewList.Values,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEffortTimesheetItem(string projectCode)
        {
            string theFormat = format2;
            var viewList = new List<EffortTimesheetItem>();
            var startTimesheet = DateTime.Now.AddMonths(-3);
            var endTimesheet = DateTime.Now;

            var projectRoles = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            {
                var prj = (from p in session.Query<Project>()
                           where p.Code == projectCode
                           select p).Single();

                var timesheets = (from tm in session.Query<Timesheet>()
                                  where tm.Project == prj
                                  select tm).ToList();

                var groupByProjectRoles = (from tm in timesheets
                                           where tm.Project == prj
                                           orderby tm.ProjectRole.Order
                                           select tm.ProjectRole.NameEN).Distinct();

                projectRoles = groupByProjectRoles.ToList();

                var query = from tm in timesheets
                            where tm.Project == prj
                            orderby tm.ActualStartDate
                            group tm by new
                            {
                                Date = tm.ActualStartDate.Value.Date,
                            } into weekGrp
                            select new
                            {
                                Date = weekGrp.Key.Date,
                                Timesheets = weekGrp.ToList()
                            };


                foreach (var item in query)
                {
                    var c = new EffortTimesheetItem
                    {
                        Date = item.Date.ToString(theFormat, forParseDate),
                        TotalItems = item.Timesheets.Count,
                    };

                    c.InitEfforts(projectRoles);

                    for (int i = 0; i < projectRoles.Count; i++)
                    {
                        var roleName = projectRoles[i];
                        var totalEffort = item.Timesheets
                            .Where(t => t.ProjectRole.NameEN == roleName)
                            .Select(t => t.ActualHourUsed).Sum();

                        c.Efforts[i] = totalEffort;
                    }

                    viewList.Add(c);
                }

                int maxItems = 50;
                if (viewList.Count > maxItems)
                {
                    int groupSize = viewList.Count / maxItems;
                    int groupID = 1;
                    for (int i = 0; i < viewList.Count; i++)
                    {
                        viewList[i].Group = groupID;
                        if ((i + 1) % groupSize == 0) ++groupID;
                    }

                    var groupBy = from r in viewList
                                  group r by r.Group into grp
                                  select new
                                  {
                                      key = grp.Key,
                                      Count = grp.Count(),
                                      Date = grp.Last().Date,
                                      EffortTimesheetItems = grp.ToList(),
                                  };

                    var newViewList = new List<EffortTimesheetItem>();
                    foreach (var item in groupBy)
                    {
                        var effortTimesheetItem = new EffortTimesheetItem
                        {
                            Date = item.Date,
                            Group = item.key,
                        };
                        effortTimesheetItem.InitEfforts(projectRoles);
                        foreach (var effort in item.EffortTimesheetItems)
                        {
                            for (int i = 0; i < effort.Efforts.Count; i++)
                            {
                                effortTimesheetItem.Efforts[i] += effort.Efforts[i];
                            }
                        }

                        newViewList.Add(effortTimesheetItem);
                    }

                    viewList = newViewList;
                }
            }

            var result = new
            {
                startDate = startTimesheet.ToString(theFormat, forParseDate),
                endDate = endTimesheet.ToString(theFormat, forParseDate),
                totalItems = viewList.Count(),
                projectRoles = projectRoles,
                data = viewList,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentTimesheetYears()
        {
            var viewList = new List<DepartmentTimesheetYear>();
            var resultList = new Dictionary<int, DepartmentTimesheetYearItem>();
            var departments = new List<string>();
            var years = new List<int>();
            using (var session = _sessionFactory.OpenSession())
            {
                //var timesheets = (from tm in session.Query<Timesheet>() select tm).ToList();

                var department_timesheets = from tm in session.Query<Timesheet>()
                                            where tm.Project.Code != "PJ-CWN000"
                                            && tm.ActualStartDate.Value.Year != 2013
                                            group tm by new
                                            {
                                                Year = tm.ActualStartDate.Value.Year,
                                                Department = tm.ActualUserDepartment.NameEN,
                                                ProjectRole = tm.ProjectRole.NameEN,
                                                ProjectRoleID = tm.ProjectRole.ID,
                                            } into yearDepartmentGrp
                                            select new
                                            {
                                                Year = yearDepartmentGrp.Key.Year,
                                                Department = yearDepartmentGrp.Key.Department,
                                                ProjectRole = yearDepartmentGrp.Key.ProjectRole,
                                                ProjectRoleID = yearDepartmentGrp.Key.ProjectRoleID,
                                                TotalEfforts = yearDepartmentGrp.Sum(t => t.ActualHourUsed),
                                            };

                var results = department_timesheets.ToList();

                departments = (from d in results
                               orderby d.Department
                               select d.Department).Distinct().ToList();

                years = (from d in results
                         orderby d.Year
                         select d.Year).Distinct().ToList();

                foreach (var item in results)
                {
                    viewList.Add(new DepartmentTimesheetYear
                    {
                        Year = item.Year,
                        Department = item.Department,
                        ProjectRole = item.ProjectRole,
                        ProjectRoleID = item.ProjectRoleID,
                        TotalEfforts = item.TotalEfforts,
                    });
                }

                foreach (var item in viewList)
                {
                    var estimated = new DateTime(item.Year, 1, 1);
                    var rojectRole = (from r in session.Query<ProjectRole>()
                                      where r.ID == item.ProjectRoleID
                                      select r).Single();

                    var roleCost = rojectRole.ProjectRoleRates
                            .GetEffectiveRatePerHours(estimated)
                            .FirstOrDefault();

                    item.TotalCost += item.TotalEfforts * roleCost;
                }

                //var departments = department_timesheets.

                var department_group = from tm in viewList
                                       group tm by new
                                       {
                                           Year = tm.Year,
                                           Department = tm.Department,
                                       } into yearDepartmentGrp
                                       select new DepartmentTimesheetYear
                                       {
                                           Year = yearDepartmentGrp.Key.Year,
                                           Department = yearDepartmentGrp.Key.Department,
                                           TotalCost = yearDepartmentGrp.Sum(t => t.TotalCost),
                                       };

                viewList = department_group.ToList();

                foreach (var item in viewList)
                {
                    if (!resultList.ContainsKey(item.Year))
                    {
                        var newD = new DepartmentTimesheetYearItem
                        {
                            Year = item.Year
                        };
                        newD.InitCosts(departments);
                        resultList.Add(item.Year, newD);
                    }

                    var d = resultList[item.Year];
                    for (int i = 0; i < departments.Count; i++)
                    {
                        if (departments[i] == item.Department)
                        {
                            d.Costs[i] += item.TotalCost;
                        }
                    }
                }
            }

            return Json(new
            {
                data = resultList.Values,
                years = years,
                departments = departments,
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTimesheetYears()
        {
            var viewList = new List<TimesheetYear>();
            //{ year: 2014, nonRecord: 260, nonProject: 150, project: 230 },
            //      { year: 2015, nonRecord: 260, nonProject: 150, project: 230 }
            using (var session = _sessionFactory.OpenSession())
            {
                var project_timesheets = from tm in session.Query<Timesheet>()
                                         group tm by new
                                         {
                                             Year = tm.ActualStartDate.Value.Date.Year,
                                         } 
                                         into yearGrp
                                         select new
                                         {
                                             Year = yearGrp.Key.Year,
                                             Timesheets = yearGrp.ToList()
                                         };

                foreach (var item in project_timesheets)
                {
                    var timesheets = item.Timesheets;
                    var projectEfforts = timesheets.Where(t => t.Project.Code != "PJ-CWN000").Sum( t => t.ActualHourUsed );
                    var nonProjectEfforts = timesheets.Where(t => t.Project.Code == "PJ-CWN000").Sum(t => t.ActualHourUsed);
                    var timesheetYear = new TimesheetYear
                    {
                        Year = item.Year,
                        ProjectEfforts = projectEfforts,
                        NonProjectEfforts = nonProjectEfforts
                    };

                    viewList.Add(timesheetYear);
                }

                viewList = viewList.Where(t => t.Year != 2013).OrderBy(t => t.Year).ToList();
            }

            return Json(viewList, JsonRequestBehavior.AllowGet);
        }
    }
}
