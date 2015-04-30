﻿using System;
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
using System.Threading.Tasks;
using TentsNTrails.Azure;

namespace TentsNTrails.Controllers
{
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<User> manager;
        private PhotoService photoService = new PhotoService();
            
        public ProfileController () 
        {
            manager = new UserManager<User>(new UserStore<User>(db));
        }

        // GET: Profile
        public ActionResult Index(String username, ProfileMessageId? message)
        {
            System.Diagnostics.Trace.WriteLine("Profile.Index");
            System.Diagnostics.Debug.WriteLine("Profile.Index");
            User profileUser = manager.FindById(User.Identity.GetUserId());
            ViewBag.IsConnected = false;
            ViewBag.HasConnectionRequest = false;
            ViewBag.HasSentRequest = false;
            ViewBag.HasRequests = false;

            if (username != null) //Traveling to another user's profile
            {
                profileUser = db.Users.Where(user => user.UserName == username).First();
                ViewBag.IsPrivate = profileUser.Private; //Tells the view what to display based on user settings
                ViewBag.IsOnOwnProfile = false;

                // Check if the two users are already connected some way (if this is a logged in user)
                User currentUser = manager.FindById(User.Identity.GetUserId());
                if (currentUser != null)
                {
                    // Check for connection
                    var connection = db.Connections.Where(c => (c.User1.UserName == currentUser.UserName && c.User2.UserName == profileUser.UserName) ||
                        (c.User2.UserName == currentUser.UserName && c.User1.UserName == profileUser.UserName));
                    if (connection.Count() > 0)
                    {
                        ViewBag.IsConnected = true;
                    }

                    // Check if the person whose profile we're looking at has sent a ConnectionRequest
                    var requestTo = db.ConnectionRequests.Where(c => (c.RequestedUser.UserName == currentUser.UserName && c.Sender.UserName == profileUser.UserName));
                    if (requestTo.Count() > 0)
                    {
                        ViewBag.HasConnectionRequest = true;
                    }
                    else
                    {
                        var requestFrom = db.ConnectionRequests.Where(c => (c.Sender.UserName == currentUser.UserName && c.RequestedUser.UserName == profileUser.UserName));
                        if (requestFrom.Count() > 0)
                        {
                            ViewBag.HasSentRequest = true;
                        }
                    }
                }
            }
            else //Traveling to personal profile page
            {
                ViewBag.IsOnOwnProfile = true;
                ViewBag.IsPrivate = false;  //A user would never be private to them self.

                var requests = db.ConnectionRequests.Where(u => u.RequestedUser.UserName == profileUser.UserName);
                if(requests.Count() > 0)
                {
                    ViewBag.HasRequests = true;
                }
            }

            //add user activities
            List<UserRecreation> ur = db.UserRecreations.Where(u => u.User == profileUser.Id).ToList();
            profileUser.UserActivities = ur;

            //add user reviews
            List<Review> uRv = db.Reviews.Where(r => r.User.Id == profileUser.Id).ToList();
            profileUser.UserReviews = uRv;

            //add user bookmarks
            List<LocationFlag> locationFlags = db.LocationFlags.Include(l => l.Location).Where(f => f.User.Id == profileUser.Id).ToList();
            if (locationFlags.Count > 0)
            {
                ViewBag.HasSaveLocations = true;
                // Fill the three lists with their respective LocationFlags
                profileUser.BeenThereLocations = locationFlags.Where(l => l.Flag == Flag.HaveBeen).ToList();
                profileUser.WantToGoLocations = locationFlags.Where(l => l.Flag == Flag.WantToGo).ToList();
                profileUser.GoAgainLocations = locationFlags.Where(l => l.Flag == Flag.GoAgain).ToList();
                ViewBag.centerLatitude = 39.8282;
                ViewBag.centerLongitude = -98.5795;
            }
            else
            {
                ViewBag.HasSaveLocations = false;
            }

            //add user's location images
            List<LocationImage> images = db.LocationImages.Where(i => i.User.Id == profileUser.Id).ToList();
            List<LocationImage> smallImage = new List<LocationImage>();
            int count = 0;
            while (smallImage.Count < 5 && count < images.Count)
            {
                smallImage.Add(images.ElementAt(count));
                count++;
            }
            ViewBag.ImagesCount = images.Count();
            profileUser.UserLocationImages = smallImage;

            //add user's location videos
            List<Video> videos = db.Videos.Where(i => i.User.Id == profileUser.Id).ToList();
            List<Video> smallVideo = new List<Video>();
            int countVid = 0;
            while (smallVideo.Count < 4 && countVid < videos.Count)
            {
                smallVideo.Add(videos.ElementAt(countVid));
                countVid++;
            }
            ViewBag.VideosCount = videos.Count();
            profileUser.UserLocationVideos = smallVideo;


            ViewBag.SuccessMessage =
                message == ProfileMessageId.ConnectionSuccess ? "Congratulations! You are now connected to " + profileUser.UserName + "."
                : message == ProfileMessageId.RequestSuccess ? "Congratulations! You have sent a connection request to " + profileUser.UserName + "."
                : message == ProfileMessageId.ConnectionFailure ? "Sorry, we couldn't make that happen."
                : message == ProfileMessageId.DenySuccess ? "You have successfully denied being connected to " + profileUser.UserName + "."
                : "";

            return View(profileUser);
        }

