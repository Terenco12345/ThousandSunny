using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThousandSunny.API.Models
{
    public class ThousandSunnyContext : DbContext
    {
        public ThousandSunnyContext(DbContextOptions<ThousandSunnyContext> options) : base(options)
        {}

        public DbSet<Ping> Ping { get; set; }
    }
}
