using AppForSNForUsers.ViewModels;
using System.Windows.Controls;

namespace AppForSNForUsers.Views.Pages
{
    /// <summary>
    /// Interaction logic for MyProjectsView.xaml
    /// </summary>
    public partial class MyProjectsView : UserControl
    {
        public MyProjectsView()
        {
            InitializeComponent();
            this.DataContext = new MyProjectsViewModel(); 

        }
    }
}
