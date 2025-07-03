using AppForSNForUsers.ViewModels;
using System.Windows.Controls;

namespace AppForSNForUsers.Views.Pages
{
    /// <summary>
    /// Interaction logic for UserWindowPage.xaml
    /// </summary>
    public partial class UserWindowPage : Page
    {
        private readonly UserProjectsViewModel _viewModel = new UserProjectsViewModel();

        public UserWindowPage()
        {
            InitializeComponent();
            DataContext = _viewModel;

            Loaded += async (s, e) =>
            {
                string token = App.CurrentUserToken;
                await _viewModel.LoadAsync(token);
            };
        }
    }
}
