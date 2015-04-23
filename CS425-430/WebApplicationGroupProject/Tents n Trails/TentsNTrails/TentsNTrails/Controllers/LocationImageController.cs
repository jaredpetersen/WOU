using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TentsNTrails.Models;

namespace TentsNTrails.Controllers
{

    /// <summary>
    /// Handles all Images for Locations.
    /// </summary>
    public class LocationImageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<User> manager;

        public LocationImageController()
        {
            db = new ApplicationDbContext();
            manager = new UserManager<User>(new UserStore<User>(db));
        }

        // **************************************
        // EDITED (Non-Standard Scaffolded Code)
        // **************************************
        // GET: LocationImage
        //
        // Shows a grid display of images for the given location.  
        // If locationID has a value, it is limited to that single Location.
        public ActionResult Index(int? locationID)
        {
            IEnumerable<LocationImage> locationImages;
            string cancelAction;
            // handle case for a single location selectedd
            if (locationID.HasValue)
            {
                locationImages = db.LocationImages.Where(i => i.LocationID == locationID);
                ViewBag.Location = db.Locations.Where(i => i.LocationID == locationID).SingleOrDefault();
                cancelAction = "Details/" + locationID;
            }

            // otherwise, show all images
            else
            {
                locationImages = db.LocationImages.ToList();
                cancelAction = "Index";
            }

            ViewBag.CancelAction = cancelAction;
            return View(locationImages);
        }

        // **************************************
        // EDITED (Non-Standard Scaffolded Code)
        // **************************************
        // GET: LocationImage/Details/#
        //

        // fromLocationImageIndex
        // fromLocationDetails


        // Displays an image details vage that displays the image with some metadata.
        // The back link in the view will dynamically redirect based on fromLocationDetails' value.
        public ActionResult Details(int? id, bool fromLocationImageIndex = false, bool fromLocationDetails = false)
        {
            // ensure id is passed
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // check if Image exists
            LocationImage locationImage = db.LocationImages.Find(id);
            if (locationImage == null)
            {
                return HttpNotFound();
            }
 
            // cancel action is used to redirect to the previous page.
            else
            {
                ViewBag.fromLocationImageIndex = fromLocationImageIndex;
                ViewBag.fromLocationDetails = fromLocationDetails;
                ViewBag.IsAdmin = User.IsInRole("Admin");
                return View(locationImage);
            }
        }


        // **************************************
        // EDITED (Non-Standard Scaffolded Code)
        // **************************************
        // GET: LocationImage/Create
        //
        // Shows a new Image Upload Page.  It takes an optional locationID field, which if specified limits the selectlist to a single
        // value, and the subsequent view will redirect to that Location's Details page.  Otherwise, the user can select any Location to
        // associate with the Image and they will be redirected to the Location index page.
        [Authorize]
        public ActionResult CreateByUrl(int locationID)
        {
            // updated to include notes from Monday's meeting           
            ViewBag.PlaceholderUrl = "~/Content/ImagePreview.png";
            ViewBag.LocationLabel = db.Locations.Find(locationID).Label;
            LocationImageUrlViewModel viewModel = new LocationImageUrlViewModel();
            viewModel.LocationID = locationID;

            return View(viewModel);
        }

        // **************************************
        // EDITED (Non-Standard Scaffolded Code)
        // **************************************
        // POST: Recreation/Edit/5
        //
        // Creates a new Image from the data input from the LocationImageViewModel.
        //
        // This is done in two steps: 
        //     1.) Saving the image to the website folder structure,
        //     2.) Saving the LocationImage model with a string url to that image in the database.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateByUrl(LocationImageUrlViewModel model)
        {

            // TODO finish

            // List of allowed image types (for hosting on web)
            var validImageTypes = new string[]
            {
                ".gif",
                ".jpg",
                ".jpeg",
                ".pjpeg", // needed for compatability with some older jpegs
                ".png"
            };


            // 1. check if a valid image was uploaded
            if (model.ImageUrl == null || model.ImageUrl.Length == 0)
            {
                Console.WriteLine("Image is null or empty");
                ModelState.AddModelError("ImageUpload", "This field is required");
            }

            // 2. check that image is of a valid filetype
            else
            {
                bool hasValidExtension = false;
                foreach (String s in validImageTypes)
                {
                    if (model.ImageUrl.EndsWith(s))
                    {
                        hasValidExtension = true;
                        break;
                    }
                }

                if (!hasValidExtension)
                {
                    Console.WriteLine("Invalid Image Type");
                    ModelState.AddModelError("ImageUpload", "Please choose either a GIF, JPG or PNG image.");
                }
            }

            // 3. create the Image entry to save to the database
            if (ModelState.IsValid)
            {
                Console.WriteLine("Model state is valid");
                // initialize image model to store in database
                var locationImage = new LocationImage
                {
                    LocationID = model.LocationID,
                    Title = model.Title,
                    AltText = "Image from " + db.Locations.Find(model.LocationID).Label,
                    DateTaken = model.DateTaken,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    ImageUrl = model.ImageUrl,
                    User = manager.FindById(User.Identity.GetUserId())
                };

                // save LocationImage model in database
                db.LocationImages.Add(locationImage);
                db.SaveChanges();
                return RedirectToAction("Media", "Location", new { locationID = model.LocationID });
            }

            // otherwise, go back to create view.
            Console.WriteLine("Model state is invalid.");

            ViewBag.LocationID = model.LocationID;
            ViewBag.LocationLabel = db.Locations.Find(model.LocationID).Label;
            ViewBag.PlaceholderUrl = "~/Content/ImagePreview.png";
            ViewBag.LocationCount = db.Locations.Count();
            return View(model);
        }


