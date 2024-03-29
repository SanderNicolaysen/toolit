﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace app.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public bool ChangePassword { get; set; }
        public bool isAdmin { get; set; }
        public bool isSuperAdmin { get; set; }

        public List<Favorite> Favorites { get; set; }

        public List<Notification> Notifications { get; set; }

        public string UserIdentifierCode { get; set; }
    }
}
