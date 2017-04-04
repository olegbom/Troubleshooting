using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Troubleshooting.Annotations;

namespace Troubleshooting.ViewModels
{
    public class DiagramEditorViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<NodeViewModel> Nodes { get; } = new ObservableCollection<NodeViewModel>();
        public ObservableCollection<ConnectionViewModel> Connections { get; } = new ObservableCollection<ConnectionViewModel>();

        public CompositeCollection DiagramItems { get; } = new CompositeCollection();

        public DiagramEditorViewModel()
        {
            Nodes.CollectionChanged += (o, e) =>
            {
                if (e.NewItems != null)
                    foreach (NodeViewModel eNewItem in e.NewItems)
                    {
                        // eNewItem.SetMouseManipulation(DiagramCanvas);
                    }


            };
            DiagramItems.Add(new CollectionContainer() {Collection = Nodes});
            DiagramItems.Add(new CollectionContainer() {Collection = Connections});
            Nodes.Add(new NodeViewModel() { Text = "1", X = 10, Y = 10 });
            Nodes.Add(new NodeViewModel() { Text = "2", X = 20, Y = 60 });
            Nodes.Add(new NodeViewModel() { Text = "3", X = 30, Y = 110 });
        }

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion  
    }
}
