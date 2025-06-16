using Microsoft.EntityFrameworkCore;
using PharmaBack.Data;
using PharmaBack.DTO;
using PharmaBack.Models;

namespace PharmaBack.Services.Brands;

public sealed class BrandService(PharmaDbContext db) : IBrandService
{
    public async Task<List<BrandDto>> GetAllAsync(CancellationToken ct) =>
        await db.Brands.Select(b => new BrandDto(b.BrandId, b.Name)).ToListAsync(ct);

    public async Task<BrandDto?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await db
            .Brands.Where(b => b.BrandId == id)
            .Select(b => new BrandDto(b.BrandId, b.Name))
            .FirstOrDefaultAsync(ct);

    public async Task<BrandDto> CreateAsync(UpsertBrandDto dto, CancellationToken ct)
    {
        var brand = new Brand { Name = dto.Name };
        db.Brands.Add(brand);
        await db.SaveChangesAsync(ct);
        return new BrandDto(brand.BrandId, brand.Name);
    }

    public async Task<bool> UpdateAsync(Guid id, UpsertBrandDto dto, CancellationToken ct)
    {
        var brand = await db.Brands.FindAsync([id], ct);
        if (brand is null)
            return false;

        brand.Name = dto.Name;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var brand = await db.Brands.FindAsync([id], ct);
        if (brand is null)
            return false;

        db.Brands.Remove(brand);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
