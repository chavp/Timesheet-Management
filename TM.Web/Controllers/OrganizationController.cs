using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PJ_CWN019.TM.Web.Controllers
{
    using NHibernate;
    using NHibernate.Linq;
    using Cwn.PM.BusinessModels.Entities;
    using PJ_CWN019.TM.Web.Models;
    using PJ_CWN019.TM.Web.Filters;
    using System.Web.SessionState;
    using System.Web.Security;
    using Cwn.PM.BusinessModels.Queries;
    using System.Globalization;
    using Cwn.PM.Reports.Values;
    using PJ_CWN019.TM.Web.Models.Providers;
    using WebMatrix.WebData;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;

    [ErrorLog]
    [ProfileLog]
    [Authorize(Roles = ConstAppRoles.Admin)]
    [FourceToChangeAttribute]
    [SessionState(SessionStateBehavior.Disabled)]
    [ValidateAntiForgeryTokenOnAllPosts]
    public class OrganizationController : Controller
    {
        ISessionFactory _sessionFactory = null;
        string nonProjectCode = "PJ-CWN000";
        string nonProjectRole = "Non-Role";

        // Features
        bool canUpdateAppRoleFeature = false;

        public OrganizationController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ActionResult Employees()
        {
            return View();
        }

        public ActionResult Structure()
        {
            return View();
        }

        public ActionResult ProjectPortfolio()
        {
            return View();
        }

        // API ******************************************************
        public JsonResult SearchEmployees(int start, int limit, string sort, string query = "")
        {
            var viewList = new List<EmployeeView>();
            int count = 0;

            using (var session = _sessionFactory.OpenSession())
            {
                var userList = (from u in session.Query<User>()
                                where (u.EmployeeID.ToString().Contains(query)
                                || u.FirstNameEN.Contains(query)
                                || u.FirstNameTH.Contains(query)
                                || u.LastNameEN.Contains(query)
                                || u.LastNameTH.Contains(query))
                                orderby u.EmployeeID, u.FirstNameTH, u.LastNameTH
                                let totalProjectMember = (from e in session.Query<ProjectMember>() where e.User == u select e).Count()
                                let totalTimesheet = (from e in session.Query<Timesheet>() where e.User == u select e).Count()
                                select new EmployeeView
                                {
                                    ID = u.ID,
                                    EmployeeID = u.EmployeeID,
                                    TitleID = (u.Title != null)? u.Title.ID : 0,
                                    FullName = u.FirstNameTH + " " + u.LastNameTH,
                                    Display = u.FirstNameTH + " " + u.LastNameEN,
                                    LastChangedPassword = u.LastPasswordChangedDate,
                                    LastLoginDate = u.LastLoginDate,
                                    Position = (u.Position != null) ? u.Position.NameTH : string.Empty,
                                    PositionID = (u.Position != null) ? u.Position.ID : 0,

                                    Division = u.Department.Division.NameTH,
                                    DivisionID = u.Department.Division.ID,

                                    Department = u.Department.NameTH,
                                    DepartmentID = u.Department.ID,

                                    Nickname = u.Nickname,

                                    Email = u.Email.Trim(),

                                    NameTH = u.FirstNameTH,
                                    LastTH = u.LastNameTH,
                                    NameEN = u.FirstNameEN,
                                    LastEN = u.LastNameEN,

                                    StartDate = u.StartDate,
                                    EndDate = u.EndDate,

                                    TotalProjectMember = totalProjectMember,
                                    TotalTimesheet = totalTimesheet,

                                    Status = u.Status.ToString()
                                });

                count = userList.Count();
                viewList = userList.Skip(start).Take(limit).ToList();

                viewList.ForEach(v =>
                {
                    var rolesDisplayeList = (from r in session.Query<AppRole>()
                                         from u in session.Query<User>()
                                         where r.Users.Contains(u)
                                         && u.EmployeeID == v.EmployeeID
                                         select r.Name).ToList();

                    //var rolesDisplayeList = Roles.GetRolesForUser(v.EmployeeID.ToString());
                    var appRole = string.Join("/", rolesDisplayeList);
                    v.AppRole = appRole;
                });
            }

            var result = new
            {
                data = viewList,
                total = count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 60 * 5)]
        public JsonResult GetTitleName(int start, int limit, string sort, string query = "")
        {
            var viewList = new List<TitleNameView>();
            int count = 0;

            using (var session = _sessionFactory.OpenSession())
            {
                var userList = (from x in session.Query<TitleName>()
                                orderby x.NameTH
                                select new TitleNameView
                                {
                                    ID = x.ID,
                                    Display = x.NameTH,
                                });

                count = userList.Count();
                viewList = userList.Skip(start).Take(limit).ToList();
            }

            var result = new
            {
                data = viewList,
                total = count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[OutputCache(Duration = 60 * 5)]
        public JsonResult GetPosition(int start, int limit, string sort, string query = "")
        {
            var viewList = new List<PositionView>();
            int count = 0;

            using (var session = _sessionFactory.OpenSession())
            {
                var userList = (from x in session.Query<Position>()
                                let empCount = (from e in session.Query<User>() where e.Position == x select e).Count()
                                let timesheetCount = (from t in session.Query<Timesheet>() where t.User.Position == x select t).Count()
                                orderby x.NameTH
                                select new PositionView
                                {
                                    ID = x.ID,
                                    Display = x.NameTH,
                                    Name = x.NameTH,
                                    PositionCost = x.Cost,

                                    EmployeeCount = empCount,
                                    TimesheetCount = timesheetCount
                                });

                count = userList.Count();
                viewList = userList.Skip(start).Take(limit).ToList();
            }

            var result = new
            {
                data = viewList,
                total = count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[OutputCache(Duration = 60 * 5)]
        public JsonResult GetDivision(int start, int limit, string sort, string query = "")
        {
            var viewList = new List<DivisionView>();
            int count = 0;

            using (var session = _sessionFactory.OpenSession())
            {
                var divisions = (from x in session.Query<Division>()
                                        select x);

                var divisionViewList = (from x in divisions
                                orderby x.NameTH
                                select new DivisionView
                                {
                                    ID = x.ID,
                                    Name = x.NameTH,
                                    DepartmentCount = x.Departments.Count
                                });

                count = divisionViewList.Count();
                viewList = divisionViewList.Skip(start).Take(limit).ToList();

                // cleansing Data
                var badString = "\r";
                foreach (var item in viewList)
                {
                    if (item.Name.Contains(badString))
                    {
                        item.Name = item.Name.Replace(badString, "");
                        var cleansing = divisions.Where(d => d.ID == item.ID).Single();
                        cleansing.ChangeName(item.Name);
                        session.Flush();
                    }
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

        //[OutputCache(Duration = 60 * 5)]
        public JsonResult GetDepartment(int start, int limit, string sort, string query = "")
        {
            var viewList = new List<DepartmentView>();
            int count = 0;

            using (var session = _sessionFactory.OpenSession())
            {
                var departments = (from x in session.Query<Department>() select x);

                var departmentViewList = (from x in departments
                                join d in session.Query<Division>() on x.Division equals d
                                where x.Division != null
                                let empCount = (from e in session.Query<User>() where e.Department == x select e).Count()
                                let timesheetCount = (from t in session.Query<Timesheet>() where t.User.Department == x select t).Count()
                                orderby x.NameTH
                                select new DepartmentView
                                {
                                    ID = x.ID,
                                    DivisionID = x.Division.ID,
                                    DivisionName = d.NameTH,
                                    Name = x.NameTH,
                                    UnderEmployees = empCount,
                                    TimesheetCount = timesheetCount
                                });

                count = departmentViewList.Count();
                viewList = departmentViewList.Skip(start).Take(limit).ToList();

                // cleansing Data
                var badString = "\r";
                foreach (var item in viewList)
                {
                    if (item.Name.Contains(badString))
                    {
                        item.Name = item.Name.Replace(badString, "");
                        var cleansing = departments.Where(d => d.ID == item.ID).Single();
                        cleansing.ChangeName(item.Name);
                        session.Flush();
                    }
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

        [HttpPost]
        public JsonResult SaveEmployee(EmployeeView employeeView)
        {
            var success = false;
            var msg = string.Empty;

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // Update
                if (employeeView.ID <= 0)
                {
                    var title = (from x in session.Query<TitleName>()
                                 where x.ID == employeeView.TitleID
                                 select x).Single();

                    var thePosition = (from x in session.Query<Position>()
                                     where x.ID == employeeView.PositionID
                                     select x).Single();

                    var department = (from x in session.Query<Department>()
                                      join y in session.Query<Division>() on x.Division equals y
                                      where y.ID == employeeView.DivisionID
                                      && x.ID == employeeView.DepartmentID
                                      select x).Single();

                    var newUser = new User
                    {
                        Title = title,
                        EmployeeID = employeeView.EmployeeID,
                        FirstNameTH = employeeView.NameTH,
                        LastNameTH = employeeView.LastTH,
                        FirstNameEN = employeeView.NameEN,
                        LastNameEN = employeeView.LastEN,
                        Nickname = employeeView.Nickname,
                        Email = employeeView.Email,

                        Position = thePosition,
                        Department = department,

                        StartDate = employeeView.StartDateText.ToNullableDateTime(),
                        EndDate = employeeView.EndDateText.ToNullableDateTime(),
                        Status = employeeView.Status.ToEmployeeStatus()
                    };

                    newUser.SetPassword(newUser.EmployeeID.ToString());

                    // update AppRoles
                    //if (this.canUpdateAppRoleFeature)
                    //{
                    //    var defaultAppRoles = (from x in session.Query<AppRole>()
                    //                           where x.Name == ConstAppRoles.Member
                    //                          || x.Name == employeeView.AppRole
                    //                           select x).ToList();

                    //    foreach (var appRole in defaultAppRoles)
                    //    {
                    //        appRole.Users.Add(newUser);
                    //    }
                    //}

                    var defaultAppRole = (from x in session.Query<AppRole>()
                                           where x.Name == ConstAppRoles.Member
                                           select x).Single();
                    defaultAppRole.Users.Add(newUser);

                    // add non-project
                    var nonProject = (from x in session.Query<Project>()
                                      where x.Code == this.nonProjectCode
                                      select x).FirstOrDefault();

                    if (nonProject != null)
                    {
                        var nonRole = (from x in session.Query<ProjectRole>()
                                       where x.NameTH == this.nonProjectRole
                                       || x.NameEN == this.nonProjectRole
                                       select x).FirstOrDefault();

                        if (nonRole != null)
                        {
                            nonProject.AddMemeber(newUser, nonRole);
                        }
                    }

                    session.Save(newUser);
                }
                else
                {
                    var theEmp = (from x in session.Query<User>()
                                  where x.ID == employeeView.ID
                                      select x).Single();

                    theEmp.Title = (from x in session.Query<TitleName>()
                                 where x.ID == employeeView.TitleID
                                 select x).Single();

                    theEmp.EmployeeID = employeeView.EmployeeID;
                    theEmp.FirstNameTH = employeeView.NameTH;
                    theEmp.LastNameTH = employeeView.LastTH;
                    theEmp.FirstNameEN = employeeView.NameEN;
                    theEmp.LastNameEN = employeeView.LastEN;
                    theEmp.Nickname = employeeView.Nickname;
                    theEmp.Email = employeeView.Email;

                    theEmp.Position = (from x in session.Query<Position>()
                                    where x.ID == employeeView.PositionID
                                    select x).Single();

                    theEmp.StartDate = employeeView.StartDateText.ToNullableDateTime();
                    theEmp.EndDate = employeeView.EndDateText.ToNullableDateTime();
                    theEmp.Status = employeeView.Status.ToEmployeeStatus();

                    var department = (from x in session.Query<Department>()
                                      join y in session.Query<Division>() on x.Division equals y
                                      where y.ID == employeeView.DivisionID
                                      && x.ID == employeeView.DepartmentID
                                      select x).Single();

                    theEmp.Department = department;

                    // clear AppRole
                    var rolesDisplayeList = (from r in session.Query<AppRole>()
                                             from u in session.Query<User>()
                                             where r.Users.Contains(u)
                                             && u.ID == employeeView.ID
                                             select r).ToList();
                    foreach (var appRole in rolesDisplayeList)
                    {
                        appRole.Users.Remove(theEmp);
                    }

                    var defaultAppRole = (from x in session.Query<AppRole>()
                                          where x.Name == ConstAppRoles.Member
                                          select x).Single();

                    var haveMember = false;
                    // add update
                    var appRoles = employeeView.AppRole.Split('/');
                    foreach (var appRoleUpdate in appRoles)
                    {
                        var appRole = (from x in session.Query<AppRole>()
                                       where x.Name == appRoleUpdate
                                              select x).Single();

                        appRole.Users.Add(theEmp);

                        if (appRole == defaultAppRole) haveMember = true;
                    }

                    if (!haveMember)
                    {
                        defaultAppRole.Users.Add(theEmp);
                    }

                    session.Update(theEmp);
                }

                transaction.Commit();
                success = true;
                msg = "การบันทึกพนักงานเสร็จสมบูรณ์";
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteEmployee(int employeeID)
        {
            var success = false;
            var msg = string.Empty;

            if (WebSecurity.CurrentUserName == employeeID.ToString())
            {
                return Json(new
                {
                    success = false,
                    message = "ระบบไม่อนุญาตให้ลบตัวเองออกจากระบบได้",
                }, JsonRequestBehavior.AllowGet);
            }

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var totalTimesheet = (from e in session.Query<Timesheet>() where e.User.EmployeeID == employeeID select e).Count();
                if (totalTimesheet > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "พบการกรอก timesheet ของพนักงานท่านนี้ในระบบ",
                    }, JsonRequestBehavior.AllowGet);
                }

                var emp = (from e in session.Query<User>()
                          where e.EmployeeID == employeeID
                          select e).Single();

                var rolesDisplayeList = (from r in session.Query<AppRole>()
                                         from u in session.Query<User>()
                                         where r.Users.Contains(u)
                                         && u.ID == emp.ID
                                         select r).ToList();

                foreach (var role in rolesDisplayeList)
                {
                    role.Users.Remove(emp);
                }

                var projectMembers = (from pm in session.Query<ProjectMember>()
                                      where pm.User == emp
                                      select pm).ToList();
                foreach (var pm in projectMembers)
                {
                    session.Delete(pm);
                }

                session.Delete(emp);

                transaction.Commit();
                success = true;
                msg = "ลบพนักงานเสร็จสมบูรณ์";
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDuplicatedEmployeeID(string employeeID)
        {
            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var query = from x in session.Query<User>()
                            where x.EmployeeID.ToString() == employeeID
                            select x;

                if (query.Count() > 0)
                {
                    return Json(new
                    {
                        valid = false,
                        success = true,
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new
            {
                valid = true,
                success = true,
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProject(int start, int limit, string sort, string query = "")
        {
            var viewList = new List<ProjectView>();
            int count = 0;
            
            using (var session = _sessionFactory.OpenSession())
            {
                var prjQuery = (from p in session.Query<Project>()
                                orderby p.StartDate descending
                                select new { 
                                    Prj = p, 
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
                    var timeSheets = project.Prj.TimeSheets;
                    var totalEffort = timeSheets.Sum(t => t.ActualHourUsed);
                    decimal totalCost = 0;
                    foreach (var item in timeSheets)
                    {
                        var roleCost = item.ProjectRole.ProjectRoleRates
                            .GetEffectiveRatePerHours(item.ActualStartDate)
                            .FirstOrDefault();

                        totalCost += item.ActualHourUsed * roleCost;
                    }

                    long prjStatus = (project.Prj.Status != null) ? project.Prj.Status.ID : 0;
                    var memberCount = (from tm in timeSheets select tm.User).Distinct().Count();
                    viewList.Add(new ProjectView
                    {
                        ID = project.Prj.ID,
                        Code = project.Prj.Code,
                        Name = project.Prj.NameTH,
                        NameTH = project.Prj.NameTH,
                        NameEN = project.Prj.NameEN,
                        //CustomerName = project.Prj.CustomerName,
                        Members = memberCount,
                        StartDate = project.Prj.StartDate,
                        EndDate = project.Prj.EndDate,
                        StatusID = prjStatus,
                        TotalEffort = totalEffort,
                        TotalCost = totalCost,
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
                            } into yearGrp
                            select new
                            {
                                Year = yearGrp.Key.Year,
                                ProjectEfforts = yearGrp
                                    .Where(d => d.Project.Code != "PJ-CWN000")
                                    .Sum(d => d.ActualHourUsed),
                                NonProjectEfforts = yearGrp
                                    .Where(d => d.Project.Code == "PJ-CWN000")
                                    .Sum(d => d.ActualHourUsed)
                            };

                foreach (var item in project_timesheets)
                {
                    var timesheetYear = new TimesheetYear
                    {
                        Year = item.Year,
                        ProjectEfforts = item.ProjectEfforts,
                        NonProjectEfforts = item.NonProjectEfforts
                    };

                    viewList.Add(timesheetYear);
                }

                viewList = viewList.Where(t => t.Year != 2013).OrderBy(t => t.Year).ToList();
            }

            return Json(viewList, JsonRequestBehavior.AllowGet);
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
                                                TotalEfforts = yearDepartmentGrp.Sum( t => t.ActualHourUsed ),
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

            return Json(new {
                data = resultList.Values,
                years = years,
                departments = departments,
            } , JsonRequestBehavior.AllowGet);
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

        string format = "yyyy-MM-dd";
        string format2 = "dd-MM-yyyy";
        CultureInfo forParseDate = new CultureInfo("en-US");

        public JsonResult GetCumulativeEffortByDate(string projectCode)
        {
            var viewList = new List<CumulativeItemByDateView>();

            string startDate = "";
            string endDate = "";
            decimal total = 0;

            using (var session = _sessionFactory.OpenSession())
            {
                var prj = (from p in session.Query<Project>()
                          where p.Code == projectCode
                          select p).Single();

                startDate = prj.StartDate.Value.ToString(format, forParseDate);
                if (prj.ContractEndDate.HasValue)
                {
                    endDate = prj.ContractEndDate.Value.AddMonths(5).ToString(format, forParseDate);
                }
                else
                {
                    endDate = prj.StartDate.Value.AddMonths(5).ToString(format, forParseDate);
                }

                var query = from tm in session.Query<Timesheet>()
                            where tm.Project == prj
                            orderby tm.ActualStartDate
                            group tm by new
                            {
                                Date = tm.ActualStartDate.Value,
                            } into weekGrp
                            select new
                            {
                                Date = weekGrp.Key.Date,
                                Timesheets = weekGrp.ToList()
                            };
                 
                foreach (var item in query)
                {
                    var c = new CumulativeItemByDateView
                    {
                        ID = item.Date.Ticks,
                        Date = item.Date.ToString(format, forParseDate),

                        PreSale = item.Timesheets.Where(t => t.Phase.ID == 1).Select(t => t.ActualHourUsed).Sum(),
                        Implement = item.Timesheets.Where(t => t.Phase.ID == 2).Select(t => t.ActualHourUsed).Sum(),
                        Warranty = item.Timesheets.Where(t => t.Phase.ID == 3).Select(t => t.ActualHourUsed).Sum(),
                        Operation = item.Timesheets.Where(t => t.Phase.ID == 4).Select(t => t.ActualHourUsed).Sum(),
                    };

                    total += c.PreSale + c.Implement + c.Warranty + c.Operation;

                    viewList.Add(c);
                }


                viewList = viewList.ToList();

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
            }

            var result = new
            {
                startDate = startDate,
                endDate = endDate,
                total = total,
                data = viewList,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCumulativeCostByDate(string projectCode)
        {
            var viewList = new List<CumulativeItemByDateView>();

            string startDate = "";
            string endDate = "";
            decimal total = 0;
            int totalItems = 0;
            int totalGroup = 0;
            string theFormat = format2;

            using (var session = _sessionFactory.OpenSession())
            {
                var prj = (from p in session.Query<Project>()
                           where p.Code == projectCode
                           select p).Single();

                if (prj.StartDate.HasValue)
                {
                    startDate = prj.StartDate.Value.ToString(theFormat, forParseDate);
                    if (prj.ContractEndDate.HasValue)
                    {
                        endDate = prj.ContractEndDate.Value.AddMonths(5).ToString(theFormat, forParseDate);
                    }
                    else
                    {
                        endDate = prj.StartDate.Value.AddMonths(5).ToString(theFormat, forParseDate);
                    }
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
            }

            var result = new
            {
                startDate = startDate,
                endDate = endDate,
                total = total,
                totalItems = totalItems,
                totalGroup = totalGroup,
                data = viewList,
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

        Func<DateTime, int> _weekProjector =
                    d => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                        d,
                        CalendarWeekRule.FirstFourDayWeek,
                        DayOfWeek.Sunday);

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
                    var period = string.Format("{0} - {1}", firstDayOfWeek.ToString("dd/MM/yyyy"), lastDayOfWeek.ToString("dd/MM/yyyy"));

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
        // END API ******************************************************

        // New API Phase 2 *********************
        [HttpPost]
        public JsonResult SaveDivision(DivisionView divisionView)
        {
            var success = false;
            var msg = string.Empty;
            var dupMessage = "พบชื่อฝ่ายนี้อยู่ในระบบแล้ว กรุณาระบุชื่อใหม่";
            var successMessage = "การบันทึกฝ่ายเสร็จสมบูรณ์";

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // Save
                if (divisionView.ID <= 0)
                {
                    // check Dup
                    var div = (from d in session.Query<Division>()
                                   where d.NameTH == divisionView.Name
                                   select d).FirstOrDefault();

                    if (div != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var newDiv = new Division(divisionView.Name, null);
                        session.Save(newDiv);
                    }
                }
                // Update
                else 
                {
                    // check Dup
                    var div = (from d in session.Query<Division>()
                               where d.NameTH == divisionView.Name
                               && d.ID != divisionView.ID
                               select d).FirstOrDefault();

                    if (div != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var updateDiv = (from d in session.Query<Division>()
                                         where d.ID == divisionView.ID
                                         select d).Single();

                        updateDiv.ChangeName(divisionView.Name);
                        session.Update(updateDiv);
                    }
                }

                transaction.Commit();
                success = true;
                msg = successMessage;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteDivision(int id)
        {
            var success = false;
            var msg = string.Empty;

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var deleteDiv = (from d in session.Query<Division>()
                                 where d.ID == id
                                 select d).Single();

                if (deleteDiv.Departments.Count > 0)
                {
                    msg = "ไม่สามารถลบฝ่ายที่มีแผนกอยู่ภายใต้ได้ กรุณาลบข้อมูลแผนกภายใต้ฝ่ายนี้ทั้งหมดก่อน";
                    return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                }

                session.Delete(deleteDiv);

                transaction.Commit();
                success = true;
                msg = "ลบฝ่ายเสร็จสมบูรณ์";
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveDepartment(DepartmentView departmentView)
        {
            var success = false;
            var msg = string.Empty;
            var dupMessage = "พบชื่อแผนกนี้อยู่ในระบบแล้ว กรุณาระบุชื่อใหม่";
            var successMessage = "การบันทึกแผนกเสร็จสมบูรณ์";

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // Save
                if (departmentView.ID <= 0)
                {
                    // check Dup
                    var dept = (from d in session.Query<Department>()
                               where d.NameTH == departmentView.Name
                               select d).FirstOrDefault();

                    if (dept != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var div = (from d in session.Query<Division>()
                                    where d.ID == departmentView.DivisionID
                                    select d).Single();

                        var newDiv = new Department(departmentView.Name, div);
                        session.Save(newDiv);
                    }
                }
                // Update
                else
                {
                    // check Dup
                    var dept = (from d in session.Query<Department>()
                                where d.NameTH == departmentView.Name
                               && d.ID != departmentView.ID
                               select d).FirstOrDefault();

                    if (dept != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var div = (from d in session.Query<Division>()
                                   where d.ID == departmentView.DivisionID
                                   select d).Single();

                        var updateDept = (from d in session.Query<Department>()
                                          where d.ID == departmentView.ID
                                         select d).Single();

                        updateDept.ChangeNameOrDivision(departmentView.Name, div);
                        session.Update(updateDept);
                    }
                }

                transaction.Commit();
                success = true;
                msg = successMessage;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteDepartment(int id)
        {
            var success = false;
            var msg = string.Empty;

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var deleteDept = (from d in session.Query<Department>()
                                 where d.ID == id
                                 select d).Single();

                var empCount = (from e in session.Query<User>() where e.Department == deleteDept select e).Count();
                if (empCount > 0)
                {
                    msg = "ไม่สามารถลบแผนกนี้ได้ กรุณาลบข้อมูลพนักงานภายใต้แผนกนี้ทั้งหมดก่อน";
                    return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                }

                var timesheetCount = (from t in session.Query<Timesheet>() where t.User.Department == deleteDept select t).Count();
                if (timesheetCount > 0)
                {
                    msg = "ไม่สามารถลบแผนกนี้ได้ กรุณาลบข้อมูลบันทึกการทำงานภายใต้แผนกนี้ทั้งหมดก่อน";
                    return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                }

                session.Delete(deleteDept);

                transaction.Commit();
                success = true;
                msg = "ลบแผนกเสร็จสมบูรณ์";
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SavePosition(PositionView positionView)
        {
            var success = false;
            var msg = string.Empty;
            var dupMessage = "พบชื่อตำแหน่งนี้อยู่ในระบบแล้ว กรุณาระบุชื่อใหม่";
            var successMessage = "การบันทึกตำแหน่งเสร็จสมบูรณ์";

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // Save
                if (positionView.ID <= 0)
                {
                    // check Dup
                    var po = (from d in session.Query<Position>()
                                where d.NameTH == positionView.Name
                                select d).FirstOrDefault();

                    if (po != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var newPo = new Position(positionView.Name, positionView.PositionCost.GetValueOrDefault());
                        session.Save(newPo);
                    }
                }
                // Update
                else
                {
                    // check Dup
                    var po = (from d in session.Query<Position>()
                              where d.NameTH == positionView.Name
                               && d.ID != positionView.ID
                                select d).FirstOrDefault();

                    if (po != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var updatePo = (from d in session.Query<Position>()
                                        where d.ID == positionView.ID
                                          select d).Single();

                        updatePo.ChangeNameOrCost(positionView.Name, positionView.PositionCost.GetValueOrDefault());
                        session.Update(updatePo);
                    }
                }

                transaction.Commit();
                success = true;
                msg = successMessage;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeletePosition(int id)
        {
            var success = false;
            var msg = string.Empty;

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var deletePo = (from d in session.Query<Position>()
                                  where d.ID == id
                                  select d).Single();

                var empCount = (from e in session.Query<User>() where e.Position == deletePo select e).Count();
                if (empCount > 0)
                {
                    msg = "ไม่สามารถลบตำแหน่งนี้ได้ กรุณาลบข้อมูลพนักงานภายใต้ตำแหน่งนี้ทั้งหมดก่อน";
                    return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                }

                var timesheetCount = (from t in session.Query<Timesheet>() where t.User.Position == deletePo select t).Count();
                if (timesheetCount > 0)
                {
                    msg = "ไม่สามารถลบตำแหน่งนี้ได้ กรุณาลบข้อมูลบันทึกการทำงานภายใต้ตำแหน่งนี้ทั้งหมดก่อน";
                    return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                }

                session.Delete(deletePo);

                transaction.Commit();
                success = true;
                msg = "ลบตำแหน่งเสร็จสมบูรณ์";
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }
        // END API ******************************************************

    }

    public class Sorting
    {
        public string property { set; get; }
        public Direction direction { set; get; }

        public enum Direction
        {
            ASC, DESC
        }
    }

    public class KeyValues
    {
        public KeyValues()
        {
            Values = new List<KeyValue>();
        }

        public string Key { get; set; }
        public List<KeyValue> Values { get; set; }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public decimal Value { get; set; }
    }
}
