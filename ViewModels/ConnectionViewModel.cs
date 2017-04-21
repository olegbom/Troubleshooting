using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PropertyChanged;
using Troubleshooting.Annotations;
using Troubleshooting.Models;


namespace Troubleshooting.ViewModels
{
    [Serializable]
    [ImplementPropertyChanged]
    public class ConnectionViewModel: INotifyPropertyChanged
    {
        public bool SelectMode { get; set; }
        public bool HitMode { get; set; }

        public bool WarSmokeMode { get; set; }

        public double Opacity => WarSmokeMode ? 0.3 : 1;

        public bool IsHitTestVisible { get; set; }



        public NodeViewModel SourceNode { get; }

        private NodeViewModel _sinkNode;

        public NodeViewModel SinkNode
        {
            get => _sinkNode;
            set
            {
                if (_sinkNode == value) return;
                _sinkNode?.InputConnections.Remove(this);
                _sinkNode = value;
                _sinkNode.InputConnections.Add(this);
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

            switch (SinkNode.OutOrientation)
            {
                case Orientation.Left:
                    if (phi < -Math.PI / 4)
                        EndConnectionOrientation = Orientation.Top;
                    else if (phi < Math.PI / 4)
                        EndConnectionOrientation = Orientation.Right;
                    else EndConnectionOrientation = Orientation.Bottom;
                    break;
                case Orientation.Top:
                    if (phi < -Math.PI / 2)
                        EndConnectionOrientation = Orientation.Left;
                    else if (phi < Math.PI / 4)
                        EndConnectionOrientation = Orientation.Right;
                    else if (phi < 3 * Math.PI / 4)
                        EndConnectionOrientation = Orientation.Bottom;
                    else EndConnectionOrientation = Orientation.Left;
                    break;
                case Orientation.Right:
                    if (phi < -3 * Math.PI / 4)
                        EndConnectionOrientation = Orientation.Left;
                    else if (phi < 0)
                        EndConnectionOrientation = Orientation.Top;
                    else if (phi < 3 * Math.PI / 4)
                        EndConnectionOrientation = Orientation.Bottom;
                    else EndConnectionOrientation = Orientation.Left;
                    break;
                case Orientation.Bottom:
                    if (phi < -3 * Math.PI / 4)
                        EndConnectionOrientation = Orientation.Left;
                    else if (phi < -Math.PI / 4)
                        EndConnectionOrientation = Orientation.Top;
                    else if (phi < Math.PI / 2)
                        EndConnectionOrientation = Orientation.Right;
                    else EndConnectionOrientation = Orientation.Left;
                    break;
            }
            SinkNode.OnInputConnectionsPositionsChanged();
        }

        public Orientation EndConnectionOrientation { get; set; }
        


        public Point LocalEndPoint => new Point(EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y);
        

        public Point LineSegment1
        {
            get
            {
                switch (StartConnectionOrientation)
                {
                    case Orientation.Bottom: return new Point(0, 10);
                    case Orientation.Right: return new Point(10, 0);
                    case Orientation.Top: return new Point(0, -10);
                    case Orientation.Left: return new Point(-10, 0);
                    default: return new Point(-10, 0);
                }
                
            }
        }

        public Point LineSegment2
        {
            get
            {
                switch (EndConnectionOrientation)
                {
                    case Orientation.Bottom: return LocalEndPoint + new Vector(0, 20);
                    case Orientation.Right: return LocalEndPoint + new Vector(20, 0);
                    case Orientation.Top: return LocalEndPoint + new Vector(0, -20);
                    case Orientation.Left: return LocalEndPoint + new Vector(-20, 0);
                    default: return LocalEndPoint + new Vector(-20, 0);
                }
            }
        }

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
                    default: return LocalEndPoint + new Vector(6, -4);
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
                    default: return LocalEndPoint + new Vector(6, 4);
                }
            }
        }


        public double X => StartPoint.X;
        public double Y => StartPoint.Y;
       




        public ConnectionViewModel(NodeViewModel source)
        {
            SourceNode = source;
            source.OutputConnections.Add(this);
            EndPoint = StartPoint;
            StartConnectionOrientation = source.OutOrientation;
        }

        public ConnectionModel ConvertToModel()
        {
            return new ConnectionModel();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RemoveDependence()
        {
            SourceNode.OutputConnections.Remove(this);
            SinkNode?.InputConnections.Remove(this);
        }

       
    }
}
