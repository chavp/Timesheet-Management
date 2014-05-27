using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models.Supports
{
    using Cwn.PM.Loggers;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using log4net.Appender;
    using log4net.Core;
    using Newtonsoft.Json;
    using NHibernate;
    using System.Configuration;

    public class LogAppender : AppenderSkeleton
    {
        ISessionFactory _sessionFactory = null;

        public LogAppender()
        {
            _sessionFactory = MvcApplication.CreateLogTimesheetSessionFactory();
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent.LoggerName.Contains("ErrorLogAttribute"))
            {
                if (loggingEvent.Level == Level.Error)
                {
                    using (var session = _sessionFactory.OpenSession())
                    {
                        var newErroLog = new ErrorLog
                        {
                            UserName = loggingEvent.UserName,
                            Name = loggingEvent.LoggerName,
                            Message = Convert.ToString(loggingEvent.MessageObject),
                            Level = loggingEvent.Level.ToString(),
                            EventDate = loggingEvent.TimeStamp,
                        };

                        if (loggingEvent.ExceptionObject != null)
                        {
                            newErroLog.ErrorMessage = loggingEvent.ExceptionObject.Message;
                            newErroLog.StackTrace = loggingEvent.ExceptionObject.StackTrace;

                        }

                        session.Save(newErroLog);

                        session.Flush();
                    }
                }
            }
            
            else if (loggingEvent.LoggerName.Contains("ProfileLogAttribute"))
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    var message = JsonConvert.DeserializeObject<ProfileLog>(Convert.ToString(loggingEvent.MessageObject));
                    
                    message.UserName = loggingEvent.UserName;
                    message.Name = loggingEvent.LoggerName;
                    message.Level = loggingEvent.Level.ToString();
                    message.EventDate = loggingEvent.TimeStamp;

                    session.Save(message);

                    session.Flush();
                }
            }
        }
    }
}