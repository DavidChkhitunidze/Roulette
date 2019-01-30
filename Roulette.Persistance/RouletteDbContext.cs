using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Roulette.Domain.Entities;
using Roulette.Persistance.Extensions;
using System;
using System.Linq;

namespace Roulette.Persistance
{
    public class RouletteDbContext : IdentityDbContext<User, UserRole, Guid>
    {
        public RouletteDbContext(DbContextOptions<RouletteDbContext> options) : base(options)
        {
            if (Database.GetPendingMigrations().Any())
            {
#if !DEBUG
                Database.Migrate();
#endif
            }
        }

        // Db model creating configurations for entity framework
        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            base.OnModelCreating(builder);

            builder.ApplyAllConfigurations();
        }
    }
}
