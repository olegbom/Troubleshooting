using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Troubleshooting.Models;
using Troubleshooting.ViewModels;


namespace Troubleshooting.Views
{
    public enum MouseHandlingMode
    {
        None,
        DraggingNode, 
        ResizeNode, 
        ConnectionRoute,
        SelectRect,
        OutConnectorRotate
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
                if (e.ChangedButton != MouseButton.Left) return;
                mouseHandlingMode = MouseHandlingMode.SelectRect;
                ViewModel.SelectRectangle.Visible = true;
                ViewModel.SelectRectangle.X = origContentMouseDownPoint.X;
                ViewModel.SelectRectangle.Y = origContentMouseDownPoint.Y;
                ViewModel.SelectRectangle.Point2 = new Point(0,0);
                e.Handled = true;
            };

            KeyDown += (o, e) =>
            {
                if (e.Key == Key.Delete)
                {
                    for (var i = 0; i < ViewModel.Nodes.Count; i++)
                    {
                        if (ViewModel.Nodes[i].SelectMode) ViewModel.Nodes.RemoveAt(i--);
                    }
                    for (var i = 0; i < ViewModel.Connections.Count; i++)
                    {
                        if (ViewModel.Connections[i].SelectMode)
                        {
                            ViewModel.Connections[i].Dispose();
                            ViewModel.Connections.RemoveAt(i--);
                        }
                    }
                }
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
                        ViewModel.RoutedConnectionViewModel.EndPoint = curContentPoint;
                        
                        break;
                    case MouseHandlingMode.SelectRect:
                        curContentPoint = e.GetPosition(DiagramGrid);
                        Vector sizeVector = curContentPoint - origContentMouseDownPoint;
                        ViewModel.SelectRectangle.Point2 = new Point(sizeVector.X, sizeVector.Y);
                        break;

                    case MouseHandlingMode.None:
                        mouseMoveConnectionBetweenDownAndUp = true;
                        break;
                    case MouseHandlingMode.OutConnectorRotate:
                        curContentPoint = e.GetPosition(DiagramGrid);
                        RotateNodeView.ViewModel.SetOrientationTo(curContentPoint);
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
                        ViewModel.RoutedConnectionViewModel.Dispose();
                        ViewModel.RoutedConnectionViewModel = null;
                        break;
                    case MouseHandlingMode.SelectRect:
                        mouseHandlingMode = MouseHandlingMode.None;
                        if((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                        {
                            foreach (var n in ViewModel.Nodes)
                                n.SelectMode = false;
                            foreach (var connection in ViewModel.Connections)
                            {
                                connection.SelectMode = false;
                            }
                        }
                            

                        Rect selectRect = ViewModel.SelectRectangle.Rect();
                        
                        foreach (var selectNode in ViewModel.Nodes.Where(n => selectRect.Contains(n.Rect())))
                        {
                            selectNode.SelectMode = true;
                        }

                        ViewModel.SelectRectangle.Visible = false;
                        break;
                    case MouseHandlingMode.OutConnectorRotate:
                        mouseHandlingMode = MouseHandlingMode.None;
                        RotateNodeView.ReleaseMouseCapture();
                        e.Handled = true;
                        break;
                }
            };
        }

        private NodeView DraggedNodeView; 
        private IEnumerable<NodeViewModel> DraggedNodeViewModels;
        private NodeView ResizeNodeView;
        private NodeView RotateNodeView;
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;
        private Point origContentMouseDownPoint;

