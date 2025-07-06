using AppForSNForUsers.Contracts;
using AppForSNForUsers.ViewModels;
using AppForSNForUsers.Views.Dashboard;
using System.Windows;
using System.Windows.Controls;

namespace AppForSNForUsers.Views.Authorization
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView(MainViewModel mainDashboardView,IAuthService service)
        {
            InitializeComponent();

            DataContext = new LoginViewModel(mainDashboardView,service);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel loginViewModel)
            {
                loginViewModel.Password = PasswordBox.Password;

                if (loginViewModel.LoginCommand.CanExecute(null))
                    loginViewModel.LoginCommand.Execute(null);
            }
        }

        private void MoveToRegister_Navigation(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel loginViewModel)
            {
                if (loginViewModel.NavigateToRegistrationCommand.CanExecute(null))
                    loginViewModel.NavigateToRegistrationCommand.Execute(null);
            }
        }
    }
}
