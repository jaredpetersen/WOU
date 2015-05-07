using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TentsNTrails.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using System.Web.Routing;
using TentsNTrails.Controllers;
using System.Xml.Linq;


namespace TentsNTrails.Controllers
{
    public class LocationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<User> manager;
            
        public LocationController () 
        {
            manager = new UserManager<User>(new UserStore<User>(db));
        }

        // ************************************************************************************************************
        // INDEX
        // ************************************************************************************************************

        // GET: Location
        public ActionResult Index(int? recreationID, string sortOrder, string currentFilter)
        {
            // *************************************************
            // sorting by rating functionality
            // *************************************************

            // ViewModel
            LocationIndexViewModel viewModel = new LocationIndexViewModel();
            viewModel.Recreations = db.Recreations.ToList();

            // *************************************************
            // filter by Recreation
            // *************************************************            
            if (recreationID.HasValue)
            {
                viewModel.Locations = db.LocationRecreations
                    .Where(lr => lr.RecreationID == recreationID)
                    .Select(lr => lr.Location).ToList();
            }
            else
            {
                viewModel.Locations = db.Locations
                    .Include(l => l.Recreations)
                    .ToList();
            }

            // *************************************************
            // sort by name, rating, and difficulty
            // *************************************************
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.RatingSortParm = sortOrder == "Rating" ? "rating_desc" : "Rating";
            ViewBag.DifficultySortParm = sortOrder == "difficulty_desc" ? "Difficulty" : "difficulty_desc";

            // *************************************************
            // LOCATION LISTS
            // *************************************************
            viewModel.TopRatedLocations = GetTopRatedLocations(5);
            viewModel.MostRecentLocations = GetMostRecentLocations(5);
            if (User.Identity.IsAuthenticated)
            {
                viewModel.PersonalRecommendations = GetPersonalRecommendations(5);
                viewModel.FriendRecommendations = GetFriendRecommendations(5);
            }
            // *************************************************
            // calculate center of map display
            // *************************************************    
            Location center = GetLatLongCenter(viewModel.Locations);
            System.Diagnostics.Debug.WriteLine("centerLatitude:  " + center.Latitude);
            System.Diagnostics.Debug.WriteLine("centerLongitude: " + center.Longitude);
            ViewBag.centerLatitude = center.Latitude;
            ViewBag.centerLongitude = center.Longitude;

            return View(viewModel);
        }


        // ************************************************************************************************************
        // LIST HELPER METHODS
        // ************************************************************************************************************

        /// <summary>
        /// Get the specified amount of Top-Rated Locations.
        /// </summary>
        /// <param name="amount">The amount to get.</param>
        /// <returns>A list of Locations.</returns>
        public List<Location> GetTopRatedLocations(int amount)
        {
            // Divide the total number of ratings by the number of locations to get
            // We decided that the top rated locations have to have at least half as many votes as average
            int LocationCount = db.Locations.Count();
            int avgRatingsPerLocation = -1;

            if (LocationCount != 0)avgRatingsPerLocation = db.Reviews.Count() / LocationCount;
            else avgRatingsPerLocation = 1;
            
            int minRatings = (int)(.5 * avgRatingsPerLocation);
            var locations =  db.Locations.Where(l => l.Reviews.Count() > minRatings);
            return SortByRatingAndTake(locations, amount);

            /*
            int minRatings = (int)(.5 * avgRatingsPerLocation);
            return db.Locations
                .Where(l => l.Reviews.Count() > minRatings)
                .OrderByDescending(l => 
                    l.Reviews.Where(r => r.Rating).Count()
                    /l.Reviews.Count()
                ).Take(amount).ToList();
             */
        }

        /// <summary>
        /// Get the specified amount of most recently added Locations.
        /// </summary>
        /// <param name="amount">The amount to get.</param>
        /// <returns>A list of Locations.</returns>
        public List<Location> GetMostRecentLocations(int amount)
        {
            return db.Locations
                .OrderByDescending(l => l.DateCreated)
                .ThenByDescending(l =>
                    l.Reviews.Where(r => r.Rating).Count()
                    / ( (double) (l.Reviews.Count() == 0 ? 1 : l.Reviews.Count()) )
                )
                .Take(amount).ToList();
        }