        private void Node_OnBorderMoveMouseDown(object sender, MouseButtonEventArgs e)
        {
           // DiagramGrid.Focus();
            //Keyboard.Focus(DiagramGrid);
            if (e.LeftButton != MouseButtonState.Pressed) return;

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
            if (e.LeftButton != MouseButtonState.Pressed) return;

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
                if (e.ChangedButton == MouseButton.Left)
                {
                    ConnectionViewModel connectionViewModel =
                        new ConnectionViewModel(node.ViewModel) {IsHitTestVisible = false};
                    ViewModel.RoutedConnectionViewModel = connectionViewModel;
                    mouseHandlingMode = MouseHandlingMode.ConnectionRoute;
                    
                } else if (e.ChangedButton == MouseButton.Right)
                {
                    RotateNodeView = node;
                    mouseHandlingMode = MouseHandlingMode.OutConnectorRotate;
                }
                e.Handled = true;
            }
        }
   

        private void MenuItemNewBlock_OnClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel nodeViewModel = new NodeViewModel()
                { X = 50, Y = 50};
            ViewModel.Nodes.Add(nodeViewModel);
            nodeViewModel.Text = (nodeViewModel.Zindex+1).ToString();
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
            
        }

        private void NodeView_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.ConnectionRoute)
                return;
            var nodeSource = ViewModel.RoutedConnectionViewModel.SourceNode;
            if (sender is NodeView node 
                && nodeSource != node.ViewModel
                && !node.ViewModel.IsThisAChild(nodeSource)
                && nodeSource.OutputConnections.All(c => c.SinkNode != node.ViewModel))
            {
                ViewModel.RoutedConnectionViewModel.IsHitTestVisible = true;
                mouseHandlingMode = MouseHandlingMode.None;
                ViewModel.RoutedConnectionViewModel.SinkNode = node.ViewModel;
                ViewModel.Connections.Add(ViewModel.RoutedConnectionViewModel);
                ViewModel.RoutedConnectionViewModel = null;
            }
        }

        private void MenuPrint_OnClick(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(DiagramGrid, "Печать схемы");
            }
        }

        private bool mouseMoveConnectionBetweenDownAndUp;
        private ConnectionView clickedConnectionView;

        private void Connection_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
                return;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                return;
            mouseMoveConnectionBetweenDownAndUp = false;
            clickedConnectionView = (ConnectionView) sender;
            clickedConnectionView.ViewModel.SelectMode = !clickedConnectionView.ViewModel.SelectMode;
            e.Handled = true;
        }

        private void Connection_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
                return;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                return;

            if (Equals((ConnectionView) sender, clickedConnectionView) && !mouseMoveConnectionBetweenDownAndUp)
            {
                mouseMoveConnectionBetweenDownAndUp = false;
            }
        }

        private void Connection_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
                return;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                return;

            if (sender is ConnectionView connection)
            {
                connection.ViewModel.HitMode = true;
            }

        }
        private void Connection_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is ConnectionView connection)
            {
                connection.ViewModel.HitMode = false;
            }
        }

        private void MenuDependencyTable_OnClick(object sender, RoutedEventArgs e)
        {
            var dependencyTableViewModel = new DependencyTableViewModel(ViewModel);
            var dependencyTableView = new DependencyTableView(dependencyTableViewModel);
            DiagramGrid.IsEnabled = false;

            dependencyTableView.Closed += (o, args) =>
            {
                DiagramGrid.IsEnabled = true;
            };
            dependencyTableView.Show();
        }


        private Point copyMousePoint;
        private void ContextCopy_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedNodes = ViewModel.Nodes.Where(x => x.SelectMode).ToList();
           var selectedNodesModels = selectedNodes.Select(x => x.ConvertToModel()).ToList();
            var selectedConnections =
                ViewModel.Connections
                .Where(c => selectedNodes.Contains(c.SourceNode) &&
                                                 selectedNodes.Contains(c.SinkNode))
                .Select(c => new ConnectionModel
                {
                    SourceNode = selectedNodesModels[selectedNodes.IndexOf(c.SourceNode)],
                    SinkNode = selectedNodesModels[selectedNodes.IndexOf(c.SinkNode)]
                }).ToList();
            var diagramEditorModel = new DiagramEditorModel
            {
                Nodes = selectedNodesModels,
                Connections = selectedConnections
            };
            copyMousePoint = origContentMouseDownPoint;
            DataObject dataObject = new DataObject();
            dataObject.SetData(diagramEditorModel);

            Clipboard.SetDataObject(dataObject, false);
            
        }

        private void ContextPaste_OnClick(object sender, RoutedEventArgs e)
        {
            DataObject dataObject = (DataObject) Clipboard.GetDataObject();
            if (!(dataObject?.GetDataPresent(typeof(DiagramEditorModel)) ?? false)) return;

            if (dataObject.GetData(typeof(DiagramEditorModel)) is DiagramEditorModel model)
            {
                var pasteMousePoint = origContentMouseDownPoint;
                var delta = pasteMousePoint - copyMousePoint;

                var nodeVms = model.Nodes.Select(n => new NodeViewModel(n)).ToArray();

                foreach (var nodeVm in nodeVms)
                {
                    nodeVm.Position += delta;
                    ViewModel.Nodes.Add(nodeVm);
                }
                foreach (var connection in model.Connections)
                {
                    var indexSource = model.Nodes.IndexOf(connection.SourceNode);
                    var indexSink = model.Nodes.IndexOf(connection.SinkNode);
                    var connectionVm = new ConnectionViewModel(nodeVms[indexSource])
                        { SinkNode = nodeVms[indexSink] };
                    ViewModel.Connections.Add(connectionVm);
                }
            }
        }

        private void ContextNewNode_OnClick(object sender, RoutedEventArgs e)
        {
            var mousePoint = origContentMouseDownPoint;
            var node = new NodeViewModel {Position = mousePoint};
            ViewModel.Nodes.Add(node);
            node.Text = (node.Zindex+1).ToString();
        }

        private void MenuNew_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Nodes.Clear();
        }

        private void MenuOpen_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".trs",
                Filter = "TroubleShooting document (.trs)|*.trs",
                AddExtension = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var model = Serializer.LoadFromBinnary<DiagramEditorModel>(openFileDialog.FileName);
                    DataContext = new DiagramEditorViewModel(model);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(this, exception.ToString());
                    throw;
                }
            }

        }

        private void MenuSave_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".trs",
                Filter = "TroubleShooting document (.trs)|*.trs",
                AddExtension = true
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                Serializer.SaveToBinnary(saveFileDialog.FileName, ViewModel.ConvertToModel());
            }
        }

        private void MenuClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }


      
    }
}
