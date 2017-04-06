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
                _sinkNode = value;
                OnPropertyChanged();
            }
        }

    
        public Point EndPoint { get; set; }
        public Point StartPoint { get; set; }

        public Point LocalEndPoint => new Point(EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y);

        public Point LineSegment1 => new Point((EndPoint.X - StartPoint.X) / 2, 0);
        public Point LineSegment2 => new Point((EndPoint.X - StartPoint.X) / 2, EndPoint.Y - StartPoint.Y);

        public Point ArrowTopPoint => LocalEndPoint + new Vector(-6,-4);
        public Point ArrowBottomPoint => LocalEndPoint + new Vector(-6,4);



        public double X => StartPoint.X;
        public double Y => StartPoint.Y;
        public double X2 => EndPoint.X - StartPoint.X;
        public double Y2 => EndPoint.Y - StartPoint.Y;
       
        public bool SelectMode { get; set; }
        public bool IsHitTestVisible { get; set; }

        public ConnectionViewModel(NodeViewModel source)
        {
            SourceNode = source;
            source.OutputConnections.Add(this);
            EndPoint = StartPoint;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
