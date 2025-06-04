using AuthUPS.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthUPS.Data
{

    public class ParcelTrackingDBContext : DbContext
    {
        public ParcelTrackingDBContext(DbContextOptions<ParcelTrackingDBContext> options) : base(options) { }
        public DbSet<User> users { get; set; }
    }

}
