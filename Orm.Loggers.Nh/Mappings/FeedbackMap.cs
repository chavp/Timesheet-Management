using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Orm.Loggers.Mappings
{
    using Cwn.PM.Loggers;
    using FluentNHibernate.Mapping;

    public class FeedbackMap
        : LogMap<Feedback>
    {
        public FeedbackMap()
        {
            Table("Feedback");

            Map(x => x.Rating);
        }
    }
}
