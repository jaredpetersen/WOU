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
using System.Text;


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

        /// <summary>
        /// GET: Location
        /// </summary>
        /// <returns>The Location/Index view.</returns> 
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
            Location center = Location.GetLatLongCenter(viewModel.Locations);
            System.Diagnostics.Debug.WriteLine("centerLatitude:  " + center.Latitude);
            System.Diagnostics.Debug.WriteLine("centerLongitude: " + center.Longitude);
            ViewBag.centerLatitude = center.Latitude;
            ViewBag.centerLongitude = center.Longitude;

            return View(viewModel);
        }


        // ************************************************************************************************************
        // HELPER METHODS
        // ************************************************************************************************************

        /// <summary>
        /// Helper method to format a collection of strings into a human-readable string,
        /// in an array-literal format (i.e. '{"hello", "world"}'.
        /// </summary>
        /// <param name="strings">The collection of strings to format.</param>
        /// <returns>A formatted string.</returns>
        public string GetArrayFormattedString(ICollection<string> strings)
        {
            // case 1 - null
            if (strings == null) return "null";

            int length = strings.Count();

            // case 2 - empty
            if (length == 0) return "{ }";

            // case 3 - general
            string[] array = strings.ToArray();
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            for (int i = 0; i < length; i++)
            {
                sb.Append(String.Format("\"{0}\"{1}", array[i], i + 1 < length ? ", " : ""));
            }
            sb.Append('}');
            return sb.ToString();
        }


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
            //System.Diagnostics.Debug.WriteLine(String.Format("GetPersonalRecommendations({0})", amount));
            // ensure user is logged in.
            if (!User.Identity.IsAuthenticated)
            {
                return new List<Location>();
            }

            User user = db.Users.Find(User.Identity.GetUserId());

            // get all Recreations the user partakes in.
            var userActivities = db.UserRecreations.Where(ur => ur.User.Equals(user.Id)).Select(r => r.Recreation);
             
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

            // union the locations.
            IQueryable<Location> locations = null;

            // case 1: use has bookmarks and reviews
            if (bookmarkedLocations.Count() != 0 && reviews.Count() != 0)
            {
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
                if (userActivities.Count() > 0)
                {
                    locations = db.Locations.Where(l =>
                        l.Recreations.Intersect(userActivities).Count() > 0);
                }
                else
                {
                    locations = db.Locations.Select(l => l);
                }
            }

            var states = locations.Select(u => u.StateID);

            // finally, get all locations in the states of the result that are not already in result, or have reviews.
            var result = db.Locations.Where(l =>
                   states.Contains(l.StateID)
                   && !locations.Contains(l)
                   && !reviews.Select(r => r.LocationID).Contains(l.LocationID)
               );

            return SortByRatingAndTake(result, amount);
        }

        /// <summary>
        /// Get a list of locations that are recommended by the website based on your friends like.
        /// </summary>
        /// <param name="amount">The number of results to return.</param>
        /// <returns>A list locations, sorted by rating.</returns>
        public List<Location> GetFriendRecommendations(int amount)
        {
            //System.Diagnostics.Debug.WriteLine("GetFriendRecommendations()");

            //find the current User
            User currentUser = db.Users.Find(User.Identity.GetUserId());
            //System.Diagnostics.Debug.WriteLine(String.Format("currentUser: {0}", currentUser.UserName));

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

            // get positive reviews by friends
            var positiveReviews = connectedUsers
                .Select(user => user.UserReviews)
                .SelectMany(review => review)
                .Where(review => review.Rating);

            var locations = positiveReviews
                .Select(r => r.Location)
                .Distinct();

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

        /// <summary>
        /// Helper method that returns the ReviewID if this user has made a rating for this location or -1 elsewise
        /// </summary>
        /// <param name="LocationID">The index of the Location to check.</param>
        /// <returns>the ReviewID4 if the current user has made a review; otherwise, returns -1.</returns>
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

        /// <summary>
        /// Helper method that returns a map of locationIDs to the rating this user has associated with it
        /// </summary>
        /// <param name="locations">The list of locations to get ratings for.</param>
        /// <returns>
        /// A Dictionary of Key/Value pairs that matches a Location with a current user's ratings. 
        /// (-1 if no rating, 0 if down rating, 1 if up rating). 
        /// </returns>
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

        /// <summary>
        /// POST: Location/SaveFlag
        /// </summary>
        /// <param name="flag">The string representation of a flag.</param>
        /// <param name="locationID">The index of the location for which to save the flag.</param>
        /// <returns>If successful, a redirect to Location/Details; otherwise, a redirect back to Location/Index.</returns>
        [Authorize]
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

                try
                {
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

        /// <summary>
        /// Get all the NaturalFeatures associated with a given Location.
        /// </summary>
        /// <param name="location">The Location from which to fetch the associated NaturalFeatures.</param>
        public List<NaturalFeature> getNaturalFeaturesFor(int locationID)
        {
            return db.LocationFeatures
                .Where(lf => lf.LocationID == locationID)
                .Select(lf => lf.NaturalFeature)
                .ToList();
        }

        // ************************************************************************************************************
        // DETAILS
        // ************************************************************************************************************

        /// <summary>
        /// GET: Location/Details/5
        /// </summary>
        /// <param name="id">The index of the Location to view the Details for.</param>
        /// <param name="success">An optional success message, used after Joining locations (see Location/Join).</param>
        /// <param name="mergedLocation">An optional name of the merged Location (see Location/Join).</param>
        /// <returns>
        /// The Location/Details view, showing all related information for the specified Location.
        /// </returns>
        public ActionResult Details(int? id, LocationMessageId? message, string mergedLocation)
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

            ViewBag.StatusMessage =
                message == LocationMessageId.ReviewSavedSuccess ? "Your review has been saved. Thanks!"
                : message == LocationMessageId.MergeLocationSuccess ? String.Format("{0} has been merged into {1}.", mergedLocation, location.Label)
                : "";
            
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

        // ************************************************************************************************************
        // CREATE
        // ************************************************************************************************************

        /// <summary>
        /// GET: Location/Create
        /// </summary>
        /// <returns>The Location/Create view.</returns>
        [Authorize]        
        public ActionResult Create()
        {
            //Set up LocationRecreation Options
            List<LocationRecreation> locRecList = new List<LocationRecreation>();
            foreach (var rec in db.Recreations)
            {
                LocationRecreation lr = new LocationRecreation();
                lr.RecreationID = rec.RecreationID;
                lr.RecreationLabel = rec.Label;
                locRecList.Add(lr);
            }

            CreateLocationViewModel viewModel = new CreateLocationViewModel();
            viewModel.RecOptions = locRecList;
            viewModel.SelectedFeatures = new List<string>();
            viewModel.AllNaturalFeatures = db.NaturalFeatures
                .Select(nf => nf.Name)
                .ToList();

            return View(viewModel);
        }
        
        /// <summary>
        /// POST: Location/Create
        /// </summary>
        /// <param name="model">The viewModel containing the model data associated with this Create method.</param>
        /// <returns>If successful, a redirect to Location/Index view; otherwise, a redirect to Location/Create.</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLocationViewModel model)
        {
            Location location = new Location();

            bool foundRecreation = false;
            foreach (var locRec in model.RecOptions)
            {
                if (locRec.IsChecked)
                {
                    foundRecreation = true;
                }
            }
            // if they didn't choose any recreations, give an error
            if (!foundRecreation)
            {
                ModelState.AddModelError("Recreations", "You must choose at least one recreation type.");
            }

            if (ModelState.IsValid)
            {
                // transfer info
                location.Label = model.Label;
                location.Latitude = model.Latitude;
                location.Longitude = model.Longitude;
                location.Description = model.Description;
                location.Difficulty = (Location.DifficultyRatings) model.Difficulty;
                // initialize DateTime Stamps
                location.DateCreated = DateTime.UtcNow;
                location.DateModified = location.DateCreated;

                location.Rating();
                location.UpVotes();
                location.DownVotes();

                string abbrev = Location.ReverseGeocodeState(location);
                location.State = db.States.Where(s => s.StateID.Equals(abbrev)).SingleOrDefault();
                
                // save changes
                db.Locations.Add(location);
                db.SaveChanges();//must save before we move on so that location gets an ID

                // edit Recreations
                EditRecreationsFor(location, model.RecOptions);

                // edit NaturalFeatures
                System.Diagnostics.Debug.WriteLine("SelectedFeatures = " + GetArrayFormattedString(model.SelectedFeatures));
                EditNaturalFeaturesFor(location, model.SelectedFeatures ?? new List<string>());

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("Overall", "You are missing one or more required fields.");
            CreateLocationViewModel viewModel = new CreateLocationViewModel();
            viewModel.Label = model.Label;
            viewModel.Latitude = model.Latitude;
            viewModel.Longitude = model.Longitude;
            viewModel.Description = model.Description;
            viewModel.RecOptions = model.RecOptions;
            viewModel.Difficulty = model.Difficulty;
            viewModel.SelectedFeatures = model.SelectedFeatures ?? new List<String>();
            viewModel.AllNaturalFeatures = db.NaturalFeatures
                .Select(nf => nf.Name)
                .ToList();

            return View(viewModel);
        }  

        /// <summary>
        /// This is the search algorithm used by Location/Index and Location/Browse views.
        /// </summary>
        /// <param name="query">The user's raw search query.</param>
        /// <returns>A List of locations matching the search query.</returns>
        public List<Location> SearchFor(String query)
        {
            query = query.ToLower();
            var locations = db.LocationFeatures  // locations with a matching NaturalFeature
                .Include(lf => lf.NaturalFeature)
                .Include(lf => lf.Location)
                .Where(lf => query.Length > 2    // ensure short searches are not flooded with irrelevant results
                    && lf.NaturalFeature.Name.ToLower().Contains(query)
                ).Select(lf => lf.Location)
                .Union(db.Locations.Where(l =>   // unioned with locations with partially matching name
                    l.Label.ToLower().Contains(query)
                    || l.StateID.ToLower().Equals(query) // or matching state information.
                    || l.State.Name.ToLower().Equals(query))
                ).OrderBy(l => l.Label) // ordered alphabetically
                .ToList();
                return locations;
        }

        // ************************************************************************************************************
        // EDIT
        // ************************************************************************************************************

        /// <summary>
        /// GET: Location/Edit/5
        /// </summary>
        /// <param name="id">The index of the Locatoin to edit.</param>
        /// <returns>The Location/Edit view.</returns>
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

            // *********************************************************
            // get LocationRecreations
            // *********************************************************
            var selectedRecreations = db.LocationRecreations
                .Where(lr => lr.LocationID == location.LocationID)
                .Select(lr => lr.Recreation);

            var allLocationRecreations = new List<LocationRecreation>();
            foreach (var rec in db.Recreations)
            {
                LocationRecreation lr = new LocationRecreation();
                lr.RecreationID = rec.RecreationID;
                lr.RecreationLabel = rec.Label;

                // check if selected.
                var result = selectedRecreations.SingleOrDefault(r => rec.RecreationID == r.RecreationID);
                if (result != null) lr.IsChecked = true;
                else lr.IsChecked = false;
                allLocationRecreations.Add(lr);
            }

            // *********************************************************
            // Create and populate the ViewModel.
            // *********************************************************
            EditLocationViewModel viewModel = new EditLocationViewModel(location);
            viewModel.RecOptions = allLocationRecreations;
            viewModel.SelectedFeatures = location.LocationFeatures
                .Select(lf => lf.NaturalFeature)
                .Select(nf => nf.Name)
                .ToList();
            viewModel.AllNaturalFeatures = db.NaturalFeatures
                .Select(nf => nf.Name)
                .ToList();

            return View(viewModel);
        }

        /// <summary>
        /// POST: Location/Edit/5
        /// </summary>
        /// <param name="location">The Location model containing any edits to be applied.</param>
        /// <returns>
        /// If successful, a redirect to Location/Index; otherwise, a redirect to the Location/Edit view.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LocationID,Label,Latitude,Longitude,DateCreated,Rating,Description,Difficulty,SelectedFeatures")] EditLocationViewModel viewModel)
        {
            // ensure SelectedFeatures is not null if no values passed.
            viewModel.SelectedFeatures = viewModel.SelectedFeatures ?? new List<string>();

            if (ModelState.IsValid)
            {
                Location location = db.Locations.Find(viewModel.LocationID);
                
                // update DateModified
                location.Label = viewModel.Label;
                location.Latitude = viewModel.Latitude;
                location.Longitude = viewModel.Longitude;
                location.Description = viewModel.Description;
                location.Difficulty = (Location.DifficultyRatings) viewModel.Difficulty;
                location.DateModified = DateTime.UtcNow;
                string abbrev = Location.ReverseGeocodeState(location);
                location.State = db.States.Where(s => s.StateID.Equals(abbrev)).SingleOrDefault();
                
                // save changes
                db.Entry(location).State = EntityState.Modified;
                db.SaveChanges();

                // edit Recreations
                System.Diagnostics.Debug.WriteLine("Editing Recreations does not work, will be implemented next sprint.");
                //EditRecreationsFor(location, viewModel.RecOptions);

                // edit NaturalFeatures
                System.Diagnostics.Debug.WriteLine("SelectedFeatures = " + GetArrayFormattedString(viewModel.SelectedFeatures));
                EditNaturalFeaturesFor(location, viewModel.SelectedFeatures ?? new List<string>());

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("Overall", "You are missing one or more required fields.");
            viewModel.AllNaturalFeatures = db.NaturalFeatures
                .Select(nf => nf.Name)
                .ToList();
            return View(viewModel);
        }

        // ************************************************************************************************************
        // DELETE
        // ************************************************************************************************************

        /// <summary>
        /// GET: Location/Delete/5
        /// </summary>
        /// <param name="id">The index of the Location to delete.</param>
        /// <returns>
        /// The Location/Delete view, confirming if the user is sure they want to delete the specified Location.
        /// </returns>
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

        /// <summary>
        /// POST: Location/Delete/5
        /// </summary>
        /// <param name="id">The index of the Location to delete.</param>
        /// <returns>A redirect to the Location/Index view.</returns>
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

        /// <summary>
        /// <para>GET: Location/Join</para>
        /// <para>View for Joining two locations together. Location A will be merged into Location B.</para>
        /// </summary>
        /// <param name="searchStringA">The user's search string to filter selections for Location A.</param>
        /// <param name="searchStringB">The user's search string to filter selections for Location A.</param>
        /// <param name="LocationA">The selection options for Location A.</param>
        /// <param name="LocationB">The selection options for Location B.</param>
        /// <param name="pageA">The page number for Location A search listings.</param>
        /// <param name="pageB">The page number for Location B search listings.</param>
        /// <returns>
        /// If successful, a redirect to Location/Details for Location B (the saved location),
        /// otherwise a redirect to Location/Details.
        /// </returns>
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

        /// <summary>
        /// Releases resources not needed by the applciation.
        /// </summary>
        /// <param name="disposing">To dispose, or not to dispose, THAT is the question.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        // ************************************************************************************************************
        // BROWSE
        // ************************************************************************************************************

        /// <summary>
        /// <para>GET: Location/Browse</para>
        /// <para>Displays a paged list view of all Locations on the site, filtered by some search criteria.</para>
        /// </summary>
        /// <param name="recreationID">An optional parameter to filter the Locations by their Recreation types.</param>
        /// <param name="sortOrder">The current sorting order.  (TODO: not accessible by UI)</param>
        /// <param name="currentFilter">The current sorting filter.  (TODO: not accessible by UI)</param>
        /// <param name="query">The search string the user has typed into the search field.  If empty, all Locations are returned.</param>
        /// <param name="page">The current page of results that is being accessed.</param>
        /// <returns>The Location/Browse view, which shows all Locations with paging.</returns>
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
                    ViewBag.Recreation = lr;
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
                ViewBag.SearchString = query;

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
            Location center = Location.GetLatLongCenter(viewModel.Locations.ToList());
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

        /// <summary>
        /// Renders a partial view of a Review Form, with the up and down votes.
        /// </summary>
        /// <param name="id">The id of the Location that the Review Form is associated with.</param>
        /// <param name="redirectAction">The name of the Action method to redirect to.</param>
        /// <param name="redirectController">The name of the Controller to redirect to.</param>
        /// <returns>
        /// A partial view containing the thumbs up and down buttons, which submit a vote on a Location 
        /// for the currently logged in user.
        /// </returns>
        public PartialViewResult ReviewForm(int id, string redirectAction, string redirectController)
        {
            Location location = db.Locations.Find(id);
            ViewBag.redirectAction = redirectAction ?? "Index";
            ViewBag.redirectController = redirectController ?? "Location";
            //System.Diagnostics.Debug.WriteLine(String.Format("LocationController.ReviewForm(LocationID: {0}) ViewBag.redirectAction:     {1}", id, redirectAction ?? "Index"));
            //System.Diagnostics.Debug.WriteLine(String.Format("LocationController.ReviewForm(LocationID: {0}) ViewBag.redirectController: {1}", id, redirectController ?? "Location"));

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

        /// <summary>
        /// renders a square Profile Picture Thumbnail that links to that user.
        /// </summary>
        /// <param name="id">The index of the Location from which to fetch the associated Image.</param>
        /// <param name="size">Optinal parameter for the height and width dimensions of the thumbnail, in pixels.  (defaults to 100)</param>
        /// <returns>A partial view of a thumbnail image of the specified Location.</returns>
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
        public PartialViewResult Summary(int? id, int? imageSize, string redirectAction, string redirectController)
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
            ViewBag.redirectAction = redirectAction ?? "Index";
            ViewBag.redirectController = redirectController ?? "Location";
            //System.Diagnostics.Debug.WriteLine(String.Format("LocationController.Summary(LocationID: {0}) ViewBag.redirectAction:     {1}", id ?? -1, redirectAction ?? "Index"));
            //System.Diagnostics.Debug.WriteLine(String.Format("LocationController.Summary(LocationID: {0}) ViewBag.redirectController: {1}", id ?? -1, redirectController ?? "Location"));
            // done
            return PartialView(location);
        }

        /// <summary>
        /// Renders a partial view of a Location Summary, to be used in Location/Join.
        /// </summary>
        /// <param name="id">The index of the Location to render the view for.</param>
        /// <returns>A partial view of a Location.</returns>
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

        // ************************************************************************************************************
        // MEDIA
        // ************************************************************************************************************

        /// <summary>
        /// <para>GET: MediaViewModel</para>
        /// <para>Shows a grid display of images and videos for the given location.</para>
        /// </summary>
        /// <param name="locationID">The index of the Location for which to display the associated Media.</param>
        /// <returns> The Location/Media view.</returns>
        public ActionResult Media(int locationID)
        {
            Location location = db.Locations.Find(locationID);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        public enum LocationMessageId
        {
            ReviewSavedSuccess,
            MergeLocationSuccess,
            Error
        }

        // ************************************************************************************************************
        // NATURAL FEATURES
        // ************************************************************************************************************

        /// <summary>
        /// Edit the NaturalFeatures for a single Location.
        /// </summary>
        /// <param name="locationID">The index of the Location to edit.</param>
        /// <returns>the Location/EditFor view.</returns>
        [Authorize]
        public ActionResult EditNaturalFeatures(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Diagnostics.Debug.WriteLine("get EditNaturalFeatures(" + id + ")");

            Location location = db.Locations.Find(id);

            if (location == null)
            {
                return HttpNotFound();
            }
            EditNaturalFeaturesViewModel viewModel = new EditNaturalFeaturesViewModel();
            viewModel.Location = location;
            viewModel.LocationID = location.LocationID;
            viewModel.LocationLabel = location.Label;
            viewModel.SelectedFeatures = location.LocationFeatures
                .Select(lf => lf.NaturalFeature)
                .Select(nf => nf.Name)
                .ToList();
            viewModel.AllNaturalFeatures = db.NaturalFeatures
                .Select(nf => nf.Name)
                .ToList();
            return View(viewModel);
        }

        /// <summary>
        /// HttpPost NaturalFeatures/EditNaturalFeatures/5
        /// </summary>
        /// <param name="viewModel">The model to validate.</param>
        /// <returns>A redirect to location details if successful, else back to EditNaturalFeatures.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult EditNaturalFeatures([Bind(Include = "LocationID, LocationLabel, AllNaturalFeatures, SelectedFeatures")] EditNaturalFeaturesViewModel viewModel)
        {
            System.Diagnostics.Debug.WriteLine("post Location/EditNaturalFeatures(viewModel)");
            System.Diagnostics.Debug.WriteLine("SelectedFeatures = " + GetArrayFormattedString(viewModel.SelectedFeatures));

            // check for no SelectedFeatures.
            if (viewModel.SelectedFeatures == null) viewModel.SelectedFeatures = new List<string>();
            Location location = db.Locations.Find(viewModel.LocationID);
            // save all changes to the db.
            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("ModelState is valid.");
                EditNaturalFeaturesFor(location, viewModel.SelectedFeatures);
                return RedirectToAction("Details", "Location", new { id = viewModel.LocationID });
            }

            // **********************************************************
            // a model state error occurred, redirect back to edit view.
            // **********************************************************                
            System.Diagnostics.Debug.WriteLine("ModelState is INVALID.");

            // reload AllNaturalFeatures from the db.
            viewModel.Location = location;
            viewModel.AllNaturalFeatures = db.NaturalFeatures
                .Select(nf => nf.Name)
                .ToList();

            return View(viewModel);
        }
        /// <summary>
        /// Helper method to edit the NaturalFeatures of the specified Location by modifying
        /// the entries of the bridge entity LocationFeatures.
        /// </summary>
        ///<param name="location">The Location to modify the associated NaturalFeatures of.</param>
        /// <param name="SelectedFeatures">
        /// <para>        
        /// The names of which NaturalFeatures to associate.
        /// </para><para>
        /// Any names which don't match an existing NaturalFeature will be
        /// added as a new NaturalFeature.  
        /// </para><para>
        /// Any NaturalFeatures which are associated with the Location
        /// but are are not in this list will be removed from the Location.
        /// </para><para>
        /// Any NaturalFeatures which not are associated with the Location
        /// but are in this list will be added to the Location.
        /// </para><para>
        /// All other NaturalFeature associations will remain untouched. 
        /// </para>
        /// </param>
        public void EditNaturalFeaturesFor(Location location, ICollection<string> SelectedFeatures)
        {
            int locationID = location.LocationID;

            System.Diagnostics.Debug.WriteLine("ModelState is valid.");

            // 1) - Set aside all previous location features for comparison.
            var associated = db.LocationFeatures
                .Include(lf => lf.NaturalFeature)
                .Where(lf => lf.LocationID == locationID)
                .ToList();

            // ****************************************************
            // 2) - add any new NaturalFeatures.
            // ****************************************************
            int newNfCount = 0;
            foreach (string name in SelectedFeatures)
            {
                var match = db.NaturalFeatures.SingleOrDefault(nf => nf.Name.Equals(name));
                if (match == null)
                {
                    db.NaturalFeatures.Add(new NaturalFeature(name));
                    newNfCount++;
                }
            }
            db.SaveChanges();
            System.Diagnostics.Debug.WriteLine(String.Format("successfully added {0} new NaturalFeatures.", newNfCount));


            // get all features to iterate over.
            var selected = db.NaturalFeatures
                .Where(nf => SelectedFeatures.Contains(nf.Name))
                .ToList();


            // ****************************************************
            // 3) - remove any LocationFeatures that are currently 
            //      associated, but aren't selected.
            // ****************************************************
            int removedLfcount = 0;
            foreach (LocationFeature item in associated)
            {
                var result = selected.SingleOrDefault(nf => nf.ID == item.NaturalFeatureID);
                if (result == null)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("'{0}' is associated with '{1}', but needs to be removed.", item.NaturalFeature.Name, location.Label));
                    db.LocationFeatures.Remove(item);
                    removedLfcount++;
                }
            }
            db.SaveChanges();
            System.Diagnostics.Debug.WriteLine(String.Format("successfully removed {0} LocationFeatures from {1}.", removedLfcount, location.Label));


            // ****************************************************
            // 4) - add any LocationFeatures that are currently 
            //      selected, but aren't associated.
            // ****************************************************
            int addedLfCount = 0;
            foreach (NaturalFeature item in selected)
            {
                var result = associated.SingleOrDefault(lf => lf.NaturalFeatureID == item.ID);
                if (result == null)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("'{0}' is not associated with '{1}', but needs to be added.", item.Name, location.Label));
                    db.LocationFeatures.Add(new LocationFeature(locationID, item.ID));
                    addedLfCount++;
                }
            }
            db.SaveChanges();
            System.Diagnostics.Debug.WriteLine(String.Format("successfully added {0} new LocationFeatures for {1}.", addedLfCount, location.Label));
        }

        // ************************************************************************************************************
        // RECREATIONS
        // ************************************************************************************************************


        /// <summary>
        /// Edit the NaturalFeatures for a single Location.
        /// </summary>
        /// <param name="locationID">The index of the Location to edit.</param>
        /// <returns>the Location/EditFor view.</returns>
        [Authorize]
        public ActionResult EditRecreations(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Diagnostics.Debug.WriteLine("get EditRecreations(" + id + ")");

            Location location = db.Locations.Find(id);

            if (location == null)
            {
                return HttpNotFound();
            }
            /*
            EditNaturalFeaturesViewModel viewModel = new EditNaturalFeaturesViewModel();
            viewModel.LocationID = location.LocationID;
            viewModel.LocationLabel = location.Label;
            viewModel.SelectedFeatures = location.LocationFeatures
                .Select(lf => lf.NaturalFeature)
                .Select(nf => nf.Name)
                .ToList();
            viewModel.AllNaturalFeatures = db.NaturalFeatures
                .Select(nf => nf.Name)
                .ToList();
            return View(viewModel);
             */
            return View();
        }

        /// <summary>
        /// Helper method to edit the Recreations for a given Location.
        /// </summary>
        /// <param name="location">The location to edit the Recreations for.</param>
        public void EditRecreationsFor(Location location, List<LocationRecreation> recOptions)
        {
            List<LocationRecreation> locrecList = new List<LocationRecreation>();
            foreach (var locRec in recOptions)
            {
                if (locRec.IsChecked)
                {
                    LocationRecreation lr = locRec;

                    int latestID = db.Locations.Where(l => l.Label == location.Label).ToList().First().LocationID;
                    Location tempL = db.Locations.Find(latestID); //get the last Location entered
                    lr.LocationID = tempL.LocationID;
                    db.LocationRecreations.Add(lr);
                }
            }
            db.SaveChanges();
        }
    }
}
