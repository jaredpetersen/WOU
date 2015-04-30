using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TentsNTrails.Models
{
    /// <summary>
    /// Notification is the base class for all Notifications.  This will be designed so that we can subclass this notification type and
    /// add new fields/data as needed, so that there can be a Friend Request Notification, a Message Notification, etc.
    /// </summary>
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        // A human-readable, succinct description of the Notification, meant for the end-user.
        public string Description { get; set; }

        // when the notification was made.
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        // when then the user sees the notification.
        [DataType(DataType.DateTime)]
        public DateTime? DateRead { get; set; }

        // Convenience Property for checking and assigning a  if the notification is read by assigning values to DateRead.
        public bool IsRead
        {
            // IsRead returns false if DateRead is null.  Otherwise, it is true.
            get
            {
                return DateRead != null;
            }

            // If value is "true" and DateRead is null, then DateRead is assigned to UtcNow.
            // otherwise, if value is "false", then DateRead is assigned back to null. 
            set
            {
                if (value && DateRead == null) DateRead = DateTime.UtcNow;
                else if (!value) DateRead = null;
            }
        }

        // each notification has exactly one associated user, which the notification is for.
        [ForeignKey("User")]
        public string UserID { get; set; }
        public virtual User User { get; set; }
    }
}