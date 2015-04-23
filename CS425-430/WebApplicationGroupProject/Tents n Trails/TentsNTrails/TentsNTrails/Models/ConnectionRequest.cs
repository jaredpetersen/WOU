using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TentsNTrails.Models
{
    public class ConnectionRequest
    {
        [Key]
        public int ConnectionRequestID { get; set; }

        // each connection is between two users and is directed
        public virtual User RequestedUser { get; set; }
        public virtual User Sender { get; set; }
    }
}