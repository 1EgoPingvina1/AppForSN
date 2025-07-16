using AppForSNForUsers.Contracts;
using AppForSNForUsers.DTOs;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AppForSNForUsers.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;
        private readonly MainViewModel _mainViewModel;


        public ICommand LoginCommand { get; }
        public RelayCommand NavigateToRegistrationCommand { get; }
        public LoginViewModel(MainViewModel mainViewModel, IAuthService authService)
        {
            _authService = authService;
            _mainViewModel = mainViewModel;
            LoginCommand = new RelayCommand(async () => await LoginAsync());
            NavigateToRegistrationCommand = new RelayCommand(OpenRegistrationWindow);

        }

        private string _username;
        public string Username
        {
            get => _username;
            set 
            { 
                _username = value; 
                OnPropertyChanged();
            }
        }

        public string Password { get; set; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }



        private async Task LoginAsync()
        {
            ErrorMessage = "";

            try
            {
                var dto = new LoginDTO { Username = Username, Password = Password };
                var user = await _authService.LoginAsync(dto);
                if(user != null) 
                {
                    Password = string.Empty;
                    App.CurrentUserToken = user.Token;
                    _mainViewModel.Navigate("Home");
                }
                ErrorMessage = "Неверный логин / пароль";
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Ошибка запроса: {ex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }


        private void OpenRegistrationWindow()
        {
            _mainViewModel.Navigate("Register");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
