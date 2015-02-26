using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Queries
{
    using Cwn.PM.BusinessModels.Entities;

    public static class QueryUser
    {
        public static IQueryable<User> QueryByEmployeeID(this IQueryable<User> queryUser, int employeeID)
        {
            return from u in queryUser
                   where u.EmployeeID == employeeID
                   select u;
        }

        public static IQueryable<User> QueryByEmployeeID(this IQueryable<User> queryUser, string employeeID)
        {
            int empID;
            int.TryParse(employeeID, out empID);

            return queryUser.QueryByEmployeeID(empID);
        }

        public static IQueryable<User> QueryByPM(this IQueryable<User> queryUser
            , IQueryable<Project> queryProject
            , IQueryable<ProjectMember> queryProjectMember)
        {
            return from u in queryUser
                   let isPM = (from p in queryProject
                               join m in queryProjectMember
                               on p equals m.Project
                               where m.User == u
                                   && m.ProjectRole.IsOwner
                               select new { Project = p, Member = m }).Count() > 0
                   where isPM
                   select u;
        }
    }
}
