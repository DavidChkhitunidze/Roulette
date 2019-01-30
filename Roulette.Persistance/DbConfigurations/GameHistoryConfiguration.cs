using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roulette.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Persistance.DbConfigurations
{
    public class GameHistoryConfiguration : IEntityTypeConfiguration<GameHistory>
    {
        public void Configure(EntityTypeBuilder<GameHistory> builder)
        {
            // Initialize GameHistory's primary key
            builder.HasKey(gh => gh.Id);

            // Initialize unique auto incrementing spin id
            builder.Property(gh => gh.SpinId).ValueGeneratedOnAdd();

            // Initialize property types for migration to trigger dababase column type
            builder.Property(gh => gh.BetAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(gh => gh.WonAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(gh => gh.CreateDate).IsRequired().HasColumnType("datetime2");

            // Initialize user for game history with foreign key
            builder.HasOne(gh => gh.User).WithMany(u => u.GameHistories).HasForeignKey(fh => fh.UserId);

            // Initialize GameHistory table name in database
            builder.ToTable("GameHistories");
        }
    }
}
