using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace TentsNTrails.Models
{
    /**
     * I separated out the ApplicationDbContext from the IdentityModels class file,
     * so it is easier to read.
     */
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        // accessible DbSets
        public DbSet<Connection> Connections { get; set; }
        public DbSet<ConnectionRequest> ConnectionRequests { get; set; }

        public DbSet<Image> Images { get; set; }
        
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationRecreation> LocationRecreations { get; set; }
        public DbSet<LocationFlag> LocationFlags { get; set; }
        public DbSet<LocationImage> LocationImages { get; set; }
        public DbSet<LocationVideo> LocationVideos { get; set; }

        //public DbSet<ProfilePicture> ProfilePictures { get; set; }
        
        public DbSet<Recreation> Recreations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        
        public DbSet<UserRecreation> UserRecreations { get; set; }
        
        public DbSet<Video> Videos { get; set; }
        
        


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        // Associate a Location with a Recreation.
        public void AddOrUpdateRecreationLocation(string locationLabel, string recreationLabel)
        {
            var location = this.Locations.SingleOrDefault(l => l.Label == locationLabel);
            var recreation = this.Recreations.SingleOrDefault(l => l.Label == recreationLabel);

            LocationRecreation locRec = new LocationRecreation();
            locRec.LocationID = location.LocationID;
            locRec.RecreationID = recreation.RecreationID;
            locRec.RecreationLabel = recreationLabel;

            LocationRecreations.Add(locRec);
        }

        public System.Data.Entity.DbSet<TentsNTrails.Models.Events> Events { get; set; }

        public System.Data.Entity.DbSet<TentsNTrails.Models.EventParticipants> EventParticipants { get; set; }

        /*
        public void AddOrUpdateLocationFlag(string flag, int locationID)
        {
            var location = this.Locations.Include(l => l.Recreations).SingleOrDefault(l => l.Label == locationLabel);
            var recreation = location.Recreations.SingleOrDefault(r => r.Label == recreationLabel);

            //i if it does not exist, register the item.
            if (recreation == null) location.Recreations.Add(this.Recreations.Single(r => r.Label == recreationLabel));
        }
        */
    }
}