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
    public partial class loginWindow : Window
    {
        public loginWindow()
        {
            InitializeComponent();
        }
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ProductManagerContext())
            {
                string enteredEmail = EmailTextBox.Text; // Отримайте введену адресу електронної пошти
                string enteredPassword = PasswordTextBox.Password; // Отримайте введений пароль

                // Знайдіть користувача в базі даних за введеною адресою електронної пошти
                var user = context.Users.FirstOrDefault(u => u.Email == enteredEmail);

                if (user != null)
                {
                    // Перевірка введеного паролю
                    if (user.Password == enteredPassword)
                    {
                        // Встановіть ім'я та прізвище користувача в UserSession
                        UserSession.UserName = user.Name;
                        UserSession.UserSurname = user.Surname;
                        UserSession.UserId = user.Id;

                        // Успішний вхід - покажіть вікно "guideWindow"
                        guideWindow objGuideWindow = new guideWindow();
                        this.Visibility = Visibility.Hidden;
                        objGuideWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Невірний пароль.");
                    }
                }
                else
                {
                    MessageBox.Show("Користувача з такою адресою електронної пошти не існує.");
                }
            }
        }

        private void OpenSingUpWindow_Click(object sender, RoutedEventArgs e)
        {
            SingUpWindow objSingUpWindow = new SingUpWindow();
            this.Visibility = Visibility.Hidden;
            objSingUpWindow.Show();
            Close();
        }


        private void DarkThemetb_Checked(object sender, RoutedEventArgs e)
        {
            AppTheme.ChangeTheme(new Uri("Themes/Dark.xaml", UriKind.Relative));
        }

        private void LightThemetb_Checked(object sender, RoutedEventArgs e)
        {
            AppTheme.ChangeTheme(new Uri("Themes/Light.xaml", UriKind.Relative));
        }
    }
}
