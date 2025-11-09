using System;

namespace AppForSNForUsers.Services
{
    public interface INavigationService
    {
        event Action CurrentViewModelChanged;

        object CurrentViewModel { get; }

        void NavigateTo<TViewModel>() where TViewModel : class;
    }
}
