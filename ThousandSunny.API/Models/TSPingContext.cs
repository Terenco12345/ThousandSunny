using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThousandSunny.API.Models
{
    public class TSPingContext : DbContext
    {
        public TSPingContext(DbContextOptions<TSPingContext> options) : base(options)
        {}

        public DbSet<Ping> Ping { get; set; }
    }
}
