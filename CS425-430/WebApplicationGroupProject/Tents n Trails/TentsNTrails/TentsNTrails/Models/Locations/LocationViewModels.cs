using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PagedList;

// File holds all ViewModels associated with Location.

namespace TentsNTrails.Models
{
    /// <summary>
    /// Used by Index to display Locations and their associated Recreation Tags.
    /// </summary>
    public class JoinLocationsViewModel
    {
        public IPagedList<Location> LocationA { get; set; }
        public IPagedList<Location> LocationB { get; set; }
    }

    /// <summary>
    /// Designed for use with the Location/Index view.
    /// </summary>
    public class LocationIndexViewModel
    {
        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<Location> TopRatedLocations { get; set; }
        public virtual ICollection<Location> MostRecentLocations { get; set; }
        public virtual ICollection<Location> PersonalRecommendations { get; set; }
        public virtual ICollection<Location> FriendRecommendations { get; set; }
        public virtual ICollection<Recreation> Recreations { get; set; }
    }

    /// <summary>
    /// Designed for use with the Location/Browse view.
    /// </summary>
    public class BrowseLocationsViewModel
    {
        public virtual IPagedList<Location> Locations { get; set; }
        public virtual ICollection<Location> AllLocations { get; set; }
        public virtual ICollection<Recreation> Recreations { get; set; }

        public virtual int? recreationID { get; set; }
        public virtual String query { get; set; }
        public virtual int? page { get; set; }
    }

    /// <summary>
    /// Used to edit a Location
    /// </summary>
    public class EditLocationViewModel
    {
        [Required]
        public int LocationID { get; set; }

        // for difficulty rating below
        public enum DifficultyRatings
        {
            [Display(Name = "Easy")]
            Easy,
            [Display(Name = "Medium")]
            Medium,
            [Display(Name = "Hard")]
            Hard,
            [Display(Name = "Varies")]
            Varies,
            [Display(Name = "NA")]
            NA
        }

        [Required]
        [Display(Name = "Name")]
        public String Label { get; set; }

        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        [StringLength(250)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public DifficultyRatings Difficulty { get; set; }

        // Location recreations
        [Display(Name = "Recreation Tags")]
        public List<LocationRecreation> RecOptions { get; set; }

        public ICollection<String> SelectedFeatures { get; set; }
        public ICollection<String> AllNaturalFeatures { get; set; }

        /// <summary>
        /// Required default constructor
        /// </summary>
        public EditLocationViewModel(){ }

        /// <summary>
        /// Initialize the values from the given location.
        /// </summary>
        /// <param name="location"></param>
        public EditLocationViewModel(Location location)
        {
            this.LocationID = location.LocationID;
            this.Label = location.Label;
            this.Latitude = location.Latitude;
            this.Longitude = location.Longitude;
            this.Description = location.Description;
        }
    }

    public class CreateLocationViewModel
    {
        // for difficulty rating below
        public enum DifficultyRatings
        {
            [Display(Name = "Easy")]
            Easy,
            [Display(Name = "Medium")]
            Medium,
            [Display(Name = "Hard")]
            Hard,
            [Display(Name = "Varies")]
            Varies,
            [Display(Name = "NA")]
            NA
        }

        [Required]
        [Display(Name = "Name")]
        public String Label { get; set; }

        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        [StringLength(250)]
        [DataType(DataType.MultilineText)]

        public string Description { get; set; }
        
        [Required]
        public DifficultyRatings Difficulty { get; set; }

        // Location recreations
        [Display(Name = "Recreation Tags")]
        public List<LocationRecreation> RecOptions { get; set; }

        public ICollection<String> SelectedFeatures { get; set; }
        public ICollection<String> AllNaturalFeatures { get; set; }
    }
}