﻿// <auto-generated />
using System;
using DioLive.Cache.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DioLive.Cache.WebUI.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DioLive.Cache.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("DioLive.Cache.Models.Budget", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId");

                    b.Property<string>("Name");

                    b.Property<byte>("Version")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue((byte)1);

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Budget");
                });

            modelBuilder.Entity("DioLive.Cache.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid?>("BudgetId");

                    b.Property<int>("Color")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("ABS(CHECKSUM(NEWID()) % 16777216)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.Property<string>("OwnerId");

                    b.Property<int?>("ParentId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ParentId");

                    b.HasIndex("BudgetId", "Name")
                        .IsUnique()
                        .HasFilter("[BudgetId] IS NOT NULL AND [Name] IS NOT NULL");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("DioLive.Cache.Models.CategoryLocalization", b =>
                {
                    b.Property<int>("CategoryId");

                    b.Property<string>("Culture")
                        .HasMaxLength(10);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("CategoryId", "Culture");

                    b.ToTable("CategoryLocalization");
                });

            modelBuilder.Entity("DioLive.Cache.Models.Options", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<int>("PurchaseGrouping");

                    b.Property<bool>("ShowPlanList");

                    b.HasKey("UserId");

                    b.ToTable("Options");
                });

            modelBuilder.Entity("DioLive.Cache.Models.Plan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AuthorId")
                        .IsRequired();

                    b.Property<Guid>("BudgetId");

                    b.Property<DateTime?>("BuyDate");

                    b.Property<string>("BuyerId");

                    b.Property<string>("Comments");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("BudgetId");

                    b.HasIndex("BuyerId");

                    b.ToTable("Plan");
                });

            modelBuilder.Entity("DioLive.Cache.Models.Purchase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId")
                        .IsRequired();

                    b.Property<Guid>("BudgetId");

                    b.Property<int>("CategoryId");

                    b.Property<string>("Comments");

                    b.Property<int>("Cost");

                    b.Property<DateTime>("CreateDate");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<string>("LastEditorId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.Property<string>("Shop");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("BudgetId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("LastEditorId");

                    b.ToTable("Purchase");
                });

            modelBuilder.Entity("DioLive.Cache.Models.Share", b =>
                {
                    b.Property<Guid>("BudgetId");

                    b.Property<string>("UserId");

                    b.Property<byte>("Access");

                    b.HasKey("BudgetId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Share");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("DioLive.Cache.Models.Budget", b =>
                {
                    b.HasOne("DioLive.Cache.Models.ApplicationUser", "Author")
                        .WithMany("Budgets")
                        .HasForeignKey("AuthorId");
                });

            modelBuilder.Entity("DioLive.Cache.Models.Category", b =>
                {
                    b.HasOne("DioLive.Cache.Models.Budget", "Budget")
                        .WithMany("Categories")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DioLive.Cache.Models.ApplicationUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.HasOne("DioLive.Cache.Models.Category", "Parent")
                        .WithMany("Subcategories")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DioLive.Cache.Models.CategoryLocalization", b =>
                {
                    b.HasOne("DioLive.Cache.Models.Category", "Category")
                        .WithMany("Localizations")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DioLive.Cache.Models.Options", b =>
                {
                    b.HasOne("DioLive.Cache.Models.ApplicationUser", "User")
                        .WithOne("Options")
                        .HasForeignKey("DioLive.Cache.Models.Options", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DioLive.Cache.Models.Plan", b =>
                {
                    b.HasOne("DioLive.Cache.Models.ApplicationUser", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DioLive.Cache.Models.Budget", "Budget")
                        .WithMany("Plans")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DioLive.Cache.Models.ApplicationUser", "Buyer")
                        .WithMany()
                        .HasForeignKey("BuyerId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("DioLive.Cache.Models.Purchase", b =>
                {
                    b.HasOne("DioLive.Cache.Models.ApplicationUser", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DioLive.Cache.Models.Budget", "Budget")
                        .WithMany("Purchases")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DioLive.Cache.Models.Category", "Category")
                        .WithMany("Purchases")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DioLive.Cache.Models.ApplicationUser", "LastEditor")
                        .WithMany()
                        .HasForeignKey("LastEditorId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("DioLive.Cache.Models.Share", b =>
                {
                    b.HasOne("DioLive.Cache.Models.Budget", "Budget")
                        .WithMany("Shares")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DioLive.Cache.Models.ApplicationUser", "User")
                        .WithMany("Shares")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("DioLive.Cache.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("DioLive.Cache.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DioLive.Cache.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("DioLive.Cache.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
