using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace oopCursed.DB
{
    public class ProductList
    {
        // Collection to store products
        public ObservableCollection<Product> Products { get; set; }
        private ProductManagerContext dbContext; // Database context

        // Constructor
        public ProductList()
        {
            Products = new ObservableCollection<Product>(); // Initialize the product collection
            dbContext = new ProductManagerContext(); // Initialize the database context
            LoadProductsFromDatabase();
        }

        // Add a new product to the collection
        public void AddProduct(Product newProduct)
        {
            newProduct.Id = 0; // Assign Id of 0 to the newly added product
            Products.Add(newProduct);
        }

        // Remove a product from the collection and database
        public void RemoveProduct(Product productToRemove)
        {
            Products.Remove(productToRemove);
            dbContext.Products.Remove(productToRemove);
            dbContext.SaveChanges(); // Save changes to the database
        }

        // Load products from the database based on the current warehouse ID
        public void LoadProductsFromDatabase()
        {
            // Fetch products from the database and add them to the ObservableCollection
            var productsFromDatabase = dbContext.Products.ToList();
            Products.Clear(); // Clear existing products
            foreach (var product in productsFromDatabase)
            {
                Products.Add(product);
            }
        }

        // Get products expiring in a specific month
        public ObservableCollection<Product> GetProductsExpiringInMonth(int selectedMonth)
        {
            // Return the filtered collection based on the selected month
            return new ObservableCollection<Product>(Products
                .Where(p => p.ManufactureDate.HasValue && p.ManufactureDate.Value.Month == selectedMonth));
        }

        // Get products within a specified price range
        public ObservableCollection<Product> GetProductsInPriceRange(float minPrice, float maxPrice)
        {
            // Return the filtered collection based on the price range
            return new ObservableCollection<Product>(Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice));
        }

        // Group products by manufacture date and type
        public Dictionary<DateTime, Dictionary<string, List<Product>>> GroupProductsByManufactureDateAndType()
        {
            return Products
                .GroupBy(p => p.ManufactureDate)
                .ToDictionary(g => g.Key ?? DateTime.MinValue,
                              g => g.GroupBy(p => p.Type).ToDictionary(typeGroup => typeGroup.Key, typeGroup => typeGroup.ToList()));
        }

        // Group and sort products by price        
        public ObservableCollection<Product> GroupProductsByPrice()
        {
            var GroupProductsByPrice = new ObservableCollection<Product>(Products.OrderBy(p => p.Price));
            return GroupProductsByPrice;
        }



        // Calculate and display the total cost of products by type
        public Dictionary<string, decimal?> CalculateAndDisplayTotalCostByType()
        {
            var totalCostByType = Products
                .GroupBy(p => p.Type)
                .ToDictionary(g => g.Key, g => (decimal?)g.Sum(p => (decimal?)p.Price * p.Quantity));

            var sortedTotalCostByType = totalCostByType
                .OrderByDescending(kv => kv.Value)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            return sortedTotalCostByType;
        }


        // Display products with the same manufacture date separately for each type
        public Dictionary<DateTime, Dictionary<string, List<Product>>> DisplayProductsByManufactureDateAndType()
        {
            // Group products by manufacture date and type
            var groupedProducts = Products
                .GroupBy(p => new { p.ManufactureDate, p.Type })
                .OrderBy(g => g.Key.ManufactureDate)
                .ThenBy(g => g.Key.Type)
                .ToDictionary(
                    g => g.Key.ManufactureDate ?? DateTime.MinValue,
                    g => g.GroupBy(p => p.Type).ToDictionary(typeGroup => typeGroup.Key, typeGroup => typeGroup.ToList())
                );

            return groupedProducts;
        }

    }
}
