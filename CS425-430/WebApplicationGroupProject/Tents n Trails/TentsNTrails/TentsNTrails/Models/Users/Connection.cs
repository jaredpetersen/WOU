using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TentsNTrails.Models
{
    public class Connection
    {
        [Key]
        public int ConnectionID { get; set; }

        // each connection is between two users
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
    }
}