﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using pms_backend.Data;

#nullable disable

namespace pms_backend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("pms_backend.Models.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("A1")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("A2")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("A3")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ForUserId")
                        .HasColumnType("integer");

                    b.Property<int>("FromUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Q1")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Q2")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Q3")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Quarter")
                        .HasColumnType("integer");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ForUserId");

                    b.HasIndex("FromUserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("pms_backend.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MicrosoftId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("MicrosoftId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("pms_backend.Models.UserManagerMapping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AssignmentTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<int>("ManagerId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ManagerId");

                    b.HasIndex("UserId");

                    b.ToTable("UserManagerMappings");
                });

            modelBuilder.Entity("pms_backend.Models.Review", b =>
                {
                    b.HasOne("pms_backend.Models.User", "ForUser")
                        .WithMany("ReviewsReceived")
                        .HasForeignKey("ForUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("pms_backend.Models.User", "FromUser")
                        .WithMany("ReviewsGiven")
                        .HasForeignKey("FromUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ForUser");

                    b.Navigation("FromUser");
                });

            modelBuilder.Entity("pms_backend.Models.UserManagerMapping", b =>
                {
                    b.HasOne("pms_backend.Models.User", "Manager")
                        .WithMany("DirectReports")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("pms_backend.Models.User", "User")
                        .WithMany("ManagerMappings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Manager");

                    b.Navigation("User");
                });

            modelBuilder.Entity("pms_backend.Models.User", b =>
                {
                    b.Navigation("DirectReports");

                    b.Navigation("ManagerMappings");

                    b.Navigation("ReviewsGiven");

                    b.Navigation("ReviewsReceived");
                });
#pragma warning restore 612, 618
        }
    }
}
