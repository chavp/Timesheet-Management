using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PJ_CWN019.TM.Web
{
    using NHibernate;
    using NHibernate.Linq;
    using Nh = NHibernate.Cfg;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using Cwn.PM.FluentMapping.Mappings;
    using PJ_CWN019.TM.Web.Properties;
    using log4net;
    using System.Reflection;
    using Cwn.PM.Orm.Loggers.Mappings;
    using NHibernate.Tool.hbm2ddl;
    using System.Web.Security;
    using System.IO;
    using WebMatrix.WebData;
    using Cwn.PM.BusinessModels.Entities;
    using System.Diagnostics;

  
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Initialization of our Unity container
            Bootstrapper.Initialise();

            log4net.Config.XmlConfigurator.Configure();

            Logger.Info("Application_Started");

            string fullFilepath = Server.MapPath(@"~\Export\");
            if (Directory.Exists(fullFilepath))
            {
                Logger.Info("Application_Deleted_Export_Folder");
                Directory.Delete(fullFilepath, true);
            }

            Directory.CreateDirectory(fullFilepath);
            Logger.Info("Application_Created_Export_Folder");

            _fac = CreateMSSQLSessionFactory();
            //Stopwatch timer = Stopwatch.StartNew();
            //initProjectStatus();
            //Logger.Info(string.Format("Application_initProjectStatus Completed, Elapsed: {0}", timer.Elapsed));
            initEmployeeStatus();
            initProjectCostAccount();
        }

        ISessionFactory _fac;
        protected void initProjectStatus()
        {
            using (var session = _fac.OpenSession())
            using (var tran = session.BeginTransaction())
            {
                // create Project Status
                var qAllProjectStatus = from ps in session.Query<ProjectStatus>()
                          select ps;

                var prjOpenStatusName = "Open";
                var openStatus = qAllProjectStatus
                    .Where(d => d.Name == prjOpenStatusName)
                    .FirstOrDefault();
                if (openStatus == null)
                {
                    openStatus = new ProjectStatus{
                        Name = prjOpenStatusName
                    };
                    session.Save(openStatus);
                }

                var prjCloseStatusName = "Close";
                var closeStatus = qAllProjectStatus
                    .Where(d => d.Name == prjCloseStatusName)
                    .FirstOrDefault();
                if (closeStatus == null)
                {
                    closeStatus = new ProjectStatus{
                        Name = prjCloseStatusName
                    };
                    session.Save(closeStatus);
                }

                var qAllProjectNotStatus = from p in session.Query<Project>()
                                           where p.Status == null
                                        select p;

                foreach (var pro in qAllProjectNotStatus)
                {
                    pro.Status = openStatus;
                }

                tran.Commit();
            }
        }
        protected void initEmployeeStatus()
        {
            using (var session = _fac.OpenSession())
            using (var tran = session.BeginTransaction())
            {
                var emps = (from e in session.Query<User>()
                           where e.Status == null
                           select e).ToList();
                foreach (var emp in emps)
                {
                    emp.Status = EmployeeStatus.Work;
                }

                tran.Commit();

                Logger.Info("initEmployeeStatus completed");
            }
        }
        protected void initProjectCostAccount()
        {
            using (var session = _fac.OpenSession())
            using (var tran = session.BeginTransaction())
            {
                var allProject = (from p in session.Query<Project>()
                                  //where p.Code == "AAAA"
                                  select p).ToList();

                foreach (var prj in allProject)
                {
                    var prjCostAcc = (from p in session.Query<ProjectCostAccount>()
                                      where p.Project == prj
                                      select p).SingleOrDefault();

                    if (prjCostAcc == null)
                    {
                        prjCostAcc = new ProjectCostAccount(prj);
                        session.Save(prjCostAcc);

                        prjCostAcc.UpdateEntries();
                        prjCostAcc.UpdateBalance();

                        Logger.Info("Save ProjectCostAccount " + prj.Code + " completed");
                    }

                    //prjCostAcc.UpdateEntries();
                }

                tran.Commit();

                Logger.Info("initProjectCostAccount completed");
            }
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            
        }

        protected void Session_End(Object sender, EventArgs e)
        {
            HttpCookie authenticationCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authenticationCookie != null)
            {
                FormsAuthenticationTicket authenticationTicket = FormsAuthentication.Decrypt(authenticationCookie.Value);
                if (!authenticationTicket.Expired)
                {

                }
            }

        }

        public static ISessionFactory CreateMSSQLSessionFactory()
        {
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(c => c
                .Server(Settings.Default.DBServer)
                .Username(Settings.Default.DBUserName)
                .Password(Settings.Default.DBPassword)
                .Database(Settings.Default.DB)))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserMap>())
                .ExposeConfiguration(TreatConfiguration)
                .BuildSessionFactory();
        }

        public static ISessionFactory CreateLogTimesheetSessionFactory()
        {
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(c => c
                .Server(Settings.Default.DBLogServer)
                .Username(Settings.Default.DBLogUserName)
                .Password(Settings.Default.DBLogPassword)
                .Database(Settings.Default.DBLog)))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ErrorLogMap>())
                .ExposeConfiguration(TreatConfiguration)
                .BuildSessionFactory();
        }

        public static void TreatConfiguration(Nh.Configuration configuration)
        {
            var update = new SchemaUpdate(configuration);
            update.Execute(false, true);
        }
    }
}