        // GET: Profile/SeeMoreImages
        public ActionResult SeeMoreImages(string username)
        {
            User profileUser = manager.FindById(User.Identity.GetUserId());

            if (username != null) //Traveling to another user's profile
            {
                profileUser = db.Users.Where(user => user.UserName == username).First();
            }
            else //Traveling to personal profile page
            {
               
            }

            //add user's location images
            List<LocationImage> images = db.LocationImages.Where(i => i.User.Id == profileUser.Id).ToList();
            profileUser.UserLocationImages = images;

            return View(profileUser);
        }

        // GET: Profile/SeeMoreVideos
        public ActionResult SeeMoreVideos(string username)
        {
            User profileUser = manager.FindById(User.Identity.GetUserId());

            if (username != null) //Traveling to another user's profile
            {
                profileUser = db.Users.Where(user => user.UserName == username).First();
            }
            else //Traveling to personal profile page
            {

            }

            //add user's location images
            List<Video> videos = db.Videos.Where(i => i.User.Id == profileUser.Id).ToList();
            ViewBag.VideosCount = videos.Count();
            profileUser.UserLocationVideos = videos;

            return View(profileUser);
        }

        // GET: Profile/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EnrollmentDate,FirstName,LastName,Private,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,About")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Profile/RequestConnection?Username=username
        // This method is only called when a visitor is being directed back to the Connect page from the Log In page, 
        // in which case they should just be taken to the profile page again.
        public ActionResult RequestConnection(string username, int? dummy) // this dummy variable is just so the method headers are not the same, even though they need to be
        {
            return RedirectToAction("Index", new { username = username });
        }

        // POST: Profile/RequestConnection?Username=username
        // Sends a connection request to the other user.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestConnection(string username)
        {
            ConnectionRequest conn = new ConnectionRequest();

            if (ModelState.IsValid)
            {
                if (username == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Find the user IDs for the users that were passed in
                User thisUser = manager.FindById(User.Identity.GetUserId());
                User otherUser = db.Users.Where(u => u.UserName.Equals(username)).First();

                // A user cannot connect with themself
                if (thisUser.Equals(otherUser))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    // Don't connect users that have already been connected or who have already have a pending request
                    var connection = db.Connections.Where(c => (c.User1.UserName == thisUser.UserName && c.User2.UserName == otherUser.UserName) ||
                        (c.User2.UserName == thisUser.UserName && c.User1.UserName == otherUser.UserName));
                    var request = db.ConnectionRequests.Where(c => (c.Sender.UserName == thisUser.UserName && c.RequestedUser.UserName == otherUser.UserName) ||
                        (c.RequestedUser.UserName == thisUser.UserName && c.Sender.UserName == otherUser.UserName));
                    if (connection.Count() > 0 || request.Count() > 0)
                    {
                        return RedirectToAction("Index", new { username = username, Message = ProfileMessageId.ConnectionFailure });
                    }
                }
                conn.Sender = thisUser;
                conn.RequestedUser = otherUser;

                // save changes
                db.ConnectionRequests.Add(conn);
                db.SaveChanges();

                return RedirectToAction("Index", new { username = username, Message = ProfileMessageId.RequestSuccess });
            }

            return RedirectToAction("Index", new { username = username, Message = ProfileMessageId.ConnectionFailure });
        }

