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
            public string ImageUrl;
        }

        // ******************************************
        //  create test Users for the site.
        // ******************************************
        public void AddUsers(ApplicationDbContext context)
        {
            var now = DateTime.UtcNow;

            var UserManager = new UserManager<User>(new UserStore<User>(context));

            string defaultUrl = Image.DEFAULT_PROFILE_PICTURE_URL;

            var usersToAdd = new List<UserStruct>()
            {
                new UserStruct{
                    Username = "fancyman55",
                    Password = "Password1*",
                    Email = "aaroncarsonart@gmail.com",
                    Firstname = "Aaron",
                    Lastname = "Carson",
                    About = "I really like to go hiking and camping.",
                    Private = false,
                    ImageUrl = "https://tentsntrails.blob.core.windows.net/images/profile-picture-fancyman555988b8eb-7728-456e-942d-719fbdaa1b71.png"
                },
                new UserStruct{
                    Username = "morningmist",
                    Password = "Password1!", 
                    Email = "oneEarlyMorning@hotmail.com",
                    Firstname = "Polly",
                    Lastname = "Hollingsworth",
                    About = "I love hiking!  Waterfalls are best viewed in the morning.",
                    Private = false,
                    ImageUrl = defaultUrl
                },
                new UserStruct{
                    Username = "pretzles",
                    Password = "Password1!",
                    Email = "pretzles@hotmail.com",
                    Firstname = "Barack",
                    Lastname = "Obama",
                    About = "",
                    Private = true,
                    ImageUrl = "https://tentsntrails.blob.core.windows.net/images/profile-picture-pretzlesf954b007-7ed5-4aa9-a969-80bc834b113a.jpg"
                },
                new UserStruct{
                    Username = "hatersGonnaHate",
                    Password = "Password1!",
                    Email = "yo_mamma@yahoo.com",
                    Firstname = "Haters",
                    Lastname = "Gonna O'Hate",
                    About = "I hate waterfalls.  And life.",
                    Private = true,
                    ImageUrl = "https://tentsntrails.blob.core.windows.net/images/profile-picture-hatersGonnaHate0d982eb9-4de9-458c-b74d-be5574104a1d.gif"
                },
                new UserStruct{
                    Username = "jgarcia",
                    Password = "Password1!",
                    Email = "jgarcia11@wou.edu",
                    Firstname = "J.J.",
                    Lastname = "Garcia",
                    About = "I love the outdoors!",
                    Private = true,
                    ImageUrl = defaultUrl
                },
                new UserStruct{
                    Username = "pandaPal",
                    Password = "Password1!",
                    Email = "halloDude@yahoo.com",
                    Firstname = "Jane",
                    Lastname = "Goodall",
                    About = "Pandas are my friends.",
                    Private = true,
                    ImageUrl = "https://tentsntrails.blob.core.windows.net/images/profile-picture-pandapal5a26cbad-e477-45f0-8f08-96ae5afefb61.jpg"
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
                user.ProfilePictureUrl = u.ImageUrl;
                UserManager.Create(user, u.Password);
            }
            /*
            List<ProfilePicture> pictures = new List<ProfilePicture>();
            foreach (UserStruct u in usersToAdd)
            {
                // make default profile picture
                ProfilePicture profilePicture = new Models.ProfilePicture()
                {
                    Title = u.Username,
                    AltText = "Image of " + u.Username,
                    ImageUrl = u.ImageUrl,
                    DateCreated = now,
                    DateModified = now,
                    DateTaken = now,
                    IsSelected = true,
                    User = UserManager.FindByName(u.Username)
                };
                pictures.Add(profilePicture);
            }
            pictures.ForEach(r => context.ProfilePictures.AddOrUpdate(s => s.Title, r));
            context.SaveChanges();
            */

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
                new Location { 
                    Label="Multnomah Falls", 
                    Latitude=45.5760, 
                    Longitude = -122.1154,
                    Description = "Multnomah Falls is a waterfall on the Oregon side of the Columbia River Gorge, located east of Troutdale, between Corbett and Dodson, along the Historic Columbia River Highway. Multnomah Falls is the tallest waterfall in the state of Oregon.",
                    Difficulty = Location.DifficultyRatings.Easy
                },
                new Location { 
                    Label="Silver Falls State Park", 
                    Latitude=44.8512, 
                    Longitude = -122.6462,
                    Description = "Silver Falls State Park is a state park in the U.S. state of Oregon, located near Silverton, about 20 miles (32 km) east-southeast of Salem. It is the largest state park in Oregon with an area of more than 9,000 acres.",
                    Difficulty = Location.DifficultyRatings.Medium
                },
                new Location { 
                    Label="Santiam State Forest", 
                    Latitude=44.715935, 
                    Longitude =  -122.446831,
                    Description = "Santiam State Forest is one of six state forests managed by the Oregon Department of Forestry. The forest is located approximately 25 miles (40 km) southeast of Salem, Oregon, and includes 47,871 acres on the western slope of the Cascade Mountains.",
                    Difficulty = Location.DifficultyRatings.Hard
                },
                new Location { 
                    Label="Grand Canyon National Park", 
                    Latitude=36.106796, 
                    Longitude = -112.113306,
                    Description = "The Grand Canyon is a steep-sided canyon carved by the Colorado River in the state of Arizona in the US. It is contained within and managed by Grand Canyon National Park, the Hualapai Tribal Nation, the Havasupai Tribe and the Navajo Nation.",
                    Difficulty = Location.DifficultyRatings.Varies
                },
                new Location { 
                    Label="Zion National Park", 
                    Latitude=37.298215,
                    Longitude = -113.026255,
                    Description = "Located in the Southwestern US, near Springdale, Utah at the junction of the Colorado Plateau, Great Basin, and Mojave Desert regions, Zion National Park's unique geography and variety of life zones allow for unusual plant and animal diversity.",
                    Difficulty = Location.DifficultyRatings.Easy
                },
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
                    Comment = "It's really nice here.",
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

            
            reviews.ForEach(r => {
                try {
                    context.Reviews.AddOrUpdate(s => s.Comment, r);
                }
                catch (InvalidOperationException ex)
                {
                    System.Diagnostics.Debug.WriteLine("{0}: Unable to add or update Review. ({1})",  ex.ToString(), ex.Message);
                }
            });
            context.SaveChanges();
        }


        // ******************************************************
        // Add images (All linked images are hosted remotely)
        // ******************************************************
        public void AddImages(ApplicationDbContext context)
        {
            //get single timestamp
            var now = DateTime.UtcNow;
 
            int sf_ID = context.Locations.Where(l => l.Label.Contains("Silver Falls"        )).Single().LocationID;
            int mf_ID = context.Locations.Where(l => l.Label.Contains("Multnomah Falls"     )).Single().LocationID;
            int gc_ID = context.Locations.Where(l => l.Label.Contains("Grand Canyon"        )).Single().LocationID;
            int zn_ID = context.Locations.Where(l => l.Label.Contains("Zion National Park"  )).Single().LocationID;
            int ss_ID = context.Locations.Where(l => l.Label.Contains("Santiam State Forest")).Single().LocationID;

            // users to associate with media
            User fancyman55      = context.Users.Where(u => u.UserName.Equals("fancyman55"     )).Single();
            User morningmist     = context.Users.Where(u => u.UserName.Equals("morningmist"    )).Single();
            User pretzles        = context.Users.Where(u => u.UserName.Equals("pretzles"       )).Single();
            User hatersGonnaHate = context.Users.Where(u => u.UserName.Equals("hatersGonnaHate")).Single();
            User pandaPal        = context.Users.Where(u => u.UserName.Equals("pandaPal"       )).Single();


            //my images
            var locationImages = new List<LocationImage>
            {
              // multnomah falls
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/01.JPG", Title = "My First Day In Oregon", LocationID = mf_ID, User = fancyman55},   
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/02.JPG", Title = "Wide Angle View", LocationID = mf_ID, User = fancyman55}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/03.JPG", Title = "The Old Bridge", LocationID = mf_ID, User = fancyman55}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/04.JPG", Title = "I Like Waterfalls", LocationID = mf_ID, User = fancyman55}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/05.JPG", Title = "Looking Up", LocationID = mf_ID, User = fancyman55}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/06.JPG", Title = "Me Waving At The Camera", LocationID = mf_ID, User = fancyman55}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/07.JPG", Title = "Looking Down From The Bridge", LocationID = mf_ID, User = fancyman55}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/08.JPG", Title = "The River Below", LocationID = mf_ID, User = morningmist}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/09.JPG", Title = "Looking Across The Bridge", LocationID = mf_ID, User = morningmist}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/10.JPG", Title = "Framed By Safety Netting", LocationID = mf_ID, User = morningmist}, 
                new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/Multnomah/11.JPG", Title = "Base of the Falls", LocationID = mf_ID, User = morningmist}, 
                
               //Silver Falls
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/01.JPG", Title = "South Falls Sign ", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/02.JPG", Title = "The Trailhead", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/03.JPG", Title = "Peering Over The Edge", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/04.JPG", Title = "South Falls Crest", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/05.JPG", Title = "The Surrounding Valley", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/06.JPG", Title = "Portrait", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/07.JPG", Title = "Landscape", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/08.JPG", Title = "Amother Crest Close-up", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/09.JPG", Title = "Into Mist", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/10.JPG", Title = "The Treeline", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/11.JPG", Title = "The Rock Wall", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/12.JPG", Title = "The Path Downard", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/13.JPG", Title = "Another View", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/14.JPG", Title = "Base of South Falls", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/15.JPG", Title = "Uprooted Tree", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/16.JPG", Title = "Another World", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/17.JPG", Title = "Looking Upward", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/18.JPG", Title = "Framed By Trees", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/19.JPG", Title = "A Little Dark", LocationID = sf_ID, User = pretzles}, 
               new LocationImage {ImageUrl = "http://www.wou.edu/~acarson13/Test/SilverFalls/20.JPG", Title = "Rock and Fern", LocationID = sf_ID, User = pretzles}, 
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
                new LocationImage {ImageUrl = "http://pubpages.unh.edu/~mpp2/project/images/raft8.jpg", Title = "In the Valley", LocationID = gc_ID, User = pandaPal}, 
                new LocationImage {ImageUrl = "http://www.thecanyon.com/assets/css/images/grandcanyon1.jpg", Title = "Breathtaking View", LocationID = gc_ID, User = pandaPal}, 
                new LocationImage {ImageUrl = "http://images.boomsbeat.com/data/images/full/653/26-jpg.jpg", Title = "Sunset", LocationID = gc_ID, User = pandaPal}, 
                new LocationImage {ImageUrl = "http://www.canyontours.com/wp-content/uploads/2013/09/West-Rim.jpg", Title = "West Rim", LocationID = gc_ID, User = pandaPal}, 
                new LocationImage {ImageUrl = "http://upload.wikimedia.org/wikipedia/commons/7/7b/Grand_Canyon_colors.jpg", Title = "Layers of Color", LocationID = gc_ID, User = pandaPal}, 
                new LocationImage {ImageUrl = "http://pubpages.unh.edu/~mpp2/project/images/raft7.jpg", Title = "Rafting - Up Close", LocationID = gc_ID, User = morningmist}, 

                // Zion National Park
                new LocationImage {ImageUrl = "http://upload.wikimedia.org/wikipedia/commons/1/10/Zion_angels_landing_view.jpg", Title = "Angels Landing View", LocationID = zn_ID, User = hatersGonnaHate}, 
                new LocationImage {ImageUrl = "http://www.utah.com/images/lf/panoZION.jpg", Title = "Panoramic View", LocationID = zn_ID, User = hatersGonnaHate}, 
                new LocationImage {ImageUrl = "http://www.globeimages.net/data/media/5/view_from_watchman_bridge_zion_national_park_utah_us.jpg", Title = "View from Watchman's Bridge", LocationID = zn_ID, User = hatersGonnaHate}, 

                // santiam national forest
                new LocationImage {ImageUrl = "http://upload.wikimedia.org/wikipedia/commons/7/75/North_Santiam_River_at_Niagara_County_Park_06268.JPG", Title = "North Santiam River", LocationID = ss_ID, User = morningmist}, 
                new LocationImage {ImageUrl = "http://fscomps.fotosearch.com/compc/UNS/UNS090/u30481530.jpg", Title = "Sign", LocationID = ss_ID, User = pandaPal}, 
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

            // users to associate with media
            User fancyman55 = context.Users.Where(u => u.UserName.Equals("fancyman55")).Single();
            User morningmist = context.Users.Where(u => u.UserName.Equals("morningmist")).Single();
            User pretzles = context.Users.Where(u => u.UserName.Equals("pretzles")).Single();
            User hatersGonnaHate = context.Users.Where(u => u.UserName.Equals("hatersGonnaHate")).Single();
            User pandaPal = context.Users.Where(u => u.UserName.Equals("pandaPal")).Single();


            var videos = new List<LocationVideo>
            {
                // Silver Falls
                new LocationVideo {
                    Description="Trail of Ten Falls at Silver Falls State Park", 
                    EmbedCode = "<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/J4HM7JDUrAA\" frameborder=\"0\" allowfullscreen></iframe>", 
                    LocationID = sf_ID,
                    User = pretzles
                },
                new LocationVideo {
                    Description="Silver Falls State Park - Day Use", 
                    EmbedCode = "<iframe width=\"420\" height=\"315\" src=\"https://www.youtube.com/embed/qYzh7JirlZI\" frameborder=\"0\" allowfullscreen></iframe>", 
                    LocationID = sf_ID,
                    User = fancyman55
                },
                new LocationVideo {
                    Description="South Falls",
                    EmbedCode = "<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/LIurcmmypDc\" frameborder=\"0\" allowfullscreen></iframe>",
                    LocationID = sf_ID,
                    User = pretzles
                },

                // Multnomah Falls
                new LocationVideo {
                    Description="Multnomah Falls, Oregon - Aerial",
                    EmbedCode = "<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/aZUx2xZhjAI\" frameborder=\"0\" allowfullscreen></iframe>",
                    LocationID = mf_ID,
                    User = pretzles
                },

                // Grand Canyon
                new LocationVideo {
                    Description="Grand Canyon National Park (Documentary)",
                    EmbedCode = "<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/bEVEsIW4OXo\" frameborder=\"0\" allowfullscreen></iframe>",
                    LocationID = gc_ID,
                    User = morningmist
                },

                // Zion National Park
                new LocationVideo {
                    Description="The Subway in Zion National Park Video Hike",
                    EmbedCode = "<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/rgNSrE0BJ8g\" frameborder=\"0\" allowfullscreen></iframe>",
                    LocationID = zn_ID,
                    User = fancyman55
                },
            };

            videos.ForEach(s => context.LocationVideos.AddOrUpdate(v => v.EmbedCode, s));
            context.SaveChanges();
        }

    }
}
