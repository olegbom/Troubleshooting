using System;
using System.ComponentModel;
using System.Data;
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
                UpdateEndConnectionOrientation();
                OnPropertyChanged();
                _sinkNode.OnInputConnectionsPositionsChanged();
            }
        }

        public Orientation StartConnectionOrientation { get; set; }
        
        

        public Point EndPoint { get; set; }
        
        public Point StartPoint { get; set; }
        public void OnStartPointChanged()
        {
            UpdateEndConnectionOrientation();
            _sinkNode?.OnInputConnectionsPositionsChanged();
        }

        public void UpdateEndConnectionOrientation()
        {
            if (SinkNode == null)
            {
                EndConnectionOrientation = StartConnectionOrientation;
                return;
            }
            var vector = StartPoint - SinkNode.CenterPoint;
            double phi = Math.Atan2(vector.Y, vector.X);

            if (phi < -3 * Math.PI / 4)
                EndConnectionOrientation = Orientation.Left;
            else if (phi < -Math.PI / 4)
                EndConnectionOrientation = Orientation.Top;
            else if (phi < Math.PI / 4)
                EndConnectionOrientation = Orientation.Right;
            else if (phi < 3 * Math.PI / 4)
                EndConnectionOrientation = Orientation.Bottom;
            else EndConnectionOrientation = Orientation.Left;
        }

        public Orientation EndConnectionOrientation { get; set; }
        


        public Point LocalEndPoint => new Point(EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y);
        public Point BezieEndPoint
        {
            get
            {
                switch (EndConnectionOrientation)
                {
                    case Orientation.Bottom: return LocalEndPoint + new Vector(0, 10);
                    case Orientation.Right: return LocalEndPoint + new Vector(10, 0);
                    case Orientation.Top: return LocalEndPoint + new Vector(0, -10);
                    case Orientation.Left: return LocalEndPoint + new Vector(-10, 0);
                    default: return LocalEndPoint + new Vector(-10, 0);
                }
            }
        }

        public Point LineSegment1
        {
            get
            {
                switch (StartConnectionOrientation)
                {
                    case Orientation.Bottom: return new Point(0, (EndPoint.Y - StartPoint.Y - 10) / 2);
                    case Orientation.Right: return new Point((EndPoint.X - StartPoint.X - 10) / 2, 0);
                    case Orientation.Top: return new Point(0, (EndPoint.Y - StartPoint.Y - 10) / 2);
                    case Orientation.Left: return new Point((EndPoint.X - StartPoint.X - 10) / 2, 0);
                    default : return new Point((EndPoint.X - StartPoint.X - 10) / 2, 0);
                }
                
            }
        }

        public Point LineSegment2
        {
            get
            {
                switch (EndConnectionOrientation)
                {
                    case Orientation.Bottom:
                        return new Point(EndPoint.X - StartPoint.X, (EndPoint.Y - StartPoint.Y - 10) / 2);
                    case Orientation.Right:
                        return new Point((EndPoint.X - StartPoint.X - 10) / 2, EndPoint.Y - StartPoint.Y);
                    case Orientation.Top:
                        return new Point(EndPoint.X - StartPoint.X, (EndPoint.Y - StartPoint.Y - 10) / 2);
                    case Orientation.Left:
                        return new Point((EndPoint.X - StartPoint.X - 10) / 2, EndPoint.Y - StartPoint.Y);
                    default:
                        return new Point((EndPoint.X - StartPoint.X - 10) / 2, EndPoint.Y - StartPoint.Y);
                }
            }
        }

        public Point ArrowTopPoint
        {
            get
            {
                switch (EndConnectionOrientation)
                {
                    case Orientation.Bottom: return LocalEndPoint + new Vector(-4, 6);
                    case Orientation.Right: return LocalEndPoint + new Vector(6, -4);
                    case Orientation.Top: return LocalEndPoint + new Vector(-4, -6);
                    case Orientation.Left: return LocalEndPoint + new Vector(-6, 4);
                    default: return LocalEndPoint + new Vector(-10, 0);
                }
            }
        }

        public Point ArrowBottomPoint
        {
            get
            {
                switch (EndConnectionOrientation)
                {
                    case Orientation.Bottom: return LocalEndPoint + new Vector(4, 6);
                    case Orientation.Right: return LocalEndPoint + new Vector(6, 4);
                    case Orientation.Top: return LocalEndPoint + new Vector(4, -6);
                    case Orientation.Left: return LocalEndPoint + new Vector(-6, -4);
                    default: return LocalEndPoint + new Vector(-10, 0);
                }
            }
        }


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
            StartConnectionOrientation = source.OutOrientation;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
