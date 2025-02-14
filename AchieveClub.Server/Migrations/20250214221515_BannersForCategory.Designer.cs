﻿// <auto-generated />
using System;
using AchieveClub.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AchieveClub.Server.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20250214221515_BannersForCategory")]
    partial class BannersForCategory
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.AchievementDbo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<bool>("IsMultiple")
                        .HasColumnType("boolean");

                    b.Property<string>("LogoURL")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.Property<int?>("TagId")
                        .HasColumnType("integer");

                    b.Property<int?>("TimeLimitInDays")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("Xp")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TagId");

                    b.ToTable("Achievements");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.CategoryDbo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AvailableBanner")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("Color")
                        .HasColumnType("text");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UnavailableBanner")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.CompletedAchievementDbo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AchieveRefId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateOfCompletion")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("SupervisorRefId")
                        .HasColumnType("integer");

                    b.Property<int>("UserRefId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AchieveRefId");

                    b.HasIndex("SupervisorRefId");

                    b.HasIndex("UserRefId");

                    b.ToTable("CompletedAchievements");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.DeliveryStatusDBO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DeliveryStatuses");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.OrderDBO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DeliveryStatusId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Price")
                        .HasColumnType("integer");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("VariantId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryStatusId");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.HasIndex("VariantId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.ProductDbo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<int?>("DefaultVariantId")
                        .HasColumnType("integer");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Price")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("DefaultVariantId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.ProductPhotoDbo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("VariantId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("VariantId");

                    b.ToTable("ProductPhotos");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.RoleDbo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.TagDbo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.UserDbo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.Property<int>("Balance")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("RefreshToken")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<int>("RoleRefId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RoleRefId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.VariantDbo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("DefaultPhotoId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DefaultPhotoId");

                    b.HasIndex("ProductId");

                    b.ToTable("Variants");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.AchievementDbo", b =>
                {
                    b.HasOne("AchieveClub.Server.RepositoryItems.TagDbo", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.CompletedAchievementDbo", b =>
                {
                    b.HasOne("AchieveClub.Server.RepositoryItems.AchievementDbo", "Achievement")
                        .WithMany()
                        .HasForeignKey("AchieveRefId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AchieveClub.Server.RepositoryItems.UserDbo", "Supervisor")
                        .WithMany()
                        .HasForeignKey("SupervisorRefId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AchieveClub.Server.RepositoryItems.UserDbo", "User")
                        .WithMany()
                        .HasForeignKey("UserRefId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Achievement");

                    b.Navigation("Supervisor");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.OrderDBO", b =>
                {
                    b.HasOne("AchieveClub.Server.RepositoryItems.DeliveryStatusDBO", "DeliveryStatus")
                        .WithMany()
                        .HasForeignKey("DeliveryStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AchieveClub.Server.RepositoryItems.ProductDbo", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AchieveClub.Server.RepositoryItems.UserDbo", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AchieveClub.Server.RepositoryItems.VariantDbo", "Variant")
                        .WithMany()
                        .HasForeignKey("VariantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeliveryStatus");

                    b.Navigation("Product");

                    b.Navigation("User");

                    b.Navigation("Variant");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.ProductDbo", b =>
                {
                    b.HasOne("AchieveClub.Server.RepositoryItems.CategoryDbo", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AchieveClub.Server.RepositoryItems.VariantDbo", "DefaultVariant")
                        .WithMany()
                        .HasForeignKey("DefaultVariantId");

                    b.Navigation("Category");

                    b.Navigation("DefaultVariant");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.ProductPhotoDbo", b =>
                {
                    b.HasOne("AchieveClub.Server.RepositoryItems.VariantDbo", "Variant")
                        .WithMany("ProductPhotos")
                        .HasForeignKey("VariantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Variant");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.UserDbo", b =>
                {
                    b.HasOne("AchieveClub.Server.RepositoryItems.RoleDbo", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleRefId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.VariantDbo", b =>
                {
                    b.HasOne("AchieveClub.Server.RepositoryItems.ProductPhotoDbo", "DefaultPhoto")
                        .WithMany()
                        .HasForeignKey("DefaultPhotoId");

                    b.HasOne("AchieveClub.Server.RepositoryItems.ProductDbo", "Product")
                        .WithMany("Variants")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DefaultPhoto");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.ProductDbo", b =>
                {
                    b.Navigation("Variants");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.RoleDbo", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("AchieveClub.Server.RepositoryItems.VariantDbo", b =>
                {
                    b.Navigation("ProductPhotos");
                });
#pragma warning restore 612, 618
        }
    }
}
