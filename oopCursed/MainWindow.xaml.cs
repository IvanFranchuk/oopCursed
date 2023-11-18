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
    public partial class MainWindow : Window
    {
        private ProductList productList;
        public bool isDescending = false;
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

        private void ShowAllProductsButton_Click(object sender, RoutedEventArgs e)
        {
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            TypePriceResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;            
            ProductDataGrid.ItemsSource = productList.Products;
            ProductListPanel.Visibility = Visibility.Visible;
        }

        //|=============================================| TASK |=============================================|

        // 1) |=================| SHOW PODUCTS IN SELECTED MOUNTH |=================|
        private void SelectMonthButton_Click(object sender, RoutedEventArgs e)
        {
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            TypePriceResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;
            ProductListPanel.Visibility = Visibility.Visible;
            SelectMonthPopup.IsOpen = true;
        }
        
        private void MonthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                int selectedMonth = int.Parse(selectedItem.Tag.ToString());
            }
        }
        private void ShowProductsByExpiryMonthButton_Click(object sender, RoutedEventArgs e)
        {
            if (MonthComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                int selectedMonth = int.Parse(selectedItem.Tag.ToString());
                var expiringProducts = productList.GetProductsExpiringInMonth(selectedMonth);

                ProductDataGrid.ItemsSource = expiringProducts;
                SelectMonthPopup.IsOpen = false;
            }
            else
            {
                MessageBox.Show("Please select a month.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 2) |=================| SHOW PODUCTS IN PRICE RANGE |=================|

        private void SelectPriceRangeButton_Click(object sender, RoutedEventArgs e)
        {
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            TypePriceResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;
            ProductListPanel.Visibility = Visibility.Visible;
            SelectPriceRangePopup.IsOpen = true;
        }
        private void ShowProductsPriceInRangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(MinPriceTextBox.Text, out int minPrice))
            {
                SelectPriceRangePopup.IsOpen = false;
                MessageBox.Show("Будь ласка, введіть числове значення для мінімальної ціни.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(MaxPriceTextBox.Text, out int maxPrice))
            {
                SelectPriceRangePopup.IsOpen = false;
                MessageBox.Show("Будь ласка, введіть числове значення для максимальної ціни.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var productsInPriceRange = productList.GetProductsInPriceRange(minPrice, maxPrice);
            SelectPriceRangePopup.IsOpen = false;

            ProductDataGrid.ItemsSource = productsInPriceRange;
        }

        // 3) |=================| SHORTEST STORAGE TIME |=================|
        private void OpenShortestAverageStorageTermPopupButton_Click(object sender, RoutedEventArgs e)
        {
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            TypePriceResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;
            ProductDataGrid.ItemsSource = productList.Products;
            string ShortestAvarageStorageTerm = productList.GetTypeWithShortestAverageStorageTerm();
            ShortestAverageStorageTermPopup.IsOpen = true;
            ShortestAverageStorageTermTextBlock.Text = ShortestAvarageStorageTerm;
            ProductListPanel.Visibility = Visibility.Visible;
        }

        private void CloseShortestAverageStorageTermPopupButton_Click(object sender, RoutedEventArgs e)
        {
            ShortestAverageStorageTermPopup.IsOpen = false;
        }

        // 4) |=================| ALL TYPES SUM + SORTING |=================|
        private void ShowTypesWithPricePopupButton_Click(object sender, RoutedEventArgs e)
        {
            ProductListPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Collapsed;

            List<KeyValuePair<string, decimal?>> TypeList = productList.GetTotalCostByTypeSorted();
            TypePriceDataGrid.ItemsSource = TypeList;

            TypePriceResultPanel.Visibility = Visibility.Visible;
        }


        //5) |=================| SAME MANAFACTURE DATE + SAPARATED BY TYPES  |=================|
        private void GroupProductsByManufactureDateAndTypeButton_Click(object sender, RoutedEventArgs e)
        {
            ProductListPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;
            TypePriceResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Collapsed;


            var productsByManufactureDateAndType = productList.GetProductsByManufactureDateAndType();

            StringBuilder resultTextBuilder = new StringBuilder();
            foreach (var kvp in productsByManufactureDateAndType)
            {
                resultTextBuilder.AppendLine($"Group: {kvp.Key}");
                foreach (var product in kvp.Value)
                {
                    CultureInfo englishCulture = new CultureInfo("en-US");
                    resultTextBuilder.AppendLine($" - Name: {product.Name}, Type: {product.Type}");
                }
                resultTextBuilder.AppendLine();
            }
            GroupByTypeAndDateResultTextBlock.Text = " ";
            GroupByTypeAndDateResultTextBlock.Text = resultTextBuilder.ToString();
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Visible;
        }




        //6) |=================| GROUP BY PRICE |=================|
        private void GroupProductsByPriceButton_Click(object sender, RoutedEventArgs e)
        {
            ProductListPanel.Visibility = Visibility.Collapsed;
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            TypePriceResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Collapsed;

            GroupByPriceResultTextBlock.Text = "";

            var productsByPrice = productList.GetProductsByPrice();

            StringBuilder resultBuilder = new StringBuilder();
            foreach (var kvp in productsByPrice)
            {
                resultBuilder.AppendLine($"Price: {kvp.Key}");
                foreach (var product in kvp.Value)
                {
                    CultureInfo englishCulture = new CultureInfo("en-US");
                    resultBuilder.AppendLine($" - Name: {product.Name}, Type: {product.Type}, Manufacture Date: {product.ManufactureDate?.ToString("dd MMM yyyy", englishCulture)}");
                }
                resultBuilder.AppendLine();
            }

            GroupByPriceResultTextBlock.Text = resultBuilder.ToString();
            GroupByPriceResultPanel.Visibility = Visibility.Visible;
        }



        private void ExitGroupByTypeAndDate_Click(object sender, RoutedEventArgs e)
        {
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            TypePriceResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;
            ProductListPanel.Visibility = Visibility.Visible;
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
            ManufactureDatePicker.SelectedDate = DateTime.Now;
            ShelfLifeDatePicker.SelectedDate = DateTime.Now;
            TypeComboBox.SelectedIndex = -1;
        }

        private void AddProductConfirm_Click(object sender, RoutedEventArgs e)
        {
            string productName = ProductNameTextBox.Text;
            int productPrice = int.Parse(PriceTextBox.Text);
            string productType = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (TypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, оберіть тип товару.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int productQuantity = int.Parse(QuantityTextBox.Text);
            //DateTime manufactureDate = ManufactureDatePicker.SelectedDate ?? DateTime.Now;
            //DateTime shelfLife = ShelfLifeDatePicker.SelectedDate ?? DateTime.Now;
            if (!DateTime.TryParse(ManufactureDatePicker.Text, out DateTime manufactureDate))
            {
                MessageBox.Show("Будь ласка, введіть коректну дату виготовлення.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DateTime.TryParse(ShelfLifeDatePicker.Text, out DateTime shelfLife))
            {
                MessageBox.Show("Будь ласка, введіть коректний термін придатності.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            productList.AddProduct(new DB.Product(0, productName, productPrice, manufactureDate, productType, productQuantity, shelfLife));

            AddProductPopup.IsOpen = false;

            ProductNameTextBox.Text = "";
            PriceTextBox.Text = "";
            QuantityTextBox.Text = "";
            ManufactureDatePicker.SelectedDate = DateTime.Now;
            ShelfLifeDatePicker.SelectedDate = DateTime.Now;
            TypeComboBox.SelectedIndex = -1;
        }

        // |=================| DELETE PRODUCT |=================|
        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductDataGrid.SelectedItem != null && ProductDataGrid.SelectedItem is DB.Product selectedProduct)
                {
                    productList.RemoveProduct(selectedProduct);
                    ProductDataGrid.ItemsSource = productList.Products;
                }
                else
                {
                    MessageBox.Show("Please select a product to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                string filePath = openFileDialog.FileName;
                productList.ReadProductsFromFile(filePath);
                Console.WriteLine($"Вибраний файл: {filePath}");
            }
            else
            {
                Console.WriteLine("Користувач скасував вибір файлу.");
            }
           
        }
        // |=================| EXPORT TO FILE |=================|
        private void ExportProductsToFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = openFileDialog.FileName;
                productList.WriteProductsToFile(filePath);
                Console.WriteLine($"Вибраний файл: {filePath}");
            }
            else
            {
                Console.WriteLine("Користувач скасував вибір файлу.");
            }
        }
        //sort direction
        private void AscendingSorttb_Checked(object sender, RoutedEventArgs e)
        {
            isDescending = false;
        }
        private void DescendingSorttb_Checked(object sender, RoutedEventArgs e)
        {
            isDescending = true;
        }
        // |==========================| SORTING |=========================|
        private void SortByNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDescending)
            {
                productList.SortProductsByNameDescending();
            }
            else
            {
                productList.SortProductsByName();
            }
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            TypePriceResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;
            ProductDataGrid.ItemsSource = productList.Products;
            ProductListPanel.Visibility = Visibility.Visible;
        }
        private void SortByTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDescending) {
                productList.SortProductsByTypeDescending();
            }
            else {
                productList.SortProductsByType();
            }
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            TypePriceResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;
            ProductDataGrid.ItemsSource = productList.Products;
            ProductListPanel.Visibility = Visibility.Visible;
        }
        private void SortByPriceButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDescending)
            {
                productList.SortProductsByPriceDescending();
            }
            else
            {
                productList.SortProductsByPrice();
            }
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            TypePriceResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;
            ProductDataGrid.ItemsSource = productList.Products;
            ProductListPanel.Visibility = Visibility.Visible;
        }
        private void SortByQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDescending)
            {
                productList.SortProductsByQuantityDescending();
            }
            else
            {
                productList.SortProductsByQuantity();
            }
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            TypePriceResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;
            ProductDataGrid.ItemsSource = productList.Products;
            ProductListPanel.Visibility = Visibility.Visible;
        }

        // |==========================| USER INFO |=========================|
        private void UserInfoButton_Click(object sender, RoutedEventArgs e)
        {            
            ProductListPanel.Visibility = Visibility.Collapsed;
            GroupByPriceResultPanel.Visibility = Visibility.Collapsed;
            GroupByTypeAndDateResultPanel.Visibility = Visibility.Collapsed;
            TypePriceResultPanel.Visibility = Visibility.Collapsed;
            UserInfoPanel.Visibility = Visibility.Visible;
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            // Display the user edit popup
            EditUserPopup.IsOpen = true;

            // Set the initial values in the textboxes for editing
            EditNameTextBox.Text = UserSession.UserName;
            EditSurnameTextBox.Text = UserSession.UserSurname;
            EditWarehouseNameTextBox.Text = GetWarehouseName(UserSession.WarehouseId);

        }
        private string GetWarehouseName(int warehouseId)
        {
            using (var context = new ProductManagerContext())
            {
                var warehouse = context.Warehouses.FirstOrDefault(w => w.Id == warehouseId);
                return warehouse?.Name;
            }
        }

        private void UpdateUserDetails_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ProductManagerContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == UserSession.UserId);
                var warehouse = context.Warehouses.FirstOrDefault(w => w.Userid == UserSession.UserId);

                if (user != null && warehouse != null)
                {
                    user.Name = EditNameTextBox.Text;
                    user.Surname = EditSurnameTextBox.Text;
                    warehouse.Name = EditWarehouseNameTextBox.Text;

                    context.SaveChanges();
                    MessageBox.Show("User details updated successfully.");

                    UserSession.UserName = user.Name;
                    UserSession.UserSurname = user.Surname;

                    // Оновлення інтерфейсу з новими даними користувача
                    Name.Text = UserSession.UserName;
                    Surname.Text = UserSession.UserSurname;
                    // Оновлення назви складу користувача на інтерфейсі
                    WarehouseName.Text = GetWarehouseName(UserSession.WarehouseId);
                }
                else
                {
                    MessageBox.Show("User not found or warehouse not set.");
                }
            }

            // Закриття вікна редагування після оновлення
            EditUserPopup.IsOpen = false;
        }
    }
}