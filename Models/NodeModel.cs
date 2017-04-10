using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PropertyChanged;
using Troubleshooting.Annotations;

namespace Troubleshooting.Models
{
    [ImplementPropertyChanged]
    public class NodeModel: INotifyPropertyChanged
    {
        public ObservableCollection<ConnectionModel> OutputConnections { get; } = new ObservableCollection<ConnectionModel>();
        public ObservableCollection<ConnectionModel> InputConnections { get; } = new ObservableCollection<ConnectionModel>();

        public int Number { get; set; }
        public bool Working { get; set; }

        public NodeModel(int num)
        {
            Number = num;
        }


        public void ErrorInfluence()
        {
            if (Working == false) return;
            Working = false;
            foreach (var b in OutputConnections) b.SinkNode.ErrorInfluence();
        }

        public HashSet<NodeModel> AllCildrens()
        {
            var result = new HashSet<NodeModel>();
            foreach (var output in OutputConnections)
            {
                result.Add(output.SinkNode);
                result.UnionWith(output.SinkNode.AllCildrens());
            }
            return result;
        }


        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
