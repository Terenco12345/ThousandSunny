using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThousandSunny.ChickenVoyage.API.Models
{
    public class TSChickenVoyageContext : DbContext
    {
        public TSChickenVoyageContext(DbContextOptions<TSChickenVoyageContext> options) : base(options)
        {}

        /// <summary>
        /// User table, holds account information for users.
        /// </summary>
        public DbSet<User> User { get; set; } 
    }
}
