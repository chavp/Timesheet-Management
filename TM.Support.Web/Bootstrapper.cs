using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using NHibernate;
using Microsoft.AspNet.SignalR;
using PJ_CWN019.TM.Support.Web.Models.Hubs;

namespace PJ_CWN019.TM.Support.Web
{
  public static class Bootstrapper
  {
    public static IUnityContainer Initialise()
    {
      var container = BuildUnityContainer();

      DependencyResolver.SetResolver(new UnityDependencyResolver(container));

      return container;
    }

    private static IUnityContainer BuildUnityContainer()
    {
      var container = new UnityContainer();

      // register all your components with the container here
      // it is NOT necessary to register your controllers

      // e.g. container.RegisterType<ITestService, TestService>();    
      RegisterTypes(container);

      return container;
    }

    public static void RegisterTypes(IUnityContainer container)
    {
        container.RegisterInstance<ISessionFactory>(MvcApplication.CreateLogTimesheetSessionFactory());
        container.RegisterInstance<IHubContext>(GlobalHost.ConnectionManager.GetHubContext<LogHub>());
    }
  }
}