using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TentsNTrails.Models
{
    public class Recreation
    {
        [Key]
        public int RecreationID { get; set; }

        [Display(Name = "Recreation")]
        public string Label { get; set; }

        // Datestamp functionality (not user-added)
        // used ideas from https://msdn.microsoft.com/en-us/magazine/dn745864.aspx

        [Display(Name = "Created")]
        [Editable(false)]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Modified")]
        [Editable(false)]
        public DateTime DateModified { get; set; }

        public virtual ICollection<Location> Locations { get; set; }

    }
}