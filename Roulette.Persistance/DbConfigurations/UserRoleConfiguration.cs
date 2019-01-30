using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roulette.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Persistance.DbConfigurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            // Initialize UserRole's primery key
            builder.HasKey(ur => ur.Id);

            // Initialize UserRole table name in database
            builder.ToTable("UserRoles");
        }
    }
}
