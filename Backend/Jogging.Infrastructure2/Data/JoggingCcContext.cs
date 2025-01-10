using System;
using System.Collections.Generic;
using Jogging.Infrastructure2.Models;
using Jogging.Infrastructure2.Models.Club;
using Jogging.Infrastructure2.Models.Account;
using Jogging.Infrastructure2.Views;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Jogging.Infrastructure2.Data;

public partial class JoggingCcContext : DbContext
{
    public JoggingCcContext()
    {
    }

    public JoggingCcContext(DbContextOptions<JoggingCcContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AddressEF> Addresses { get; set; }

    public virtual DbSet<AgeCategoryEF> AgeCategories { get; set; }

    public virtual DbSet<CompetitionEF> Competitions { get; set; }

    public virtual DbSet<CompetitionPerCategoryEF> CompetitionPerCategories { get; set; }

    public virtual DbSet<PersonEF> People { get; set; }

    public virtual DbSet<Personview> Personviews { get; set; }

    public virtual DbSet<ProfileEF> Profiles { get; set; }

    public virtual DbSet<RegistrationEF> Registrations { get; set; }

    public virtual DbSet<SchoolEF> Schools { get; set; }

    public virtual DbSet<ClubEF> Clubs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("server=mysql-28d63a87-linabencheikh-nt.f.aivencloud.com;port=21290;database=Jogging_CC;uid=avnadmin;pwd=AVNS_vEpmT_7ygfJjKSPWezk", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.30-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AddressEF>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<AgeCategoryEF>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<CompetitionEF>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Active).HasDefaultValueSql("'0'");
            entity.Property(e => e.RankingActive).HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<CompetitionPerCategoryEF>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Gender).IsFixedLength();
        });

        modelBuilder.Entity<PersonEF>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Gender).HasDefaultValueSql("''");

                entity.HasOne(p => p.Club)
                      .WithMany(c => c.Members)
                      .HasForeignKey(p => p.ClubId)
                      .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Personview>(entity =>
        {
            entity.ToView("personview");

            entity.Property(e => e.Gender).HasDefaultValueSql("''");
        });

        modelBuilder.Entity<ProfileEF>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<RegistrationEF>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<SchoolEF>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<ClubEF>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasMany(e => e.Members)
                .WithOne(e => e.Club)
                .HasForeignKey(e => e.ClubId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
