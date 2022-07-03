using ForLet.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Infrastructure
{
    public class ForLetDbContext : IdentityDbContext<AppUser>
    {
        // inject the db context into the constructor 
        public ForLetDbContext(DbContextOptions<ForLetDbContext> options) : base(options)
        {
        }

        public DbSet<State> States { get; set; }
        public DbSet<LGA> Lgas { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>()
        //        .HasMany(p => p.Rentals);
        //}

    }

    

}
