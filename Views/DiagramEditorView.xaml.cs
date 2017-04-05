using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Troubleshooting.Annotations;
using Troubleshooting.ViewModels;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

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

            
            DiagramGrid.MouseDown += (o, e) =>
            {
                origContentMouseDownPoint = e.GetPosition(DiagramGrid);
                mouseHandlingMode = MouseHandlingMode.SelectRect;
                ViewModel.SelectRectangle.Visible = true;
                ViewModel.SelectRectangle.X = origContentMouseDownPoint.X;
                ViewModel.SelectRectangle.Y = origContentMouseDownPoint.Y;
                ViewModel.SelectRectangle.Point2 = new Point(0,0);
                e.Handled = true;
            };

            NodeViewModel intersectsNode = null;
            bool mouseMoveBetweenDownAndUp = false;
            DiagramGrid.MouseMove += (o, e) =>
            {
                Point curContentPoint;
                switch (mouseHandlingMode)
                {
                    case MouseHandlingMode.DraggingNode:
                        mouseMoveBetweenDownAndUp = true;
                        curContentPoint = e.GetPosition(DiagramGrid);
                        Vector dragVector = curContentPoint - origContentMouseDownPoint;
                        
                        foreach (var n in ViewModel.Nodes)
                            n.IntersectsMode = false;
                        
                        foreach (var draggedNode in DraggedNodeViewModels)
                        {
                            Point newPosition = draggedNode.OldPosition + dragVector;
                            intersectsNode = ViewModel.Nodes.FirstOrDefault(
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
                        curContentPoint = e.GetPosition(DiagramGrid);
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
                        curContentPoint = e.GetPosition(DiagramGrid);
                        ConnectionRoute.EndPoint = curContentPoint;
                        
                        break;
                    case MouseHandlingMode.SelectRect:
                        curContentPoint = e.GetPosition(DiagramGrid);
                        Vector sizeVector = curContentPoint - origContentMouseDownPoint;
                        ViewModel.SelectRectangle.Point2 = new Point(sizeVector.X, sizeVector.Y);
                        break;
                }
            };

            DiagramGrid.MouseUp += (o, e) =>
            {
                Point curContentPoint;
                switch (mouseHandlingMode)
                {
                    case MouseHandlingMode.DraggingNode:
                        mouseHandlingMode = MouseHandlingMode.None;
                        foreach (var n in ViewModel.Nodes) n.IntersectsMode = false;
                        curContentPoint = e.GetPosition(DiagramGrid);
                        if (curContentPoint == origContentMouseDownPoint && !mouseMoveBetweenDownAndUp)
                            DraggedNodeView.ViewModel.SelectMode = !DraggedNodeView.ViewModel.SelectMode;
                        mouseMoveBetweenDownAndUp = false;
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
                        if((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                            foreach (var n in ViewModel.Nodes)
                                n.SelectMode = false;
                            

                        Rect selectRect = ViewModel.SelectRectangle.Rect();
                        
                        foreach (var selectNode in ViewModel.Nodes.Where(n => selectRect.Contains(n.Rect())))
                        {
                            selectNode.SelectMode = true;
                        }

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
           // DiagramGrid.Focus();
            //Keyboard.Focus(DiagramGrid);
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                return;
            
            if (mouseHandlingMode != MouseHandlingMode.None)
                return;
            origContentMouseDownPoint = e.GetPosition(DiagramGrid);

           
            if (sender is NodeView node)
            {
                node.CaptureMouse();
                DraggedNodeView = node;
                DraggedNodeViewModels = node.ViewModel.SelectMode 
                    ? ViewModel.Nodes.Where(n => n.SelectMode) 
                    : new []{node.ViewModel};
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
            origContentMouseDownPoint = e.GetPosition(DiagramGrid);

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
            origContentMouseDownPoint = e.GetPosition(DiagramGrid);

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
                node.ViewModel.InputConnectors.Add(new ConnectorViewModel());
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
                node.ViewModel.EditMode = true;
                e.Handled = true;
            }
        }

        private void NodeView_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is NodeView node)
            {
                node.ViewModel.EditMode = false;
                e.Handled = true;
            }
        }

        private void MenuItemEnd_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }


       
    }
}
