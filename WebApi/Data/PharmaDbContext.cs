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
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PackageItem> PackageItems => Set<PackageItem>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionItem> TransactionItems => Set<TransactionItem>();
    public DbSet<ProductHistory> ProductHistories => Set<ProductHistory>();
    public DbSet<ProductHistoryChange> ProductHistoryChanges => Set<ProductHistoryChange>();
    public DbSet<ProductRestock> ProductRestocks => Set<ProductRestock>();
    public DbSet<ProductRestockItem> ProductRestockItems => Set<ProductRestockItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasIndex(p => p.Barcode).IsUnique();

        modelBuilder.Entity<PackageItem>().Property(bi => bi.Quantity).HasDefaultValue(0);
        modelBuilder
            .Entity<ProductHistory>()
            .HasMany(h => h.Changes)
            .WithOne(c => c.ProductHistory)
            .HasForeignKey(c => c.ProductHistoryId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder
            .Entity<AppUser>()
            .Property(u => u.Photo)
            .HasColumnType("bytea")
            .IsRequired(false);

        modelBuilder
            .Entity<Product>()
            .HasMany(p => p.History)
            .WithOne()
            .HasForeignKey(h => h.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductRestock>()
        .HasMany(r => r.Items)
        .WithOne(i => i.ProductRestock)
        .HasForeignKey(i => i.ProductRestockId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductRestockItem>()
            .HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

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
