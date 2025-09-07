using AppForSNForUsers.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows;

namespace AppForSNForUsers.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private object _currentViewModel;

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            // Подписываемся на событие смены ViewModel
            _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentViewModel = _navigationService.CurrentViewModel;
            });
        }

        public void Dispose()
        {
            _navigationService.CurrentViewModelChanged -= OnCurrentViewModelChanged;
        }
    }
}
