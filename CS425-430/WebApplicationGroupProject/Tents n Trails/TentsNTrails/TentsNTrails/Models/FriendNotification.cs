using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TentsNTrails.Models
{
    // friend notifications have an additional field, a potential friend.
    public class FriendNotification
    {
        // the friend who the notification is about.
        public User PotentialFriend { get; set; }


        // thinking about this additional field, depends on implementation needs
        /*
        public FriendNotifyType FriendNotifyType { get; set; }

        public enum FriendNotifyType
        {
            // seen when another user requests a connection with you.
            Request, 
            
            // seen when another user confirms to connect with you.
            Confirm, 
            
            // seen when another user denies to connect with you.
            Deny, 
        }
        */
    }
}