using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieBaseBack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MovieBaseBack.Data
{
    public class MovieDbContext:IdentityDbContext<AppUser>
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {

        }

        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Watched> Watcheds { get; set; }
        public DbSet<WatchList> WatchLists { get; set; }
        public DbSet<UserCred> Userss { get; set; }
    }
}
