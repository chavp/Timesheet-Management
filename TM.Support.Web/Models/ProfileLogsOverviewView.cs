using Cwn.PM.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Support.Web.Models
{
    public class ProfileLogsOverviewView
    {
        public string ID 
        {
            get
            {
                return string.Format("profile-logs-overview-{0}-{1}-{2}", Action, Controller, LatestEventDate.ToString("ddMMyyyy"));
            }
            set
            {
            }
        }
        public string Action { get; set; }
        public string Controller { get; set; }
        public double AvgElapsedTime { get; set; }
        public int TotalCall { get; set; }
        public DateTime LatestEventDate { get; set; }
    }
}