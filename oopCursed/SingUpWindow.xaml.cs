using oopCursed.DB;
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
using System.Windows.Shapes;

namespace oopCursed
{
    /// <summary>
    /// Interaction logic for SingUpWindow.xaml
    /// </summary>
    public partial class SingUpWindow : Window
    {
        private readonly ProductManagerContext dbContext;
        public SingUpWindow()
        {
            InitializeComponent();
            dbContext = new ProductManagerContext();
        }
        
        private void CreateWarehouse_ButtonClick(object sender, RoutedEventArgs e)
        {
            WarehouseInfoPopup.IsOpen = true;
        }
        
        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {

            string name = NameTextBox.Text;
            string surname = SuranameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Password;
            string confirmPassword = ConfirmPasswordTextBox.Password;
            string warehouseName = WarehouseNameTextBox.Text;

            // Perform simple validation
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if the user with the same email already exists
            if (dbContext.Users.Any(u => u.Email == email))
            {
                MessageBox.Show("User with this email already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create a new User entity
            var newUser = new User
            {
                Name = name,
                Surname = surname,
                Email = email,
                Password = password // Note: In a real application, you should hash the password before storing it
            };

            // Create a new Warehouse entity for the user
            var newWarehouse = new Warehouse
            {
                Name = warehouseName,
                Location = "Default Location",
                User = newUser
            };

            // Add the new user and warehouse to the database
            dbContext.Users.Add(newUser);
            dbContext.Warehouses.Add(newWarehouse);
            dbContext.SaveChanges();

            MessageBox.Show("User registered successfully with a new warehouse.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            WarehouseInfoPopup.IsOpen = false;

            loginWindow objLoginWindow = new loginWindow();
            this.Visibility = Visibility.Hidden;
            objLoginWindow.Show();
            Close();
        }

    }
}
