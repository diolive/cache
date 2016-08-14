using DioLive.Cache.WebUI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DioLive.Cache.WebUI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>()
                .HasIndex(c => new { c.OwnerId, c.Name })
                .IsUnique();

            builder.Entity<Purchase>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Purchases)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

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

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Budgets)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId);

            builder.Entity<Share>()
                .HasKey(s => new { s.BudgetId, s.UserId });

            builder.Entity<Budget>()
                .HasMany(b => b.Shares)
                .WithOne(s => s.Budget)
                .HasForeignKey(s => s.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Shares)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Options)
                .WithOne(o => o.User)
                .HasForeignKey<Options>(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Purchase>()
                .HasOne(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Purchase>()
                .HasOne(p => p.LastEditor)
                .WithMany()
                .HasForeignKey(p => p.LastEditorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Plan>()
                .HasOne(p => p.Budget)
                .WithMany(b => b.Plans)
                .HasForeignKey(p => p.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Plan>()
                .HasOne(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Plan>()
                .HasOne(p => p.Buyer)
                .WithMany()
                .HasForeignKey(p => p.BuyerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Category>()
                .HasMany(c => c.Localizations)
                .WithOne(l => l.Category)
                .HasForeignKey(l => l.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CategoryLocalization>()
                .HasKey(l => new { l.CategoryId, l.Culture });
        }

        public DbSet<Category> Category { get; set; }

        public DbSet<Purchase> Purchase { get; set; }

        public DbSet<Budget> Budget { get; set; }
    }
}