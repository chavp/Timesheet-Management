using Cwn.PM.BusinessModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class CumulativeEffortByWeekView
    {
        public long ID { get; set; }
        public string WeekOfTheYear { get; set; }
        public decimal PreSale { get; set; }
        public decimal ImplementAndDev { get; set; }
        public decimal Warranty { get; set; }
        public decimal Operation { get; set; }

    }
}