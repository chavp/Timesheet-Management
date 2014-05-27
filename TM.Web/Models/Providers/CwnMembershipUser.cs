using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace PJ_CWN019.TM.Web.Models.Providers
{
    public class CwnMembershipUser : MembershipUser
    {
        public CwnMembershipUser(
            string providerName, 
            string name, 
            object providerUserKey, 
            string email, 
            string passwordQuestion, 
            string comment, 
            bool isApproved, 
            bool isLockedOut, 
            DateTime creationDate, 
            DateTime lastLoginDate, 
            DateTime lastActivityDate, 
            DateTime lastPasswordChangedDate, 
            DateTime lastLockoutDate) :
            base(
            providerName, 
            name, 
            providerUserKey,
            email, 
            passwordQuestion, 
            comment, 
            isApproved, 
            isLockedOut,
            creationDate,
            lastLoginDate,
            lastActivityDate,
            lastPasswordChangedDate,
            lastLockoutDate)
        {

        }

        public string PositionName { get; set; }
    }
}