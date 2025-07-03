using AppForSNForUsers.Services;
using AppForSNForUsers.ViewModels;
using System.Net.Http;
using System.Windows;

namespace AppForSNForUsers.Views.Dashboard
{
    /// <summary>
    /// Interaction logic for MainDashboardView.xaml
    /// </summary>
    public partial class MainDashboardView : Window
    {

        public MainDashboardView()
        {
            InitializeComponent();
            var httpClient = new HttpClient();
            var authService = new AuthService(httpClient);
            DataContext = new MainViewModel(authService);

        }
    }
}
