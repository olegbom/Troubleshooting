using System.Windows.Controls;
using Troubleshooting.ViewModels;

namespace Troubleshooting.Views
{
    /// <summary>
    /// Логика взаимодействия для ConnectionView.xaml
    /// </summary>
    public partial class ConnectionView : UserControl
    {
        public ConnectionViewModel ViewModel => (ConnectionViewModel) DataContext;

        public ConnectionView()
        {
            InitializeComponent();
        }
    }
}
