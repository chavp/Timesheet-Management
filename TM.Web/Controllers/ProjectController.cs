using NHibernate;
using PJ_CWN019.TM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace PJ_CWN019.TM.Web.Controllers
{
    using PowerfulExtensions.Linq;
    using Cwn.PM.BusinessModels.Entities;
    using NHibernate;
    using NHibernate.Linq;
    using WebMatrix.WebData;
    using System.Web.Security;
    using PJ_CWN019.TM.Web.Filters;
    using System.Globalization;
    using PJ_CWN019.TM.Web.Extensions;
    using System.Threading.Tasks;
    using Cwn.PM.BusinessModels.Values;
    using PJ_CWN019.TM.Web.Models.Providers;

    [ErrorLog]
    [ProfileLog]
    [Authorize(Roles = ConstAppRoles.ProjectOwner+ ", " + ConstAppRoles.Manager + ", " + ConstAppRoles.Admin)]
    [FourceToChangeAttribute]
    [SessionState(SessionStateBehavior.Disabled)]
    [ValidateAntiForgeryTokenOnAllPosts]
    public class ProjectController : Controller
    {
        string dateFormat = "dd/MM/yyyy";
        CultureInfo forParseDate = new CultureInfo("en-US");

        ISessionFactory _sessionFactory = null;
        public ProjectController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        [Authorize(Roles = ConstAppRoles.Manager + ", " + ConstAppRoles.Admin)]
        public ActionResult Search()
        {
            if (Roles.IsUserInRole(ConstAppRoles.Manager))
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    var manager = (from u in session.Query<User>()
                                   where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                   select u).First();
                    ViewBag.UserDivisionID = manager.Department.Division.ID;
                    ViewBag.UserDepartmentID = manager.Department.ID;
                }
            }
            return View();
        }

        public ActionResult SearchReadOnly()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var manager = (from u in session.Query<User>()
                               where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                               select u).First();

                ViewBag.UserDivisionID = manager.Department.Division.ID;
                ViewBag.UserDepartmentID = manager.Department.ID;
            }
            return View();
        }

        public ActionResult ProjectProgress()
        {
            ViewBag.MaxDate = DateTime.Now.ToString(dateFormat, forParseDate);

            return View();
        }

        public ActionResult Activities()
        {
            return View();
        }

        // WEB API *************************************************************
        public JsonResult FindProject(
            string projectCode, string projectName, 
            string fromDateText, string toDateText,
            bool isOwner,
            int start, int limit, string sort, string query = "", bool isForDepartment = false)
        {
            var viewList = new List<ProjectView>();
            int count;

            using (var session = _sessionFactory.OpenSession())
            {
                if (isOwner)
                {
                    var manager = (from u in session.Query<User>()
                                             where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                             select u).First();

                    var prjQuery = (from p in session.Query<Project>()
                                    join m in session.Query<ProjectMember>() on p equals m.Project
                                    let totalTimesheet = (from t in session.Query<Timesheet>() where t.Project == p select t).Count()
                                    where p.NameTH.Contains(projectName)
                                    && p.Code.Contains(projectCode)
                                    && m.User == manager
                                    && m.ProjectRole.IsOwner
                                    select new { 
                                        Prj = p, 
                                        Members = p.Members.Count, 
                                        TotalTimesheet = totalTimesheet 
                                    });

                    if (!string.IsNullOrEmpty(query))
                    {
                        prjQuery = prjQuery.Where(p =>
                            p.Prj.Code.Contains(query) 
                            || p.Prj.NameTH.Contains(query)
                            || p.Prj.NameEN.Contains(query));
                    }
                    else
                    {
                        prjQuery = prjQuery.OrderByDescending(p => p.Prj.StartDate);
                    }

                    prjQuery = prjQuery.OrderBy(prj => prj.Prj.Code);
                    count = prjQuery.Count();

                    foreach (var project in prjQuery.Skip(start).Take(limit))
                    {
                        var vm = project.Prj.ToViewModel(project.Members, project.TotalTimesheet);

                        viewList.Add(vm);
                    }
                }
                else
                {

                    var manager = (from u in session.Query<User>()
                                   where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                   select u).Single();

                    var managerDepartment = manager.Department;

                    var prjQuery = (from p in session.Query<Project>()
                                    let members = (from m in session.Query<ProjectMember>()
                                                   where m.Project == p
                                                   && m.User.Department == managerDepartment
                                                   select m).Count()
                                    let owner = (from pm in session.Query<ProjectMember>()
                                                 where pm.User == manager 
                                                 && pm.Project == p
                                                 && pm.ProjectRole.IsOwner
                                                 select pm).FirstOrDefault()
                                    let totalTimesheet = (from t in session.Query<Timesheet>() where t.Project == p select t).Count()
                                    where p.NameTH.Contains(projectName)
                                    && p.Code.Contains(projectCode)
                                    //&& members > 0
                                    select new { 
                                        Prj = p, 
                                        Members = members, 
                                        TotalMembers = p.Members.Count, 
                                        TotalTimesheet = totalTimesheet,
                                        Owner = owner
                                    });

                    if (isForDepartment)
                    {
                        prjQuery = prjQuery.Where(p => p.Members > 0);

                        viewList.Add(new ProjectView
                        {
                            ID = -1,
                            Name = "ทั้งหมด",
                            Code = "",
                            //Name = project.Prj.NameTH,
                            //Members = project.TotalMembers,
                            //StartDate = project.Prj.StartDate,
                            //EndDate = project.Prj.EndDate
                        });
                    }

                    if (!string.IsNullOrEmpty(query))
                    {
                        prjQuery = prjQuery.Where(p => 
                            p.Prj.Code.Contains(query) 
                            || p.Prj.NameTH.Contains(query)
                            || p.Prj.NameEN.Contains(query));
                        prjQuery = prjQuery.OrderBy(p => p.Prj.Code);
                    }
                    else
                    {
                        prjQuery = prjQuery.OrderByDescending(p => p.Prj.StartDate);
                    }

                    prjQuery = prjQuery.OrderBy(prj => prj.Prj.Code);
                    count = prjQuery.Count();

                    foreach (var project in prjQuery.Skip(start).Take(limit))
                    {
                        var vm = project.Prj.ToViewModel(project.TotalMembers, project.TotalTimesheet);
                        vm.IsOwner = (project.Owner != null) ? true : false;
                        viewList.Add(vm);
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

        [OutputCache(Duration = 60 * 5)]
        public JsonResult FindProjectForOwner(
            string projectCode, string projectName,
            string fromDateText, string toDateText,
            bool isOwner,
            int start, int limit, string sort, string query = "")
        {
            var viewList = new List<ProjectView>();
            int count;

            using (var session = _sessionFactory.OpenSession())
            {
                var owner = (from u in session.Query<User>()
                                         where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                         select u).First();

                var prjQuery = (from p in session.Query<Project>()
                                let cPM = (from m in session.Query<ProjectMember>()
                                           where m.User == owner 
                                           && m.Project == p 
                                           && m.ProjectRole.IsOwner
                                            select m).Count()
                                //let members = p.Members.Count
                                where p.NameTH.Contains(projectName)
                                && p.Code.Contains(projectCode)
                                && cPM > 0
                                select new { Prj = p, Members = p.Members.Count });

                if (!string.IsNullOrEmpty(query))
                {
                    prjQuery = prjQuery.Where(p => p.Prj.Code.StartsWith(query));
                    prjQuery = prjQuery.OrderBy(p => p.Prj.Code);
                }
                else
                {
                    prjQuery = prjQuery.OrderByDescending(p => p.Prj.StartDate);
                }

                prjQuery = prjQuery.OrderBy(prj => prj.Prj.Code);
                count = prjQuery.Count();
                
                foreach (var project in prjQuery.Skip(start).Take(limit))
                {
                    var vm = project.Prj.ToViewModel(project.Members);
                    viewList.Add(vm);
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

        [OutputCache(Duration = 60 * 5)]
        public JsonResult GetDivision(string includeAll = null)
        {
            var viewList = new List<DivisionView>();
            int count;
            if (!string.IsNullOrEmpty(includeAll))
            {
                viewList.Add(new DivisionView
                {
                    ID = -1,
                    Name = "ทั้งหมด",
                });
            }

            using (var session = _sessionFactory.OpenSession())
            {
                var query = (from d in session.Query<Division>()
                             select d);

                count = query.Count();
                foreach (var project in query)
                {
                    viewList.Add(new DivisionView
                    {
                        ID = project.ID,
                        Name = project.NameTH,
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

        [OutputCache(Duration = 60 * 5)]
        public JsonResult GetProjectRole(string includeAll = null)
        {
            var viewList = new List<ProjectRoleView>();
            int count;

            using (var session = _sessionFactory.OpenSession())
            {
                var query = (from d in session.Query<ProjectRole>()
                             select d);

                if (Roles.IsUserInRole("ProjectOwner")
                    && !Roles.IsUserInRole("Manager")
                    && !Roles.IsUserInRole("Admin"))
                {
                    query = query.Where(pr => !pr.IsOwner);
                }

                query = query.OrderBy(r => r.Order);
                count = query.Count();
                foreach (var project in query)
                {
                    viewList.Add(new ProjectRoleView
                    {
                        ProjectRoleID = project.ID,
                        ProjectRoleName = project.NameTH,
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

        [OutputCache(Duration = 60 * 5)]
        public JsonResult GetProjectStatus(string includeAll = null)
        {
            var viewList = new List<ProjectStatusView>();
            int count;
            if (!string.IsNullOrEmpty(includeAll))
            {
                viewList.Add(new ProjectStatusView
                {
                    ID = -1,
                    Name = "All",
                });
            }

            using (var session = _sessionFactory.OpenSession())
            {
                var query = (from d in session.Query<ProjectStatus>()
                             select d);

                count = query.Count();
                foreach (var project in query)
                {
                    viewList.Add(new ProjectStatusView
                    {
                        ID = project.ID,
                        Name = project.Name,
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

        public JsonResult SaveMemberProjectRole(string projectCode, List<ProjectMemberView> projectMemberViewList)
        {
            var success = false;
            var msg = string.Empty;

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var project = (from p in session.Query<Project>()
                               where p.Code == projectCode
                               select p).Single();

                foreach (var prjMember in projectMemberViewList)
                {
                    var projectRole = (from pr in session.Query<ProjectRole>()
                                       where pr.NameTH == prjMember.ProjectRoleName
                                       select pr).Single();

                    var member = (from m in session.Query<ProjectMember>()
                                  where m.ID == prjMember.ID
                                  select m).FirstOrDefault();

                    if (member != null)
                    {
                        member.ProjectRole = projectRole;
                    }
                    else
                    {
                        if (project.ContainsMember(prjMember.EmployeeID))
                        {
                            return Json(new { 
                                success = false,
                                message = "รหัสพนักงาน " + prjMember.EmployeeID + " ถูกกำหนดในโครงการนี้แล้ว กรุณากำหนดรหัสพนักงานท่านใหม่"
                            }, JsonRequestBehavior.AllowGet);
                        }

                        var user = (from u in session.Query<User>()
                                    where u.EmployeeID == prjMember.EmployeeID
                                    select u).Single();

                        project.AddMemeber(user, projectRole);
                    }
                }

                transaction.Commit();
                success = true;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = ConstAppRoles.Manager + ", " + ConstAppRoles.Admin)]
        public JsonResult RemoveProjectMember(string projectCode, List<long> projectMemberIDList)
        {
            var success = false;
            var msg = string.Empty;

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var project = (from p in session.Query<Project>()
                               where p.Code == projectCode
                               select p).Single();

                foreach (var pmID in projectMemberIDList)
                {
                    var pm = (from u in session.Query<ProjectMember>()
                              where u.ID == pmID
                              select u).Single();

                    session.Delete(pm);
                    //project.Members.Remove(pm);
                }
                transaction.Commit();

                msg = "ลบพนักงานออกจากโครงการนี้เสร็จสมบูรณ์";
                success = true;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 60 * 5)]
        public JsonResult GetDepartment(long divisionID = -1, string includeAll = null)
        {
            var viewList = new List<DepartmentView>();
            int count;
            if (!string.IsNullOrEmpty(includeAll))
            {
                viewList.Add(new DepartmentView
                {
                    ID = -1,
                    DivisionID = -1,
                    Name = "ทั้งหมด",
                });
            }

            using (var session = _sessionFactory.OpenSession())
            {
                var query = (from d in session.Query<Department>()
                             select d);

                if (divisionID != -1)
                {
                    query = query.Where(d => d.Division.ID == divisionID);
                }
                count = query.Count();
                foreach (var d in query)
                {
                    viewList.Add(new DepartmentView
                    {
                        ID = d.ID,
                        DivisionID = d.Division.ID,
                        Name = d.NameTH,
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

        [OutputCache(Duration = 60 * 5)]
        public JsonResult FindEmployee(
            long divisionID, long departmentID,
            string employeeID, string employeeFirstName, string employeeLastName,
            string projectCode,
            List<int> withoutEmpIDList,
            bool isOwner,
            int start, int limit, string query = "")
        {
            var viewList = new List<EmployeeView>();
            int count;

            withoutEmpIDList = withoutEmpIDList ?? new List<int>();

            using (var session = _sessionFactory.OpenSession())
            {
                var manager = (from u in session.Query<User>()
                               where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                               select u).First();

                if (isOwner)
                {
                    var memberPMProjects = (from p in session.Query<Project>()
                                            join m in session.Query<ProjectMember>() on p equals m.Project
                                            where m.User == manager
                                            select p);

                    var mebers = new List<ProjectMember>();
                    foreach (var p in memberPMProjects)
                    {
                        mebers.AddRange(p.Members);
                    }

                    mebers = mebers
                        .OrderBy(u => u.User.EmployeeID)
                        .Distinct(u => u.User.ID).ToList();

                    if (!string.IsNullOrEmpty(query))
                    {
                        mebers = mebers.Where(
                            u => u.User.EmployeeID.ToString().Contains(query)
                                || u.User.FirstNameTH.ToString().Contains(query)
                                || u.User.LastNameTH.ToString().Contains(query)
                                || u.User.FirstNameEN.ToString().Contains(query)
                                || u.User.LastNameEN.ToString().Contains(query)).ToList();
                    }

                    count = mebers.Count();
                    foreach (var pm in mebers.Skip(start).Take(limit))
                    {
                        var user = pm.User;
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
                else
                {
                    var allMemberProjects = (from p in session.Query<Project>()
                                            join m in session.Query<ProjectMember>() on p equals m.Project
                                             where p.Code == projectCode
                                            select m.User.EmployeeID).ToList();

                    allMemberProjects.AddRange(withoutEmpIDList);

                    if (Roles.IsUserInRole("Admin"))
                    {
                        var userQuery = (from u in session.Query<User>()
                                         where u.Status == EmployeeStatus.Work
                                         && u.EmployeeID.ToString().Contains(employeeID)
                                         && !allMemberProjects.Contains(u.EmployeeID)
                                         && u.FirstNameTH.Contains(employeeFirstName)
                                         && u.LastNameTH.Contains(employeeLastName)
                                         select u);

                        if (divisionID > 0)
                        {
                            userQuery = userQuery.Where(u => u.Department.Division.ID == divisionID);
                        }
                        if (departmentID > 0)
                        {
                            userQuery = userQuery.Where(u => u.Department.ID == departmentID);
                        }

                        if (!string.IsNullOrEmpty(query))
                        {
                            userQuery = userQuery.Where(
                                u => u.EmployeeID.ToString().Contains(query)
                                    || u.FirstNameTH.ToString().Contains(query)
                                    || u.LastNameTH.ToString().Contains(query)
                                    || u.FirstNameEN.ToString().Contains(query)
                                    || u.LastNameEN.ToString().Contains(query));
                        }

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
                    else 
                    {
                        // for Department Manager
                        var userQuery = (from u in session.Query<User>()
                                         where u.Status == EmployeeStatus.Work
                                         && u.EmployeeID.ToString().Contains(employeeID)
                                         && !allMemberProjects.Contains(u.EmployeeID)
                                         && u.FirstNameTH.Contains(employeeFirstName)
                                         && u.LastNameTH.Contains(employeeLastName)
                                         && u.Department == manager.Department
                                         select u);

                        if (divisionID > 0)
                        {
                            userQuery = userQuery.Where(u => u.Department.Division.ID == divisionID);
                        }
                        if (departmentID > 0)
                        {
                            userQuery = userQuery.Where(u => u.Department.ID == departmentID);
                        }

                        if (!string.IsNullOrEmpty(query))
                        {
                            userQuery = userQuery.Where(
                                u => u.EmployeeID.ToString().Contains(query)
                                    || u.FirstNameTH.ToString().Contains(query)
                                    || u.LastNameTH.ToString().Contains(query)
                                    || u.FirstNameEN.ToString().Contains(query)
                                    || u.LastNameEN.ToString().Contains(query));
                        }

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

        [OutputCache(Duration = 60 * 5)]
        public JsonResult GetAllCustomer()
        {
            var viewList = new List<CustomerView>();
            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var query = (from x in session.Query<Customer>() select x);
                foreach (var item in query)
                {
                    var cust = new CustomerView
                    {
                        ID = item.ID,
                        Name = item.Name,
                    };

                    viewList.Add(cust);
                }
            }
            return Json(viewList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FindProjectMember(
            string projectCode,
            int start, int limit)
        {
            var viewList = new List<ProjectMemberView>();
            int count;

            using (var session = _sessionFactory.OpenSession())
            {

                var query = (from pm in session.Query<ProjectMember>()
                             orderby pm.User.EmployeeID
                             join r in session.Query<ProjectRole>() on pm.ProjectRole equals r
                             let projectRoleID = (pm.ProjectRole == null) ? 0 : pm.ProjectRole.ID
                             let projectRoleName = (pm.ProjectRole == null) ? string.Empty : pm.ProjectRole.NameTH
                             let projectRoleOrder = (pm.ProjectRole == null) ? int.MaxValue : pm.ProjectRole.Order
                             where pm.Project.Code == projectCode
                             select new
                             {
                                 ID = pm.ID,
                                 EmployeeID = pm.User.EmployeeID,
                                 DepartmentID = pm.User.Department.ID,

                                 FullName = string.Format("{0} {1}", pm.User.FirstNameTH, pm.User.LastNameTH),
                                 Position = pm.User.Position.NameTH,
                                 ProjectCode = pm.Project.Code,

                                 ProjectRoleID = projectRoleID,
                                 ProjectRoleName = projectRoleName,
                                 ProjectRoleOrder = projectRoleOrder,

                                 IsOwner = r.IsOwner,
                             });

                count = query.Count();
                var queryPage = query.Skip(start).Take(limit).ToList();

                foreach (var pm in queryPage)
                {
                    var mv = new ProjectMemberView
                    {
                        ID = pm.ID,
                        EmployeeID = pm.EmployeeID,
                        DepartmentID = pm.DepartmentID,

                        FullName = pm.FullName,
                        Position = pm.Position,
                        ProjectCode = pm.ProjectCode,
                        ProjectRoleID = pm.ProjectRoleID,
                        ProjectRoleName = pm.ProjectRoleName,
                    };
                    viewList.Add(mv);

                    if (Roles.IsUserInRole("ProjectOwner") && !pm.IsOwner)
                    {
                        mv.CanEditProjectRole = true;
                    }

                    if (Roles.IsUserInRole("Manager"))
                    {
                        var manager = (from u in session.Query<User>()
                                       where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                       select u).First();

                        if (mv.DepartmentID == manager.Department.ID)
                        {
                            mv.CanRemove = true;
                            mv.CanEditProjectRole = true;
                        }

                        var cYourOwner = (from myPM in session.Query<ProjectMember>()
                                         where myPM.User == manager
                                         && myPM.Project.Code == pm.ProjectCode
                                         && myPM.ProjectRole.IsOwner
                                          select myPM).Count();

                        if (cYourOwner > 0)
                        {
                            mv.CanEditProjectRole = true;
                        }
                    }

                    if (Roles.IsUserInRole("Admin"))
                    {
                        mv.CanRemove = true;
                        mv.CanEditProjectRole = true;
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

        public JsonResult FindProjectProgressUpdateLog(
            string projectCode,
            int start, int limit)
        {
            var viewList = new List<ProjectProgressUpdateLogView>();
            int count = 0;

            var result = new
            {
                data = viewList,
                total = count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCumulativeEffortByWeek(string projectCode)
        {
            var viewList = new List<CumulativeEffortByWeekView>();

            using (var session = _sessionFactory.OpenSession())
            {
                Func<DateTime, int> weekProjector =
                    d => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                        d,
                        CalendarWeekRule.FirstFourDayWeek,
                        DayOfWeek.Sunday);


                var query = from tm in session.Query<Timesheet>()
                            where tm.Project.Code == projectCode
                            group tm by new { 
                                Week = weekProjector(tm.ActualStartDate.Value), 
                                ForYear =  tm.ActualStartDate.Value.Year,
                                ForMonth = tm.ActualStartDate.Value.Month
                            } into weekGrp
                            select new
                            {
                                Week = weekGrp.Key.Week,
                                ForYear = weekGrp.Key.ForYear,
                                ForMonth = weekGrp.Key.ForMonth,
                                Timesheets = weekGrp.ToList()
                            };

                foreach (var item in query)
                {
                    var dayOfWeek = item.Timesheets.ToList().First().ActualStartDate.Value;
                    var firstDayOfWeek = dayOfWeek.FirstDayOfWeek();
                    var lastDayOfWeek = dayOfWeek.LastDayOfWeek();

                    var c = new CumulativeEffortByWeekView
                    {
                        ID = item.Week,
                        WeekOfTheYear = firstDayOfWeek.ToString("dd") + "-" + lastDayOfWeek.ToString("dd") + "/" + item .ForMonth + "/" + item.ForYear,
                        PreSale = item.Timesheets.Where(t => t.Phase.ID == 1).Select(t => t.ActualHourUsed).Sum(),
                        ImplementAndDev = item.Timesheets.Where(t => t.Phase.ID == 2).Select(t => t.ActualHourUsed).Sum(),
                        Warranty = item.Timesheets.Where(t => t.Phase.ID == 3).Select(t => t.ActualHourUsed).Sum(),
                        Operation = item.Timesheets.Where(t => t.Phase.ID == 4).Select(t => t.ActualHourUsed).Sum(),
                    };

                    viewList.Add(c);
                }


                viewList = viewList.OrderBy(t => t.ID).ToList();

                decimal sumPreSale = 0;
                decimal sumImplementAndDev = 0;
                decimal sumWarranty = 0;
                decimal sumOperation = 0;
                foreach (var item in viewList)
                {
                    sumPreSale = sumPreSale + item.PreSale;
                    item.PreSale = sumPreSale;

                    sumImplementAndDev = sumImplementAndDev + item.ImplementAndDev;
                    item.ImplementAndDev = sumImplementAndDev ;

                    sumWarranty = sumWarranty + item.Warranty;
                    item.Warranty = sumWarranty ;

                    sumOperation = sumOperation + item.Operation;
                    item.Operation = sumOperation;
                }


            }

            var result = new
            {
                data = viewList,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDuplicatedProjectCode(string projectCode)
        {
            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var query = from p in session.Query<Project>()
                            where p.Code == projectCode
                            select p;

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

        public static IEnumerable<double> CumulativeSum(IEnumerable<double> sequence)
        {
            double sum = 0;
            foreach (var item in sequence)
            {
                sum += item;
                yield return sum;
            }
        }

        [HttpPost]
        [Authorize(Roles = ConstAppRoles.Admin + ", " + ConstAppRoles.ProjectOwner)]
        public JsonResult SaveProject(ProjectView projectView)
        {
            var success = false;
            var msg = string.Empty;

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var theStatus = (from ps in session.Query<ProjectStatus>()
                                        where ps.ID == projectView.StatusID
                                         select ps).FirstOrDefault();

                var cust = (from c in session.Query<Customer>() 
                            where c.ID == projectView.CustomerID select c)
                            .FirstOrDefault();
                
                if (projectView.ID <= 0)
                {
                    var newProject = new Project
                    {
                        Code = projectView.Code,
                        NameTH = projectView.NameTH,
                        NameEN = projectView.NameEN,
                        StartDate = projectView.StartDate,
                        EndDate = projectView.EndDate,
                        Status = theStatus,
                        Customer = cust,

                        ContractStartDate = projectView.ContractStartDate,
                        ContractEndDate = projectView.ContractEndDate,
                        DeliverDate = projectView.DeliverDate,
                        WarrantyStartDate = projectView.WarrantyStartDate,
                        WarrantyEndDate = projectView.WarrantyEndDate,
                        EstimateProjectValue = projectView.EstimateProjectValue,
                    };

                    foreach (var iniMemberID in projectView.InitMembers)
                    {
                        var iniMemebr = (from initM in session.Query<InitialMember>()
                                         where initM.ID == iniMemberID
                                         select initM).Single();
                        newProject.AddMemeber(iniMemebr.User, iniMemebr.ProjectRole);
                    }
                    session.Save(newProject);
                }
                else
                {
                    var theProject = (from ps in session.Query<Project>()
                                     where ps.ID == projectView.ID
                                     select ps).Single();

                    theProject.Code = projectView.Code;
                    theProject.NameEN = projectView.NameEN;
                    theProject.NameTH = projectView.NameTH;
                    theProject.StartDate = projectView.StartDate;
                    theProject.EndDate = projectView.EndDate;
                    theProject.Customer = cust;

                    theProject.ContractStartDate = projectView.ContractStartDate;
                    theProject.ContractEndDate = projectView.ContractEndDate;
                    theProject.DeliverDate = projectView.DeliverDate;
                    theProject.WarrantyStartDate = projectView.WarrantyStartDate;
                    theProject.WarrantyEndDate = projectView.WarrantyEndDate;
                    theProject.EstimateProjectValue = projectView.EstimateProjectValue;
                    theProject.Status = theStatus;

                    session.Update(theProject);
                }

                transaction.Commit();
                success = true;
                msg = "การบันทึกโครงการเสร็จสมบูรณ์";
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllInitialMember(int start, int limit)
        {
            var viewList = new List<InitialMemberView>();
            int count;

            using (var session = _sessionFactory.OpenSession())
            {
                var query = (from i in session.Query<InitialMember>()
                             select new InitialMemberView
                             {
                                 ID = i.ID,
                                 EmployeeID = i.User.EmployeeID,
                                 FullName = string.Format("{0} {1}", i.User.FirstNameTH, i.User.LastNameTH),
                                 ProjectRoleID = i.ProjectRole.ID,
                                 ProjectRoleName = i.ProjectRole.NameTH,
                             });

                count = query.Count();

                viewList = query.OrderBy(u => u.EmployeeID).ToList();
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
        public JsonResult DeleteProject(long projectID)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var validateResult = validateHaveTimesheetInProjectCode(projectID, session);
                if (validateResult != null) return validateResult;

                var prj = (from x in session.Query<Project>()
                           where x.ID == projectID
                           select x).Single();

                
                session.Delete(prj);
                
                transaction.Commit();
            }

            return Json(new
            {
                success = true,
                message = "",
            }, JsonRequestBehavior.AllowGet);
        }

        string _haveTimesheetInPrjMessage = "ไม่สามารถลบโปรเจ็กต์นี้ออกจากระบบได้ เนื่องจากพบการบันทึกเวลาทำงานของโปรเจ็กต์นี้ในระบบ";
        private JsonResult validateHaveTimesheetInProjectCode(long projectID, ISession session)
        {
            var countTimesheet = (from t in session.Query<Timesheet>()
                                  where t.Project.ID == projectID
                                  select t).Count();

            if (countTimesheet > 0)
            {
                return Json(new
                {
                    success = false,
                    id = projectID,
                    message = _haveTimesheetInPrjMessage,
                }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }
        //END WEB API *************************************************************

        //NEW WEB API Phase II
        public JsonResult GetPhases()
        {
            var viewList = new List<PhaseView>();

            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var q = from p in session.Query<Phase>()
                        let timesheetCount = (from t in session.Query<Timesheet>() where t.Phase == p select t).Count()
                        orderby p.Order
                        select new { Phase = p, TimesheetCount = timesheetCount };

                foreach (var p in q)
                {
                    viewList.Add(new PhaseView
                    {
                        ID = p.Phase.ID,
                        Name = p.Phase.NameTH,
                        Order = p.Phase.Order,
                        TimesheetCount = p.TimesheetCount,
                    });
                }
            }

            return Json(viewList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SavePhase(PhaseView phaseView)
        {
            var success = false;
            var msg = string.Empty;
            var dupMessage = "พบชื่อช่วงโครงการนี้อยู่ในระบบแล้ว กรุณาระบุชื่อใหม่";
            var successMessage = "การบันทึกช่วงโครงการเสร็จสมบูรณ์";

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // Save
                if (phaseView.ID <= 0)
                {
                    // check Dup
                    var y = (from x in session.Query<Phase>()
                               where x.NameTH == phaseView.Name
                               select x).FirstOrDefault();

                    if (y != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var newY = new Phase(phaseView.Name, phaseView.Order);
                        session.Save(newY);
                    }
                }
                // Update
                else
                {
                    // check Dup
                    var y = (from x in session.Query<Phase>()
                             where x.NameTH == phaseView.Name
                               && x.ID != phaseView.ID
                               select x).FirstOrDefault();

                    if (y != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var updateY = (from d in session.Query<Phase>()
                                         where d.ID == phaseView.ID
                                         select d).Single();

                        updateY.ChangeNameOrOrder(phaseView.Name, phaseView.Order);
                        session.Update(updateY);
                    }
                }

                transaction.Commit();
                success = true;
                msg = successMessage;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeletePhase(int id)
        {
            var success = false;
            var msg = string.Empty;
            var targetMsg = "ช่วงโครงการ";

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var y = (from x in session.Query<Phase>()
                                 where x.ID == id
                                 select x).Single();

                var timesheetCount = (from t in session.Query<Timesheet>() where t.Phase == y select t).Count();
                if (timesheetCount > 0)
                {
                    msg = string.Format(
                        "ไม่สามารถลบ{0}นี้ได้ กรุณาลบข้อมูลบันทึกการทำงานในกลุ่ม{0}นี้ทั้งหมดก่อน", targetMsg);
                    return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                }

                session.Delete(y);

                transaction.Commit();
                success = true;
                msg = string.Format("ลบ{0}เสร็จสมบูรณ์", targetMsg);
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTaskTypes()
        {
            var viewList = new List<TaskTypeView>();

            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var q = from p in session.Query<TaskType>()
                        let timesheetCount = (from t in session.Query<Timesheet>() where t.TaskType == p select t).Count()
                        orderby p.Order
                        select new { TaskType = p, TimesheetCount = timesheetCount };

                foreach (var p in q)
                {
                    viewList.Add(new TaskTypeView
                    {
                        ID = p.TaskType.ID,
                        Name = p.TaskType.NameTH,
                        Order = p.TaskType.Order,
                        TimesheetCount = p.TimesheetCount,
                    });
                }
            }

            return Json(viewList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveTaskType(TaskTypeView taskTypeView)
        {
            var success = false;
            var msg = string.Empty;
            var targetMsg = "ประเภทงาน";
            var dupMessage = string.Format("พบชื่อ{0}นี้อยู่ในระบบแล้ว กรุณาระบุชื่อใหม่", targetMsg);
            var successMessage = string.Format("การบันทึก{0}การเสร็จสมบูรณ์", targetMsg);

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // Save
                if (taskTypeView.ID <= 0)
                {
                    // check Dup
                    var y = (from x in session.Query<TaskType>()
                             where x.NameTH == taskTypeView.Name
                             select x).FirstOrDefault();

                    if (y != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var newY = new TaskType(taskTypeView.Name, taskTypeView.Order);
                        session.Save(newY);
                    }
                }
                // Update
                else
                {
                    // check Dup
                    var y = (from x in session.Query<TaskType>()
                             where x.NameTH == taskTypeView.Name
                               && x.ID != taskTypeView.ID
                             select x).FirstOrDefault();

                    if (y != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var updateY = (from d in session.Query<TaskType>()
                                       where d.ID == taskTypeView.ID
                                       select d).Single();

                        updateY.ChangeNameOrOrder(taskTypeView.Name, taskTypeView.Order);
                        session.Update(updateY);
                    }
                }

                transaction.Commit();
                success = true;
                msg = successMessage;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteTaskType(int id)
        {
            var success = false;
            var msg = string.Empty;
            var targetMsg = "ประเภทงาน";

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var y = (from x in session.Query<TaskType>()
                         where x.ID == id
                         select x).Single();

                var timesheetCount = (from t in session.Query<Timesheet>() where t.TaskType == y select t).Count();
                if (timesheetCount > 0)
                {
                    msg = string.Format(
                        "ไม่สามารถลบ{0}นี้ได้ กรุณาลบข้อมูลบันทึกการทำงานในกลุ่ม{0}นี้ทั้งหมดก่อน", targetMsg);
                    return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                }

                session.Delete(y);

                transaction.Commit();
                success = true;
                msg = string.Format("ลบ{0}เสร็จสมบูรณ์", targetMsg);
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMainTasks()
        {
            var viewList = new List<MainTaskView>();

            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var q = from p in session.Query<MainTask>()
                        orderby p.Desc
                        select p;

                foreach (var p in q)
                {
                    viewList.Add(new MainTaskView
                    {
                        ID = p.ID,
                        Name = p.Desc,
                    });
                }
            }

            return Json(viewList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveMainTask(MainTaskView mainTaskView)
        {
            var success = false;
            var msg = string.Empty;
            var targetMsg = "งานหลัก";
            var dupMessage = string.Format("พบชื่อ{0}นี้อยู่ในระบบแล้ว กรุณาระบุชื่อใหม่", targetMsg);
            var successMessage = string.Format("การบันทึก{0}การเสร็จสมบูรณ์", targetMsg);

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // Save
                if (mainTaskView.ID <= 0)
                {
                    // check Dup
                    var y = (from x in session.Query<MainTask>()
                             where x.Desc == mainTaskView.Name
                             select x).FirstOrDefault();

                    if (y != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var newY = new MainTask(mainTaskView.Name);
                        session.Save(newY);
                    }
                }
                // Update
                else
                {
                    // check Dup
                    var y = (from x in session.Query<MainTask>()
                             where x.Desc == mainTaskView.Name
                               && x.ID != mainTaskView.ID
                             select x).FirstOrDefault();

                    if (y != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var updateY = (from d in session.Query<MainTask>()
                                       where d.ID == mainTaskView.ID
                                       select d).Single();

                        updateY.Desc = mainTaskView.Name;
                        session.Update(updateY);
                    }
                }

                transaction.Commit();
                success = true;
                msg = successMessage;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteMainTask(int id)
        {
            var success = false;
            var msg = string.Empty;
            var targetMsg = "งานหลัก";

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var y = (from x in session.Query<MainTask>()
                         where x.ID == id
                         select x).Single();

                session.Delete(y);

                transaction.Commit();
                success = true;
                msg = string.Format("ลบ{0}เสร็จสมบูรณ์", targetMsg);
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectRoles()
        {
            var viewList = new List<ProjectRoleView>();

            using (var session = _sessionFactory.OpenSession())
            {
                var q = from p in session.Query<ProjectRole>()

                        orderby p.Order
                        let projectMemberCount = (from x in session.Query<ProjectMember>() where x.ProjectRole == p select x).Count()
                        let timesheetCount = (from x in session.Query<Timesheet>() where x.ProjectRole == p select x).Count()
                        select new
                        {
                            ProjectRole = p,
                            ProjectMemberCount = projectMemberCount,
                            TimesheetCount = timesheetCount
                        };

                foreach (var p in q)
                {
                    decimal cost = p.ProjectRole.ProjectRoleRates
                            .OrderByDescending(prr => prr.EffectiveStart)
                            .Select(prr => prr.Cost)
                            .FirstOrDefault();

                    //p.ProjectRole.ProjectRoleRates.Get
                    viewList.Add(new ProjectRoleView
                    {
                        ProjectRoleID = p.ProjectRole.ID,
                        ProjectRoleName = p.ProjectRole.NameTH,
                        ProjectRoleCost = cost,
                        Order = p.ProjectRole.Order,
                        ProjectMemberCount = p.ProjectMemberCount,
                        TimesheetCount = p.TimesheetCount
                    });
                }
            }

            return Json(viewList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveProjectRole(ProjectRoleView projectRoleView)
        {
            var success = false;
            var msg = string.Empty;
            var targetMsg = "หน้าที่ในโครงการ";
            var dupMessage = string.Format("พบชื่อ{0}นี้อยู่ในระบบแล้ว กรุณาระบุชื่อใหม่", targetMsg);
            var successMessage = string.Format("การบันทึก{0}การเสร็จสมบูรณ์", targetMsg);

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // Save
                if (projectRoleView.ProjectRoleID <= 0)
                {
                    // check Dup
                    var y = (from x in session.Query<ProjectRole>()
                             where x.NameTH == projectRoleView.ProjectRoleName
                             select x).FirstOrDefault();

                    if (y != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var newY = new ProjectRole(projectRoleView.ProjectRoleName, projectRoleView.Order);
                        if (projectRoleView.ProjectRoleCost > 0)
                        {
                            var projectRoleRate = new ProjectRoleRate()
                            {
                                Cost = projectRoleView.ProjectRoleCost
                            };

                            newY.ProjectRoleRates.Add(projectRoleRate);
                        }
                        session.Save(newY);
                    }
                }
                // Update
                else
                {
                    // check Dup
                    var y = (from x in session.Query<ProjectRole>()
                             where x.NameTH == projectRoleView.ProjectRoleName
                               && x.ID != projectRoleView.ProjectRoleID
                             select x).FirstOrDefault();

                    if (y != null)
                    {
                        msg = dupMessage;
                        return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var updateY = (from d in session.Query<ProjectRole>()
                                       where d.ID == projectRoleView.ProjectRoleID
                                       select d).Single();

                        updateY.ChangeNameOrOrder(projectRoleView.ProjectRoleName, projectRoleView.Order);

                        var costRate = updateY.ProjectRoleRates
                            .OrderByDescending(prr => prr.EffectiveStart)
                            .FirstOrDefault();

                        if (costRate != null)
                        {
                            costRate.Cost = projectRoleView.ProjectRoleCost;
                        }
                        else
                        {
                            if (projectRoleView.ProjectRoleCost > 0)
                            {
                                var projectRoleRate = new ProjectRoleRate()
                                {
                                    Cost = projectRoleView.ProjectRoleCost
                                };

                                updateY.ProjectRoleRates.Add(projectRoleRate);
                            }
                        }

                        session.Update(updateY);
                    }
                }

                transaction.Commit();
                success = true;
                msg = successMessage;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteProjectRole(int id)
        {
            var success = false;
            var msg = string.Empty;
            var targetMsg = "หน้าที่ในโครงการ";

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var y = (from x in session.Query<ProjectRole>()
                         where x.ID == id
                         select x).Single();

                session.Delete(y);

                foreach (var item in y.ProjectRoleRates)
                {
                    session.Delete(item);
                }

                transaction.Commit();
                success = true;
                msg = string.Format("ลบ{0}เสร็จสมบูรณ์", targetMsg);
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }
        //END WEB API Phase II *************************************************************
    }
}
