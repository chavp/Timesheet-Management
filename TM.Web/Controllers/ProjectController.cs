using NHibernate;
using PJ_CWN019.TM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PJ_CWN019.TM.Web.Controllers
{
    using PowerfulExtensions.Linq;
    using Cwn.PM.BusinessModels.Entities;
    using NHibernate;
    using NHibernate.Linq;
    using WebMatrix.WebData;
    using System.Web.Security;
    using PJ_CWN019.TM.Web.Filters;

    [ErrorLog]
    [ProfileLog]
    [Authorize(Roles = "ProjectOwner, Manager, Admin")]
    [FourceToChangeAttribute]
    public class ProjectController : Controller
    {
        ISessionFactory _sessionFactory = null;
        public ProjectController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        [Authorize(Roles = "Manager, Admin")]
        public ActionResult Search()
        {
            if (Roles.IsUserInRole("Manager"))
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

        public JsonResult FindProject(
            string projectCode, string projectName, 
            string fromDateText, string toDateText,
            bool isOwner,
            int start, int limit, string query = "", bool isForDepartment = false)
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
                                    where p.NameTH.Contains(projectName)
                                    && p.Code.Contains(projectCode)
                                    && m.User == manager
                                    && m.ProjectRole.IsOwner
                                    select new { Prj = p, Members = p.Members.Count });

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
                        viewList.Add(new ProjectView
                        {
                            ID = project.Prj.ID,
                            Code = project.Prj.Code,
                            Name = project.Prj.NameTH,
                            Members = project.Members,
                            StartDate = project.Prj.StartDate,
                            EndDate = project.Prj.EndDate
                        });
                    }
                }
                else
                {

                    var managerDepartment = (from u in session.Query<User>()
                                             where u.EmployeeID.ToString() == WebSecurity.CurrentUserName
                                             select u.Department).First();

                    var prjQuery = (from p in session.Query<Project>()
                                    let members = (from m in session.Query<ProjectMember>()
                                                   where m.Project == p
                                                   && m.User.Department == managerDepartment
                                                   select m).Count()
                                    where p.NameTH.Contains(projectName)
                                    && p.Code.Contains(projectCode)
                                    //&& members > 0
                                    select new { Prj = p, Members = members, TotalMembers = p.Members.Count });

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
                        viewList.Add(new ProjectView
                        {
                            ID = project.Prj.ID,
                            Code = project.Prj.Code,
                            Name = project.Prj.NameTH,
                            Members = project.TotalMembers,
                            StartDate = project.Prj.StartDate,
                            EndDate = project.Prj.EndDate
                        });
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

        public JsonResult FindProjectForOwner(
            string projectCode, string projectName,
            string fromDateText, string toDateText,
            bool isOwner,
            int start, int limit, string query = "")
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
                    viewList.Add(new ProjectView
                    {
                        ID = project.Prj.ID,
                        Code = project.Prj.Code,
                        Name = project.Prj.NameTH,
                        Members = project.Members,
                        StartDate = project.Prj.StartDate,
                        EndDate = project.Prj.EndDate
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

        public JsonResult SaveProjectRole(string projectCode, List<ProjectMemberView> projectMemberViewList)
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

        //[Authorize(Roles = "Manager, Admin")]
        //public JsonResult AssignProjectMember(string projectCode, List<long> employeeIDList)
        //{
        //    var success = false;
        //    var msg = string.Empty;

        //    using (var session = _sessionFactory.OpenSession())
        //    using (var transaction = session.BeginTransaction())
        //    {
        //        var project = (from p in session.Query<Project>()
        //                     where p.Code == projectCode
        //                     select p).Single();

        //        foreach (var empID in employeeIDList)
        //        {
        //            var user = (from u in session.Query<User>()
        //                     where u.ID == empID
        //                     select u).Single();

        //            project.AddMemeber(user, null);
        //        }
        //        transaction.Commit();
        //        success = true;
        //    }

        //    return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        //}

        [Authorize(Roles = "Manager, Admin")]
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

                msg = "ลบพนักงานออกจากโครงการนี้แล้ว";
                success = true;
            }

            return Json(new { success = success, message = msg }, JsonRequestBehavior.AllowGet);
        }

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

        public JsonResult FindEmployee(
            long divisionID, long departmentID,
            string employeeID, string employeeFirstName, string employeeLastName,
            List<long> withoutEmpIDList,
            bool isOwner,
            int start, int limit, string query = "")
        {
            var viewList = new List<EmployeeView>();
            int count;

            withoutEmpIDList = withoutEmpIDList ?? new List<long>();

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

                    mebers = mebers.OrderBy(u => u.User.EmployeeID).Distinct(u => u.User.ID).ToList();

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
                else if (Roles.IsUserInRole("Admin"))
                {
                    var userQuery = (from u in session.Query<User>()
                                     where u.EmployeeID.ToString().Contains(employeeID)
                                     && !withoutEmpIDList.Contains(u.EmployeeID)
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
                else // for Department Manager
                {
                    var userQuery = (from u in session.Query<User>()
                                     where u.EmployeeID.ToString().Contains(employeeID)
                                     && !withoutEmpIDList.Contains(u.EmployeeID)
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

            var result = new
            {
                data = viewList,
                total = count,
                success = true,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
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
                             join r in session.Query<ProjectRole>() on pm.ProjectRole equals r
                             let projectRoleID = (pm.ProjectRole == null) ? 0 : pm.ProjectRole.ID
                             let projectRoleName = (pm.ProjectRole == null) ? string.Empty : pm.ProjectRole.NameTH
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

                                 IsOwner = r.IsOwner,
                             }).OrderBy(pm => pm.EmployeeID);

                count = query.Count();
                foreach (var pm in query.Skip(start).Take(limit))
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
    }
}
