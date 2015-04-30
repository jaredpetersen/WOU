using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TentsNTrails.Models
{
    public class EventComments
    {
        [Key]
        public int EventID { get; set; }

        [Display(Name = "Comment")]
        [Required]
        public String Comment { get; set; }

        [Display(Name = "Event Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public virtual User Author { get; set; }
    }
}