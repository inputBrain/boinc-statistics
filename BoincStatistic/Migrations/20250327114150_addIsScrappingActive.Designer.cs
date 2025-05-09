﻿// <auto-generated />
using System;
using BoincStatistic.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BoincStatistic.Migrations
{
    [DbContext(typeof(PostgreSqlContext))]
    [Migration("20250327114150_addIsScrappingActive")]
    partial class addIsScrappingActive
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BoincStatistic.Database.CountryStatistic.CountryStatisticModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CountryName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreditAvarage")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreditDay")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreditMonth")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreditUser")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreditWeek")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<string>("Rank")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TotalCredit")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("DetailedProjectStats");
                });

            modelBuilder.Entity("BoincStatistic.Database.ProjectStatistic.ProjectStatisticModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CountryStatisticUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Divider")
                        .HasColumnType("integer");

                    b.Property<bool>("IsScrappingActive")
                        .HasColumnType("boolean");

                    b.Property<string>("ProjectCategory")
                        .HasColumnType("text");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProjectStatisticUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("TotalCredit")
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("ProjectStats");
                });

            modelBuilder.Entity("BoincStatistic.Database.CountryStatistic.CountryStatisticModel", b =>
                {
                    b.HasOne("BoincStatistic.Database.ProjectStatistic.ProjectStatisticModel", "ProjectModel")
                        .WithMany("CountryStatistics")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProjectModel");
                });

            modelBuilder.Entity("BoincStatistic.Database.ProjectStatistic.ProjectStatisticModel", b =>
                {
                    b.Navigation("CountryStatistics");
                });
#pragma warning restore 612, 618
        }
    }
}
