using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// File holds all ViewModels associated with Location.

namespace TentsNTrails.Models
{
    /// <summary>
    /// Used by Index to display Locations and their associated Recreation Tags.
    /// </summary>
    public class LocationViewModel
    {
        public virtual PagedList.IPagedList<Location> Locations { get; set; }
        public virtual ICollection<Location> TopRatedLocations { get; set; }
        public virtual ICollection<Recreation> Recreations { get; set; }
        public virtual Dictionary<int, int> Ratings { get; set; }

        // Used for the Join/Merge multi-search functionality
        public PagedList.IPagedList<Location> LocationA { get; set; }
        public PagedList.IPagedList<Location> LocationB { get; set; }
    }

    /// <summary>
    /// Used to create or edit a new Location
    /// </summary>
    public class EditLocationViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public String Label { get; set; }

        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        [Display(Name = "Recreation Tags")]
        public List<LocationRecreation> RecOptions { get; set; }
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
        
        public DifficultyRatings Difficulty { get; set; }

        // Location recreations
        [Display(Name = "Recreation Tags")]
        public List<LocationRecreation> RecOptions { get; set; }

    }
}