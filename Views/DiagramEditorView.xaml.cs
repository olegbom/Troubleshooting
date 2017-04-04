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
using System.Windows.Shapes;
using Troubleshooting.ViewModels;

namespace Troubleshooting.Views
{

    public enum MouseHandlingMode
    {
        None,
        DraggingNode, 
        ResizeNode
    }
    /// <summary>
    /// Логика взаимодействия для DiagramEditorView.xaml
    /// </summary>
    public partial class DiagramEditorView : Window
    {
        public DiagramEditorViewModel ViewModel => (DiagramEditorViewModel) DataContext;

        public DiagramEditorView()
        {
            InitializeComponent();


            NodeViewModel intersectsNode = null;
            MouseMove += (o, e) =>
            {
                Point curContentPoint;
                switch (mouseHandlingMode)
                {
                    case MouseHandlingMode.DraggingNode:
                        NodeViewModel draggedNodeViewModel = DraggedNodeView.ViewModel;
                        curContentPoint = e.GetPosition(DiagramCanvas);
                        Vector dragVector = curContentPoint - origContentMouseDownPoint;
                       
                        Point newPosition = draggedNodeViewModel.OldPosition + dragVector;

                        if (intersectsNode != null) intersectsNode.IntersectsMode = false;
                        intersectsNode =
                            ViewModel.Nodes.FirstOrDefault(
                                nodeViewModel =>
                                    nodeViewModel != draggedNodeViewModel &&
                                    draggedNodeViewModel.IntersectsWith(nodeViewModel, newPosition));
                        if(intersectsNode == null)
                            draggedNodeViewModel.Position = newPosition;
                        else
                        {
                            intersectsNode.IntersectsMode = true;
                        }
                        break;
                    case MouseHandlingMode.ResizeNode:
                        NodeViewModel resizeNodeViewModel = ResizeNodeView.ViewModel;
                        curContentPoint = e.GetPosition(DiagramCanvas);
                        Vector resizeVector = curContentPoint - origContentMouseDownPoint;
                        Vector newSize = resizeNodeViewModel.OldSize + resizeVector;
                        newSize = new Vector(Math.Max(0, newSize.X), Math.Max(0, newSize.Y));

                        if (intersectsNode != null) intersectsNode.IntersectsMode = false;
                        intersectsNode =
                            ViewModel.Nodes.FirstOrDefault(
                                nodeViewModel =>
                                    nodeViewModel != resizeNodeViewModel &&
                                    resizeNodeViewModel.IntersectsWith(nodeViewModel, newSize));
                        if (intersectsNode == null)
                            resizeNodeViewModel.Size = newSize;
                        else
                        {
                            intersectsNode.IntersectsMode = true;
                        }
                        break;
                }
            };

            MouseUp += (o, e) =>
            {
                if (mouseHandlingMode == MouseHandlingMode.DraggingNode)
                {
                    mouseHandlingMode = MouseHandlingMode.None;
                    if (intersectsNode != null) intersectsNode.IntersectsMode = false;
                    DraggedNodeView.ReleaseMouseCapture();
                    //e.Handled = true;
                }
                else if (mouseHandlingMode == MouseHandlingMode.ResizeNode)
                {
                    mouseHandlingMode = MouseHandlingMode.None;
                    if (intersectsNode != null) intersectsNode.IntersectsMode = false;
                    ResizeNodeView.ReleaseMouseCapture();
                    //e.Handled = true;
                }
            };
        }

        private NodeView DraggedNodeView;
        private NodeView ResizeNodeView;
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;
        private Point origContentMouseDownPoint;

        private void Node_OnBorderMoveMouseDown(object sender, MouseButtonEventArgs e)
        {
           // DiagramCanvas.Focus();
            //Keyboard.Focus(DiagramCanvas);
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                return;
            
            if (mouseHandlingMode != MouseHandlingMode.None)
                return;
            origContentMouseDownPoint = e.GetPosition(DiagramCanvas);

           
            if (sender is NodeView node)
            {
                node.CaptureMouse();
                node.ViewModel.OldPosition = node.ViewModel.Position;
                mouseHandlingMode = MouseHandlingMode.DraggingNode;
                DraggedNodeView = node;
                e.Handled = true;
            }
        }

        private void NodeView_OnRectSizeMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                return;

            if (mouseHandlingMode != MouseHandlingMode.None)
                return;
            origContentMouseDownPoint = e.GetPosition(DiagramCanvas);

            if (sender is NodeView node)
            {
                node.CaptureMouse();
                node.ViewModel.OldSize = node.ViewModel.Size;
                mouseHandlingMode = MouseHandlingMode.ResizeNode;
                ResizeNodeView = node;
                e.Handled = true;
            }
        }

        private void MenuItemNewBlock_OnClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel nodeViewModel = new NodeViewModel{Text = "Название", X = 50, Y = 50};
            ViewModel.Nodes.Add(nodeViewModel);
        }
    }
}