        // GET: Profile/DenyConnect?Username=username
        // This method is only called when a visitor is being directed back to the Connect page from the Log In page, 
        // in which case they should just be taken to the profile page again.
        //public ActionResult DenyConnect(string username, int? dummy) // this dummy variable is just so the method headers are not the same, even though they need to be
        //{
        //    return RedirectToAction("Index", new { username = username });
        //}

        // GET: Profile/DenyConnect?Username=username
        // The connection request is deleted.
        [Authorize]
        //[ValidateAntiForgeryToken]
        public ActionResult DenyConnect(string username)
        {
            ConnectionRequest conn = new ConnectionRequest();

            if (ModelState.IsValid)
            {
                if (username == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Find the user IDs for the users that were passed in
                User thisUser = manager.FindById(User.Identity.GetUserId());
                User otherUser = db.Users.Where(u => u.UserName.Equals(username)).First();

                // A user cannot connect with themself
                if (thisUser.Equals(otherUser))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                
                // Find the request to delete: the otherUser is the sender of the request
                var request = db.ConnectionRequests.Where(c => (c.Sender.UserName == otherUser.UserName && c.RequestedUser.UserName == thisUser.UserName));
                if (request.Count() == 0)
                {
                    return RedirectToAction("Index", new { username = username, Message = ProfileMessageId.ConnectionFailure });
                }

                // Delete the connection request
                var reqToDelete = request.First();
                db.ConnectionRequests.Remove(reqToDelete);
                db.SaveChanges();

                return RedirectToAction("Index", new { username = username, Message = ProfileMessageId.DenySuccess });
            }

            return RedirectToAction("Index", new { username = username, Message = ProfileMessageId.ConnectionFailure });
        }

        // GET: Profile/ConfirmConnect?Username=username
        // Connects the two users and deletes the request from the request table.
        [Authorize]
        public ActionResult ConfirmConnect(string username)
        {
            Connection conn = new Connection();

            if (ModelState.IsValid)
            {
                if (username == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Find the user IDs for the users that were passed in
                User thisUser = manager.FindById(User.Identity.GetUserId());
                User otherUser = db.Users.Where(u => u.UserName.Equals(username)).First();

                // A user cannot connect with themself
                if (thisUser.Equals(otherUser))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                
                // Make sure there was a request originally and that they aren't already connected
                var request = db.ConnectionRequests.Where(c => (c.RequestedUser.UserName == thisUser.UserName && c.Sender.UserName == otherUser.UserName));
                var connection = db.Connections.Where(c => (c.User1.UserName == thisUser.UserName && c.User2.UserName == otherUser.UserName) ||
                        (c.User2.UserName == thisUser.UserName && c.User1.UserName == otherUser.UserName));
                if (request.Count() == 0 || connection.Count() > 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                conn.User1 = thisUser;
                conn.User2 = otherUser;

                // Add this connection and delete the connection request 
                db.Connections.Add(conn);
                var reqToDelete = request.First();
                db.ConnectionRequests.Remove(reqToDelete);
                db.SaveChanges();

                return RedirectToAction("Index", new { username = username, Message = ProfileMessageId.ConnectionSuccess });
            }

            return RedirectToAction("Index", new { username = username, Message = ProfileMessageId.ConnectionFailure });
        }

        // GET: Profile/RequestList
        [Authorize]
        public ActionResult RequestList()
        {
            string thisUser = manager.FindById(User.Identity.GetUserId()).UserName;
            ICollection<ConnectionRequest> requests = db.ConnectionRequests.Where(c => c.RequestedUser.UserName.Equals(thisUser)).ToList();

            RequestListViewModel viewModel = new RequestListViewModel();
            viewModel.Requests = requests;
            viewModel.RowCount = requests.Count();

            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET Profile/UploadProfilePicture
        [Authorize]
        public ActionResult UploadProfilePicture()
        {
            return View(new ProfilePictureViewModel());
        }

        // POST: Profile/UploadProfilePicture
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadProfilePicture(ProfilePictureViewModel viewModel)
        {
            System.Diagnostics.Debug.WriteLine("----------------------------------------");
            System.Diagnostics.Debug.WriteLine("ProfileController.UploadProfilePicture()");

            // List of allowed image types (for hosting on web)
            var validImageTypes = new string[]
            {
                "image/gif",
                "image/jpeg",
                "image/pjpeg", // needed for compatability with some older jpegs
                "image/png"
            };

            // 1. check if file is null, or of zero length
            if (viewModel.ImageUpload == null || viewModel.ImageUpload.ContentLength == 0)
            {
                System.Diagnostics.Trace.WriteLine("Image is null");
                ModelState.AddModelError("ImageUpload", "This field is required.");
            }

            // 2. check if file is null, or of zero length
            else if (viewModel.ImageUpload == null || viewModel.ImageUpload.ContentLength == 0)
            {
                System.Diagnostics.Trace.WriteLine("Uploaded file is empty");
                ModelState.AddModelError("ImageUpload", "Image file cannot be empty.");
            }

            // 3. check that image is of a valid filetype
            else if (!validImageTypes.Contains(viewModel.ImageUpload.ContentType))
            {
                System.Diagnostics.Trace.WriteLine("Invalid Image Type");
                ModelState.AddModelError("ImageUpload", "Please choose either a GIF, JPG or PNG image.");
            }


            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("ModelState is valid!");
                User user = manager.FindById(User.Identity.GetUserId());
                System.Diagnostics.Debug.WriteLine("Loaded User: " + user.UserName);

                /*
                // add profile picture to db.
                ProfilePicture picture = new ProfilePicture();
                picture.User = user;
                picture.Title = "Profile Picture";
                picture.AltText = "Image of " + picture.User.UserName;
                picture.DateCreated = picture.DateModified = picture.DateTaken = DateTime.UtcNow;
                picture.IsSelected = true;
                //picture.ImageUrl = await photoService.UploadPhotoAsync(viewModel.ImageUpload);
                picture.ImageUrl = await photoService.UploadProfilePictureAsync(viewModel.ImageUpload, user.UserName);

                System.Diagnostics.Debug.WriteLine("----------------------------------------");
                System.Diagnostics.Debug.WriteLine("Uploading new ProfilePicture.");
                System.Diagnostics.Debug.WriteLine("picture.User:       " + picture.User.Id);
                System.Diagnostics.Debug.WriteLine("picture.Title:      " + picture.Title);
                System.Diagnostics.Debug.WriteLine("picture.AltText:    " + picture.AltText);
                System.Diagnostics.Debug.WriteLine("picture.IsSelected: " + picture.IsSelected);
                System.Diagnostics.Debug.WriteLine("picture.ImageUrl:   " + picture.ImageUrl);
                db.ProfilePictures.Add(picture);
                await db.SaveChangesAsync();
                 * */
                
                // update user's profile picture reference
                //user.ProfilePicture = db.ProfilePictures.Where(i => i.ImageUrl.Equals(picture.ImageUrl)).Single();
               //ystem.Diagnostics.Debug.WriteLine("user.ProfilePicture.ImageUrl: " + user.ProfilePicture.ImageUrl);
                user.ProfilePictureUrl = await photoService.UploadProfilePictureAsync(viewModel.ImageUpload, user);
                

                var result = await manager.UpdateAsync(user);
                System.Diagnostics.Debug.WriteLine("result.Succeeded: " + result.Succeeded);

                return RedirectToAction("Index");
            }

            return View(viewModel);
        }
        
    }

    

    public enum ProfileMessageId
    {
        ConnectionSuccess,
        ConnectionFailure,
        RequestSuccess,
        DenySuccess
    }
}
