using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PropertyChanged;
using Troubleshooting.Annotations;

namespace Troubleshooting.ViewModels
{
    [ImplementPropertyChanged]
    public class ConnectionViewModel: INotifyPropertyChanged
    {

        public NodeViewModel SourceNode { get; }

        private NodeViewModel _sinkNode;

        public NodeViewModel SinkNode
        {
            get => _sinkNode;
            set
            {
                if (_sinkNode == value) return;
                if (_sinkNode != null) _sinkNode.PropertyChanged -= SinkNode_PropertyChanged;
                _sinkNode = value;
                _sinkNode.PropertyChanged += SinkNode_PropertyChanged;
                EndPoint = _sinkNode.ConnectorInPos;
                OnPropertyChanged();
            }
        }

        private void SinkNode_PropertyChanged(object o, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NodeViewModel.ConnectorInPos):
                    EndPoint = SinkNode.ConnectorInPos;
                    break;
            }
        }


        public Point EndPoint { get; set; }
        public Point StartPoint { get; set; }
      
        public double X => StartPoint.X;
        public double Y => StartPoint.Y;
        public double X2 => EndPoint.X - StartPoint.X;
        public double Y2 => EndPoint.Y - StartPoint.Y;
       


        public ConnectionViewModel(NodeViewModel source)
        {
            SourceNode = source;
            source.ConnectionOut = this;
            StartPoint = SourceNode.ConnectorOutPos;
            EndPoint = SourceNode.ConnectorOutPos;
            SourceNode.PropertyChanged += SourceNode_PropertyChanged;

        }

        private void SourceNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NodeViewModel.ConnectorOutPos):
                    StartPoint = SourceNode.ConnectorOutPos; break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
