using AppForSN.Views;
using System.Windows;

namespace AppForSN
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigateToAuthPage();

        }

        public void NavigateToAuthPage()
        {
            MainFrame.Navigate(new AuthPage(this));
        }

        public void NavigateToRegisterPage()
        {
            MainFrame.Navigate(new RegisterPage(this));
        }
    }
}
