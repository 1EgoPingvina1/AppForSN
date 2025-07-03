using System.Windows;
using System;
using System.Windows.Controls;

namespace AppForSNForUsers.Views.Pages
{

    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(6);
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                NavigationService?.Navigate(new UserWindowPage());
            };
            timer.Start();
        }
    }
}
