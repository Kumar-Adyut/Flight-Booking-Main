using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CityService.Repository;

public partial class CityDbContext : DbContext
{
    public CityDbContext()
    {
    }

    public CityDbContext(DbContextOptions<CityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=(local)\\sqlexpress; database=CityDb; integrated security=sspi; trustservercertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__City__F2D21B76903257EC");

            entity.ToTable("City");

            entity.Property(e => e.AirportCharge).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CityCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CityName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
