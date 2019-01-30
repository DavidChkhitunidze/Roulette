using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roulette.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Persistance.DbConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Initialize GameHistory's primary key
            builder.HasKey(u => u.Id);

            // Initialize string properties max length in database
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(30);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);

            // Initialize property types for migration to trigger dababase column type
            builder.Property(u => u.UserBalance).IsRequired().HasColumnType("decimal(18,2)");

            // Initialize GameHistory collection for User with foreign key
            builder.HasMany(u => u.GameHistories).WithOne(gh => gh.User).HasForeignKey(gh => gh.UserId);

            // Initialize User table name in database
            builder.ToTable("Users");
        }
    }
}
