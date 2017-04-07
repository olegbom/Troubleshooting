using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using PropertyChanged;
using Troubleshooting.Annotations;

namespace Troubleshooting.ViewModels
{
    public enum Orientation
    {
        Left, Top, Right, Bottom
    }


    [ImplementPropertyChanged]
    public class NodeViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<ConnectionViewModel> OutputConnections { get; } =
            new ObservableCollection<ConnectionViewModel>();

        public ObservableCollection<ConnectionViewModel> InputConnections { get; } =
            new ObservableCollection<ConnectionViewModel>();

        public string Text { get; set; }
        public bool EditMode { get; set; }
        public bool SelectMode { get; set; }
        public bool InvSelectMode => !SelectMode;
        public bool IntersectsMode { get; set; }
        
        public Point OldPosition { get; set; }

        public Point Position
        {
            get => new Point(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
                OnPropertyChanged();
            }

        }

        private double _x;
        public double X
        {
            get => _x;
            set
            {
                value = Math.Round(value / 10) * 10;
                if (_x == value) return;
                _x = value;
                OnPropertyChanged();
            }
        }

        private double _y;
        public double Y
        {
            get => _y;
            set
            {
                value = Math.Round(value / 10) * 10;
                if (_y == value) return;
                _y = value;
                foreach (var c in OutputConnections) c.SinkNode.OnInputConnectionsPositionsChanged();
                OnInputConnectionsPositionsChanged();
                OnPropertyChanged();
            }
        }
        public Vector OldSize { get; set; }
        public Vector Size
        {
            get { return new Vector(Width,Height);}
            set
            {
                Width = value.X;
                Height = value.Y;
                OnPropertyChanged();
            }
        }

        private double _minWidth;
        public double MinWidth
        {
            get { return _minWidth; }
            set
            {
                value = Math.Round(value / 10) * 10;
                if (_minWidth == value) return;
                _minWidth = value;
                if (Width < _minWidth) Width = _minWidth;
                OnPropertyChanged();
            }
        }


        private double _minHeight;
        public double MinHeight
        {
            get { return _minHeight; }
            set
            {
                value = Math.Round(value / 10) * 10;
                if (_minHeight == value) return;
                _minHeight = value;
                if (Height < _minHeight) Height = _minHeight;
                OnPropertyChanged();
            }
        }

        private double _width;
        public double Width
        {
            get { return _width; }
            set
            {
                value = Math.Round(value / 10) * 10;
                if (value < MinWidth) value = MinWidth;
                if(_width == value) return;
                _width = value;
                OnPropertyChanged();
            }
        }

        private double _height;
        public double Height
        {
            get { return _height; }
            set
            {
                value = Math.Round(value / 10) * 10;
                if (value < MinHeight) value = MinHeight;
                if (_height == value) return;
                _height = value;
                OnPropertyChanged();
            }
        }

        public Point[] InputConnectionsPositions
        {
            get
            {
                var grouped = InputConnections.GroupBy(c => c.EndConnectionOrientation);
                var connCount = InputConnections.Count;
                var result = new Point[connCount];
                foreach (var gr in grouped)
                {
                    ConnectionViewModel[] ordered;
                    switch (gr.Key)
                    {
                        case Orientation.Left:
                            ordered = gr.OrderBy(c => c.Y).ToArray();
                            for (int i = 0, count = ordered.Length; i < count; i++)
                                result[InputConnections.IndexOf(ordered[i])] = new Point(X, Y + Height * (i + 1) / (count + 1));
                            break;
                        case Orientation.Bottom:
                            ordered = gr.OrderBy(c => c.X).ToArray();
                            for (int i = 0, count = ordered.Length; i < count; i++)
                                result[InputConnections.IndexOf(ordered[i])] = new Point(X + Width * (i + 1) / (count + 1), Y + Height);
                            break;
                        case Orientation.Right:
                            ordered = gr.OrderBy(c => c.Y).ToArray();
                            for (int i = 0, count = ordered.Length; i < count; i++)
                                result[InputConnections.IndexOf(ordered[i])] = new Point(X + Width, Y + Height * (i + 1) / (count + 1));
                            break;
                        case Orientation.Top:
                            ordered = gr.OrderBy(c => c.X).ToArray();
                            for (int i = 0, count = ordered.Length; i < count; i++)
                                result[InputConnections.IndexOf(ordered[i])] = new Point(X + Width * (i + 1) / (count + 1), Y );
                            break;
                    }
                }
                return result;
            }
        }
        
        public void OnInputConnectionsPositionsChanged()
        {
            var connectionsPositions = InputConnectionsPositions;
            for (var i = 0; i < InputConnections.Count; i++)
                InputConnections[i].EndPoint = connectionsPositions[i];
        }


