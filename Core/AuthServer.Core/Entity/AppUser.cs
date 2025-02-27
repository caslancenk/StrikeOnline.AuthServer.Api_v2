﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Entity
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string City { get; set; }
    }
}
