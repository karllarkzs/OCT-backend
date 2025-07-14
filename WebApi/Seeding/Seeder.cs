using System.Text.Json;
using PharmaBack.WebApi.Data;
using PharmaBack.WebApi.Models;

namespace PharmaBack.WebApi.Seeding;

public static class Seeder
{
    public static async Task SeedAsync(PharmaDbContext db, IWebHostEnvironment env)
    {
        if (db.Products.Any())
            return;

        if (db.Products.Any())
            return;

        var products = new List<Product>
        {
            new()
            {
                Barcode = "0000000",
                Brand = "Biogesic",
                Generic = "Paracetamol",
                RetailPrice = 26.77M,
                WholesalePrice = 5.38M,
                Quantity = 200,
                Location = "Shelf 5",
                MinStock = 11,

                IsDeleted = false,
                Category = "Analgesic",
                Formulation = "Tablet",
                Company = "Pharma Inc",
                Type = "OTC",
            },
            new()
            {
                Barcode = "0000001",
                Brand = "Alaxan FR",
                Generic = "Ibuprofen",
                RetailPrice = 32.89M,
                WholesalePrice = 22.09M,
                Quantity = 110,

                Location = "Shelf 1",
                MinStock = 10,

                IsDeleted = false,
                Category = "Antihistamine",
                Formulation = "Capsule",
                Company = "MediHealth",
                Type = "Prescription",
            },
            new()
            {
                Barcode = "0000002",
                Brand = "Neozep",
                Generic = "Phenylephrine",
                RetailPrice = 22.46M,
                WholesalePrice = 14.53M,
                Quantity = 13,

                Location = "Shelf 1",
                MinStock = 6,

                IsDeleted = false,
                Category = "Cough",
                Formulation = "Syrup",
                Company = "WellnessCorp",
                Type = "OTC",
            },
            new()
            {
                Barcode = "0000003",
                Brand = "Solmux",
                Generic = "Loratadine",
                RetailPrice = 42.24M,
                WholesalePrice = 21.07M,
                Quantity = 128,

                Location = "Shelf 2",
                MinStock = 15,

                IsDeleted = false,
                Category = "Cold",
                Formulation = "Suspension",
                Company = "BioPharm",
                Type = "Prescription",
            },
            new()
            {
                Barcode = "0000004",
                Brand = "Diatabs",
                Generic = "Cetirizine",
                RetailPrice = 24.48M,
                WholesalePrice = 19.43M,
                Quantity = 111,

                Location = "Shelf 3",
                MinStock = 18,

                IsDeleted = false,
                Category = "Digestive",
                Formulation = "Lozenge",
                Company = "CureAll",
                Type = "OTC",
            },
            new()
            {
                Barcode = "0000005",
                Brand = "Tempra",
                Generic = "Diphenhydramine",
                RetailPrice = 27.05M,
                WholesalePrice = 25.44M,
                Quantity = 164,

                Location = "Shelf 1",
                MinStock = 16,

                IsDeleted = false,
                Category = "Supplement",
                Formulation = "Drop",
                Company = "Pharma Inc",
                Type = "Prescription",
            },
            new()
            {
                Barcode = "0000006",
                Brand = "Tuseran Forte",
                Generic = "Ambroxol",
                RetailPrice = 32.14M,
                WholesalePrice = 28.44M,
                Quantity = 159,

                Location = "Shelf 1",
                MinStock = 10,

                IsDeleted = false,
                Category = "Analgesic",
                Formulation = "Inhaler",
                Company = "MediHealth",
                Type = "OTC",
            },
            new()
            {
                Barcode = "0000007",
                Brand = "Bioflu",
                Generic = "Loperamide",
                RetailPrice = 26.91M,
                WholesalePrice = 29.73M,
                Quantity = 108,

                Location = "Shelf 2",
                MinStock = 16,

                IsDeleted = false,
                Category = "Antihistamine",
                Formulation = "Tablet",
                Company = "WellnessCorp",
                Type = "Prescription",
            },
            new()
            {
                Barcode = "0000008",
                Brand = "Decolgen",
                Generic = "Ascorbic Acid",
                RetailPrice = 16.68M,
                WholesalePrice = 8.29M,
                Quantity = 161,

                Location = "Shelf 1",
                MinStock = 7,

                IsDeleted = false,
                Category = "Cough",
                Formulation = "Capsule",
                Company = "BioPharm",
                Type = "OTC",
            },
            new()
            {
                Barcode = "0000009",
                Brand = "Benadryl",
                Generic = "Aluminum Hydroxide",
                RetailPrice = 13.67M,
                WholesalePrice = 15.66M,
                Quantity = 19,

                Location = "Shelf 2",
                MinStock = 8,

                IsDeleted = false,
                Category = "Cold",
                Formulation = "Syrup",
                Company = "CureAll",
                Type = "Prescription",
            },
            new()
            {
                Barcode = "0000010",
                Brand = "Claritin",
                Generic = "Paracetamol",
                RetailPrice = 18.1M,
                WholesalePrice = 10.0M,
                Quantity = 27,

                Location = "Shelf 4",
                MinStock = 14,

                IsDeleted = false,
                Category = "Digestive",
                Formulation = "Suspension",
                Company = "Pharma Inc",
                Type = "OTC",
            },
            new()
            {
                Barcode = "0000011",
                Brand = "Mucinex",
                Generic = "Ibuprofen",
                RetailPrice = 22.46M,
                WholesalePrice = 6.06M,
                Quantity = 96,

                Location = "Shelf 3",
                MinStock = 6,

                IsDeleted = false,
                Category = "Supplement",
                Formulation = "Lozenge",
                Company = "MediHealth",
                Type = "Prescription",
            },
            new()
            {
                Barcode = "0000012",
                Brand = "Zyrtec",
                Generic = "Phenylephrine",
                RetailPrice = 36.52M,
                WholesalePrice = 9.17M,
                Quantity = 59,

                Location = "Shelf 2",
                MinStock = 7,

                IsDeleted = false,
                Category = "Analgesic",
                Formulation = "Drop",
                Company = "WellnessCorp",
                Type = "OTC",
            },
            new()
            {
                Barcode = "0000013",
                Brand = "Cetirizine",
                Generic = "Loratadine",
                RetailPrice = 42.6M,
                WholesalePrice = 29.7M,
                Quantity = 71,

                Location = "Shelf 4",
                MinStock = 9,

                IsDeleted = false,
                Category = "Antihistamine",
                Formulation = "Inhaler",
                Company = "BioPharm",
                Type = "Prescription",
            },
            new()
            {
                Barcode = "0000014",
                Brand = "Ibuprofen",
                Generic = "Cetirizine",
                RetailPrice = 26.64M,
                WholesalePrice = 23.63M,
                Quantity = 140,

                Location = "Shelf 1",
                MinStock = 10,

                IsDeleted = false,
                Category = "Cough",
                Formulation = "Tablet",
                Company = "CureAll",
                Type = "OTC",
            },
            new()
            {
                Barcode = "0000015",
                Brand = "Paracetamol",
                Generic = "Diphenhydramine",
                RetailPrice = 37.33M,
                WholesalePrice = 19.81M,
                Quantity = 96,

                Location = "Shelf 4",
                MinStock = 19,

                IsDeleted = false,
                Category = "Cold",
                Formulation = "Capsule",
                Company = "Pharma Inc",
                Type = "Prescription",
            },
            new()
            {
                Barcode = "0000016",
                Brand = "Ambroxol",
                Generic = "Ambroxol",
                RetailPrice = 27.97M,
                WholesalePrice = 28.13M,
                Quantity = 24,

                Location = "Shelf 4",
                MinStock = 9,

                IsDeleted = false,
                Category = "Digestive",
                Formulation = "Syrup",
                Company = "MediHealth",
                Type = "OTC",
            },
            new()
            {
                Barcode = "0000017",
                Brand = "Loperamide",
                Generic = "Loperamide",
                RetailPrice = 49.25M,
                WholesalePrice = 19.82M,
                Quantity = 49,

                Location = "Shelf 5",
                MinStock = 15,

                IsDeleted = false,
                Category = "Supplement",
                Formulation = "Suspension",
                Company = "WellnessCorp",
                Type = "Prescription",
            },
            new()
            {
                Barcode = "0000018",
                Brand = "Antacid",
                Generic = "Ascorbic Acid",
                RetailPrice = 49.26M,
                WholesalePrice = 19.09M,
                Quantity = 66,

                Location = "Shelf 1",
                MinStock = 14,

                IsDeleted = false,
                Category = "Analgesic",
                Formulation = "Lozenge",
                Company = "BioPharm",
                Type = "OTC",
            },
            new()
            {
                Barcode = "0000019",
                Brand = "Vitamin C",
                Generic = "Aluminum Hydroxide",
                RetailPrice = 21.38M,
                WholesalePrice = 15.44M,
                Quantity = 35,

                Location = "Shelf 5",
                MinStock = 19,

                IsDeleted = false,
                Category = "Antihistamine",
                Formulation = "Drop",
                Company = "CureAll",
                Type = "Prescription",
            },
        };

        db.Products.AddRange(products);
        await db.SaveChangesAsync();
    }
}
