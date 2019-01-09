using DioLive.Cache.Storage.Legacy.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.Storage.Legacy.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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

			ConfigureApplicationUser();
			ConfigureBudget();
			ConfigureCategory();
			ConfigureCategoryLocalization();
			ConfigureOptions();
			ConfigurePlan();
			ConfigurePurchase();
			ConfigureShare();

			void ConfigureApplicationUser()
			{
				builder.Entity<ApplicationUser>()
					.HasMany(u => u.Budgets)
					.WithOne(b => b.Author)
					.HasForeignKey(b => b.AuthorId);

				builder.Entity<ApplicationUser>()
					.HasOne(u => u.Options)
					.WithOne(o => o.User)
					.HasForeignKey<Options>(o => o.UserId)
					.OnDelete(DeleteBehavior.Cascade);

				builder.Entity<ApplicationUser>()
					.HasMany(u => u.Shares)
					.WithOne(s => s.User)
					.HasForeignKey(s => s.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			}

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
					.WithOne(s => s.Budget)
					.HasForeignKey(s => s.BudgetId)
					.OnDelete(DeleteBehavior.Cascade);

				builder.Entity<Budget>().Property(b => b.Version)
					.HasDefaultValue((byte)1);

				builder.Entity<Budget>().Property(b => b.Name)
					.IsRequired()
					.HasMaxLength(200);

				builder.Entity<Budget>()
					.HasOne(b => b.Author)
					.WithMany(a => a.Budgets)
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
					.WithOne(l => l.Category)
					.HasForeignKey(l => l.CategoryId)
					.OnDelete(DeleteBehavior.Cascade);

				builder.Entity<Category>()
					.HasOne(c => c.Parent)
					.WithMany(c => c.Subcategories)
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
			}

			void ConfigurePlan()
			{
				builder.Entity<Plan>()
					.HasOne(p => p.Author)
					.WithMany()
					.HasForeignKey(p => p.AuthorId)
					.OnDelete(DeleteBehavior.Restrict);

				builder.Entity<Plan>()
					.HasOne(p => p.Budget)
					.WithMany(b => b.Plans)
					.HasForeignKey(p => p.BudgetId)
					.OnDelete(DeleteBehavior.Cascade);

				builder.Entity<Plan>()
					.HasOne(p => p.Buyer)
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
					.HasOne(p => p.Author)
					.WithMany()
					.HasForeignKey(p => p.AuthorId)
					.OnDelete(DeleteBehavior.Restrict);

				builder.Entity<Purchase>()
					.HasOne(p => p.Category)
					.WithMany(c => c.Purchases)
					.HasForeignKey(p => p.CategoryId)
					.OnDelete(DeleteBehavior.Restrict);

				builder.Entity<Purchase>()
					.HasOne(p => p.LastEditor)
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

				builder.Entity<Share>().Property(s => s.UserId)
					.IsRequired();
			}
		}
	}
}