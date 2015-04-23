using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TentsNTrails.Models
{
    //Encapsulates all data needed for a Profile Picture.
    public class ProfilePictureViewModel
    {
        public User User { get; set; }

        // Holds the uploaded image file
        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Image")]

        public HttpPostedFileBase ImageUpload { get; set; }
    }

    public class RequestListViewModel
    {
        public ICollection<ConnectionRequest> Requests { get; set; }
        public int RowCount { get; set; }

    }
}