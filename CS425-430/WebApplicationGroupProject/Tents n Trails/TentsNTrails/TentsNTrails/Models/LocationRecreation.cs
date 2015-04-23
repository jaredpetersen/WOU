using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TentsNTrails.Models
{
    public class LocationRecreation
    {
        [Key]
        [Column(Order = 0)]
        public int LocationID { get; set; }
        [Key]
        [Column(Order = 1)]
        public int RecreationID { get; set; }

        public String RecreationLabel { get; set; }

        public bool IsChecked { get; set; }

        public virtual Location Location { get; set; }
        public virtual Recreation Recreation { get; set; }

    }
}