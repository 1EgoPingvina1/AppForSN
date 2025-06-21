using AppForSNForUsers.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AppForSNForUsers.Views.Dashboard
{
    /// <summary>
    /// Interaction logic for MainDashboardView.xaml
    /// </summary>
    public partial class MainDashboardView : Window
    {
        private readonly ApiClient _apiClient;

        public MainDashboardView(ApiClient apiClient)
        {
            InitializeComponent();
            DataContext = new MainDashboardView(apiClient);
        }

        private async void GetProjects_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var projects = await _apiClient.GetAsync<List<Project>>("/api/projects");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }
}
