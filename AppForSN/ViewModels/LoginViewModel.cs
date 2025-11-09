using AppForSNForUsers.Contracts;
using AppForSNForUsers.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AppForSNForUsers.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;
        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
        }

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        public string _password { get; set; }

        [RelayCommand]
        private async Task LoginAsync()
        {
            bool success = await _authService.LoginAsync(Username, Password);
            if (success)
            {
                // Если логин успешен, переходим на страницу проектов
                _navigationService.NavigateTo<ProjectsViewModel>();
            }
            else
            {
                // Показать ошибку
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
