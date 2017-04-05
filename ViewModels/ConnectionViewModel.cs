using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PropertyChanged;
using Troubleshooting.Annotations;

namespace Troubleshooting.ViewModels
{
    public class ConnectionViewModel: INotifyPropertyChanged
    {

        public NodeViewModel SourceNode { get; }

        private NodeViewModel _sinkNode;

        public NodeViewModel SinkNode
        {
            get { return _sinkNode; }
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

        private Point _endPoint;
        public Point EndPoint
        {
            get { return _endPoint; }
            set
            {
                if (_endPoint == value) return;
                _endPoint = value;
                OnPropertyChanged();
            }
        }

        private Point _startPoint;
        public Point StartPoint 
        {
            get { return _startPoint; }
            set
            {
                if (_startPoint == value) return;
                _startPoint = value;
                OnPropertyChanged();
            }
        }

        [DependsOn(nameof(StartPoint))]
        public double X => StartPoint.X;

        [DependsOn(nameof(StartPoint))]
        public double Y => StartPoint.Y;
        
        
        [DependsOn(nameof(StartPoint), nameof(EndPoint))]
        public double X2 => EndPoint.X - StartPoint.X;

        [DependsOn(nameof(StartPoint), nameof(EndPoint))]
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
