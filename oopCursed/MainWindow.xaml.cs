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
using oopCursed.code1;
using DynamicData;

namespace oopCursed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProductList productList;
        public Dictionary<DateTime, Dictionary<string, List<DB.Product>>> GroupedProducts { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Name.Text = UserSession.UserName;
            Surname.Text = UserSession.UserSurname;

            LoadWarehouseName();
            productList = new ProductList();
            productList.AddProduct(new DB.Product(1, "bsefsfes", 231, new DateTime(2008, 3, 1, 7, 0, 0), "bebeb", 234, new DateTime(2009, 3, 1, 7, 0, 0), "P"));
            productList.AddProduct(new DB.Product(2, "bsefsfes", 231, new DateTime(2008, 3, 1, 7, 0, 0), "bebeb", 234, new DateTime(2013, 3, 1, 7, 0, 0), "P"));
            productList.AddProduct(new DB.Product(3, "bsefsfes", 231, new DateTime(2008, 5, 1, 7, 0, 0), "bebeb", 234, new DateTime(2013, 3, 1, 7, 0, 0), "P"));

            ProductDataGrid.ItemsSource = productList.Products;




            var converter = new BrushConverter();
        }

        //public ObservableCollection<DB.Product> products = new ObservableCollection<DB.Product>();
       //private List<DB.Product> ProductList = new List<DB.Product>();


        private void ShowProductsPriceInRangeButton_Click(object sender, RoutedEventArgs e)
        {
            float minPrice = 0.0f; // Replace with the actual minimum price
            float maxPrice = 11.0f; // Replace with the actual maximum price
            var productsInPriceRange = productList.GetProductsInPriceRange(minPrice, maxPrice);

            // Update the DataGrid with the filtered products
            ProductDataGrid.ItemsSource = productsInPriceRange;
        }
        private void ShowProductsByExpiryMonthButton_Click(object sender, RoutedEventArgs e) {
            int selectedMonth = 11; // You can replace this with the actual selected month
            var expiringProducts = productList.GetProductsExpiringInMonth(selectedMonth);

            // Update the DataGrid with the filtered products
            ProductDataGrid.ItemsSource = expiringProducts;
        }       
        private void GroupProductsByPriceButton_Click(object sender, RoutedEventArgs e) {
            var sortedProducts = productList.GroupProductsByPrice();
            ProductDataGrid.ItemsSource = sortedProducts;
        }


        private void GroupProductsByManufactureDateAndTypeButton_Click(object sender, RoutedEventArgs e)
        {
            GroupedProducts = productList.GroupProductsByManufactureDateAndType();
            ProductDataGrid.ItemsSource = null; // Clear the existing data
            ProductDataGrid.ItemsSource = GroupedProducts.SelectMany(dateGroup => dateGroup.Value.Select(typeGroup => new { ManufactureDate = dateGroup.Key, Type = typeGroup.Key, Products = typeGroup.Value }));
        }



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
            int productPrice = int.Parse(PriceTextBox.Text);
            string productType = TypeTextBox.Text;
            int productQuantity = int.Parse(QuantityTextBox.Text);
            DateTime manufactureDate = ManufactureDatePicker.SelectedDate ?? DateTime.Now;
            DateTime shelfLife = ShelfLifeDatePicker.SelectedDate ?? DateTime.Now;
            string charecter = CharacterTextBox.Text;

            // Створіть новий об'єкт продукту
            DB.Product newProduct = new DB.Product
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
            var productList = (ObservableCollection<DB.Product>)ProductDataGrid.ItemsSource;

            // Додайте новий продукт до списку
            productList.Add(newProduct);

            // Закрийте викидне вікно
            AddProductPopup.IsOpen = false;

            // Скиньте значення елементів введення
            ProductNameTextBox.Text = "";
            PriceTextBox.Text = "";
            TypeTextBox.Text = "";
            QuantityTextBox.Text = "";
            ManufactureDatePicker.SelectedDate = DateTime.Now;
            ShelfLifeDatePicker.SelectedDate = DateTime.Now;
            CharacterTextBox.Text = "";

            // Немає потреби в окремому методі для оновлення вмісту таблиці, оскільки ObservableColletion вже було змінено і DataGrid автоматично відобразить зміни.
        }


    }
}