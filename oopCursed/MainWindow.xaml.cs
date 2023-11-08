using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using oopCursed.DB;
using System.Collections.ObjectModel;

namespace oopCursed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Name.Text = UserSession.UserName;
            Surname.Text = UserSession.UserSurname;

            LoadWarehouseName();

            var converter = new BrushConverter();
            ObservableCollection<Product> products = new ObservableCollection<Product>();
            products.Add(new Product(1, "bsefsfes", 231, new DateTime(2008, 3, 1, 7, 0, 0), "bebeb", 234, 2312, "P"));
            products.Add(new Product(products[0]));
            products.Add(new Product {Id = 4, Name = "4142fbebe", Price = 10, ManufactureDate = new DateTime(2008, 3, 1, 7, 0, 0), Type = "bebebww", Quantity = 100, ShelfLife = 120,/* MarkColor = (Brush)converter.ConvertFromString("#a4fffff"),*/ Character = "P" });
            
            ProductDataGrid.ItemsSource = products;
            LoadProductsFromDatabase();
        }


        private List<Product> ProductList = new List<Product>();
        private void SelectionSortButton_Click(object sender, RoutedEventArgs e) { }
        private void ShellSortButton_Click(object sender, RoutedEventArgs e) { }
        private void QuickSortButton_Click(object sender, RoutedEventArgs e) { }
        private void MergeSortButton_Click(object sender, RoutedEventArgs e) { }
        private void CountSortButton_Click(object sender, RoutedEventArgs e) { }

        private void DarkThemetb_Checked(object sender, RoutedEventArgs e)
        {
            AppTheme.ChangeTheme(new Uri("Themes/Dark.xaml", UriKind.Relative));
        }

        private void LightThemetb_Checked(object sender, RoutedEventArgs e)
        {
            AppTheme.ChangeTheme(new Uri("Themes/Light.xaml", UriKind.Relative));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void LoadWarehouseName()
        {
            // Отримайте ім'я складу користувача з бази даних, здійсніть відповідний запит до БД
            using (var context = new ProductManagerContext())
            {
                int userId = UserSession.UserId; // Отримайте ідентифікатор користувача

                if (userId != 0)
                {
                    var warehouse = context.Warehouses.FirstOrDefault(w => w.Userid == userId);

                    if (warehouse != null)
                    {
                        WarehouseName.Text = warehouse.Name;
                        UserSession.WarehouseId = warehouse.Id;
                    }
                }
            }
        }

        

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddProductPopup.IsOpen = true;
        }

        private void AddProductConfirm_Click(object sender, RoutedEventArgs e)
        {
            // Отримайте значення з елементів введення інформації про продукт
            string productName = ProductNameTextBox.Text;
            float productPrice = float.Parse(PriceTextBox.Text);
            string productType = TypeTextBox.Text;
            int productQuantity = int.Parse(QuantityTextBox.Text);
            DateTime manufactureDate = ManufactureDatePicker.SelectedDate ?? DateTime.Now;
            int shelfLife = int.Parse(ShelfLifeTextBox.Text);
            string charecter = CharacterTextBox.Text;

            // Створіть новий об'єкт продукту
            Product newProduct = new Product
            {
                Name = productName,
                Price = productPrice,
                Type = productType,
                Quantity = productQuantity,
                ManufactureDate = manufactureDate,
                ShelfLife = shelfLife,
                Character = charecter
                // Встановіть інші властивості продукту
            };

            // Отримайте джерело даних (ObservableCollection) для DataGrid
            var products = (ObservableCollection<Product>)ProductDataGrid.ItemsSource;

            // Додайте новий продукт до списку
            products.Add(newProduct);

            // Закрийте викидне вікно
            AddProductPopup.IsOpen = false;

            // Скиньте значення елементів введення
            ProductNameTextBox.Text = "";
            PriceTextBox.Text = "";
            TypeTextBox.Text = "";
            QuantityTextBox.Text = "";
            ManufactureDatePicker.SelectedDate = DateTime.Now;
            ShelfLifeTextBox.Text = "";
            CharacterTextBox.Text = "";

            // Немає потреби в окремому методі для оновлення вмісту таблиці, оскільки ObservableColletion вже було змінено і DataGrid автоматично відобразить зміни.
        }



        private void LoadProductsFromDatabase()
        {
            using (var context = new ProductManagerContext())
            {
                int warehouseId = UserSession.WarehouseId; // Get the current warehouse ID from UserSession

                if (warehouseId != 0)
                {
                    // Retrieve products that belong to the current warehouse
                    var warehouseProducts = context.Products.Where(p => p.WarehouseId == warehouseId).ToList();

                    // Initialize the products collection if it's null
                    var products = (ObservableCollection<Product>)ProductDataGrid.ItemsSource;
                    if (products == null)
                    {
                        products = new ObservableCollection<Product>();
                        ProductDataGrid.ItemsSource = products;
                    }

                    // Clear the existing products list (if needed)
                    products.Clear();

                    // Add the retrieved products to the list
                    foreach (var product in warehouseProducts)
                    {
                        products.Add(product);
                    }
                }
            }
        }





    }
}