﻿using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TentsNTrails.Models
{

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        // In DB by default:
        //  UserID
        //  UserName
        //  Email

        [Required]
        public DateTime EnrollmentDate { get; set; }

        [Required]
        public String FirstName { get; set; }

        [Required]
        public String LastName { get; set; }
        public bool Private { get; set; }
        public string About { get; set; }
        private string profilePictureUrl;
        public string ProfilePictureUrl
        { 
            get
            {
                return profilePictureUrl == null ? Image.DEFAULT_PROFILE_PICTURE_URL : profilePictureUrl;
            }
            set
            {
                profilePictureUrl = value;
            }
        }

        public List<UserRecreation> UserActivities { get; set; }
        public virtual List<Review> UserReviews { get; set; }
        public virtual List<LocationFlag> BeenThereLocations { get; set; }
        public virtual List<LocationFlag> WantToGoLocations { get; set; }
        public virtual List<LocationFlag> GoAgainLocations { get; set; }
        public virtual List<LocationImage> UserLocationImages { get; set; }
        public virtual List<Video> UserLocationVideos { get; set; }
        public virtual List<Notification> Notifications { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}