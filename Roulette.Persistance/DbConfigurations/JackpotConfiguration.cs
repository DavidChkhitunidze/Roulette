using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roulette.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Roulette.Persistance.DbConfigurations
{
    public class JackpotConfiguration : IEntityTypeConfiguration<Jackpot>
    {
        public void Configure(EntityTypeBuilder<Jackpot> builder)
        {
            // Initialize Jackpot's primary key
            builder.HasKey(j => j.Id);

            // Initialize property types for migration to trigger dababase column type
            builder.Property(j => j.JackpotAmount).IsRequired().HasColumnType("decimal(18,2)");

            // Initialize ConcurrencyToken for multiple update at the same time
            builder.Property(j => j.RowVersion).IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();

            // Initialize Jackpot table name in database
            builder.ToTable("Jackpots");
        }
    }
}
