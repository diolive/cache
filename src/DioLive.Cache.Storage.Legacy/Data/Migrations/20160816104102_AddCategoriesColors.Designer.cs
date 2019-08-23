using System;

using DioLive.Cache.Storage.Legacy.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DioLive.Cache.WebUI.Data;

namespace DioLive.Cache.WebUI.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160816104102_AddCategoriesColors")]
    partial class AddCategoriesColors
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.Budget", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Budget");
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BudgetId");

                    b.Property<int>("Color")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("ABS(CHECKSUM(NEWID()) % 16777216)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 300);

                    b.Property<string>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("BudgetId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("OwnerId", "Name")
                        .IsUnique();

                    b.ToTable("Category");
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.CategoryLocalization", b =>
                {
                    b.Property<int>("CategoryId");

                    b.Property<string>("Culture")
                        .HasAnnotation("MaxLength", 10);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("CategoryId", "Culture");

                    b.HasIndex("CategoryId");

                    b.ToTable("CategoryLocalization");
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.Options", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<int>("PurchaseGrouping");

                    b.Property<bool>("ShowPlanList");

                    b.HasKey("UserId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Options");
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.Plan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId")
                        .IsRequired();

                    b.Property<Guid>("BudgetId");

                    b.Property<DateTime?>("BuyDate");

                    b.Property<string>("BuyerId");

                    b.Property<string>("Comments");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 300);

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("BudgetId");

                    b.HasIndex("BuyerId");

                    b.ToTable("Plan");
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.Purchase", b =>
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
                        .HasAnnotation("MaxLength", 300);

                    b.Property<string>("Shop");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("BudgetId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("LastEditorId");

                    b.ToTable("Purchase");
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.Share", b =>
                {
                    b.Property<Guid>("BudgetId");

                    b.Property<string>("UserId");

                    b.Property<byte>("Access");

                    b.HasKey("BudgetId", "UserId");

                    b.HasIndex("BudgetId");

                    b.HasIndex("UserId");

                    b.ToTable("Share");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.Budget", b =>
                {
                    b.HasOne("DioLive.Cache.WebUI.Models.ApplicationUser", "Author")
                        .WithMany("Budgets")
                        .HasForeignKey("AuthorId");
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.Category", b =>
                {
                    b.HasOne("DioLive.Cache.WebUI.Models.Budget", "Budget")
                        .WithMany("Categories")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DioLive.Cache.WebUI.Models.ApplicationUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.CategoryLocalization", b =>
                {
                    b.HasOne("DioLive.Cache.WebUI.Models.Category", "Category")
                        .WithMany("Localizations")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.Options", b =>
                {
                    b.HasOne("DioLive.Cache.WebUI.Models.ApplicationUser", "User")
                        .WithOne("Options")
                        .HasForeignKey("DioLive.Cache.WebUI.Models.Options", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.Plan", b =>
                {
                    b.HasOne("DioLive.Cache.WebUI.Models.ApplicationUser", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("DioLive.Cache.WebUI.Models.Budget", "Budget")
                        .WithMany("Plans")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DioLive.Cache.WebUI.Models.ApplicationUser", "Buyer")
                        .WithMany()
                        .HasForeignKey("BuyerId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.Purchase", b =>
                {
                    b.HasOne("DioLive.Cache.WebUI.Models.ApplicationUser", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("DioLive.Cache.WebUI.Models.Budget", "Budget")
                        .WithMany("Purchases")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DioLive.Cache.WebUI.Models.Category", "Category")
                        .WithMany("Purchases")
                        .HasForeignKey("CategoryId");

                    b.HasOne("DioLive.Cache.WebUI.Models.ApplicationUser", "LastEditor")
                        .WithMany()
                        .HasForeignKey("LastEditorId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("DioLive.Cache.WebUI.Models.Share", b =>
                {
                    b.HasOne("DioLive.Cache.WebUI.Models.Budget", "Budget")
                        .WithMany("Shares")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DioLive.Cache.WebUI.Models.ApplicationUser", "User")
                        .WithMany("Shares")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("DioLive.Cache.WebUI.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("DioLive.Cache.WebUI.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DioLive.Cache.WebUI.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
