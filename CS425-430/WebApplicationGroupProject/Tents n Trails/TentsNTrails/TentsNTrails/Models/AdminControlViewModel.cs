using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TentsNTrails.Models
{
    public class AdminControlViewModel
    {
        public User User { get; set; }
        public IEnumerable<IdentityRole> Roles { get; set; }
        //public IdentityRole Roles { get; set; }
    }
}