        public Point CenterPoint => new Point(X + Width / 2, Y + Height / 2);

        public void OnCenterPointChanged()
        {
            foreach (var connection in InputConnections)
            {
                connection.UpdateEndConnectionOrientation();
            }
        }

        public void SetOrientationTo(Point point)
        {
            var vector = point - CenterPoint;
            double phi = Math.Atan2(vector.Y, vector.X);

            if (phi < -3 * Math.PI / 4)
                OutOrientation = Orientation.Left;
            else if (phi < -Math.PI / 4)
                OutOrientation = Orientation.Top;
            else if (phi < Math.PI / 4)
                OutOrientation = Orientation.Right;
            else if (phi < 3 * Math.PI / 4)
                OutOrientation = Orientation.Bottom;
            else OutOrientation = Orientation.Left;
        }


        public Orientation OutOrientation { get; set; } = Orientation.Right;

        public void OnOutOrientationChanged()
        {
            foreach (var connection in OutputConnections)
            {
                connection.StartConnectionOrientation = OutOrientation;
            }
        }

        public Point OutputConnectionPosition
        {
            get
            {
                switch (OutOrientation)
                {
                    case Orientation.Bottom: return new Point(X + Width / 2, Y + Height);
                    case Orientation.Right: return new Point(X + Width, Y + Height / 2 - 1);
                    case Orientation.Top: return new Point(X + Width / 2, Y);
                    case Orientation.Left: return new Point(X, Y + Height / 2 - 1);
                    default: return new Point(X + Width, Y + Height / 2 - 1);
                }

            }
        }

        public void OnOutputConnectionPositionChanged()
        {
            var connectionPosition = OutputConnectionPosition;
            foreach (var c in OutputConnections)
                c.StartPoint = connectionPosition;
        }


        public HorizontalAlignment ConnectorOutHorizontalAlignment
        {
            get
            {
                switch (OutOrientation)
                {
                    case Orientation.Bottom: return HorizontalAlignment.Center;
                    case Orientation.Right: return HorizontalAlignment.Right;
                    case Orientation.Top: return HorizontalAlignment.Center;
                    case Orientation.Left: return HorizontalAlignment.Left;
                    default: return HorizontalAlignment.Right;
                }
            }
        }

        public VerticalAlignment ConnectorOutVerticalAlignment
        {
            get
            {
                switch (OutOrientation)
                {
                    case Orientation.Bottom: return VerticalAlignment.Bottom;
                    case Orientation.Right: return VerticalAlignment.Center;
                    case Orientation.Top: return VerticalAlignment.Top;
                    case Orientation.Left: return VerticalAlignment.Center;
                    default: return VerticalAlignment.Center;
                }
            }
        }

        public int LineConnectorGridRow
        {
            get
            {
                switch (OutOrientation)
                {
                    case Orientation.Bottom: return 2;
                    case Orientation.Right: return 1;
                    case Orientation.Top: return 0;
                    case Orientation.Left: return 1;
                    default: return 1;
                }
            }
        }

        public int LineConnectorGridColumn
        {
            get
            {
                switch (OutOrientation)
                {
                    case Orientation.Bottom: return 1;
                    case Orientation.Right: return 2;
                    case Orientation.Top: return 1;
                    case Orientation.Left: return 0;
                    default: return 2;
                }
            }
        }

        public int LineConnectorRotate
        {
            get
            {
                switch (OutOrientation)
                {
                    case Orientation.Bottom: return 90;
                    case Orientation.Right: return 0;
                    case Orientation.Top: return 90;
                    case Orientation.Left: return 0;
                    default: return 0;
                }
            }
        }



        public Rect Rect() => new Rect(Position, new Size(Width, Height));
        public Rect Rect(Point newPos) => new Rect(newPos, new Size(Width, Height));
        public Rect Rect(Vector newSize) => new Rect(Position, new Size(newSize.X, newSize.Y));

        public bool IntersectsWith(NodeViewModel node) => Rect().IntersectsWith(node.Rect());
        public bool IntersectsWith(NodeViewModel node, Point newPos) => Rect(newPos).IntersectsWith(node.Rect());
        public bool IntersectsWith(NodeViewModel node, Vector newSize) => Rect(newSize).IntersectsWith(node.Rect());
        
        public NodeViewModel()
        {
            Width = 40;
            Height = 50;
            MinWidth = 40;
            MinHeight = 40;
            InputConnections.CollectionChanged += (o, e) => OnInputConnectionsPositionsChanged();
            ;
            OutputConnections.CollectionChanged += (o, e) => OnOutputConnectionPositionChanged();
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
