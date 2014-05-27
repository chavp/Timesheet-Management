using Cwn.PM.BusinessModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace PJ_CWN019.TM.Web.Models.Providers
{
    using NHibernate;
    using NHibernate.Linq;
    using PJ_CWN019.TM.Web;

    public class CwnRoleProvider : RoleProvider
    {
        ISessionFactory _sessionFactory = null;
        public CwnRoleProvider()
        {
            _sessionFactory = MvcApplication.CreateMSSQLSessionFactory();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            //throw new NotImplementedException();
            var roles = new List<string>();
            using (var session = _sessionFactory.OpenSession())
            {
                roles = (from r in session.Query<AppRole>()
                                from u in session.Query<User>() 
                                where r.Users.Contains(u)
                                && u.EmployeeID.ToString() == username
                                select r.Name).ToList();

                // is ProjectOwner
                //check PM Role
                var memberPMProjects = (from p in session.Query<Project>()
                                        join m in session.Query<ProjectMember>() on p equals m.Project
                                        where m.User.EmployeeID.ToString() == username
                                        && m.ProjectRole.IsOwner
                                        select new { Project = p, Member = m });
                if (memberPMProjects.Count() > 0)
                {
                    roles.Add("ProjectOwner");
                }
            }
            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}