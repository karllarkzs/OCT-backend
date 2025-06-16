using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmaBack.Models;

namespace PharmaBack.Data;

public class PharmaDbContext(DbContextOptions<PharmaDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole, string>(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ConsumableExtension> ConsumableExtensions => Set<ConsumableExtension>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Formulation> Formulations => Set<Formulation>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<InventoryBatch> InventoryBatches => Set<InventoryBatch>();
    public DbSet<Bundle> Bundles => Set<Bundle>();
    public DbSet<BundleItem> BundleItems => Set<BundleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasIndex(p => p.Barcode).IsUnique();

        modelBuilder
            .Entity<ConsumableExtension>()
            .HasOne(r => r.Product)
            .WithOne(p => p.Consumable)
            .HasForeignKey<ConsumableExtension>(r => r.ProductId);

        modelBuilder
            .Entity<BundleItem>()
            .HasOne(bi => bi.Bundle)
            .WithMany(b => b.BundleItems)
            .HasForeignKey(bi => bi.BundleId);

        modelBuilder
            .Entity<BundleItem>()
            .HasOne(bi => bi.Product)
            .WithMany(p => p.BundleItems)
            .HasForeignKey(bi => bi.ProductId);
        modelBuilder
            .Entity<AppUser>()
            .Property(u => u.Photo)
            .HasColumnType("bytea")
            .IsRequired(false);
    }
}