        // *******************************************************************************************************************
        // CREATE (via File Uploader)
        // *******************************************************************************************************************
        // Does not work on azure.
        // These are being saved for now, but temporarily replaced with a new set of methods.


        // **************************************
        // EDITED (Non-Standard Scaffolded Code)
        // **************************************
        // GET: LocationImage/Create
        //
        // Shows a new Image Upload Page.  It takes an optional locationID field, which if specified limits the selectlist to a single
        // value, and the subsequent view will redirect to that Location's Details page.  Otherwise, the user can select any Location to
        // associate with the Image and they will be redirected to the Location index page.
        [Authorize]
        public ActionResult Create(int? locationID)
        {
            IQueryable<Location> locations;
            string cancelAction;

            // if a location is specified, only return a single value.
            if (locationID.HasValue)
            {
                locations = db.Locations.Where(l => l.LocationID == locationID);
                cancelAction = "Details/" + locationID;
            }

            // otherwise, all locations are available.
            else
            {
                locations = db.Locations.Where(l => true);
                cancelAction = "Index";
            }

            // make viewbag variables
            ViewBag.LocationID = new SelectList(locations, "LocationID", "Label");
            ViewBag.PlaceholderUrl = "~/Content/ImagePreview.png";
            ViewBag.CancelAction = cancelAction;

            return View(new LocationImageViewModel());
        }

        // **************************************
        // EDITED (Non-Standard Scaffolded Code)
        // **************************************
        // POST: Recreation/Edit/5
        //
        // Creates a new Image from the data input from the LocationImageViewModel.
        //
        // This is done in two steps: 
        //     1.) Saving the image to the website folder structure,
        //     2.) Saving the LocationImage model with a string url to that image in the database.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LocationImageViewModel model)
        {
            // List of allowed image types (for hosting on web)
            var validImageTypes = new string[]
            {
                "image/gif",
                "image/jpeg",
                "image/pjpeg", // needed for compatability with some older jpegs
                "image/png"
            };


            // 1. check if a valid image was uploaded
            if (model.ImageUpload == null || model.ImageUpload.ContentLength == 0)
            {
                Console.WriteLine("Image is null or empty");
                ModelState.AddModelError("ImageUpload", "This field is required");
            }

            // 2. check that image is of a valid filetype
            else if (!validImageTypes.Contains(model.ImageUpload.ContentType))
            {
                Console.WriteLine("Invalid Image Type");
                ModelState.AddModelError("ImageUpload", "Please choose either a GIF, JPG or PNG image.");
            }

            // 3. create the Image entry to save to the database
            if (ModelState.IsValid)
            {
                Console.WriteLine("Model state is valid");
                // initialize image model to store in database
                var locationImage = new LocationImage
                {
                    LocationID = model.LocationID,
                    Title = model.Title,
                    AltText = model.ImageUpload.FileName,
                    DateTaken = model.DateTaken,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    ImageUrl = Path.Combine(LocationImage.UPLOAD_DIRECTORY, model.ImageUpload.FileName),
                    User = manager.FindById(User.Identity.GetUserId())
                };

                //save image to local storage
                model.ImageUpload.SaveAs(Path.Combine(Server.MapPath(LocationImage.UPLOAD_DIRECTORY), model.ImageUpload.FileName));

                // save LocationImage model in database
                db.LocationImages.Add(locationImage);
                db.SaveChanges();
                return RedirectToAction("Index", "LocationImage", new { locationID = model.LocationID });
            }

            // otherwise, go back to create view.
            Console.WriteLine("Model state is invalid.");

            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "Label", model.LocationID);
            ViewBag.PlaceholderUrl = "~/Content/ImagePreview.png";
            ViewBag.LocationCount = db.Locations.Count();
            return View(model);
        }





        // GET: LocationImage/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocationImage locationImage = db.LocationImages.Find(id);
            if (locationImage == null)
            {
                return HttpNotFound();
            }

            ViewBag.PlaceholderUrl = "~/Content/ImagePreview.png";
            return View(locationImage);
        }

        // POST: LocationImage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ImageID,Title,ImageUrl,DateTaken")] LocationImage locationImage)
        {
            if (ModelState.IsValid)
            {
                locationImage.DateModified = DateTime.UtcNow;
                db.Entry(locationImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Media", "Location", new { locationID = locationImage.LocationID });
            }
            return View(locationImage);
        }

        // GET: LocationImage/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocationImage locationImage = db.LocationImages.Find(id);
            if (locationImage == null)
            {
                return HttpNotFound();
            }
            return View(locationImage);
        }

        // POST: LocationImage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LocationImage locationImage = db.LocationImages.Find(id);
            db.Images.Remove(locationImage);
            db.SaveChanges();
            return RedirectToAction("Media", "Location", new { locationID = locationImage.LocationID });
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
