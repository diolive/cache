using DioLive.Cache.Storage.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Budget = DioLive.Cache.Storage.Legacy.Models.Budget;
using Category = DioLive.Cache.Storage.Legacy.Models.Category;
using Purchase = DioLive.Cache.Storage.Legacy.Models.Purchase;

namespace DioLive.Cache.Storage.Legacy.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Category> Category { get; set; }

		public DbSet<Purchase> Purchase { get; set; }

		public DbSet<Budget> Budget { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			ConfigureBudget();
			ConfigureCategory();
			ConfigureCategoryLocalization();
			ConfigureOptions();
			ConfigurePlan();
			ConfigurePurchase();
			ConfigureShare();

			void ConfigureBudget()
			{
				builder.Entity<Budget>()
					.HasMany(b => b.Categories)
					.WithOne(c => c.Budget)
					.HasForeignKey(c => c.BudgetId)
					.OnDelete(DeleteBehavior.Cascade);

				builder.Entity<Budget>()
					.HasMany(b => b.Purchases)
					.WithOne(p => p.Budget)
					.HasForeignKey(p => p.BudgetId)
					.OnDelete(DeleteBehavior.Cascade);

				builder.Entity<Budget>()
					.HasMany(b => b.Shares)
					.WithOne()
					.HasForeignKey(s => s.BudgetId)
					.OnDelete(DeleteBehavior.Cascade);

				builder.Entity<Budget>().Property(b => b.Version)
					.HasDefaultValue((byte)1);

				builder.Entity<Budget>().Property(b => b.Name)
					.IsRequired()
					.HasMaxLength(200);

				builder.Entity<Budget>()
					.HasOne<IdentityUser>()
					.WithMany()
					.HasForeignKey(b => b.AuthorId)
					.OnDelete(DeleteBehavior.Restrict);
			}

			void ConfigureCategory()
			{
				builder.Entity<Category>()
					.HasIndex(c => new { c.BudgetId, c.Name })
					.IsUnique();

				builder.Entity<Category>()
					.HasMany(c => c.Localizations)
					.WithOne()
					.HasForeignKey(l => l.CategoryId)
					.OnDelete(DeleteBehavior.Cascade);

				builder.Entity<Category>()
					.HasOne<Category>()
					.WithMany()
					.HasForeignKey(c => c.ParentId)
					.OnDelete(DeleteBehavior.Restrict);

				builder.Entity<Category>().Property(c => c.Color)
					.HasDefaultValueSql("ABS(CHECKSUM(NEWID()) % 16777216)");

				builder.Entity<Category>().Property(c => c.Name)
					.IsRequired()
					.HasMaxLength(300);
			}

			void ConfigureCategoryLocalization()
			{
				builder.Entity<CategoryLocalization>()
					.HasKey(l => new { l.CategoryId, l.Culture });

				builder.Entity<CategoryLocalization>().Property(l => l.Culture)
					.IsRequired()
					.HasMaxLength(10);

				builder.Entity<CategoryLocalization>().Property(l => l.Name)
					.IsRequired()
					.HasMaxLength(50);
			}

			void ConfigureOptions()
			{
				builder.Entity<Options>()
					.HasKey(o => o.UserId);

				builder.Entity<Options>()
					.HasOne<IdentityUser>()
					.WithOne()
					.HasForeignKey<Options>(o => o.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			}

			void ConfigurePlan()
			{
				builder.Entity<Plan>()
					.HasOne<IdentityUser>()
					.WithMany()
					.HasForeignKey(p => p.AuthorId)
					.OnDelete(DeleteBehavior.Restrict);

				builder.Entity<Plan>()
					.HasOne<Budget>()
					.WithMany()
					.HasForeignKey(p => p.BudgetId)
					.OnDelete(DeleteBehavior.Cascade);

				builder.Entity<Plan>()
					.HasOne<IdentityUser>()
					.WithMany()
					.HasForeignKey(p => p.BuyerId)
					.OnDelete(DeleteBehavior.SetNull);

				builder.Entity<Plan>().Property(p => p.AuthorId)
					.IsRequired();

				builder.Entity<Plan>().Property(p => p.Name)
					.IsRequired()
					.HasMaxLength(300);

				builder.Entity<Plan>().Property(p => p.Comments)
					.HasMaxLength(500);
			}

			void ConfigurePurchase()
			{
				builder.Entity<Purchase>()
					.HasOne<IdentityUser>()
					.WithMany()
					.HasForeignKey(p => p.AuthorId)
					.OnDelete(DeleteBehavior.Restrict);

				builder.Entity<Purchase>()
					.HasOne(p => p.Category)
					.WithMany(c => c.Purchases)
					.HasForeignKey(p => p.CategoryId)
					.OnDelete(DeleteBehavior.Restrict);

				builder.Entity<Purchase>()
					.HasOne<IdentityUser>()
					.WithMany()
					.HasForeignKey(p => p.LastEditorId)
					.OnDelete(DeleteBehavior.SetNull);

				builder.Entity<Purchase>().Property(p => p.AuthorId)
					.IsRequired();

				builder.Entity<Purchase>().Property(p => p.Date)
					.HasColumnType("date");

				builder.Entity<Purchase>().Property(p => p.Name)
					.IsRequired()
					.HasMaxLength(300);

				builder.Entity<Purchase>().Property(p => p.Shop)
					.HasMaxLength(200);

				builder.Entity<Purchase>().Property(p => p.Comments)
					.HasMaxLength(500);
			}

			void ConfigureShare()
			{
				builder.Entity<Share>()
					.HasKey(s => new { s.BudgetId, s.UserId });

				builder.Entity<Share>()
					.HasOne<IdentityUser>()
					.WithMany()
					.HasForeignKey(s => s.UserId)
					.OnDelete(DeleteBehavior.Cascade);

				builder.Entity<Share>().Property(s => s.UserId)
					.IsRequired();
			}
		}
	}
}