using System;
using Microsoft.Extensions.DependencyInjection;


namespace AppForSNForUsers.Services
{
    public class NavigationService : INavigationService
    {
        public event Action CurrentViewModelChanged;
        private object _currentViewModel;
        private readonly IServiceProvider _serviceProvider;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                _currentViewModel = value;
                // Оповещаем подписчиков (наш MainViewModel) о том, что ViewModel изменилась
                CurrentViewModelChanged?.Invoke();
            }
        }

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo<TViewModel>() where TViewModel : class
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<TViewModel>();
        }
    }
}
