using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Troubleshooting.ViewModels;

namespace Troubleshooting.Views
{

    public enum MouseHandlingMode
    {
        None,
        DraggingNode, 
        ResizeNode, 
        ConnectionRoute,
        SelectRect
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


            MouseDown += (o, e) =>
            {

                origContentMouseDownPoint = e.GetPosition(DiagramCanvas);
                mouseHandlingMode = MouseHandlingMode.SelectRect;
                ViewModel.SelectRectangle.Visible = true;
                ViewModel.SelectRectangle.X = origContentMouseDownPoint.X;
                ViewModel.SelectRectangle.Y = origContentMouseDownPoint.Y;
                ViewModel.SelectRectangle.Point2 = new Point(0,0);
                e.Handled = true;
            };

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
                       
                        

                        foreach (var n in ViewModel.Nodes)
                            n.IntersectsMode = false;
                        
                        foreach (var draggedNode in DraggedNodeViewModels)
                        {
                            Point newPosition = draggedNode.OldPosition + dragVector;
                            intersectsNode =
                            ViewModel.Nodes.FirstOrDefault(
                                nodeViewModel =>
                                    nodeViewModel != draggedNode &&
                                    draggedNode.IntersectsWith(nodeViewModel, newPosition));
                            if (intersectsNode == null)
                                draggedNode.Position = newPosition;
                            else
                            {
                                intersectsNode.IntersectsMode = true;
                            }
                        }
                        e.Handled = true;
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
                        e.Handled = true;
                        break;
                    case MouseHandlingMode.ConnectionRoute:
                        curContentPoint = e.GetPosition(DiagramCanvas);
                        ConnectionRoute.EndPoint = curContentPoint;
                        
                        break;
                    case MouseHandlingMode.SelectRect:
                        curContentPoint = e.GetPosition(DiagramCanvas);
                        Vector sizeVector = curContentPoint - origContentMouseDownPoint;
                        ViewModel.SelectRectangle.Point2 = new Point(sizeVector.X, sizeVector.Y);
                        break;
                }
            };

            MouseUp += (o, e) =>
            {
                switch (mouseHandlingMode)
                {
                    case MouseHandlingMode.DraggingNode:
                        mouseHandlingMode = MouseHandlingMode.None;
                        foreach (var n in ViewModel.Nodes) n.IntersectsMode = false;
                        DraggedNodeView.ReleaseMouseCapture();
                        e.Handled = true;
                        break;
                    case MouseHandlingMode.ResizeNode:
                        mouseHandlingMode = MouseHandlingMode.None;
                        if (intersectsNode != null) intersectsNode.IntersectsMode = false;
                        ResizeNodeView.ReleaseMouseCapture();
                        e.Handled = true;
                        break;
                    case MouseHandlingMode.ConnectionRoute:
                        mouseHandlingMode = MouseHandlingMode.None;
                        ViewModel.Connections.Remove(ConnectionRoute);
                        break;
                    case MouseHandlingMode.SelectRect:
                        mouseHandlingMode = MouseHandlingMode.None;
                        ViewModel.SelectRectangle.Visible = false;
                        break;
                }
            };
        }

        private NodeView DraggedNodeView; 
        private IEnumerable<NodeViewModel> DraggedNodeViewModels;
        private NodeView ResizeNodeView;
        private ConnectionViewModel ConnectionRoute;
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
                DraggedNodeView = node;
                DraggedNodeViewModels = ViewModel.Nodes.TakeWhile(n => n.SelectMode);
                foreach (var n in DraggedNodeViewModels)
                    n.OldPosition = n.Position;
                
                mouseHandlingMode = MouseHandlingMode.DraggingNode;
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

        private void NodeView_OnConnectorOutMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                return;

            if (mouseHandlingMode != MouseHandlingMode.None)
                return;
            origContentMouseDownPoint = e.GetPosition(DiagramCanvas);

            if (sender is NodeView node)
            {
                ConnectionViewModel connectionViewModel = new ConnectionViewModel(node.ViewModel);
                ViewModel.Connections.Add(connectionViewModel);
                mouseHandlingMode = MouseHandlingMode.ConnectionRoute;
                ConnectionRoute = connectionViewModel;
                e.Handled = true;
            }
        }
        private void NodeView_OnConnectorInMouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                return;

            if (mouseHandlingMode != MouseHandlingMode.ConnectionRoute)
                return;

            if (sender is NodeView node)
            {
                
                mouseHandlingMode = MouseHandlingMode.None;
                ConnectionRoute.SinkNode = node.ViewModel;
            }
            

        }

        private void MenuItemNewBlock_OnClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel nodeViewModel = new NodeViewModel{Text = "Название", X = 50, Y = 50};
            ViewModel.Nodes.Add(nodeViewModel);
        }


        private void NodeView_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is NodeView node)
            {
                if (node.ViewModel.SelectMode)
                    node.ViewModel.EditMode = true;
                else
                    node.ViewModel.PreSelectMode = true;
                e.Handled = true;
            }
        }

        private void NodeView_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is NodeView node)
            {
                if (node.ViewModel.SelectMode)
                    node.ViewModel.EditMode = false;
                else
                    node.ViewModel.PreSelectMode = false;
                e.Handled = true;
            }
        }

        private void MenuItemEnd_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private NodeView _nodeViewMouseDown; 
        private void NodeView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }


        private void NodeView_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void NodeView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is NodeView node && !node.ViewModel.SelectMode)
            {
                _nodeViewMouseDown = node;
                e.Handled = true;
            }
        }

        private void NodeView_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is NodeView node && !node.ViewModel.SelectMode)
            {
                if (Equals(_nodeViewMouseDown, node))
                {
                    node.ViewModel.SelectMode = true;
                    _nodeViewMouseDown = null;
                    e.Handled = true;
                }
            }
        }

        private void NodeView_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is NodeView node && node.ViewModel.SelectMode)
            {
                _nodeViewMouseDown = node;
                e.Handled = true;
            }
        }

        private void NodeView_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is NodeView node && node.ViewModel.SelectMode)
            {
                if (Equals(_nodeViewMouseDown, node))
                {
                    node.ViewModel.SelectMode = false;
                    _nodeViewMouseDown = null;
                    e.Handled = true;
                }
            }
        }
    }
}
