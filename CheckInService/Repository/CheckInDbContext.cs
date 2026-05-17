using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Models;

namespace CheckInService.Repository;

public partial class CheckInDbContext : DbContext
{
    public CheckInDbContext()
    {
    }

    public CheckInDbContext(DbContextOptions<CheckInDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CheckIn> CheckIns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=(local)\\sqlexpress; database=CheckInDb; integrated security=sspi; trustservercertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CheckIn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CheckIn__3214EC07022DF2FE");

            entity.ToTable("CheckIn");

            entity.HasIndex(e => e.CheckInId, "UQ__CheckIn__E64976859D0F5E88").IsUnique();

            entity.Property(e => e.CheckInId).HasMaxLength(20);
            entity.Property(e => e.CheckInTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReferenceNumber).HasMaxLength(20);
            entity.Property(e => e.SeatNumber).HasMaxLength(5);
            entity.Property(e=>e.PassengerName).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
