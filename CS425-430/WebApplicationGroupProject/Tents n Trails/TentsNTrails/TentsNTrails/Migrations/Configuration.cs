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
        protected override void Seed(TentsNTrails.Models.ApplicationDbContext db)
        {
            Console.WriteLine("MIGRATION!");   
            CreateAdmin(db);
            AddUsers(db);
            Add50States(db);
            AddLocationData(db);
            AddStatesToLocations(db);
            AddReviews(db);
            AddImages(db);
            AddVideos(db);
            
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
        public void AddUsers(ApplicationDbContext db)
        {
            System.Diagnostics.Debug.WriteLine("AddUsers()");
            var now = DateTime.UtcNow;

            var UserManager = new UserManager<User>(new UserStore<User>(db));

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
            pictures.ForEach(r => db.ProfilePictures.AddOrUpdate(s => s.Title, r));
            db.SaveChanges();
            */

        }

        // ******************************************
        //  create an Admin for the website.
        // ******************************************
        public void CreateAdmin(ApplicationDbContext db)
        {
            System.Diagnostics.Debug.WriteLine("CreateAdmin()");
            var UserManager = new UserManager<User>(new UserStore<User>(db));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

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
        public void AddLocationData(ApplicationDbContext db)
        {
            System.Diagnostics.Debug.WriteLine("AddLocationData()");

            //get single timestamp
            var now = DateTime.UtcNow;

            // add Location data
            System.Diagnostics.Debug.WriteLine("Adding Locations ...");
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
                new Location { 
                    Label="Mount Hood", 
                    Latitude=45.3735,
                    Longitude = -121.6959,
                    Description = "Called Wy'east by the now-extinct Multnomah tribe, Mount Hood is a stratovolcano in the Cascade Volcanic Arc of northern Oregon. In addition to being Oregon's highest mountain it is one of the loftiest mountains in the nation based on its prominence.",
                    Difficulty = Location.DifficultyRatings.Hard
                },
                new Location { 
                    Label="Yosemite National Park",
                    Latitude=37.8499,
                    Longitude = -119.5677,
                    Description = "Yosemite National Park is a United States National Park spanning eastern portions of Tuolumne, Mariposa and Madera counties in the central eastern portion of the U.S. state of California. Over 3.7 million people visit Yosemite each year.",
                    Difficulty = Location.DifficultyRatings.Varies
                },
                new Location { 
                    Label="Yellowstone National Park", 
                    Latitude=44.6000,
                    Longitude = -110.5000,
                    Description = "Yellowstone National Park is a national park located primarily in the U.S. state of Wyoming, although it also extends into Montana and Idaho. It was established by the U.S. Congress and signed into law by President Ulysses S. Grant on March 1, 1872.",
                    Difficulty = Location.DifficultyRatings.Varies
                },
                new Location { 
                    Label="Mount Rainier", 
                    Latitude=46.8529,
                    Longitude = -121.7604,
                    Description = "Mount Rainier is the highest mountain of the Cascade Range of the Pacific Northwest, and the highest mountain in the state of Washington. It is a large active stratovolcano located 54 miles (87 km) southeast of Seattle.",
                    Difficulty = Location.DifficultyRatings.Medium
                },
                new Location { 
                    Label="Bryce Canyon National Park", 
                    Latitude=37.6283,
                    Longitude = -112.1677,
                    Description = "Located in southwestern Utah in the United States, the major feature of the park is Bryce Canyon, which despite its name, is not a canyon, but a collection of giant natural amphitheaters along the eastern side of the Paunsaugunt Plateau.",
                    Difficulty = Location.DifficultyRatings.Medium
                },

            };

            foreach (Location l in locations)
            {
                l.DateCreated = now;
                l.DateModified = now;
                l.Difficulty = Location.DifficultyRatings.Varies;
                string state = Location.ReverseGeocodeState(l);
                l.State = db.States.Find(state);
                //l.State = db.States.Where(s => s.StateID.Equals(state)).SingleOrDefault();
            }

            locations.ForEach(s => db.Locations.AddOrUpdate(p => p.Label, s));
            db.SaveChanges();

            // add Recreation data
            System.Diagnostics.Debug.WriteLine("Adding Recreations ...");
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
            recreations.ForEach(s => db.Recreations.AddOrUpdate(p => p.Label, s));
            db.SaveChanges();

            // recreationID's
            int hikeID = db.Recreations.Where(l => l.Label.Contains("Hiking")).Single().RecreationID;
            int campID = db.Recreations.Where(l => l.Label.Contains("Camping")).Single().RecreationID;

            // location ID's
            int mf_ID = db.Locations.Where(l => l.Label.Contains("Multnomah Falls")).Single().LocationID;
            int sf_ID = db.Locations.Where(l => l.Label.Contains("Silver Falls")).Single().LocationID;
            int ss_ID = db.Locations.Where(l => l.Label.Contains("Santiam State Forest")).Single().LocationID;
            int gc_ID = db.Locations.Where(l => l.Label.Contains("Grand Canyon")).Single().LocationID;
            int zn_ID = db.Locations.Where(l => l.Label.Contains("Zion National Park")).Single().LocationID;

            // Hood Yosemite Yellowstone Rainier Bryce
            int hood_ID = db.Locations.Where(l => l.Label.Contains("Mount Hood")).Single().LocationID;
            int yose_ID = db.Locations.Where(l => l.Label.Contains("Yosemite")).Single().LocationID;
            int yell_ID = db.Locations.Where(l => l.Label.Contains("Yellowstone")).Single().LocationID;
            int rain_ID = db.Locations.Where(l => l.Label.Contains("Mount Rainier")).Single().LocationID;
            int bryc_ID = db.Locations.Where(l => l.Label.Contains("Bryce Canyon")).Single().LocationID;

            // add RecreationLocation data
            System.Diagnostics.Debug.WriteLine("Adding LocationRecreations ...");
            var locationrecreations = new List<LocationRecreation>()
            {
                new LocationRecreation { LocationID = mf_ID, RecreationID = hikeID, RecreationLabel = "Hiking", IsChecked = true},
                new LocationRecreation { LocationID = mf_ID, RecreationID = campID, RecreationLabel = "Camping", IsChecked = true},
                new LocationRecreation { LocationID = sf_ID, RecreationID = hikeID, RecreationLabel = "Camping", IsChecked = true},
                new LocationRecreation { LocationID = ss_ID, RecreationID = hikeID, RecreationLabel = "Hiking", IsChecked = true},
                new LocationRecreation { LocationID = gc_ID, RecreationID = hikeID, RecreationLabel = "Hiking", IsChecked = true},
                new LocationRecreation { LocationID = gc_ID, RecreationID = campID, RecreationLabel = "Camping", IsChecked = true},
                new LocationRecreation { LocationID = zn_ID, RecreationID = hikeID, RecreationLabel = "Hiking", IsChecked = true},
                new LocationRecreation { LocationID = zn_ID, RecreationID = campID, RecreationLabel = "Camping", IsChecked = true},

                new LocationRecreation { LocationID = hood_ID, RecreationID = hikeID, RecreationLabel = "Hiking", IsChecked = true},
                new LocationRecreation { LocationID = yose_ID, RecreationID = hikeID, RecreationLabel = "Hiking", IsChecked = true},
                new LocationRecreation { LocationID = yose_ID, RecreationID = campID, RecreationLabel = "Camping", IsChecked = true},
                new LocationRecreation { LocationID = yell_ID, RecreationID = hikeID, RecreationLabel = "Hiking", IsChecked = true},
                new LocationRecreation { LocationID = yell_ID, RecreationID = campID, RecreationLabel = "Camping", IsChecked = true},
                new LocationRecreation { LocationID = rain_ID, RecreationID = hikeID, RecreationLabel = "Hiking", IsChecked = true},
                new LocationRecreation { LocationID = bryc_ID, RecreationID = hikeID, RecreationLabel = "Hiking", IsChecked = true},
                
                
            };
            locationrecreations.ForEach(lr => db.LocationRecreations.AddOrUpdate(rl => new { rl.LocationID, rl.RecreationID, rl.RecreationLabel, rl.IsChecked }, lr));
            db.SaveChanges();// INSERT statement conflicted with the FOREIGN KEY constraint error pops here but I don't know how to fix it because
                                  // we can't require that the location has a list of LocationRecreations until after we make them. We can't make the
                                  // LocationRecreation entries until we have a valid LocationID, so we can't make that first either.
            
            /*
            db.AddOrUpdateRecreationLocation("Multnomah Falls", "Hiking");
            db.AddOrUpdateRecreationLocation("Silver Falls State Park", "Hiking");
            db.AddOrUpdateRecreationLocation("Silver Falls State Park", "Camping");
            db.AddOrUpdateRecreationLocation("Santiam State Forest", "Hiking");
            db.AddOrUpdateRecreationLocation("Grand Canyon National Park", "Hiking");
            db.AddOrUpdateRecreationLocation("Zion National Park", "Hiking");
            db.AddOrUpdateRecreationLocation("Zion National Park", "Camping");
             * */
        }

        public void AddReviews(ApplicationDbContext db)
        {
            System.Diagnostics.Debug.WriteLine("AddReviews()");
            //get single timestamp
            var now = DateTime.UtcNow;

            // location id's
            int silver_falls_ID = db.Locations.Where(l => l.Label.Contains("Silver Falls")).Single().LocationID;
            int multnomah_falls_ID = db.Locations.Where(l => l.Label.Contains("Multnomah Falls")).Single().LocationID;
            int grand_canyon_ID = db.Locations.Where(l => l.Label.Contains("Grand Canyon")).Single().LocationID;
            int zion_park_ID = db.Locations.Where(l => l.Label.Contains("Zion National Park")).Single().LocationID;
            int santiam_park_ID = db.Locations.Where(l => l.Label.Contains("Santiam State Forest")).Single().LocationID;

            // users to associate with reviews
            User fancyman55 = db.Users.Where(u => u.UserName == "fancyman55").Single();
            User morningmist = db.Users.Where(u => u.UserName == "morningmist").Single();
            User pretzles = db.Users.Where(u => u.UserName == "pretzles").Single();
            User hatersGonnaHate = db.Users.Where(u => u.UserName == "hatersGonnaHate").Single();

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
                    db.Reviews.AddOrUpdate(s => s.Comment, r);
                }
                catch (InvalidOperationException ex)
                {
                    System.Diagnostics.Debug.WriteLine("{0}: Unable to add or update Review. ({1})",  ex.ToString(), ex.Message);
                }
            });
            db.SaveChanges();
        }


        // ******************************************************
        // Add images (All linked images are hosted remotely)
        // ******************************************************
        public void AddImages(ApplicationDbContext db)
        {
            System.Diagnostics.Debug.WriteLine("AddImages()");
            //get single timestamp
            var now = DateTime.UtcNow;
 
            int sf_ID = db.Locations.Where(l => l.Label.Contains("Silver Falls"        )).Single().LocationID;
            int mf_ID = db.Locations.Where(l => l.Label.Contains("Multnomah Falls"     )).Single().LocationID;
            int gc_ID = db.Locations.Where(l => l.Label.Contains("Grand Canyon"        )).Single().LocationID;
            int zn_ID = db.Locations.Where(l => l.Label.Contains("Zion National Park"  )).Single().LocationID;
            int ss_ID = db.Locations.Where(l => l.Label.Contains("Santiam State Forest")).Single().LocationID;

            // Hood Yosemite Yellowstone Rainier Bryce
            int hood_ID = db.Locations.Where(l => l.Label.Contains("Mount Hood")).Single().LocationID;
            int yosemite_ID = db.Locations.Where(l => l.Label.Contains("Yosemite")).Single().LocationID;
            int yellowstone_ID = db.Locations.Where(l => l.Label.Contains("Yellowstone")).Single().LocationID;
            int rainier_ID = db.Locations.Where(l => l.Label.Contains("Mount Rainier")).Single().LocationID;
            int bryce_ID = db.Locations.Where(l => l.Label.Contains("Bryce Canyon")).Single().LocationID;

            // users to associate with media
            User fancyman55      = db.Users.Where(u => u.UserName.Equals("fancyman55"     )).Single();
            User morningmist     = db.Users.Where(u => u.UserName.Equals("morningmist"    )).Single();
            User pretzles        = db.Users.Where(u => u.UserName.Equals("pretzles"       )).Single();
            User hatersGonnaHate = db.Users.Where(u => u.UserName.Equals("hatersGonnaHate")).Single();
            User pandaPal        = db.Users.Where(u => u.UserName.Equals("pandaPal"       )).Single();


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
                image.AltText = db.Locations.Find(image.LocationID).Label + " " + i;
                i++;
            }
            locationImages.ForEach(s => db.LocationImages.AddOrUpdate(p => p.ImageUrl, s));
            db.SaveChanges();

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

                // mt hood
                new LocationImage{ImageUrl = "https://tentsntrails.blob.core.windows.net/images/user-photo-cfbadcef-a37f-4a7b-a2b6-12ffe3c6d64b.jpg", Title = "Trillium Lake", LocationID = hood_ID, User = morningmist },
                new LocationImage{ImageUrl = "https://tentsntrails.blob.core.windows.net/images/user-photo-a0c91eea-c478-4282-9e12-a273d2c192f7.jpg", Title = "Mount Rainier on a sunny day", LocationID = rainier_ID, User = morningmist },
                new LocationImage{ImageUrl = "https://tentsntrails.blob.core.windows.net/images/user-photo-a0e4b555-acea-402f-ad44-12b1616cd210.jpg", Title = "a beautiful view down the spires", LocationID = bryce_ID, User = morningmist },
                new LocationImage{ImageUrl = "https://tentsntrails.blob.core.windows.net/images/user-photo-b26043a3-8bbe-48c4-b2fd-6fc2d06d1b78.jpg", Title = "Waterfall in the valley.", LocationID = yellowstone_ID, User = morningmist },
                new LocationImage{ImageUrl = "https://tentsntrails.blob.core.windows.net/images/user-photo-adcbf9e0-109d-4d9a-b40b-02180a81ac87.jpg", Title = "One of Yosemite's lakes", LocationID = yosemite_ID, User = morningmist },
            };

            i = 0;
            foreach (LocationImage image in locationImages)
            {
                image.DateTaken = now;
                image.DateCreated = now;
                image.DateModified = now;
                image.AltText = db.Locations.Find(image.LocationID).Label + " " + i;
                i++;
            }
            locationImages.ForEach(s => db.LocationImages.AddOrUpdate(p => p.ImageUrl, s));
            db.SaveChanges();
        }

        // ******************************************************
        // Add Videos
        // ******************************************************
        public void AddVideos(ApplicationDbContext db)
        {
            System.Diagnostics.Debug.WriteLine("AddVideos()");
            var now = DateTime.UtcNow;

            int sf_ID = db.Locations.Where(l => l.Label.Contains("Silver Falls")).Single().LocationID;
            int mf_ID = db.Locations.Where(l => l.Label.Contains("Multnomah Falls")).Single().LocationID;
            int gc_ID = db.Locations.Where(l => l.Label.Contains("Grand Canyon")).Single().LocationID;
            int zn_ID = db.Locations.Where(l => l.Label.Contains("Zion National Park")).Single().LocationID;
            int ss_ID = db.Locations.Where(l => l.Label.Contains("Santiam State Forest")).Single().LocationID;

            // users to associate with media
            User fancyman55 = db.Users.Where(u => u.UserName.Equals("fancyman55")).Single();
            User morningmist = db.Users.Where(u => u.UserName.Equals("morningmist")).Single();
            User pretzles = db.Users.Where(u => u.UserName.Equals("pretzles")).Single();
            User hatersGonnaHate = db.Users.Where(u => u.UserName.Equals("hatersGonnaHate")).Single();
            User pandaPal = db.Users.Where(u => u.UserName.Equals("pandaPal")).Single();


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

            videos.ForEach(s => db.LocationVideos.AddOrUpdate(v => v.EmbedCode, s));
            db.SaveChanges();
        }
        
        // using this website http://www.50states.com/abbreviations.htm#.VUfIiflViko as reference for data
        public void Add50States(ApplicationDbContext db)
        {
            System.Diagnostics.Debug.WriteLine("Add50States()");
            List<State> states = new List<State>();
            states.Add(new State("AL", "Alabama"));
            states.Add(new State("AK", "Alaska"));
            states.Add(new State("AZ", "Arizona"));
            states.Add(new State("AR", "Arkansas"));
            states.Add(new State("CA", "California"));
            states.Add(new State("CO", "Colorado"));
            states.Add(new State("CT", "Connecticut"));
            states.Add(new State("DE", "Delaware"));
            states.Add(new State("FL", "Florida"));
            states.Add(new State("HA", "Georgia")); // 10
            states.Add(new State("HI", "Hawaii"));
            states.Add(new State("ID", "Idaho"));
            states.Add(new State("IL", "Illinois"));
            states.Add(new State("IN", "Indiana"));
            states.Add(new State("IA", "Iowa"));
            states.Add(new State("KS", "Kansas"));
            states.Add(new State("KY", "Kentucky"));
            states.Add(new State("LA", "Louisiana"));
            states.Add(new State("ME", "Maine"));
            states.Add(new State("MD", "Maryland")); // 20
            states.Add(new State("MA", "Massachusetts"));
            states.Add(new State("MI", "Michigan"));
            states.Add(new State("MN", "Minnesota"));
            states.Add(new State("MS", "Mississippi"));
            states.Add(new State("MO", "Missouri"));
            states.Add(new State("MT", "Montana"));
            states.Add(new State("NE", "Nebraska"));
            states.Add(new State("NV", "Nevada"));
            states.Add(new State("NH", "New Hampshire"));
            states.Add(new State("NJ", "New Jersey")); // 30
            states.Add(new State("NM", "New Mexico"));
            states.Add(new State("NY", "New York"));
            states.Add(new State("NC", "North Carolina"));
            states.Add(new State("ND", "North Dakota"));
            states.Add(new State("OH", "Ohio"));
            states.Add(new State("OK", "Oklahoma"));
            states.Add(new State("OR", "Oregon"));
            states.Add(new State("PA", "Pennsylvania"));
            states.Add(new State("RI", "Rhode Island"));
            states.Add(new State("SC", "South Carolina")); // 40
            states.Add(new State("SD", "South Dakota"));
            states.Add(new State("TN", "Tennessee"));
            states.Add(new State("TX", "Texas"));
            states.Add(new State("UT", "Utah"));
            states.Add(new State("VT", "Vermont"));
            states.Add(new State("VA", "Virginia"));
            states.Add(new State("WA", "Washington"));
            states.Add(new State("WV", "West Virginia"));
            states.Add(new State("WI", "Wisconsin"));
            states.Add(new State("WY", "Wyoming")); // 50

            states.ForEach(s => db.States.AddOrUpdate(p => p.StateID, s));
            db.SaveChanges();
        }

        /// <summary>
        /// Adds The state data to the locations using the Geocoding API.
        /// </summary>
        /// <param name="db">The DbContext to update.</param>
        public void AddStatesToLocations(ApplicationDbContext db)
        {
            System.Diagnostics.Debug.WriteLine("AddStatesToLocations()");
            foreach (Location l in db.Locations.ToList())
            {

                string state = Location.ReverseGeocodeState(l);
                l.State = db.States.Where(s => s.StateID.Equals(state)).SingleOrDefault();
                db.Entry(l).State = EntityState.Modified;
            }
            db.SaveChanges();
        }
            
    }
}
