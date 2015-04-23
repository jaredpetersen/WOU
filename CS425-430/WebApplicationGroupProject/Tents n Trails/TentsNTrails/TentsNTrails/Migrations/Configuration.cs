namespace TentsNTrails.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using TentsNTrails.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TentsNTrails.Models.ApplicationDbContext>
    {

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "TentsNTrails.Models.ApplicationDbContext";
        }

        // ******************************************
        //  Seed Method
        // ******************************************
        //
        // This has been revamped to encapsulate the seed code for each model so you
        // can enable and disable the seeding as needed (exclude unneeded changes)
        //
        protected override void Seed(TentsNTrails.Models.ApplicationDbContext context)
        {
            CreateAdmin(context);
            AddUsers(context);
            AddLocationData(context);
            AddReviews(context);
            AddImages(context);
            AddVideos(context);
        }


        // used in the CreateUsers method to conveniently add users.
        public struct UserStruct
        {
            public string Username;
            public string Password;
            public string Email;
            public string Firstname;
            public string Lastname;
            public string About;
            public bool Private;
        }

        // ******************************************
        //  create test Users for the site.
        // ******************************************
        public void AddUsers(ApplicationDbContext context)
        {
            var now = DateTime.UtcNow;

            var UserManager = new UserManager<User>(new UserStore<User>(context));
          
            var usersToAdd = new List<UserStruct>()
            {
                new UserStruct{
                    Username = "fancyman55",
                    Password = "Password1*",
                    Email = "aaroncarsonart@gmail.com",
                    Firstname = "Aaron",
                    Lastname = "Carson",
                    About = "I really like to go hiking and camping.",
                    Private = false
                },
                new UserStruct{
                    Username = "morningmist",
                    Password = "Password1!",
                    Email = "oneEarlyMorning@hotmail.com",
                    Firstname = "Polly",
                    Lastname = "Hollingsworth",
                    About = "I love hiking!  Waterfalls are best viewed in the morning.",
                    Private = false
                },
                new UserStruct{
                    Username = "pretzles",
                    Password = "Password1!",
                    Email = "pretzles@hotmail.com",
                    Firstname = "Barack",
                    Lastname = "Obama",
                    About = "",
                    Private = true
                },
                new UserStruct{
                    Username = "hatersGonnaHate",
                    Password = "Password1!",
                    Email = "yo_mamma@yahoo.com",
                    Firstname = "Haters",
                    Lastname = "Gonna O'Hate",
                    About = "I hate waterfalls.  And life.",
                    Private = true
                },
                new UserStruct{
                    Username = "jgarcia",
                    Password = "Password1!",
                    Email = "jgarcia11@wou.edu",
                    Firstname = "J.J.",
                    Lastname = "Garcia",
                    About = "I love the outdoors!",
                    Private = true
                }
            };

            foreach (UserStruct u in usersToAdd)
            {
                var user = new User();
                user.UserName = u.Username;
                user.Email = u.Email;
                user.FirstName = u.Firstname;
                user.LastName = u.Lastname;
                user.About = u.About;
                user.Private = u.Private;
                user.EnrollmentDate = now;
                UserManager.Create(user, u.Password);
            }
        }

        // ******************************************
        //  create an Admin for the website.
        // ******************************************
        public void CreateAdmin(ApplicationDbContext context)
        {

            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            string name = "Admin";
            string password = "Password1!";
            string email = "superuser@gmail.com";
            string firstname = "Super";
            string lastname = "User";


            // Create Admin Role if it does not already exist
            if (!RoleManager.RoleExists(name))
            {
                var roleresult = RoleManager.Create(new IdentityRole(name));
            }

            // Create User=Admin with password=Password1!
            var user = new User();
            user.UserName = name;
            user.FirstName = firstname;
            user.Email = email;
            user.LastName = lastname;
            user.EnrollmentDate = System.DateTime.Now;
            var adminresult = UserManager.Create(user, password);

            // Add User Admin to Role Admin
            if (adminresult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, name);
            }
        }

        // ******************************************
        // add Location and Recreation Data
        // ******************************************
        public void AddLocationData(ApplicationDbContext context)
        {
            //get single timestamp
            var now = DateTime.UtcNow;

            // add Location data
            var locations = new List<Location>
            {
                new Location { Label="Multnomah Falls", Latitude=45.5760, Longitude = -122.1154},
                new Location { Label="Silver Falls State Park", Latitude=44.8512, Longitude = -122.6462},
                new Location { Label="Santiam State Forest", Latitude=44.715935, Longitude =  -122.446831},
                new Location { Label="Grand Canyon National Park", Latitude=36.106796, Longitude = -112.113306},
                new Location { Label="Zion National Park", Latitude=37.298215, Longitude = -113.026255},
            };
            foreach (Location l in locations)
            {
                l.DateCreated = now;
                l.DateModified = now;
                l.Difficulty = Location.DifficultyRatings.Varies;
                l.RetrieveFormatedAddress();
            }
            locations.ForEach(s => context.Locations.AddOrUpdate(p => p.Label, s));
            context.SaveChanges();

            // add Recreation data
            var recreations = new List<Recreation>
            {
                new Recreation { Label = "Hiking"},
                new Recreation { Label = "Camping"}               
            };
            foreach (Recreation r in recreations)
            {
                r.DateCreated = now;
                r.DateModified = now;

            }
            recreations.ForEach(s => context.Recreations.AddOrUpdate(p => p.Label, s));
            context.SaveChanges();

            // add RecreationLocation data

            // add RecreationLocation data
            var locationrecreations = new List<LocationRecreation>()
            {
                new LocationRecreation { LocationID = 1, RecreationID = 1, RecreationLabel = "Hiking", IsChecked = true},
                new LocationRecreation { LocationID = 1, RecreationID = 2, RecreationLabel = "Camping", IsChecked = true},

                new LocationRecreation { LocationID = 2, RecreationID = 1, RecreationLabel = "Camping", IsChecked = true},

                new LocationRecreation { LocationID = 4, RecreationID = 1, RecreationLabel = "Hiking", IsChecked = true},
                new LocationRecreation { LocationID = 4, RecreationID = 2, RecreationLabel = "Camping", IsChecked = true},
                
            };
            locationrecreations.ForEach(lr => context.LocationRecreations.AddOrUpdate(rl => new { rl.LocationID, rl.RecreationID, rl.RecreationLabel, rl.IsChecked }, lr));
            context.SaveChanges();// INSERT statement conflicted with the FOREIGN KEY constraint error pops here but I don't know how to fix it because
                                  // we can't require that the location has a list of LocationRecreations until after we make them. We can't make the
                                  // LocationRecreation entries until we have a valid LocationID, so we can't make that first either.
            
            /*
            context.AddOrUpdateRecreationLocation("Multnomah Falls", "Hiking");
            context.AddOrUpdateRecreationLocation("Silver Falls State Park", "Hiking");
            context.AddOrUpdateRecreationLocation("Silver Falls State Park", "Camping");
            context.AddOrUpdateRecreationLocation("Santiam State Forest", "Hiking");
            context.AddOrUpdateRecreationLocation("Grand Canyon National Park", "Hiking");
            context.AddOrUpdateRecreationLocation("Zion National Park", "Hiking");
            context.AddOrUpdateRecreationLocation("Zion National Park", "Camping");
             * */
        }

        public void AddReviews(ApplicationDbContext context)
        {
            //get single timestamp
            var now = DateTime.UtcNow;

            // location id's
            int silver_falls_ID = context.Locations.Where(l => l.Label.Contains("Silver Falls")).Single().LocationID;
            int multnomah_falls_ID = context.Locations.Where(l => l.Label.Contains("Multnomah Falls")).Single().LocationID;
            int grand_canyon_ID = context.Locations.Where(l => l.Label.Contains("Grand Canyon")).Single().LocationID;
            int zion_park_ID = context.Locations.Where(l => l.Label.Contains("Zion National Park")).Single().LocationID;
            int santiam_park_ID = context.Locations.Where(l => l.Label.Contains("Santiam State Forest")).Single().LocationID;

            // users to associate with reviews
            User fancyman55 = context.Users.Where(u => u.UserName == "fancyman55").Single();
            User morningmist = context.Users.Where(u => u.UserName == "morningmist").Single();
            User pretzles = context.Users.Where(u => u.UserName == "pretzles").Single();
            User hatersGonnaHate = context.Users.Where(u => u.UserName == "hatersGonnaHate").Single();

            // for ratings
            bool like = true;
            bool dislike = false;

            // make a list of Reviews
            var reviews = new List<Review>
            {
                // fancyman55
                new Review { 
                    LocationID = silver_falls_ID, 
                    ReviewDate = now, 
                    Rating = like,
                    Comment = "One of my favorite places to go hiking with my wife.",
                    User = fancyman55
                },
                new Review { 
                    LocationID = multnomah_falls_ID, 
                    ReviewDate = now, 
                    Rating = like,
                    Comment = "One of my favorite places to go hiking with my wife.",
                    User = fancyman55
                },

                // morningmist
                new Review { 
                    LocationID = silver_falls_ID, 
                    ReviewDate = now, 
                    Rating = like,
                    User = morningmist
                },
                new Review { 
                    LocationID = multnomah_falls_ID, 
                    ReviewDate = now, 
                    Rating = like,
                    User = morningmist
                },
                new Review { 
                    LocationID = grand_canyon_ID, 
                    ReviewDate = now, 
                    Rating = like,
                    Comment = "Such a gorgeous view!",
                    User = morningmist
                },
                new Review { 
                    LocationID = zion_park_ID, 
                    ReviewDate = now, 
                    Rating = like,
                    Comment = "The colors present are simply breathtaking.  A must see for any outdoors enthusiast.",
                    User = morningmist
                },
                new Review { 
                    LocationID = santiam_park_ID, 
                    ReviewDate = now, 
                    Rating = like,
                    User = morningmist
                },

                // hatersGonnaHate
                new Review { 
                    LocationID = silver_falls_ID, 
                    ReviewDate = now, 
                    Rating = dislike,
                    User = hatersGonnaHate
                },
                new Review { 
                    LocationID = multnomah_falls_ID, 
                    ReviewDate = now, 
                    Rating = dislike,
                    Comment = "I hate waterfalls.",
                    User = hatersGonnaHate
                },
                new Review { 
                    LocationID = grand_canyon_ID, 
                    ReviewDate = now, 
                    Rating = like,
                    Comment = "I hate canyons.",
                    User = hatersGonnaHate
                },
                new Review { 
                    LocationID = zion_park_ID, 
                    ReviewDate = now, 
                    Rating = like,
                    Comment = "Even I can't hate this, just go and see for yourself.",
                    User = hatersGonnaHate
                },

                // pretzles
                new Review { 
                    LocationID = silver_falls_ID, 
                    ReviewDate = now, 
                    Rating = like,
                    User = pretzles
                },
                new Review { 
                    LocationID = multnomah_falls_ID, 
                    ReviewDate = now, 
                    Rating = like,
                    Comment = "When I get the chance, I love to mozey on down here.  It's so nice to walk across the bridge.  In my age, it's getting hard to walk up the high distance, though.",
                    User = pretzles
                },
                new Review { 
                    LocationID = grand_canyon_ID, 
                    ReviewDate = now, 
                    Rating = like,
                    Comment = "What a national symbol of America, go team USA!",
                    User = pretzles
                }

            };
            reviews.ForEach(r => context.Reviews.AddOrUpdate(s => s.Comment, r));
            context.SaveChanges();
        }


        // ******************************************************
        // Add images (All linked images are hosted remotely)
        // ******************************************************
        public void AddImages(ApplicationDbContext context)
        {
            //get single timestamp
            var now = DateTime.UtcNow;
 
            int sf_ID = context.Locations.Where(l => l.Label.Contains("Silver Falls")).Single().LocationID;
            int mf_ID = context.Locations.Where(l => l.Label.Contains("Multnomah Falls")).Single().LocationID;
            int gc_ID = context.Locations.Where(l => l.Label.Contains("Grand Canyon")).Single().LocationID;
            int zn_ID = context.Locations.Where(l => l.Label.Contains("Zion National Park")).Single().LocationID;
            int ss_ID = context.Locations.Where(l => l.Label.Contains("Santiam State Forest")).Single().LocationID;

            //my images
            var locationImages = new List<LocationImage>
            {
              // multnomah falls
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/01.JPG", Title = "My First Day In Oregon", LocationID = mf_ID},   
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/02.JPG", Title = "Wide Angle View", LocationID = mf_ID}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/03.JPG", Title = "The Old Bridge", LocationID = mf_ID}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/04.JPG", Title = "I Like Waterfalls", LocationID = mf_ID}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/05.JPG", Title = "Looking Up", LocationID = mf_ID}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/06.JPG", Title = "Me Waving At The Camera", LocationID = mf_ID}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/07.JPG", Title = "Looking Down From The Bridge", LocationID = mf_ID}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/08.JPG", Title = "The River Below", LocationID = mf_ID}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/09.JPG", Title = "Looking Across The Bridge", LocationID = mf_ID}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/10.JPG", Title = "Framed By Safety Netting", LocationID = mf_ID}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/11.JPG", Title = "Base of the Falls", LocationID = mf_ID}, 
                
               //Silver Falls
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/01.JPG", Title = "South Falls Sign ", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/02.JPG", Title = "The Trailhead", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/03.JPG", Title = "Peering Over The Edge", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/04.JPG", Title = "South Falls Crest", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/05.JPG", Title = "The Surrounding Valley", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/06.JPG", Title = "Portrait", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/07.JPG", Title = "Landscape", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/08.JPG", Title = "Amother Crest Close-up", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/09.JPG", Title = "Into Mist", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/10.JPG", Title = "The Treeline", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/11.JPG", Title = "The Rock Wall", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/12.JPG", Title = "The Path Downard", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/13.JPG", Title = "Another View", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/14.JPG", Title = "Base of South Falls", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/15.JPG", Title = "Uprooted Tree", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/16.JPG", Title = "Another World", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/17.JPG", Title = "Looking Upward", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/18.JPG", Title = "Framed By Trees", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/19.JPG", Title = "A Little Dark", LocationID = sf_ID}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/20.JPG", Title = "Rock and Fern", LocationID = sf_ID}, 
            };

            int i = 0;
            foreach (LocationImage image in locationImages)
            {
                image.DateTaken = now;
                image.DateCreated = now;
                image.DateModified = now;
                image.AltText = context.Locations.Find(image.LocationID).Label + " " + i;
                i++;
            }
            locationImages.ForEach(s => context.LocationImages.AddOrUpdate(p => p.ImageUrl, s));
            context.SaveChanges();

            // images found online
            locationImages = new List<LocationImage>
            {
                // Grand Canyon
                new LocationImage {ImageUrl = "http://pubpages.unh.edu/~mpp2/project/images/raft8.jpg", Title = "In the Valley", LocationID = gc_ID}, 
                new LocationImage {ImageUrl = "http://www.thecanyon.com/assets/css/images/grandcanyon1.jpg", Title = "Breathtaking View", LocationID = gc_ID}, 
                new LocationImage {ImageUrl = "http://images.boomsbeat.com/data/images/full/653/26-jpg.jpg", Title = "Sunset", LocationID = gc_ID}, 
                new LocationImage {ImageUrl = "http://www.canyontours.com/wp-content/uploads/2013/09/West-Rim.jpg", Title = "West Rim", LocationID = gc_ID}, 
                new LocationImage {ImageUrl = "http://upload.wikimedia.org/wikipedia/commons/7/7b/Grand_Canyon_colors.jpg", Title = "Layers of Color", LocationID = gc_ID}, 
                new LocationImage {ImageUrl = "http://pubpages.unh.edu/~mpp2/project/images/raft7.jpg", Title = "Rafting - Up Close", LocationID = gc_ID}, 

                // Zion National Park
                new LocationImage {ImageUrl = "http://upload.wikimedia.org/wikipedia/commons/1/10/Zion_angels_landing_view.jpg", Title = "Angels Landing View", LocationID = zn_ID}, 
                new LocationImage {ImageUrl = "http://www.utah.com/images/lf/panoZION.jpg", Title = "Panoramic View", LocationID = zn_ID}, 
                new LocationImage {ImageUrl = "http://www.globeimages.net/data/media/5/view_from_watchman_bridge_zion_national_park_utah_us.jpg", Title = "View from Watchman's Bridge", LocationID = zn_ID}, 

                // santiam national forest
                new LocationImage {ImageUrl = "http://upload.wikimedia.org/wikipedia/commons/7/75/North_Santiam_River_at_Niagara_County_Park_06268.JPG", Title = "North Santiam River", LocationID = ss_ID}, 
                new LocationImage {ImageUrl = "http://fscomps.fotosearch.com/compc/UNS/UNS090/u30481530.jpg", Title = "Sign", LocationID = ss_ID}, 
            };

            i = 0;
            foreach (LocationImage image in locationImages)
            {
                image.DateTaken = now;
                image.DateCreated = now;
                image.DateModified = now;
                image.AltText = context.Locations.Find(image.LocationID).Label + " " + i;
                i++;
            }
            locationImages.ForEach(s => context.LocationImages.AddOrUpdate(p => p.ImageUrl, s));
            context.SaveChanges();
        }

        // ******************************************************
        // Add Videos
        // ******************************************************
        public void AddVideos(ApplicationDbContext context)
        {
            var now = DateTime.UtcNow;

            int sf_ID = context.Locations.Where(l => l.Label.Contains("Silver Falls")).Single().LocationID;
            int mf_ID = context.Locations.Where(l => l.Label.Contains("Multnomah Falls")).Single().LocationID;
            int gc_ID = context.Locations.Where(l => l.Label.Contains("Grand Canyon")).Single().LocationID;
            int zn_ID = context.Locations.Where(l => l.Label.Contains("Zion National Park")).Single().LocationID;
            int ss_ID = context.Locations.Where(l => l.Label.Contains("Santiam State Forest")).Single().LocationID;

            var videos = new List<LocationVideo>
            {
                // Silver Falls
                new LocationVideo {
                    Description="Trail of Ten Falls at Silver Falls State Park", 
                    EmbedCode = "<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/J4HM7JDUrAA\" frameborder=\"0\" allowfullscreen></iframe>", 
                    LocationID = sf_ID
                },
                new LocationVideo {
                    Description="Silver Falls State Park - Day Use", 
                    EmbedCode = "<iframe width=\"420\" height=\"315\" src=\"https://www.youtube.com/embed/qYzh7JirlZI\" frameborder=\"0\" allowfullscreen></iframe>", 
                    LocationID = sf_ID
                },
                new LocationVideo {
                    Description="South Falls",
                    EmbedCode = "<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/LIurcmmypDc\" frameborder=\"0\" allowfullscreen></iframe>",
                    LocationID = sf_ID
                },

                // Multnomah Falls
                new LocationVideo {
                    Description="Multnomah Falls, Oregon - Aerial",
                    EmbedCode = "<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/aZUx2xZhjAI\" frameborder=\"0\" allowfullscreen></iframe>",
                    LocationID = mf_ID
                },

                // Grand Canyon
                new LocationVideo {
                    Description="Grand Canyon National Park (Documentary)",
                    EmbedCode = "<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/bEVEsIW4OXo\" frameborder=\"0\" allowfullscreen></iframe>",
                    LocationID = gc_ID
                },

                // Zion National Park
                new LocationVideo {
                    Description="The Subway in Zion National Park Video Hike",
                    EmbedCode = "<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/rgNSrE0BJ8g\" frameborder=\"0\" allowfullscreen></iframe>",
                    LocationID = zn_ID
                },
            };

            videos.ForEach(s => context.LocationVideos.AddOrUpdate(v => v.EmbedCode, s));
            context.SaveChanges();
        }

    }
}
