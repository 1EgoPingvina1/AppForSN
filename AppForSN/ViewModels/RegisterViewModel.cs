using AppForSNForUsers.Contracts;
using AppForSNForUsers.DTOs;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppForSNForUsers.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IAuthService _authService;

        public ICommand RegisterCommand { get; }
        public RelayCommand NavigateToLoginCommand { get; }

        public RegisterViewModel(MainViewModel mainViewModel, IAuthService authService)
        {
            _mainViewModel = mainViewModel;
            _authService = authService;

            RegisterCommand = new RelayCommand(async () => await RegisterAsync());
            NavigateToLoginCommand = new RelayCommand(OpenLoginWindow);
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); }
        }

        private string _secondName;
        public string SecondName
        {
            get => _secondName;
            set { _secondName = value; OnPropertyChanged(); }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password { get; set; }

        private string _gender;
        public string Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged(); }
        }

        private DateTime? _birthDate;
        public DateTime? BirthDate
        {
            get => _birthDate;
            set { _birthDate = value; OnPropertyChanged(); }
        }

        private bool _isAuthor;
        public bool IsAuthor
        {
            get => _isAuthor;
            set { _isAuthor = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private async Task RegisterAsync()
        {
            ErrorMessage = "";

            try
            {
                var dto = new RegisterDTO
                {
                    FirstName = FirstName,
                    SecondName = SecondName,
                    LastName = LastName,
                    Username = Username,
                    Password = Password,
                    Gender = Gender,
                    Birthday = BirthDate,
                    IsAuthor = IsAuthor
                };

                var result = await _authService.RegisterAsync(dto);
                if (result != null)
                {
                    MessageBox.Show("Регистрация прошла успешно");
                    _mainViewModel.Navigate("Home");
                }
                else
                {
                    ErrorMessage = "Не удалось зарегистрироваться.";
                }
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

        public void OpenLoginWindow() => _mainViewModel.Navigate("Login");

        // 🔁 OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
