using AppForSNForUsers.Contracts;
using AppForSNForUsers.Views.Authorization;
using AppForSNForUsers.Views.Pages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AppForSNForUsers.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private readonly IAuthService _authService;
        public ICommand NavigateCommand { get; }

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        public bool HasErrors => _errors.Any();

        public MainViewModel(IAuthService authService)
        {
            _authService = authService;
            NavigateCommand = new RelayCommand(() => Navigate("Login")); 
            Navigate("Login");
        }

        protected void OnErrorsChanged(string propertyName)
       => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        // Добавляем ошибку для свойства
        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
                OnErrorsChanged(propertyName);
        }

        public void Navigate(string viewName)
        {
            switch (viewName)
            {
                case "Home":
                    CurrentView = new HomePage(this); 
                    break;

                case "Login":
                    CurrentView = new LoginView(this,_authService); 
                    break;
                case "Register":
                    CurrentView = new RegisterView(this,_authService);
                    break;
                case "MyProjects":
                    CurrentView = new MyProjectsView();
                    break;
                case "UserHomePage":
                    CurrentView = new UserWindowPage();
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
                return Enumerable.Empty<string>();

            return _errors[propertyName];
        }
    }
}
