using System.Windows;
using System;
using System.Windows.Controls;
using AppForSNForUsers.ViewModels;

namespace AppForSNForUsers.Views.Pages
{

    public partial class HomePage : UserControl
    {
        private readonly MainViewModel _mainViewModel;
        public HomePage(MainViewModel mainViewModel)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(6);
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                _mainViewModel.Navigate("UserHomePage");
            };
            timer.Start();
        }
    }
}
