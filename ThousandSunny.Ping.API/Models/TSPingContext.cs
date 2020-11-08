using Microsoft.EntityFrameworkCore;

namespace ThousandSunny.Ping.API.Models
{
    public class TSPingContext : DbContext
    {
        public TSPingContext(DbContextOptions<TSPingContext> options) : base(options)
        {}

        public DbSet<Ping> Ping { get; set; }
    }
}
