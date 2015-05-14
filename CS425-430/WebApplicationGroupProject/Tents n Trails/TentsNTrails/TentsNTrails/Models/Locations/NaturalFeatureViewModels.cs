using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TentsNTrails.Models
{

    /// <summary>
    /// Used for the NaturalFeatures/Details view.
    /// </summary>
    public class NaturalFeatureDetailsViewModel
    {
        /// <summary>
        /// The NaturalFeature to see the details of.
        /// </summary>
        public NaturalFeature NaturalFeature { get; set; }

        /// <summary>
        /// The Locations associated with this NaturalFeature.
        /// </summary>
        public IPagedList<Location> Locations { get; set; }
    }
}