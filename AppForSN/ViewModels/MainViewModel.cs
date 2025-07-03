using AppForSNForUsers.Contracts;
using AppForSNForUsers.Views.Authorization;
using AppForSNForUsers.Views.Pages;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AppForSNForUsers.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;
        public ICommand NavigateCommand { get; }

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        public MainViewModel(IAuthService authService)
        {
            _authService = authService;
            NavigateCommand = new RelayCommand<string>(param => Navigate(param));
            Navigate("Login");
        }

        public void Navigate(string viewName)
        {
            switch (viewName)
            {
                case "Home":
                    CurrentView = new HomePage(); 
                    break;

                case "Login":
                    CurrentView = new LoginView(this,_authService); 
                    break;
                case "Register":
                    CurrentView = new RegisterView();
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
