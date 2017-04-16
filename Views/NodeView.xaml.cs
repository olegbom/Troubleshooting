using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public event MouseButtonEventHandler ConnectorOutMouseDown;

        public NodeView()
        {
            InitializeComponent();
        }


        private void BorderMove_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            BorderMoveMouseDown?.Invoke(this, e);
        }

        private void RectSizeBR_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            RectSizeMouseDown?.Invoke(this, e);
        }

        private void ConnectorOut_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ConnectorOutMouseDown?.Invoke(this, e);
        }


    }
}
