﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBaseBack.Data
{
    public class AppUser:IdentityUser
    {
        public int UserID { get; set; }
    }
}
