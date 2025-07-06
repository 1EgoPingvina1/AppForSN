using AppForSNForUsers.Contracts;
using System.Windows.Input;

namespace AppForSNForUsers.ViewModels
{
    public class SideBarViewModel
    {
        private readonly MainViewModel _mainViewModel;

        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }

        public SideBarViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            NavigateCommand = new RelayCommand(() => _mainViewModel.Navigate("Home")); // или нужный параметр
            LogoutCommand = new RelayCommand(() => Logout());
        }

        private void Logout()
        {
            AppState.Clear();
            _mainViewModel.Navigate("Login");
        }
    }
}
