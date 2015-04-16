using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public static class ConstPage
    {
        public const string ProjectManagementTitle = "Projects";
        public const string ProjectManagementIcon = "glyphicon glyphicon-th-large";

        public const string ProjectTitle = "Management";
        public const string ProjectIcon = "glyphicon glyphicon-th";

        public const string ProjectActivitiesTitle = "Activities";
        public const string ProjectActivitiesIcon = "glyphicon glyphicon-th-list";

        public const string ProjectProgressTitle = "Project Progress";
        public const string ProjectProgressIcon = "glyphicon glyphicon-th-list";


        public static readonly string FormatDefault = "dd/MM/yyyy";
        public static readonly string Format = "yyyy-MM-dd";
        public static readonly string Format2 = "dd-MM-yyyy";
        public static readonly CultureInfo ForParseDate = new CultureInfo("en-US");

        public static readonly Func<DateTime, int> WeekProjector = 
                    d => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                        d,
                        CalendarWeekRule.FirstFourDayWeek,
                        DayOfWeek.Sunday);
    }
}