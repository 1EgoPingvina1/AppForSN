using AppForSNForUsers.Contracts;
using AppForSNForUsers.DTOs;
using AppForSNForUsers.Services;
using AppForSNForUsers.Views.Authorization;
using AppForSNForUsers.Views.Dashboard;
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
        private readonly AuthService _authService;

        // Это событие будет вызвано при успешной авторизации
        public event Action LoginSucceeded;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(async () => await LoginAsync(), () => true);

            NavigateToRegistrationCommand = new RelayCommand(OpenRegistrationWindow);
        }

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password { get; set; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public ICommand NavigateToRegistrationCommand { get; set; }

        private async Task LoginAsync()
        {
            ErrorMessage = "";

            try
            {
                var dto = new LoginDTO { Username = Username, Password = Password };
                var user = await _authService.LoginAsync(dto);
                ErrorMessage = $"Добро пожаловать, {user.Username}";

                LoginSucceeded?.Invoke(); // Сообщаем о входе
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                var registerView = new RegisterView(); // Убедись, что это Window
                registerView.Show();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
