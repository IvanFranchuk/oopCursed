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
using Microsoft.Win32;

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

            ProductDataGrid.ItemsSource = productList.Products;
            var converter = new BrushConverter();
        }          

        private void QuickSortButton_Click(object sender, RoutedEventArgs e) { }
        private void MergeSortButton_Click(object sender, RoutedEventArgs e) { }
        private void CountSortButton_Click(object sender, RoutedEventArgs e) { }

        //theme choose
        private void DarkThemetb_Checked(object sender, RoutedEventArgs e)
        {
            AppTheme.ChangeTheme(new Uri("Themes/Dark.xaml", UriKind.Relative));
        }
        private void LightThemetb_Checked(object sender, RoutedEventArgs e)
        {
            AppTheme.ChangeTheme(new Uri("Themes/Light.xaml", UriKind.Relative));
        }

        //window control
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

        //get users warehouse
        private void LoadWarehouseName()
        {
            using (var context = new ProductManagerContext())
            {
                int userId = UserSession.UserId;

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

        //|=============================================| TASK |=============================================|

        // 1) |=================| SHOW PODUCTS IN SELECTED MOUNTH |=================|
        private void SelectMonthButton_Click(object sender, RoutedEventArgs e)
        {
            ProductListPanel.Visibility = Visibility.Visible;
            SelectMounthPopup.IsOpen = true;
        }
        private void ShowProductsByExpiryMonthButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedMonth = int.Parse(MounthTextBox.Text);
            var expiringProducts = productList.GetProductsExpiringInMonth(selectedMonth);
            
            ProductDataGrid.ItemsSource = expiringProducts;
            SelectMounthPopup.IsOpen = false;
            MounthTextBox.Text = "";
        }

        // 2) |=================| SHOW PODUCTS IN PRICE RANGE |=================|

        private void SelectPriceRangeButton_Click(object sender, RoutedEventArgs e)
        {
            ProductListPanel.Visibility = Visibility.Visible;
            SelectPriceRangePopup.IsOpen = true;
        }
        private void ShowProductsPriceInRangeButton_Click(object sender, RoutedEventArgs e)
        {
            int minPrice = int.Parse(MinPriceTextBox.Text); // Replace with the actual minimum price
            int maxPrice = int.Parse(MaxPriceTextBox.Text); // Replace with the actual maximum price
            var productsInPriceRange = productList.GetProductsInPriceRange(minPrice, maxPrice);
            SelectPriceRangePopup.IsOpen = false;

            // Update the DataGrid with the filtered products
            ProductDataGrid.ItemsSource = productsInPriceRange;
        }

        // 3) |=================| SHORTEST STORAGE TIME |=================|
        private void OpenShortestAverageStorageTermPopupButton_Click(object sender, RoutedEventArgs e)
        {
            ProductListPanel.Visibility = Visibility.Visible;
            string ShortestAvarageStorageTerm = productList.GetTypeWithShortestAverageStorageTerm();
            ShortestAverageStorageTermPopup.IsOpen = true;

            ShortestAverageStorageTermTextBlock.Text = ShortestAvarageStorageTerm;
        }

        private void CloseShortestAverageStorageTermPopupButton_Click(object sender, RoutedEventArgs e)
        {
            ShortestAverageStorageTermPopup.IsOpen = false;
        }

        // 4) |=================| ALL TYPES SUM + SORTING |=================|
        private void ShowTypesWithPricePopupButton_Click(object sender, RoutedEventArgs e)
        {
            ProductListPanel.Visibility = Visibility.Visible;
            TypeWithPricePopup.IsOpen = true;
            List<KeyValuePair<string, decimal?>> TypeList = productList.GetTotalCostByTypeSorted();
            StringBuilder TypeListSB = new StringBuilder();
            foreach (var item in TypeList)
            {
                TypeListSB.AppendLine($"Type: {item.Key}, Total Cost: {item.Value}");
            }
            TypeListTextBox.Text = TypeListSB.ToString();
        }
        private void CloseTypeWithPricePopupButton_Click(object sender, RoutedEventArgs e)
        {
            TypeWithPricePopup.IsOpen = false;
        }

        //5) |=================| SAME MANAFACTURE DATE + SAPARATED BY TYPES  |=================|
        private void GroupProductsByManufactureDateAndTypeButton_Click(object sender, RoutedEventArgs e)
        {
            ProductListPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;

            var productsByManufactureDateAndType = productList.GetProductsByManufactureDateAndType();

            StringBuilder resultTextBuilder = new StringBuilder();
            foreach (var kvp in productsByManufactureDateAndType)
            {
                resultTextBuilder.AppendLine($"Key: {kvp.Key}"); // Додаємо ключ (тип товару та дата виготовлення)
                resultTextBuilder.AppendLine("Products:");
                foreach (var product in kvp.Value)
                {
                    resultTextBuilder.AppendLine($" - Name: {product.Name}, Type: {product.Type}, Manufacture Date: {product.ManufactureDate}");
                    // Додаємо додаткову інформацію про товар (назва, тип, дата виготовлення)
                }
            }
            // clear
            GroupByTypeAndDateResultTextBlock.Text = " ";
            // Відображаємо отриманий текст у блоку тексту
            GroupByTypeAndDateResultTextBlock.Text = resultTextBuilder.ToString();
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Visible;
        }



        //6) |=================| GROUP BY PRICE |=================|
        private void GroupProductsByPriceButton_Click(object sender, RoutedEventArgs e)
        {
            ProductListPanel.Visibility = Visibility.Collapsed;
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultTextBlock.Text = ""; // Очищення вмісту текстового блоку

            // Отримання словника товарів за однаковими цінами
            var productsByPrice = productList.GetProductsByPrice();

            // Виведення інформації про товари з однаковими цінами у текстовий блок
            StringBuilder resultBuilder = new StringBuilder();
            foreach (var kvp in productsByPrice)
            {
                resultBuilder.AppendLine($"Price: {kvp.Key}");
                foreach (var product in kvp.Value)
                {
                    resultBuilder.AppendLine($" - Name: {product.Name}, Type: {product.Type}, Manufacture Date: {product.ManufactureDate}");
                    // Додаткова інформація про товар (назва, тип, дата виготовлення)
                }
                resultBuilder.AppendLine();
            }

            GroupByPriceResultTextBlock.Text = resultBuilder.ToString(); // Оновлення вмісту текстового блоку
            GroupByPriceResultPanel.Visibility = Visibility.Visible;
        }

















        //|=============================================| EDIT CONTENT IN LIST |=============================================|
        // |=================| ADD NEW PRODUCT |=================|
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddProductPopup.IsOpen = true;
        }

        private void AddProductCancel_Click(object sender, RoutedEventArgs e)
        {
            AddProductPopup.IsOpen = false;
            // Скинути значення елементів введення
            ProductNameTextBox.Text = "";
            PriceTextBox.Text = "";
            QuantityTextBox.Text = "";
            CharacterTextBox.Text = "";
            ManufactureDatePicker.SelectedDate = DateTime.Now;
            ShelfLifeDatePicker.SelectedDate = DateTime.Now;
            TypeComboBox.SelectedIndex = -1;
        }

        private void AddProductConfirm_Click(object sender, RoutedEventArgs e)
        {
            // Отримати значення з елементів введення інформації про продукт
            string productName = ProductNameTextBox.Text;
            int productPrice = int.Parse(PriceTextBox.Text);
            string productType = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            int productQuantity = int.Parse(QuantityTextBox.Text);
            DateTime manufactureDate = ManufactureDatePicker.SelectedDate ?? DateTime.Now;
            DateTime shelfLife = ShelfLifeDatePicker.SelectedDate ?? DateTime.Now;
            string character = CharacterTextBox.Text;

            // Додати продукт до списку
            productList.AddProduct(new DB.Product(0, productName, productPrice, manufactureDate, productType, productQuantity, shelfLife, character));

            // Закрити викидне вікно
            AddProductPopup.IsOpen = false;

            // Скинути значення елементів введення
            ProductNameTextBox.Text = "";
            PriceTextBox.Text = "";
            QuantityTextBox.Text = "";
            CharacterTextBox.Text = "";
            ManufactureDatePicker.SelectedDate = DateTime.Now;
            ShelfLifeDatePicker.SelectedDate = DateTime.Now;
            TypeComboBox.SelectedIndex = -1;
        }

        // |=================| DELETE PRODUCT |=================|
        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if any item is selected in the DataGrid
                if (ProductDataGrid.SelectedItem != null && ProductDataGrid.SelectedItem is DB.Product selectedProduct)
                {
                    // Call the RemoveProduct method to remove the selected product
                    productList.RemoveProduct(selectedProduct);
                }
                else
                {
                    // Provide feedback to the user that no item is selected
                    MessageBox.Show("Please select a product to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it, show an error message)
                Console.WriteLine($"Error deleting product: {ex.Message}");
            }
        }

        // |=================| READ FROM FILE |=================|
        private void AddFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Отримати шлях до вибраного файлу
                string filePath = openFileDialog.FileName;
                productList.ReadProductsFromFile(filePath);

                // Тепер ви можете використовувати filePath для читання файлу чи інших операцій
                Console.WriteLine($"Вибраний файл: {filePath}");
            }
            else
            {
                Console.WriteLine("Користувач скасував вибір файлу.");
            }

            

            // Call the ReadProductsFromFile function
           
        }

    }
}