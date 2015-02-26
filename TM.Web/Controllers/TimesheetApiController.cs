using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PJ_CWN019.TM.Web.Controllers
{
    using System.Web.Mvc;
    using System.Web.SessionState;
    using PJ_CWN019.TM.Web.Filters;
    using PJ_CWN019.TM.Web.Models;
    using NHibernate;
    using NHibernate.Linq;
    using Cwn.PM.BusinessModels.Entities;
    using Cwn.PM.BusinessModels.Queries;
    using System.Globalization;
    using WebMatrix.WebData;

    [ErrorLog]
    [ProfileLog]
    [Authorize(Roles = "Member, Manager, Admin, ProjectOwner")]
    [SessionState(SessionStateBehavior.Disabled)]
    public class TimesheetApiController : ApiController
    {
        string dateFormat = "dd/MM/yyyy";
        private readonly ISessionFactory _sessionFactory = null;
        public TimesheetApiController()
        {
            _sessionFactory = MvcApplication.CreateMSSQLSessionFactory();
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        public dynamic Get(int projectID, string fromDateText, string toDateText,
            int start, int limit)
        {
            var viewList = new List<TimesheetView>();
            int count;

            var fromDate = DateTime.ParseExact(fromDateText, dateFormat, new CultureInfo("en-US"));
            var toDate = DateTime.ParseExact(toDateText, dateFormat, new CultureInfo("en-US"));

            using (var session = _sessionFactory.OpenSession())
            {
                var user = session.Query<User>().QueryByEmployeeID(WebSecurity.CurrentUserName).Single();

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
                q = q.OrderByDescending(t => t.CreatedAt);

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

            return result;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}