using AppForSNForUsers.DTOs;
using AppForSNForUsers.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSNForUsers.ViewModels
{
    public class UserProjectsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ProjectViewDTO> Projects { get; set; } = new ObservableCollection<ProjectViewDTO>();

        public async Task LoadAsync(string token)
        {
            var service = new ProjectApiService(token);
            var data = await service.GetUserProjectsAsync();

            Projects.Clear();

            foreach (var item in data)
                Projects.Add(item);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
