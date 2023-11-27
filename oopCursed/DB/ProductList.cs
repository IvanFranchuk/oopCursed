using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
//using OfficeOpenXml;

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
                // Show an error message for exceptions
                MessageBox.Show($"Error adding product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //remove product from list and database
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
                // Show an error message for exceptions
                MessageBox.Show($"Error removing product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //Read from file
        public void ReadProductsByTypeFromFile(string filePath)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] values = line.Split(',');

                    if (values.Length == 6)
                    {
                        string productName = values[0].Trim();
                        int productPrice = int.Parse(values[1].Trim());
                        DateTime manufactureDate = DateTime.Parse(values[2].Trim());
                        string productType = values[3].Trim();
                        int productQuantity = int.Parse(values[4].Trim());
                        DateTime shelfLife = DateTime.Parse(values[5].Trim());

                        if (productType.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                        {
                            Product newProduct = new Product
                            {
                                Name = productName,
                                Price = productPrice,
                                ManufactureDate = manufactureDate,
                                Type = productType,
                                Quantity = productQuantity,
                                ShelfLife = shelfLife,
                            };
                            AddProduct(newProduct);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Invalid line: {line}", "Invalid Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading products from file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ReadProductsFromFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] values = line.Split(',');

                    if (values.Length == 6)
                    {
                        string productName = values[0].Trim();
                        int productPrice = int.Parse(values[1].Trim());
                        DateTime manufactureDate = DateTime.Parse(values[2].Trim());
                        string productType = values[3].Trim();
                        int productQuantity = int.Parse(values[4].Trim());
                        DateTime shelfLife = DateTime.Parse(values[5].Trim());

                        Product newProduct = new Product
                        {
                            Name = productName,
                            Price = productPrice,
                            ManufactureDate = manufactureDate,
                            Type = productType,
                            Quantity = productQuantity,
                            ShelfLife = shelfLife,
                        };

                        AddProduct(newProduct);
                    }
                    else
                    {
                        MessageBox.Show($"Invalid line: {line}", "Invalid Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading products from file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Write to file
        public void WriteProductsByTypeToFile(string filePath)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                List<string> productStrings = Products
                    .Where(p => p.Type.Equals(fileName, StringComparison.OrdinalIgnoreCase)) 
                    .Select(p => $"{p.Name},{p.Price},{p.ManufactureDate:yyyy-MM-dd},{p.Type},{p.Quantity},{p.ShelfLife:yyyy-MM-dd}")
                    .ToList();

                File.WriteAllLines(filePath, productStrings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing products to file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void WriteProductsToFile(string filePath)
        {
            try
            {
                List<string> linesToWrite = new List<string>();

                foreach (var product in Products)
                {
                    string line = $"{product.Name},{product.Price},{product.ManufactureDate:yyyy-MM-dd},{product.Type},{product.Quantity},{product.ShelfLife:yyyy-MM-dd}";
                    linesToWrite.Add(line);
                }

                File.WriteAllLines(filePath, linesToWrite);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing products to file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        // Load products from the database based on the current user's ID
        public void LoadProductsFromDatabase()
        {
            if (UserSession.UserId != 0)
            {                
                var productsFromDatabase = dbContext.Products
                                                    .Where(p => p.Warehouse.Userid == UserSession.UserId)
                                                    .ToList();

                Products.Clear();
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
            return new ObservableCollection<Product>(Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice));
        }

        //Get type with shortest storage term
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

        //Get cost and sort by type 
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

        //group by date and type 
        public Dictionary<string, List<Product>> GetProductsByManufactureDateAndType()
        {
            var productsByManufactureDateAndType = Products
                .Where(p => p.ManufactureDate.HasValue) 
                .GroupBy(p => new { p.ManufactureDate.Value.Date, p.Type }) 
                .ToDictionary(
                    g => $"{g.Key.Type}-{g.Key.Date:yyyy-MM-dd}",
                    g => g.ToList()
                );

            return productsByManufactureDateAndType;
        }


        //group by price
        public Dictionary<int?, List<Product>> GetProductsByPrice()
        {
            var productsByPrice = Products
                .GroupBy(p => p.Price)
                .ToDictionary(
                    g => g.Key, 
                    g => g.ToList()
                );

            return productsByPrice;
        }


        //sorting
        public void SortProductsByName()
        {
            Products = new ObservableCollection<Product>(Products.OrderBy(p => p.Name));
        }

        public void SortProductsByQuantity()
        {
            Products = new ObservableCollection<Product>(Products.OrderBy(p => p.Quantity));
        }

        public void SortProductsByPrice()
        {
            Products = new ObservableCollection<Product>(Products.OrderBy(p => p.Price));
        }

        public void SortProductsByType()
        {
            Products = new ObservableCollection<Product>(Products.OrderBy(p => p.Type));
        }
        public void SortProductsByNameDescending()
        {
            Products = new ObservableCollection<Product>(Products.OrderByDescending(p => p.Name));
        }

        public void SortProductsByTypeDescending()
        {
            Products = new ObservableCollection<Product>(Products.OrderByDescending(p => p.Type));
        }

        public void SortProductsByPriceDescending()
        {
            Products = new ObservableCollection<Product>(Products.OrderByDescending(p => p.Price));
        }

        public void SortProductsByQuantityDescending()
        {
            Products = new ObservableCollection<Product>(Products.OrderByDescending(p => p.Quantity));
        }
    }
}
