using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Data;
using PropertyChanged;
using Troubleshooting.Annotations;


namespace Troubleshooting.ViewModels
{
    [Serializable]
    [ImplementPropertyChanged]
    public class DiagramEditorViewModel: INotifyPropertyChanged
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

        public  CompositeCollection DiagramItems { get; } = new CompositeCollection();

        public Visibility InputConnectorVisiblity { get; set; }
        
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
                    }
                for (var i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].Zindex = i;
                }
            };

            Connections.CollectionChanged += (o, e) =>
            {
                var oldItems = e.OldItems;
                if(oldItems != null)
                    foreach (ConnectionViewModel connection in oldItems)
                    {
                        connection.SourceNode.OutputConnections.Remove(connection);
                        connection.SinkNode.InputConnections.Remove(connection);
                    }    
            };

            Nodes.Add(new NodeViewModel() { Text = "1", X = 10, Y = 10 });
            Nodes.Add(new NodeViewModel() { Text = "2", X = 10, Y = 70 });
            Nodes.Add(new NodeViewModel() { Text = "3", X = 10, Y = 130 });
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

        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
