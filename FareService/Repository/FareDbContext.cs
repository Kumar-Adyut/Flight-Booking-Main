using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FareService.Repository;

public partial class FareDbContext : DbContext
{
    public FareDbContext()
    {
    }

    public FareDbContext(DbContextOptions<FareDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Fare> Fares { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=(local)\\sqlexpress; database=FareDb; integrated security=sspi; trustservercertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fare>(entity =>
        {
            entity.HasKey(e => e.FareId).HasName("PK__Fare__1261FA36DAE43D89");

            entity.ToTable("Fare");

            entity.Property(e => e.FareId).HasColumnName("FareID");
            entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ConvenienceFee).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FlightId).HasColumnName("FlightID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
