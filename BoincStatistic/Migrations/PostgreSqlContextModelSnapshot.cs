﻿// <auto-generated />
using BoincStatistic.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BoincStatistic.Migrations
{
    [DbContext(typeof(PostgreSqlContext))]
    partial class PostgreSqlContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BoincStatistic.Database.BoincProjectStats.BoincProjectStatsModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ProjectCategory")
                        .HasColumnType("text");

                    b.Property<string>("ProjectName")
                        .HasColumnType("text");

                    b.Property<string>("TotalCredit")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ProjectStats");
                });

            modelBuilder.Entity("BoincStatistic.Database.BoincStats.BoincStatsModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CountryName")
                        .HasColumnType("text");

                    b.Property<string>("CreditAvarage")
                        .HasColumnType("text");

                    b.Property<string>("CreditDay")
                        .HasColumnType("text");

                    b.Property<string>("CreditMonth")
                        .HasColumnType("text");

                    b.Property<string>("CreditUser")
                        .HasColumnType("text");

                    b.Property<string>("CreditWeek")
                        .HasColumnType("text");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<string>("Rank")
                        .HasColumnType("text");

                    b.Property<string>("TotalCredit")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("DetailedProjectStats");
                });

            modelBuilder.Entity("BoincStatistic.Database.BoincStats.BoincStatsModel", b =>
                {
                    b.HasOne("BoincStatistic.Database.BoincProjectStats.BoincProjectStatsModel", "ProjectModel")
                        .WithMany("DetailedStatistics")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProjectModel");
                });

            modelBuilder.Entity("BoincStatistic.Database.BoincProjectStats.BoincProjectStatsModel", b =>
                {
                    b.Navigation("DetailedStatistics");
                });
#pragma warning restore 612, 618
        }
    }
}
