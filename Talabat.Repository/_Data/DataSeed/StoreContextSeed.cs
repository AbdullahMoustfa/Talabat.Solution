using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities.Oreder_Aggragate;
using Talabat.Core.Entities.Product;
using Talabat.Repository.Data;

namespace Talabat.Repository.Data.DataSeed
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext _dbContext)
        {  // SeedData Of ProductBrand,ProductCategory and Product

            if (!_dbContext.ProductBrands.Any())
            {
                var BrandData = File.ReadAllText("../Talabat.Repository/_Data/DataSeed/brands.json");
                var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandData);

                if (Brands?.Count > 0) // Brands is not null && Brands.Count > 0
                {
                    foreach (var brand in Brands)
                        await _dbContext.Set<ProductBrand>().AddAsync(brand);

                    await _dbContext.SaveChangesAsync();
                }
            }

            if (!_dbContext.ProductCategories.Any())
            {
                var CategoryData = File.ReadAllText("../Talabat.Repository/_Data/DataSeed/categories.json"); ;

                var Categories = JsonSerializer.Deserialize<List<ProductCategory>>(CategoryData);

                if (Categories?.Count > 0) // Categories is not null && Categories.Count>0
                {
                    foreach (var category in Categories)
                        await _dbContext.Set<ProductCategory>().AddAsync(category);

                    await _dbContext.SaveChangesAsync();
                }
            }

            if (!_dbContext.Products.Any())
            {
                var ProductData = File.ReadAllText("../Talabat.Repository/_Data/DataSeed/products.json");

                var Products = JsonSerializer.Deserialize<List<Product>>(ProductData);

                if (Products?.Count > 0)
                {
                    foreach (var product in Products)
                        await _dbContext.Set<Product>().AddAsync(product);

                    await _dbContext.SaveChangesAsync();
                }
            }

            if (!_dbContext.DeliveryMethods.Any())
            {
                var deliveryMethodsData = File.ReadAllText("../Talabat.Repository/_Data/DataSeed/delivery.json");

                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);

                if (deliveryMethods?.Count > 0)
                {
                    foreach (var deliveryMethod in deliveryMethods)
                        await _dbContext.Set<DeliveryMethod>().AddAsync(deliveryMethod);

                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
