﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    public class AppRole : Entity
    {
        public AppRole()
        {
            Users = new List<User>();
            AppUsers = new List<AppUser>();
        }

        public virtual string Name { get; set; }

        public virtual IList<User> Users { get; protected set; }
        public virtual IList<AppUser> AppUsers { get; protected set; }
    }
}