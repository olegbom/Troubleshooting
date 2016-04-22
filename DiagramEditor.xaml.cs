using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Troubleshooting.Annotations;

namespace Troubleshooting
{
    /// <summary>
    /// Логика взаимодействия для DiagramEditor.xaml
    /// </summary>
    public partial class DiagramEditor : Window, INotifyPropertyChanged
    {

        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        public DiagramEditor()
        {
            InitializeComponent();
            Node1.SetMouseManipulation(DiagramCanvas);
            Node2.SetMouseManipulation(DiagramCanvas);
            Node3.SetMouseManipulation(DiagramCanvas);
            
        }

        private void MenuItem_Click_CreateNewBlock(object sender, RoutedEventArgs e)
        {
            Node node = new Node { Text = "Название", X = 10, Y = 80};
            node.SetMouseManipulation(DiagramCanvas);

            DiagramCanvas.Children.Add(node);
        }


        private Point origContentMouseDownPoint;

        

        private void Node_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DiagramCanvas.Focus();
            Keyboard.Focus(DiagramCanvas);
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                return;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
               return;
            }

            
            origContentMouseDownPoint = e.GetPosition(DiagramCanvas);


            Node node = (Node)sender;
            node.CaptureMouse();
            node.SaveOldPosition();

            mouseHandlingMode = MouseHandlingMode.DraggingNode;

            e.Handled = true;
        }

        private void Node_MouseMove(object sender, MouseEventArgs e)
        {

            if (mouseHandlingMode != MouseHandlingMode.DraggingNode)
            {
                return;
            }

            Point curContentPoint = e.GetPosition(DiagramCanvas);
            Vector dragVector = curContentPoint - origContentMouseDownPoint;
            

            Node node = (Node)sender;
            node.Position = node.OldPosition + dragVector;
            
            e.Handled = true;
        }

        private void Node_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.DraggingNode)
            {
                return;
            }

            mouseHandlingMode = MouseHandlingMode.None;


            Node node = (Node)sender;
            node.ReleaseMouseCapture();
            e.Handled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum MouseHandlingMode
    { 
        None,
        DraggingNode
    }
}
