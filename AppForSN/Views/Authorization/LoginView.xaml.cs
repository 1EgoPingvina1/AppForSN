using AppForSNForUsers.DTOs;
using AppForSNForUsers.Services;
using AppForSNForUsers.ViewModels;
using AppForSNForUsers.Views.Dashboard;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace AppForSNForUsers.Views.Authorization
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:60766/api/")
            };

            var authService = new AuthService(httpClient);
            _viewModel = new LoginViewModel(authService);
            _viewModel.LoginSucceeded += OnLoginSuccess;

            DataContext = _viewModel;
        }

        private void OnLoginSuccess()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var apiClient = new ApiClient("https://localhost:60766/api/");

                var dashboard = new MainDashboardView(apiClient);
                dashboard.Show();
                this.Close();
            });
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
            _viewModel.LoginCommand.Execute(null);
        }
    }
}
