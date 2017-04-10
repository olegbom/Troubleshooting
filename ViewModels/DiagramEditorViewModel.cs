using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using MugenMvvmToolkit.ViewModels;
using PropertyChanged;
using Troubleshooting.Models;

namespace Troubleshooting.ViewModels
{
    [ImplementPropertyChanged]
    public class DiagramEditorViewModel: ViewModelBase
    {
        public ObservableCollection<NodeViewModel> Nodes { get; } = new ObservableCollection<NodeViewModel>();
        public ObservableCollection<ConnectionViewModel> Connections { get; } = new ObservableCollection<ConnectionViewModel>();

        private ConnectionViewModel _routedConnectionViewModel;
        public ConnectionViewModel RoutedConnectionViewModel
        {
            get => _routedConnectionViewModel;
            set
            {
                if (_routedConnectionViewModel == value) return;
                if(_routedConnectionViewModel != null)
                    DiagramItems.Remove(_routedConnectionViewModel);
                _routedConnectionViewModel = value;
                if (_routedConnectionViewModel != null)
                    DiagramItems.Add(_routedConnectionViewModel);
                OnPropertyChanged();
            }
        }
        
        public SelectRectangleViewModel SelectRectangle { get; } = new SelectRectangleViewModel();
        public CompositeCollection DiagramItems { get; } = new CompositeCollection();

        public Visibility InputConnectorVisiblity { get; set; }
        
        public DiagramEditorModel Model { get; } = new DiagramEditorModel();
        
        public DiagramEditorViewModel()
        {
            


            DiagramItems.Add(new CollectionContainer() {Collection = Nodes});
            DiagramItems.Add(new CollectionContainer() {Collection = Connections});
            DiagramItems.Add(SelectRectangle);

            Nodes.CollectionChanged += (o, e) =>
            {
                var oldItems = e.OldItems;
                if(oldItems !=null)
                    foreach (NodeViewModel node in oldItems)
                    {
                        for (var i = node.InputConnections.Count - 1; i >= 0; i--)
                        {
                            var connection = node.InputConnections[i];
                            Connections.Remove(connection);
                        }

                        node.InputConnections.Clear();
                        for (var i = node.OutputConnections.Count - 1; i >= 0; i--)
                        {
                            var connection = node.OutputConnections[i];
                            Connections.Remove(connection);
                        }

                        node.OutputConnections.Clear();

                        Model.Nodes.Remove(node.Model);
                    }
                var newItems = e.NewItems;
                if (newItems!=null)
                    foreach (NodeViewModel node in newItems)
                    {
                        Model.Nodes.Add(node.Model);
                    }
            };

            Connections.CollectionChanged += (o, e) =>
            {
                if(e.OldItems !=null)
                    foreach (ConnectionViewModel connection in e.OldItems)
                    {
                        connection.SourceNode.OutputConnections.Remove(connection);
                        connection.SinkNode?.InputConnections.Remove(connection);
                    }    
            };

            Nodes.Add(new NodeViewModel(new NodeModel(Nodes.Count + 1)) { Text = "1", X = 10, Y = 10 });
            Nodes.Add(new NodeViewModel(new NodeModel(Nodes.Count + 1)) { Text = "2", X = 20, Y = 60 });
            Nodes.Add(new NodeViewModel(new NodeModel(Nodes.Count + 1)) { Text = "3", X = 30, Y = 110 });
        }

        public FunctionalDiagram GenerateFuctionalDiagram()
        {
            var result = new FunctionalDiagram(Nodes.Count);
            foreach (var connection in Connections)
            {
                var a = Nodes.IndexOf(connection.SourceNode)+1;
                var b = Nodes.IndexOf(connection.SinkNode)+1;
                result.Connect(a, b);
            }
            return result;
        }

   
    }
}
