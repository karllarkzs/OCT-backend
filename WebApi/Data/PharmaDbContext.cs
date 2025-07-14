using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmaBack.WebApi.Models;

namespace PharmaBack.WebApi.Data;

public class PharmaDbContext(DbContextOptions<PharmaDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole, string>(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Bundle> Bundles => Set<Bundle>();
    public DbSet<BundleItem> BundleItems => Set<BundleItem>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionItem> TransactionItems => Set<TransactionItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasIndex(p => p.Barcode).IsUnique();

        modelBuilder.Entity<BundleItem>().Property(bi => bi.Quantity).HasDefaultValue(0);

        modelBuilder
            .Entity<AppUser>()
            .Property(u => u.Photo)
            .HasColumnType("bytea")
            .IsRequired(false);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                continue;

            var param = Expression.Parameter(entityType.ClrType, "e");
            var prop = Expression.Property(param, nameof(ISoftDelete.IsDeleted));
            var body = Expression.Not(prop);
            var lambda = Expression.Lambda(body, param);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
}
