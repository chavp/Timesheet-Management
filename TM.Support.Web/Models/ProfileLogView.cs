using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Support.Web.Models
{
    public class ProfileLogView
    {
        public int ID
        {
            get
            {
                if (string.IsNullOrEmpty(UserID))
                {
                    return 0;
                }
                //var random = new Random(int.Parse(UserID));
                return int.Parse(UserID) % 100;
            }
            set
            {
            }
        }
        public string UserID { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string EventTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public double ElapsedTimeSeconds 
        {
            get
            {
                return ElapsedTime.TotalSeconds;
            }
            set
            {
            }
        }
    }
}