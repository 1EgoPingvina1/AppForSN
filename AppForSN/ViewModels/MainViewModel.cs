using AppForSNForUsers.Contracts;
using AppForSNForUsers.Views.Pages;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AppForSNForUsers.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentPage;
        public object CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateCommand { get; }

        public MainViewModel()
        {
            NavigateCommand = new NavigateCommand(Navigate);
            // Стартовая страница
            CurrentPage = new HomePage();
        }

        private void Navigate(object page)
        {
            switch (page)
            {
                case "Home":
                    CurrentPage = new HomePage();
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
