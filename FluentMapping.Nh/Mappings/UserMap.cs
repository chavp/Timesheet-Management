using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.FluentMapping.Mappings
{
    using Cwn.PM.BusinessModels.Entities;

    public class UserMap
        : EntityMap<User>
    {
        public UserMap()
            : base("AUT")
        {
            Table("AUT_USER");

            Map(x => x.EmployeeID, "EMP_ID").Not.Nullable().Unique();
            Map(x => x.LastNameEN, "EMP_LNAME_EN").Length(100).Not.Nullable().UniqueKey("KEY_FULL_NAME");
            Map(x => x.FirstNameEN, "EMP_FNAME_EN").Length(100).Not.Nullable().UniqueKey("KEY_FULL_NAME");
            Map(x => x.LastNameTH, "EMP_LNAME_TH").Length(100).Not.Nullable().UniqueKey("KEY_FULL_NAME");
            Map(x => x.FirstNameTH, "EMP_FNAME_TH").Length(100).Not.Nullable().UniqueKey("KEY_FULL_NAME");
            Map(x => x.Nickname, "EMP_NICKNAME").Length(30);
            Map(x => x.Email, "EMP_EMAIL").Length(100);
            Map(x => x.StartDate, "EMP_START_DATE");
            Map(x => x.EndDate, "EMP_END_DATE");

            Map(x => x.Status, "EMP_STATUS").Length(30);

            Map(x => x.Password, "AUT_PASSWORD");
            Map(x => x.LastLoginIP, "AUT_LAST_LOGIN_IP").Length(50);
            Map(x => x.LastLockoutIP, "AUT_LAST_LOCKOUT_IP").Length(50);
            Map(x => x.LastLoginDate, "AUT_LAST_LOGIN_DATE");
            Map(x => x.LastActivityDate, "AUT_LAST_ACTIVITY_DATE");
            Map(x => x.LastPasswordChangedDate, "AUT_LAST_PASSWORD_CHANGED_DATE");
            Map(x => x.LastLockoutDate, "AUT_LAST_LOCKOUT_DATE");
            Map(x => x.IsLockedOut, "AUT_LOCKED_OUT");

            References(x => x.Title, "TITLE_ID");
            References(x => x.Department, "DEPT_ID").Not.Nullable().UniqueKey("KEY_FULL_NAME");
            References(x => x.Position, "POSITION_ID");

            References(x => x.Lead, "LEAD_AUT_ID");

            //HasManyToMany(x => x.AddRoles);
            //HasManyToMany(x => x.Projects);

            MapVersion();
        }
    }
}
