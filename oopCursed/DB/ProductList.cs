using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;


namespace oopCursed.DB
{
    public class ProductList
    {
       
        public ObservableCollection<Product> Products { get; set; }  // Collection to store products
        private ProductManagerContext dbContext; // Database context

        // Constructor
        public ProductList()
        {
            Products = new ObservableCollection<Product>(); // Initialize the product collection
            dbContext = new ProductManagerContext(); // Initialize the database context
            LoadProductsFromDatabase();
        }

        // Add a new product to the collection and database
        public void AddProduct(Product newProduct)
        {
            try
            {
                newProduct.Id = 0; // Assign Id of 0 to the newly added product
                newProduct.WarehouseId = UserSession.WarehouseId;
                Products.Add(newProduct);
                dbContext.Products.Add(newProduct); // Add the new product to the database
                dbContext.SaveChanges(); // Save changes to the database
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it, show an error message)
                Console.WriteLine($"Error adding product: {ex.Message}");
            }
        }
        public void RemoveProduct(Product productToRemove)
        {
            try
            {
                Products.Remove(productToRemove); // Remove from the collection
                dbContext.Products.Remove(productToRemove); // Remove from the database
                dbContext.SaveChanges(); // Save changes to the database
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it, show an error message)
                Console.WriteLine($"Error removing product: {ex.Message}");
            }
        }

        public void ReadProductsFromFile(string filePath)
        {
            try
            {
                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    // Split the line into individual values
                    string[] values = line.Split(',');

                    // Check if the line contains the expected number of values
                    if (values.Length == 7)
                    {
                        // Parse values and create a new Product
                        string productName = values[0].Trim();
                        int productPrice = int.Parse(values[1].Trim());
                        DateTime manufactureDate = DateTime.Parse(values[2].Trim());
                        string productType = values[3].Trim();
                        int productQuantity = int.Parse(values[4].Trim());
                        DateTime shelfLife = DateTime.Parse(values[5].Trim());
                        string character = values[6].Trim();

                        // Create a new Product
                        Product newProduct = new Product
                        {
                            Name = productName,
                            Price = productPrice,
                            ManufactureDate = manufactureDate,
                            Type = productType,
                            Quantity = productQuantity,
                            ShelfLife = shelfLife,
                            Character = character
                        };

                        // Add the new product to the collection
                        AddProduct(newProduct);
                    }
                    else
                    {
                        // Log a warning or handle invalid lines as needed
                        Console.WriteLine($"Invalid line: {line}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log, display an error message)
                Console.WriteLine($"Error reading products from file: {ex.Message}");
            }
        }



        // Load products from the database based on the current user's ID
        public void LoadProductsFromDatabase()
        {
            if (UserSession.UserId != 0)
            {
                // Fetch products from the database for the current user and add them to the ObservableCollection
                var productsFromDatabase = dbContext.Products
                                                    .Where(p => p.Warehouse.Userid == UserSession.UserId)
                                                    .ToList();

                Products.Clear(); // Clear existing products
                foreach (var product in productsFromDatabase)
                {
                    Products.Add(product);
                }
            }
        }
        // Get products expiring in a specific month
        public ObservableCollection<Product> GetProductsExpiringInMonth(int selectedMonth)
        {
            // Return the filtered collection based on the selected month
            return new ObservableCollection<Product>(Products
                .Where(p => p.ShelfLife.HasValue && p.ShelfLife.Value.Month == selectedMonth));
        }

        // Get products within a specified price range
        public ObservableCollection<Product> GetProductsInPriceRange(float minPrice, float maxPrice)
        {
            // Return the filtered collection based on the price range
            return new ObservableCollection<Product>(Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice));
        }

        public string GetTypeWithShortestAverageStorageTerm()
        {
            var typeWithShortestAverageStorageTerm = Products
                .GroupBy(p => p.Type)
                .Select(group => new
                {
                    Type = group.Key,
                    AverageStorageTerm = group.Average(p => ((p.ShelfLife - p.ManufactureDate)?.Days) ?? 0)
                })
                .OrderBy(item => item.AverageStorageTerm)
                .FirstOrDefault()?.Type;

            return typeWithShortestAverageStorageTerm;
        }


        public List<KeyValuePair<string, decimal?>> GetTotalCostByTypeSorted()
        {
            var totalCostByType = Products
                .GroupBy(p => p.Type)
                .ToDictionary(g => g.Key, g => (decimal?)g.Sum(p => p.Price * p.Quantity));

            var sortedTotalCostByType = QuickSort(totalCostByType.ToList());

            return sortedTotalCostByType;
        }

        private List<KeyValuePair<string, decimal?>> QuickSort(List<KeyValuePair<string, decimal?>> list)
        {
            if (list.Count <= 1)
                return list;

            int pivotIndex = list.Count / 2;
            var pivot = list[pivotIndex];
            list.RemoveAt(pivotIndex);

            var lesser = new List<KeyValuePair<string, decimal?>>();
            var greater = new List<KeyValuePair<string, decimal?>>();

            foreach (var element in list)
            {
                if (element.Value <= pivot.Value)
                    lesser.Add(element);
                else
                    greater.Add(element);
            }

            var sortedList = new List<KeyValuePair<string, decimal?>>();
            sortedList.AddRange(QuickSort(lesser));
            sortedList.Add(pivot);
            sortedList.AddRange(QuickSort(greater));

            return sortedList;
        }

        public Dictionary<string, Dictionary<DateTime, List<Product>>> GetProductsByManufactureDateAndType()
        {
            var productsByManufactureDateAndType = Products
                .GroupBy(p => new { p.Type, p.ManufactureDate })
                .ToDictionary(
                    group => group.Key.Type,
                    group => group.GroupBy(p => p.ManufactureDate)
                                  .ToDictionary(innerGroup => innerGroup.Key ?? DateTime.MinValue, innerGroup => innerGroup.ToList())
                );

            return productsByManufactureDateAndType;
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
