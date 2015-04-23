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


        // GET: Location
        public ActionResult Index(int? recreationID, string sortOrder, string currentFilter, string searchString, int? page)
        {
            // *************************************************
            // sorting by rating functionality
            // *************************************************

            // calculate lat/long centerpoint for map
            double centerLatitude = 0;
            double centerLongitude = 0;


            // ViewModel
            LocationViewModel viewModel = new LocationViewModel();
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
                foreach(LocationRecreation lr in locationRecreations)
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


            // *************************************************
            // calculate center of map display
            // *************************************************            
            //safety check for if no coordinates (default to Center of USA)
            int count = locations.Count();

            // TODO: center calculation is wrong.  http://www.geomidpoint.com/calculation.html
            /*
            if (count == 0)
            {
                centerLatitude = 39.8282;
                centerLongitude = -98.5795;
            }
            // find center of coordinates
            
            else
            {
                // count them up
                foreach (var location in db.Locations)
                {
                    centerLatitude += location.Latitude;
                    centerLongitude += location.Longitude;
                }

                // average them
                centerLatitude /= count;
                centerLongitude /= count;
            }
            
            //set values to viewbag
            ViewBag.centerLatitude = centerLatitude;
            ViewBag.centerLongitude = centerLongitude;

            */

            ViewBag.centerLatitude = 39.8282;
            ViewBag.centerLongitude = -98.5795;
            // *************************************************
            // sort by name, rating, and difficulty
            // *************************************************
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.RatingSortParm = sortOrder == "Rating" ? "rating_desc" : "Rating";
            ViewBag.DifficultySortParm = sortOrder == "difficulty_desc" ? "Difficulty" : "difficulty_desc";
            
            if (!String.IsNullOrEmpty(searchString))
            {
                page = 1;
                locations = locations.Where(l => l.Label.ToLower().Contains(searchString.ToLower()) || l.State == searchString.ToUpper()).ToList();
                // If the search returns only one result, just go to the details page automatically
                if (locations.Count == 1)
                {
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

            //TOP RATED LOCATIONS LIST
            int topRatingCount = 5;
            // Divide the total number of ratings by the number of locations to get
            // We decided that the top rated locations have to have at least half as many votes as average
            int avgRatingsPerLocation = -1;
            if (locations.Count() != 0)
            {
                avgRatingsPerLocation = db.Reviews.Count() / locations.Count();
            }
            else
            {
                avgRatingsPerLocation = 1;
            }
            int minRatings = (int) (.5 * avgRatingsPerLocation);
            foreach (var l in locations)
            {

            }
            viewModel.TopRatedLocations = locations.Where(l => (l.UpVotes() + l.DownVotes()) > minRatings)
                .OrderByDescending(l => l.Rating()).Take(topRatingCount).ToList();

            // PAGING
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            viewModel.Locations = locations.ToPagedList(pageNumber, pageSize);

            //Find if current user is an admin
            ViewBag.IsAdmin = User.IsInRole("Admin");

            // FOR RATING THUMB COLORS
            viewModel.Ratings = getRatingsForLocations(locations);

            return View(viewModel);
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
        public ActionResult Details(int? id, bool? success)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (success == true)
            {
                ViewBag.SuccessMessage = "Location A has been merged into Location B.";
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
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
                location.RetrieveFormatedAddress();

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
                        return RedirectToAction("Details", new { id = locationB, success = true });
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

                        LocationViewModel viewModel = new LocationViewModel();

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

                    LocationViewModel viewModel = new LocationViewModel();

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

                LocationViewModel viewModel = new LocationViewModel();

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
    }
}
