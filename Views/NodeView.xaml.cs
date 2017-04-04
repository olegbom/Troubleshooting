using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Troubleshooting.ViewModels;

namespace Troubleshooting.Views
{
    /// <summary>
    /// Логика взаимодействия для NodeView.xaml
    /// </summary>
    public partial class NodeView : UserControl
    {
        public NodeViewModel ViewModel => (NodeViewModel) DataContext;
        public event MouseButtonEventHandler BorderMoveMouseDown;
        public event MouseButtonEventHandler RectSizeMouseDown;

        public NodeView()
        {
            InitializeComponent();

            MouseEnter += (o, e) => ViewModel.EditMode = true;
            
            MouseLeave += (o, e) => ViewModel.EditMode = false;

        }


        private void BorderMove_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            BorderMoveMouseDown?.Invoke(this, e);
        }

        private void RectSizeBR_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            RectSizeMouseDown?.Invoke(this, e);
        }
    }
}
