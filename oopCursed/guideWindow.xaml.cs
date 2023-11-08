using oopCursed.guidePages;
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
    /// Interaction logic for guideWindow.xaml
    /// </summary>
    public partial class guideWindow : Window
    {
        public guideWindow()
        {
            InitializeComponent();
        }

        int currentQuidePage = 0;

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            currentQuidePage++;

            if (currentQuidePage == 0)
            {
                guide1.Visibility = Visibility.Visible;
                guide2.Visibility = Visibility.Collapsed;
                guide3.Visibility = Visibility.Collapsed;
            }
            else if (currentQuidePage == 1)
            {
                guide1.Visibility = Visibility.Collapsed;
                guide2.Visibility = Visibility.Visible;
                guide3.Visibility = Visibility.Collapsed;
            }
            else if (currentQuidePage == 2)
            {
                guide1.Visibility = Visibility.Collapsed;
                guide2.Visibility = Visibility.Collapsed;
                guide3.Visibility = Visibility.Visible;
            }
            else
            {
                MainWindow objMainWindow = new MainWindow();
                this.Visibility = Visibility.Hidden;
                objMainWindow.Show();
            }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow objMainWindow = new MainWindow();
            this.Visibility = Visibility.Hidden;
            objMainWindow.Show();
        }
    }
}
