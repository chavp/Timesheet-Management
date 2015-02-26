using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class IndustryView
    {
        public long ID { get; set; }
        public string Code { get; set; }

        string display = string.Empty;
        public string Display 
        {
            get
            {
                if (string.IsNullOrEmpty(this.display))
                {
                    return FormatDisplay(Code, NameTH, Name);
                }
                else
                {
                    return this.display;
                }
            }
            set {
                this.display = value;
            }
        }
        public string Name { get; set; }
        public string NameTH { get; set; }

        public static string FormatDisplay(string code, string nameFirst, string nameSecond)
        {
            return string.Format("{0}: {1} / {2}", code, nameFirst, nameSecond);
        }
    }
}