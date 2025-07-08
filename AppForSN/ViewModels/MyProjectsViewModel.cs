using AppForSNForUsers.DTOs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace AppForSNForUsers.ViewModels
{
    public class MyProjectsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ProjectViewDTO> Projects { get; set; }

        public Visibility NoProjectsVisibility => Projects.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        public MyProjectsViewModel()
        {
            Projects = new ObservableCollection<ProjectViewDTO>();

            // Пример данных:
            Projects.Add(new ProjectViewDTO { Name = "DTC UI Redesign", StatusName = "Редизайн интерфейса системы" });
            Projects.Add(new ProjectViewDTO { Name = "Соц. сеть", StatusName = "Модуль постов и комментариев" });

            OnPropertyChanged(nameof(NoProjectsVisibility));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
