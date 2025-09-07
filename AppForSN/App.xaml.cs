using AppForSNForUsers.Contracts;
using AppForSNForUsers.Services;
using AppForSNForUsers.ViewModels;
using AppForSNForUsers.Views.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace AppForSNForUsers
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
       public static string CurrentUserToken { get; set; }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAuthService, AuthService>();
            // Навигация должна быть Singleton, чтобы состояние было общим для всего приложения
            services.AddSingleton<INavigationService, NavigationService>();

            // --- ViewModel'и ---
            // ViewModel'и должны быть Transient, чтобы каждый раз создавался новый экземпляр
            // Это предотвращает утечки данных между сеансами
            services.AddTransient<LoginViewModel>();
            services.AddTransient<ProjectsViewModel>();
            services.AddSingleton<MainViewModel>(); // Главная ViewModel - Singleton

            // --- Окна ---
            services.AddSingleton<MainDashboardView>();
        }
    }
}
