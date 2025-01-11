﻿// <auto-generated />
using System;
using Jogging.Infrastructure2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Jogging.Infrastructure2.Migrations
{
    [DbContext(typeof(JoggingCcContext))]
    [Migration("20250111131112_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb4");
            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Jogging.Infrastructure2.Models.Account.ProfileEF", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("id");

                    b.Property<string>("Role")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("role");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("Profile");
                });

            modelBuilder.Entity("Jogging.Infrastructure2.Models.AddressEF", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("HouseNumber")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Street")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ZipCode")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("Jogging.Infrastructure2.Models.AgeCategoryEF", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("MaximumAge")
                        .HasColumnType("int");

                    b.Property<int?>("MinimumAge")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("AgeCategory");
                });

            modelBuilder.Entity("Jogging.Infrastructure2.Models.Club.ClubEF", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Logo")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("Club");
                });

            modelBuilder.Entity("Jogging.Infrastructure2.Models.CompetitionEF", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool?>("Active")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValueSql("'0'");

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime");

                    b.Property<string>("ImgUrl")
                        .HasColumnType("text")
                        .HasColumnName("img_url");

                    b.Property<string>("Information")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<bool?>("RankingActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValueSql("'0'");

                    b.Property<string>("Url")
                        .HasColumnType("text")
                        .HasColumnName("url");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("Competition");
                });

            modelBuilder.Entity("Jogging.Infrastructure2.Models.CompetitionPerCategoryEF", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AgeCategoryId")
                        .HasColumnType("int");

                    b.Property<int?>("CompetitionId")
                        .HasColumnType("int");

                    b.Property<float?>("DistanceInKm")
                        .HasColumnType("float");

                    b.Property<string>("DistanceName")
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Gender")
                        .HasMaxLength(1)
                        .HasColumnType("char(1)")
                        .IsFixedLength();

                    b.Property<DateTime?>("GunTime")
                        .HasColumnType("datetime");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("CompetitionPerCategory");
                });

            modelBuilder.Entity("Jogging.Infrastructure2.Models.PersonEF", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<DateOnly>("BirthDate")
                        .HasColumnType("date");

                    b.Property<int?>("ClubId")
                        .HasColumnType("int");

                    b.Property<string>("ConfirmationToken")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Gender")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasDefaultValueSql("''");

                    b.Property<string>("Ibannumber")
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("IBANNumber");

                    b.Property<bool>("IsEmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordResetToken")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("SchoolId")
                        .HasColumnType("int");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex("ClubId");

                    b.ToTable("Person");
                });

            modelBuilder.Entity("Jogging.Infrastructure2.Models.RegistrationEF", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CompetitionEFId")
                        .HasColumnType("int");

                    b.Property<int>("CompetitionId")
                        .HasColumnType("int");

                    b.Property<int>("CompetitionPerCategoryId")
                        .HasColumnType("int");

                    b.Property<bool?>("Paid")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("PersonEFId")
                        .HasColumnType("int");

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.Property<short?>("RunNumber")
                        .HasColumnType("smallint");

                    b.Property<string>("RunTime")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex("CompetitionEFId");

                    b.HasIndex("PersonEFId");

                    b.ToTable("Registration");
                });

            modelBuilder.Entity("Jogging.Infrastructure2.Models.SchoolEF", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("School");
                });

            modelBuilder.Entity("Jogging.Infrastructure2.Views.Personview", b =>
                {
                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<DateOnly>("BirthDate")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasDefaultValueSql("''");

                    b.Property<string>("Ibannumber")
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("IBANNumber");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("SchoolId")
                        .HasColumnType("int");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("char(36)");

                    b.ToTable((string)null);

                    b.ToView("personview", (string)null);
                });

            modelBuilder.Entity("Jogging.Infrastructure2.Models.PersonEF", b =>
                {
                    b.HasOne("Jogging.Infrastructure2.Models.Club.ClubEF", "Club")
                        .WithMany("Members")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Club");
                });

            modelBuilder.Entity("Jogging.Infrastructure2.Models.RegistrationEF", b =>
                {
                    b.HasOne("Jogging.Infrastructure2.Models.CompetitionEF", "CompetitionEF")
                        .WithMany()
                        .HasForeignKey("CompetitionEFId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jogging.Infrastructure2.Models.PersonEF", "PersonEF")
                        .WithMany()
                        .HasForeignKey("PersonEFId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CompetitionEF");

                    b.Navigation("PersonEF");
                });

            modelBuilder.Entity("Jogging.Infrastructure2.Models.Club.ClubEF", b =>
                {
                    b.Navigation("Members");
                });
#pragma warning restore 612, 618
        }
    }
}