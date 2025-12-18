using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DataAccess.Context
{
    public class RestaurantDbContext : IdentityDbContext<IdentityUser>
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
            : base(options)
        {
        }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

    }
}