        /// <summary>
        /// Get a list of locations that are recommended by the website based on your personal interests.
        /// </summary>
        /// <returns>A list of Locations.</returns>
        public List<Location> GetPersonalRecommendations(int amount)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("GetPersonalRecommendations({0}", amount));
            // ensure user is logged in.
            if (!User.Identity.IsAuthenticated)
            {
                System.Diagnostics.Debug.WriteLine("User not authenticated.");
                return new List<Location>();
            }

            User user = db.Users.Find(User.Identity.GetUserId());

            // get all Recreations the user partakes in.
            var userActivities = db.UserRecreations.Where(r =>
                    r.User.Equals(user.Id)
                    && r.IsChecked
                ).Select(r => r.Recreation);
            //if (userActivities.Count() == 0) userActivities = db.Recreations;
            
            // get all positively reviewed Locations by the user
            var reviews = db.Reviews.Where(r =>
                    r.User.Id.Equals(user.Id)
                );

            // get all matching bookmarked Locations by the user
            var bookmarkedLocations = db.LocationFlags.Where(f =>
                    f.User.Id.Equals(user.Id)
                    && f.Flag == Flag.HaveBeen
                    || f.Flag == Flag.WantToGo
                ).Select(f => f.Location);

            System.Diagnostics.Debug.WriteLine(String.Format("userActivities.Count(): {0}", userActivities.Count()));
            foreach (Recreation r in userActivities)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Recreation: {0}", r.Label));
            }
            System.Diagnostics.Debug.WriteLineIf(userActivities.Count() > 0, "");



            System.Diagnostics.Debug.WriteLine(String.Format("reviews.Count(): {0}", reviews.Count()));
            foreach (Review r in reviews)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Location: {0}    Vote: {1}    Review: {2}", r.Location.Label, (r.Rating ? "Up":"Down"), (r.Comment ?? "NULL")));
            }
            System.Diagnostics.Debug.WriteLineIf(reviews.Count() > 0, "");



            System.Diagnostics.Debug.WriteLine(String.Format("bookmarkedLocations.Count(): {0}", bookmarkedLocations.Count()));
            foreach (Location l in bookmarkedLocations)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("State: {0}    Location: {1}", l.StateID, l.Label));
            }
            System.Diagnostics.Debug.WriteLineIf(bookmarkedLocations.Count() > 0, "");



            // union the locations.
            IQueryable<Location> locations = null;

            // case 1: use has bookmarks and reviews
            if (bookmarkedLocations.Count() != 0 && reviews.Count() != 0)
            {
                System.Diagnostics.Debug.WriteLine("case 1: use has bookmarks and reviews");
                if (userActivities.Count() > 0)
                {
                    locations = reviews
                        .Where(r => r.Rating)
                        .Select(r=> r.Location)
                        .Union(bookmarkedLocations)
                        .Where(l => l.Recreations.Intersect(userActivities).Count() > 0);
                }
                else
                {
                    locations = reviews
                        .Where(r => r.Rating)
                        .Select(r => r.Location)
                        .Union(bookmarkedLocations);
                }
                
            }
            // case 2: user has no bookmarks
            else if (bookmarkedLocations.Count() == 0 && reviews.Count() != 0) 
            {
                System.Diagnostics.Debug.WriteLine("case 2: user has no bookmarks but has reviews");
                if (userActivities.Count() > 0)
                {
                    locations = reviews
                        .Where(r => r.Rating)
                        .Select(r => r.Location)
                        .Where(l => l.Recreations.Intersect(userActivities).Count() > 0);
                }
                else
                {
                    locations = reviews
                        .Where(r => r.Rating)
                        .Select(r => r.Location); 
                }
            }

            // case 3: user has no reviews
            else if (reviews.Count() == 0)
            {
                System.Diagnostics.Debug.WriteLine("case 3: user has bookmarks but no reviews");
                if (userActivities.Count() > 0)
                {
                    locations = bookmarkedLocations.Where(l => 
                        l.Recreations.Intersect(userActivities).Count() > 0);
                }
                else
                {
                    locations = bookmarkedLocations;
                }
            }
            // case 4: user has no bookmarks or reviews
            else
            {
                System.Diagnostics.Debug.WriteLine("case 3: user has neither bookmarks or reviews");
                if (userActivities.Count() > 0)
                {
                    System.Diagnostics.Debug.WriteLine("case 3: user has bookmarks but no reviews");
                    locations = db.Locations.Where(l =>
                        l.Recreations.Intersect(userActivities).Count() > 0);
                }
                else
                {
                    locations = db.Locations.Select(l => l);
                }
            }

            System.Diagnostics.Debug.WriteLine(String.Format("locations.Count(): {0}", locations.Count()));
            foreach (Location l in locations)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("State: {0}    Location: {1}", l.StateID, l.Label));
            }
            System.Diagnostics.Debug.WriteLineIf(locations.Count() > 0, "");


            var states = locations.Select(u => u.StateID);

            System.Diagnostics.Debug.WriteLine(String.Format("states.Count(): {0}", states.Count()));
            foreach (string s in states)
            {
                System.Diagnostics.Debug.WriteLine(s);
            }
            System.Diagnostics.Debug.WriteLineIf(states.Count() > 0, "");


            // finally, get all locations in the states of the result that are not already in result, or have reviews.
            var result = db.Locations.Where(l =>
                   states.Contains(l.StateID)
                   && !locations.Contains(l)
                   && !reviews.Select(r => r.LocationID).Contains(l.LocationID)
               );

           

            System.Diagnostics.Debug.WriteLine(String.Format("result.Count(): {0}", result.Count()));
            foreach (Location l in bookmarkedLocations)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("State: {0}    Location: {1}", l.StateID, l.Label));
            }
            System.Diagnostics.Debug.WriteLineIf(result.Count() > 0, "");

            return SortByRatingAndTake(result, amount);
        }

        /// <summary>
        /// Get a list of locations that are recommended by the website based on your friends like.
        /// </summary>
        /// <param name="amount">The number of results to return.</param>
        /// <returns>A list locations, sorted by rating.</returns>
        public List<Location> GetFriendRecommendations(int amount)
        {
            System.Diagnostics.Debug.WriteLine("GetFriendRecommendations()");

            //find the current User
            User currentUser = db.Users.Find(User.Identity.GetUserId());
            System.Diagnostics.Debug.WriteLine(String.Format("currentUser: {0}", currentUser.UserName));

            // Union Connection where the current User matches either User1 or User2, but select the other one (the connected User) 
            var connectedUsers =
                db.Connections.Where(c =>
                    c.User1.UserName.Equals(currentUser.UserName)
                )
                .Select(c => c.User2)
                .Union(
                    db.Connections.Where(c =>
                        c.User2.UserName.Equals(currentUser.UserName)
                    )
                    .Select(c => c.User1)
                );

            // print friend details
            System.Diagnostics.Debug.WriteLine(String.Format("connectedUsers.Count(): {0}", connectedUsers.Count()));
            foreach (User user in connectedUsers)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("User: {0}", user.UserName));
            }
            System.Diagnostics.Debug.WriteLineIf(connectedUsers.Count() > 0, "");

            // get positive reviews by friends
            var positiveReviews = connectedUsers
                .Select(user => user.UserReviews)
                .SelectMany(review => review)
                .Where(review => review.Rating);

            // print review details
            System.Diagnostics.Debug.WriteLine(String.Format("positiveReviews.Count(): {0}", positiveReviews.Count()));
            foreach (Review r in positiveReviews)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Location: {0}    Vote: {1}    Review: {2}", r.Location.Label, (r.Rating ? "Up" : "Down"), (r.Comment ?? "NULL")));

            }
            System.Diagnostics.Debug.WriteLineIf(positiveReviews.Count() > 0, "");

            var locations = positiveReviews
                .Select(r => r.Location)
                .Distinct();

            System.Diagnostics.Debug.WriteLine(String.Format("locations.Count(): {0}", locations.Count()));
            foreach (Location l in locations)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("State: {0}    Location: {1}", l.StateID, l.Label));
            }
            System.Diagnostics.Debug.WriteLineIf(locations.Count() > 0, "");

            return SortByRatingAndTake(locations, amount);
        }

        /// <summary>
        /// Convenience method for taking a raw IQueryable of type Location, ordering in descending order by
        /// the rating, and taking a set amount to return as a proper List.
        /// </summary>
        /// <param name="locations">The raw IQueryable data</param>
        /// <param name="amount">The amount to take</param>
        /// <returns>A List of Locations</returns>
        public List<Location> SortByRatingAndTake(IQueryable<Location> locations, int amount)
        {
            return locations.OrderByDescending(l =>
                l.Reviews.Where(r => r.Rating).Count()
                / ( (double) (l.Reviews.Count() == 0 ? 1 : l.Reviews.Count()) )
            )
            .Take(amount)
            .ToList();
        }


        /// <summary>
        /// Get All locations which match the passed collection's State, but do not match the ID.
        /// </summary>
        /// <param name="locations">A Collection of States.</param>
        /// <returns>A Collection of States.</returns>
        public IQueryable<Location> GetOtherLocationsMatchingState(IQueryable<Location> locations)
        {
            return db.Locations.Where(l =>
                   locations.Select(u => u.StateID).Contains(l.StateID)
                   && !locations.Contains(l)
               );
        }


        // Helper method that returns the ReviewID if this user has made a rating for this location or -1 elsewise
        public int getIdIfRated(int? LocationID)
        {
            var currentUserName = manager.FindById(User.Identity.GetUserId()).UserName;
            var reviews = db.Reviews.Where(r => r.Location.LocationID == LocationID);
            var userReviews = reviews.Where(r => r.User.UserName == currentUserName).ToList();

            if (userReviews.Count > 0)
            {
                return userReviews.First().ReviewID;
            }
            else
            {
                return -1;
            }
        }

        // Helper method that returns a map of locationIDs to the rating this user has associated with it
        public Dictionary<int, int> getRatingsForLocations(List<Location> locations)
        {
            var ratings = new Dictionary<int, int>(locations.Count());
            if (User.Identity.IsAuthenticated)
            {
                // -1 if no rating by this user, 0 if down rating, 1 if up rating.
                foreach (Location loc in locations)
                {
                    int reviewID = getIdIfRated(loc.LocationID);
                    if (reviewID != -1) // this user has rated this location
                    {
                        var review = db.Reviews.Where(r => r.ReviewID == reviewID).First();
                        if (review.Rating)
                        {
                            ratings.Add(loc.LocationID, 1);
                        }
                        else
                        {
                            ratings.Add(loc.LocationID, 0);
                        }
                    }
                    else
                    {
                        ratings.Add(loc.LocationID, -1);
                    }
                }
            }
            else
            {
                foreach (Location loc in locations)
                {
                    ratings.Add(loc.LocationID, -1);
                }
            }

            return ratings;
        }

        // ************************************************************************************************************
        // MEDIA
        // ************************************************************************************************************

        // GET: MediaViewModel
        //
        // Shows a grid display of images and videos for the given location.  
        public ActionResult Media(int locationID)
        {
            /*
            LocationMediaViewModel media = new LocationMediaViewModel();
            media.Images = db.LocationImages.Where(i => i.LocationID == locationID).ToList();
            media.Videos = db.LocationVideos.Where(i => i.LocationID == locationID).ToList();
            
            ViewBag.Location = db.Locations.Where(i => i.LocationID == locationID).SingleOrDefault();
            ViewBag.CancelAction = "Details/" + locationID;

            return View(media);
             */
            Location location = db.Locations.Find(locationID);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // GET: Location/Details/5
        public ActionResult Details(int? id, bool? success, string mergedLocation)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            if (success == true && mergedLocation != null)
            {
                ViewBag.SuccessMessage = String.Format("{0} has been merged into {1}.", mergedLocation, location.Label);
            }
            // See if there are any reviews for this location
            var reviews = db.Reviews.Include(r => r.Location);
            var reviewList = reviews.Where(r => r.LocationID == id)
                                .Where(r => !r.Comment.Equals(""))
                                .Where(r => r.Comment != null).ToList();
            if (reviewList.Count == 0)
            {
                ViewBag.HasReviews = false;
            }
            else
            {
                ViewBag.HasReviews = true;
                ViewBag.Rating = location.Rating();
            }

            ViewBag.UpVotes = location.UpVotes();
            ViewBag.DownVotes = location.DownVotes();

            // FOR RATING THUMB COLORS
            List<Location> locations = new List<Location>();
            locations.Add(location);
            var ratings = getRatingsForLocations(locations);
            ViewBag.Rating = ratings[location.LocationID];

            // Help the view to know the current flag (if one exists)
            ViewBag.HasHaveBeenFlag = false;
            ViewBag.HasWantToGoFlag = false;
            ViewBag.HasGoAgainFlag = false; 
            User currentUser = manager.FindById(User.Identity.GetUserId());

            if (currentUser != null) {
                var locationFlags = db.LocationFlags
                    .Where(f => f.LocationID == id)
                    .Where(f => f.User.Id == currentUser.Id);
            
                if (locationFlags != null && locationFlags.Count() > 0)
                {
                    Flag flag = locationFlags.Single().Flag;
                    if (flag == Flag.HaveBeen)
                    {
                        ViewBag.HasHaveBeenFlag = true;
                    }
                    else if (flag == Flag.WantToGo)
                    {
                        ViewBag.HasWantToGoFlag = true;
                    }
                    else if (flag == Flag.GoAgain)
                    {
                        ViewBag.HasGoAgainFlag = true;
                    }
                }
            }

            // ************************************************************
            // add images
            // ************************************************************
            var locationImages = db.LocationImages.Where(l => l.LocationID == id);
            int imageDisplayCount = Math.Min(locationImages.Count(), 5);
            
            // randomly accessing data using method from:
            // http://stackoverflow.com/questions/26201681/how-to-get-random-entries-from-database-in-mvc4-using-linq
            ViewBag.LocationImages = locationImages.OrderBy(c => Guid.NewGuid()).Take(imageDisplayCount).ToList();

            //
            // add RecOptions
            //
            List<LocationRecreation> locRecList = new List<LocationRecreation>();

            foreach (var locrec in db.LocationRecreations)
            {
                if (locrec.LocationID == location.LocationID)
                {
                    locRecList.Add(locrec);
                }
            }
            location.RecOptions = locRecList;

            return View(location);
        }

        // POST: Location/SaveFlag
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SaveFlag([Bind(Include = "LocationID")] LocationFlag location, String flag)
        public ActionResult SaveFlag(String flag, int? locationID)
        {
            if (ModelState.IsValid && flag != null && locationID != null)
            {
                User currentUser = manager.FindById(User.Identity.GetUserId());
                Flag newFlag;

                if (flag.Equals(Flag.HaveBeen.ToString()))
                {
                    newFlag = Flag.HaveBeen;
                }
                else if (flag.Equals(Flag.WantToGo.ToString()))
                {
                    newFlag = Flag.WantToGo;
                }
                else 
                {
                    newFlag = Flag.GoAgain;
                }


                //// Find if there are any flags already in the DB for this user for this location
                //var oldLocationFlag = db.LocationFlags
                //    .Where(f => f.LocationID == locationID)
                //    .Where(f => f.User.Id == currentUser.Id)
                //    .Single();
                try {
                    // Find if there are any flags already in the DB for this user for this location
                    // Throws an exception if there's not exactly one LocationFlag returned, in which case we'll add it
                    var oldLocationFlag = db.LocationFlags
                        .Where(f => f.LocationID == locationID)
                        .Where(f => f.User.Id == currentUser.Id)
                        .Single();
                    
                    // set the new flag (the user and location ID will stay the same)
                    oldLocationFlag.Flag = newFlag;

                    // update what's in the DB
                    db.Entry(oldLocationFlag).State = EntityState.Modified;
                }
                catch (InvalidOperationException e)
                {
                    LocationFlag locationFlag = new LocationFlag();
                    locationFlag.LocationID = (int)locationID;
                    locationFlag.User = currentUser;
                    locationFlag.Flag = newFlag;
                    //add it to the DB
                    db.LocationFlags.Add(locationFlag);
                }
                db.SaveChanges();

                return RedirectToAction("Details/" + locationID, "Location");
            }

            return RedirectToAction("Index", "Location");
        }


        // ************************************************************************************************************
        // CREATE
        // ************************************************************************************************************

        [Authorize]
        // GET: Location/Create
        public ActionResult Create()
        {
            //Set up LocationRecreation Options
            CreateLocationViewModel locViewModel = new CreateLocationViewModel();
            List<LocationRecreation> locRecList = new List<LocationRecreation>();

            foreach (var rec in db.Recreations)
            {
                LocationRecreation lr = new LocationRecreation();
                lr.RecreationID = rec.RecreationID;
                lr.RecreationLabel = rec.Label;

                locRecList.Add(lr);
            }
            locViewModel.RecOptions = locRecList;
            return View(locViewModel);
        }

        // POST: Location/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLocationViewModel model)
        {
            Location location = new Location();

            if (ModelState.IsValid)
            {
                // transfer info
                location.Label = model.Label;
                location.Latitude = model.Latitude;
                location.Longitude = model.Longitude;
                location.Description = model.Description;
                location.Difficulty = (Location.DifficultyRatings)model.Difficulty;
                // initialize DateTime Stamps
                location.DateCreated = DateTime.UtcNow;
                location.DateModified = location.DateCreated;

                location.Rating();
                location.UpVotes();
                location.DownVotes();
                /*
                location.State = db.States.Where(s =>
                    s.Abbreviation.Equals(Location.ReverseGeocodeState(location))
                ).SingleOrDefault();
                */
                // save changes
                db.Locations.Add(location);
                db.SaveChanges();//must save before we move on so that location gets an ID

                // update LocationRecreation Table
                List<LocationRecreation> locrecList = new List<LocationRecreation>();
                foreach (var locRec in model.RecOptions)
                {
                    if (locRec.IsChecked)
                    {
                        LocationRecreation lr = locRec;

                        int latestID = db.Locations.Where(l => l.Label == location.Label).ToList().First().LocationID;
                        Location tempL = db.Locations.Find(latestID);//get the last Location entered
                        lr.LocationID = tempL.LocationID;
                        db.LocationRecreations.Add(lr);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(location);
        }  

        /// <summary>
        /// This is the search algorithm used by Location/Index and Location/Browse views.
        /// </summary>
        /// <param name="query">The user's raw search query.</param>
        /// <returns>A List of locations matching the search query.</returns>
        public List<Location> SearchFor(String query)
        {
            query = query.ToLower();
            List<Location> locations = db.Locations.Where(l => 
                l.Label.ToLower().Contains(query)
                || l.StateID.Equals(query)
                || l.State.Name.Equals(query)
            ).OrderBy(l => l.Label)
            .ToList();
            return locations;
        }

        // ************************************************************************************************************
        // EDIT
        // ************************************************************************************************************

        // GET: Location/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }

            //List<LocationRecreation> locRecList = new List<LocationRecreation>();

            //foreach (var rec in db.Recreations)
            //{
            //    LocationRecreation lr = new LocationRecreation();
            //    lr.RecreationID = rec.RecreationID;
            //    lr.RecreationLabel = rec.Label;

            //    locRecList.Add(lr);
            //}
            //location.RecOptions = locRecList;

            return View(location);
        }

        // POST: Location/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LocationID,Label,Latitude,Longitude,DateCreated,Rating,Description,Difficulty")] Location location)
        {
            if (ModelState.IsValid)
            {
                // update DateModified
                location.DateModified = DateTime.UtcNow;

                // save changes
                db.Entry(location).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(location);
        }

        // ************************************************************************************************************
        // DELETE
        // ************************************************************************************************************

        // GET: Location/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: Location/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Location location = db.Locations.Find(id);
            db.Locations.Remove(location);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ************************************************************************************************************
        // JOIN
        // ************************************************************************************************************

        // GET: Location
        [Authorize(Roles = "Admin")]
        public ActionResult Join(string searchStringA, string searchStringB, int[] LocationA, int[] LocationB, int? pageA, int? pageB)
        {
            // Check to see if the user selected items for merging
            if ((LocationA != null) && (LocationB != null))
            {
                // Check to see if the user only selected one item in each column
                // TO DO: Allow multiple location A's
                if ((LocationA.Length == 1) && (LocationB.Length == 1))
                {
                    int locationA = LocationA[0];
                    int locationB = LocationB[0];

                    if (locationA != locationB)
                    {

                        // TO DO: Change the various IDs for the reviews and media items associated with Location A to
                        // be associated with Location B

                        // Migrate images
                        var allImages = db.LocationImages.Where(i => i.LocationID.Equals(locationA)); ;
                        foreach (var image in allImages)
                        {
                            image.LocationID = locationB;
                        }

                        // Migrate videos
                        var allVideos = db.LocationVideos.Where(v => v.LocationID.Equals(locationA));
                        foreach (var video in allVideos)
                        {
                            video.LocationID = locationB;
                        }

                        // Migrate reviews
                        var allReviewsA = db.Reviews.Where(r => r.LocationID.Equals(locationA)).ToList();
                        var allReviewsB = db.Reviews.Where(r => r.LocationID.Equals(locationB)).ToList();
                        bool duplicateReviewFlag = false;

                        foreach (var reviewA in allReviewsA)
                        {
                            foreach (var reviewB in allReviewsB)
                            {
                                if (reviewA.User.Id == reviewB.User.Id)
                                {
                                    duplicateReviewFlag = true;
                                }

                                if (duplicateReviewFlag == true)
                                {
                                    break;
                                }
                            }

                            if (duplicateReviewFlag == false)
                            {
                                // A review was not found under Location B for the specified user
                                // Change location ID for review under location A
                                reviewA.LocationID = locationB;
                            }
                            duplicateReviewFlag = false;
                        }

                        // Migrate recreations
                        var allRecreationA = db.LocationRecreations.Where(s => s.LocationID.Equals(locationA)).ToList();
                        var allRecreationB = db.LocationRecreations.Where(s => s.LocationID.Equals(locationB)).ToList();
                        bool duplicateRecreationFlag = false;

                        foreach (var recreationA in allRecreationA)
                        {
                            foreach (var recreationB in allRecreationB)
                            {
                                if (recreationA.RecreationID == recreationB.RecreationID)
                                {
                                    duplicateRecreationFlag = true;
                                }

                                if (duplicateRecreationFlag == true)
                                {
                                    break;
                                }
                            }

                            if (duplicateRecreationFlag == false)
                            {
                                LocationRecreation locRec = new LocationRecreation();
                                locRec.LocationID = locationB;
                                locRec.RecreationID = recreationA.RecreationID;
                                locRec.RecreationLabel = recreationA.RecreationLabel;
                                locRec.IsChecked = true;
                                db.LocationRecreations.Add(locRec);
                            }
                            duplicateRecreationFlag = false;
                        }

                        // Migrate Saved Locations
                        var allSavedA = db.LocationFlags.Where(s => s.LocationID.Equals(locationA)).ToList();
                        var allSavedB = db.LocationFlags.Where(s => s.LocationID.Equals(locationB)).ToList();
                        bool duplicateSavedFlag = false;

                        foreach (var savedA in allSavedA)
                        {
                            foreach (var savedB in allSavedB)
                            {
                                if (savedA.User.Id == savedB.User.Id)
                                {
                                    duplicateSavedFlag = true;
                                }

                                if (duplicateSavedFlag == true)
                                {
                                    break;
                                }
                            }

                            if (duplicateReviewFlag == false)
                            {
                                savedA.LocationID = locationB;
                            }
                            duplicateSavedFlag = false;
                        }

                        // Remove Location A
                        Location deleteLocation = db.Locations.Find(locationA);
                        db.Locations.Remove(deleteLocation);

                        //db.LocationRecreations.Remove(
                        db.LocationRecreations.RemoveRange(db.LocationRecreations.Where(r => r.LocationID == locationA));

                        // Save All Changes
                        db.SaveChanges();

                        ViewBag.SuccessMessage = "Location A successfully merged into Location B.";

                        // Redirect to the target location at the end of the merging
                        return RedirectToAction("Details", new { id = locationB, success = true, mergedLocation = deleteLocation.Label});
                    }
                    else
                    {
                        // Need to redirect to the Join page with an error message
                        // Location A and B are the same
                        //return RedirectToAction("Join");
                        //RedirectToAction("Detail", new { "errorID", 1 });
                        int pageSize = 3;

                        int pageANumber = (pageA ?? 1);
                        int pageBNumber = (pageB ?? 1);

                        JoinLocationsViewModel viewModel = new JoinLocationsViewModel();

                        if (!String.IsNullOrEmpty(searchStringA))
                        {
                            viewModel.LocationA = db.Locations.OrderBy(l => l.Label).Where(l => l.Label.ToLower().Contains(searchStringA.ToLower())).ToPagedList(pageANumber, pageSize);
                        }
                        else
                        {
                            viewModel.LocationA = db.Locations
                            .OrderBy(l => l.Label)
                            .ToPagedList(pageANumber, pageSize);
                        }

                        if (!String.IsNullOrEmpty(searchStringB))
                        {
                            viewModel.LocationB = db.Locations.OrderBy(l => l.Label).Where(l => l.Label.ToLower().Contains(searchStringB.ToLower())).ToPagedList(pageBNumber, pageSize);
                        }
                        else
                        {
                            viewModel.LocationB = db.Locations
                            .OrderBy(l => l.Label)
                            .ToPagedList(pageBNumber, pageSize);
                        }

                        ViewBag.ErrorMessage = "";

                        if ((LocationA.Length == 1) || (LocationB.Length == 1))
                        {
                            ViewBag.ErrorMessage = "Please select a Location A and Location B.";
                        }

                        //Find if current user is an admin
                        ViewBag.IsAdmin = User.IsInRole("Admin");

                        return View(viewModel);
                    }
                }
                else
                {
                    // Need to redirect to the Join page with an error message
                    // Need to select a single location A and location B -- This is just for now. If we want to, we can modify this later
                    int pageSize = 3;

                    int pageANumber = (pageA ?? 1);
                    int pageBNumber = (pageB ?? 1);

                    JoinLocationsViewModel viewModel = new JoinLocationsViewModel();

                    if (!String.IsNullOrEmpty(searchStringA))
                    {
                        viewModel.LocationA = db.Locations.OrderBy(l => l.Label).Where(l => l.Label.ToLower().Contains(searchStringA.ToLower())).ToPagedList(pageANumber, pageSize);
                    }
                    else
                    {
                        viewModel.LocationA = db.Locations
                        .OrderBy(l => l.Label)
                        .ToPagedList(pageANumber, pageSize);
                    }

                    if (!String.IsNullOrEmpty(searchStringB))
                    {
                        viewModel.LocationB = db.Locations.OrderBy(l => l.Label).Where(l => l.Label.ToLower().Contains(searchStringB.ToLower())).ToPagedList(pageBNumber, pageSize);
                    }
                    else
                    {
                        viewModel.LocationB = db.Locations
                        .OrderBy(l => l.Label)
                        .ToPagedList(pageBNumber, pageSize);
                    }

                    ViewBag.ErrorMessage = "Please select a single Location A and single Location B.";

                    //Find if current user is an admin
                    ViewBag.IsAdmin = User.IsInRole("Admin");

                    return View(viewModel);
                }
            }
            // Do not merge, just perform the search
            else
            {
                int pageSize = 3;

                int pageANumber = (pageA ?? 1);
                int pageBNumber = (pageB ?? 1);

                JoinLocationsViewModel viewModel = new JoinLocationsViewModel();

                if (!String.IsNullOrEmpty(searchStringA))
                {
                    viewModel.LocationA = db.Locations.OrderBy(l => l.Label).Where(l => l.Label.ToLower().Contains(searchStringA.ToLower())).ToPagedList(pageANumber, pageSize);
                }
                else
                {
                    viewModel.LocationA = db.Locations
                    .OrderBy(l => l.Label)
                    .ToPagedList(pageANumber, pageSize);
                }

                if (!String.IsNullOrEmpty(searchStringB))
                {
                    viewModel.LocationB = db.Locations.OrderBy(l => l.Label).Where(l => l.Label.ToLower().Contains(searchStringB.ToLower())).ToPagedList(pageBNumber, pageSize);
                }
                else
                {
                    viewModel.LocationB = db.Locations
                    .OrderBy(l => l.Label)
                    .ToPagedList(pageBNumber, pageSize);
                }

                if ((LocationA != null) || (LocationB != null))
                {
                    ViewBag.ErrorMessage = "Please select a single Location A and single Location B.";
                }

                //Find if current user is an admin
                ViewBag.IsAdmin = User.IsInRole("Admin");

                return View(viewModel);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Convenience method to calculate the centerpoint of latlong coordinates using trigonometry
        /// (solution used from http://stackoverflow.com/questions/6671183/calculate-the-center-point-of-multiple-latitude-longitude-coordinate-pairs)
        /// </summary>
        /// <param name="locations">The list of locations with latlong data to average.</param>
        /// <returns>A temp location containing the averaged data.</returns>
        private Location GetLatLongCenter(ICollection<Location> locations)
        {
            int count = locations.Count;

            // return center of US if no locations.
            if (count == 0)
            {
                // for now, use this: the below logic doesn't seem to be too accurate.  it is close, though ...
                return new Location()
                {
                    Latitude = 39.8282,
                    Longitude = -98.5795
                };
            }

            double x = 0;
            double y = 0;
            double z = 0;

            foreach(Location l in locations)
            {
                // convert to radians
                double latitude = l.Latitude * Math.PI / 180;
                double longitude = l.Longitude * Math.PI / 180;

                // convert to cartesian coordinates
                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            // average cartesian coordinates
            x /= count;
            y /= count;
            z /= count;

            // convert back to latitude/longitude
            double hypoteneuse = Math.Sqrt(x * x + y * y);
            double centerLatitude  = Math.Atan2(z, hypoteneuse) * 180 / Math.PI;
            double centerLongitude = Math.Atan2(y, x) * 180 / Math.PI;

            // store results in a temporary location.
            return new Location()
            {
                Latitude = centerLatitude,
                Longitude = centerLongitude
            };
        }

        // ************************************************************************************************************
        // VIEW ALL (simple index: no recommendations or extra lists)
        // ************************************************************************************************************

        // GET: Location
        public ActionResult Browse(int? recreationID, string sortOrder, string currentFilter, string query, int? page)
        {
            // ViewModel
            BrowseLocationsViewModel viewModel = new BrowseLocationsViewModel();
            viewModel.Recreations = db.Recreations.ToList();
            var locations = new List<Location>();

            // *************************************************
            // filter by Recreation
            // *************************************************            
            if (recreationID.HasValue)
            {
                List<LocationRecreation> locationRecreations = db.LocationRecreations
                    .Include(lr => lr.Location)
                    .Where(lr => lr.RecreationID == recreationID)
                    .ToList();
                foreach (LocationRecreation lr in locationRecreations)
                {
                    locations.Add(lr.Location);
                }
            }
            else
            {
                locations = db.Locations
                    .Include(l => l.Recreations)
                    .ToList();
            }

            int count = locations.Count();

            // *************************************************
            // sort by name, rating, and difficulty
            // *************************************************
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.RatingSortParm = sortOrder == "Rating" ? "rating_desc" : "Rating";
            ViewBag.DifficultySortParm = sortOrder == "difficulty_desc" ? "Difficulty" : "difficulty_desc";

            if (!String.IsNullOrEmpty(query))
            {
                page = 1;
                locations = SearchFor(query);

                if (locations.Count == 1)
                {
                    // If the search returns only one result, just go to the details page automatically
                    return RedirectToAction("Details/" + locations.ElementAt(0).LocationID, "Location");
                }
            }
            switch (sortOrder)
            {
                case "name_desc":
                    locations = locations.OrderByDescending(l => l.Label).ToList();
                    break;
                case "Rating":
                    locations = locations.OrderBy(l => l.Rating()).ToList();
                    break;
                case "rating_desc":
                    locations = locations.OrderByDescending(l => l.Rating()).ToList();
                    break;
                case "Difficulty":
                    locations = locations.OrderBy(l => l.Difficulty).ToList();
                    break;
                case "difficulty_desc":
                    locations = locations.OrderByDescending(l => l.Difficulty).ToList();
                    break;
                default:
                    locations = locations.OrderBy(l => l.Label).ToList();
                    break;
            }

            // PAGING
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            viewModel.Locations = locations.ToPagedList(pageNumber, pageSize); ;

            // *************************************************
            // calculate center of map display
            // *************************************************            
            Location center = GetLatLongCenter(viewModel.Locations.ToList());
            System.Diagnostics.Debug.WriteLine("centerLatitude:  " + center.Latitude);
            System.Diagnostics.Debug.WriteLine("centerLongitude: " + center.Longitude);
            ViewBag.centerLatitude = center.Latitude;
            ViewBag.centerLongitude = center.Longitude;

            //Find if current user is an admin
            ViewBag.IsAdmin = User.IsInRole("Admin");

            return View(viewModel);
        }


        // ************************************************************************************************************
        // REVIEW FORM
        // ************************************************************************************************************

        // Renders a partial view of a Review Form, with the up and down votes.
        public PartialViewResult ReviewForm(int id, string redirectAction, string redirectController)
        {
            Location location = db.Locations.Find(id);
            ViewBag.redirectAction = redirectAction ?? "Index";
            ViewBag.redirectController = redirectController ?? "Location";
            /*
            System.Diagnostics.Debug.WriteLineIf(redirectAction != null, "/Location/ReviewForm?redirectAction=" + redirectAction);
            System.Diagnostics.Debug.WriteLineIf(redirectAction == null, "/Location/ReviewForm?redirectAction=NULL");
            System.Diagnostics.Debug.WriteLineIf(redirectController != null, "/Location/ReviewForm?redirectController=" + redirectController);
            System.Diagnostics.Debug.WriteLineIf(redirectController == null, "/Location/ReviewForm?redirectController=NULL");
            */
            if (User.Identity.IsAuthenticated)
            {
                String userID = User.Identity.GetUserId();
                // get review of current location and logged in user
                Review review = db.Reviews.Where(
                    r => r.LocationID == id && 
                    r.User.Id.Equals(userID)
                    ).SingleOrDefault();

                // set viewbag values according to if voted.
                if (review != null)
                {
                    if (review.Rating) ViewBag.UpVoted = true;
                    else ViewBag.DownVoted = true;
                }
            }

            return PartialView(location);
        }

        // ************************************************************************************************************
        // LOCATION THUMBNAIL
        // ************************************************************************************************************

        // renders a square Profile Picture Thumbnail that links to that user.
        public PartialViewResult LocationThumbnail(int id, int? size)
        {
            // get user matching the username, or the current user if it is not present.
            Location location = db.Locations.Find(id);
           
            // put size in the viewbag
            if (size.HasValue) ViewBag.Size = size;
            else ViewBag.Size = 100;
            return PartialView(location);
        }

        // ************************************************************************************************************
        // SUMMARY
        // ************************************************************************************************************

        /// <summary>
        /// Renders a list display of a Location.  This is used for the new box views of the Location Index.
        /// </summary>
        /// <param name="id">The id of the location.</param>
        /// <param name="imageSize">The size of the image thumbnail to display, in pixels.</param>
        /// <returns>A partial view with displays some information about a Location.</returns>
        public PartialViewResult Summary(int? id, int? imageSize)
        {
            // find location
            Location location = db.Locations
                .Include(l => l.Reviews)
                .Where(l => l.LocationID == id)
                .SingleOrDefault();

            // error checking for null case
            if (location == null) return PartialView(location);

            // load LocationRecreations
            location.RecOptions = db.LocationRecreations
                .Include(lr => lr.Recreation)
                .Where(lr => lr.LocationID == id).ToList();

            // load Recreations
            location.Recreations = new List<Recreation>();
            foreach (LocationRecreation lr in location.RecOptions)
            {
                location.Recreations.Add(lr.Recreation);
            }
            ViewBag.Size = imageSize ?? 100;

            // done
            return PartialView(location);
        }

        public PartialViewResult JoinSummary(int? id)
        {
            // find location
            Location location = db.Locations
                .Include(l => l.Reviews)
                .Where(l => l.LocationID == id)
                .SingleOrDefault();

            // error checking for null case
            if (location == null) return PartialView(location);

            // load LocationRecreations
            location.RecOptions = db.LocationRecreations
                .Include(lr => lr.Recreation)
                .Where(lr => lr.LocationID == id).ToList();

            // load Recreations
            location.Recreations = new List<Recreation>();
            foreach (LocationRecreation lr in location.RecOptions)
            {
                location.Recreations.Add(lr.Recreation);
            }

            // done
            return PartialView(location);
        }
    }
}
