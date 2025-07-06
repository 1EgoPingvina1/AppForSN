using AppForSNForUsers.Contracts;
using AppForSNForUsers.ViewModels;
using AppForSNForUsers.Views.Dashboard;
using System.Windows;
using System.Windows.Controls;

namespace AppForSNForUsers.Views.Authorization
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public RegisterView(MainViewModel mainDashboardView, IAuthService service)
        {
            InitializeComponent();
            DataContext = new RegisterViewModel(mainDashboardView, service);
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
