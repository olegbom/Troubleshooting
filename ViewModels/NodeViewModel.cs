using MugenMvvmToolkit.ViewModels;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Troubleshooting.ViewModels
{
    public enum Orientation
    {
        Left, Top, Right, Bottom
    }


    [ImplementPropertyChanged]
    public class NodeViewModel: WorkspaceViewModel
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
        

        public bool IsWork { get; set; }


        

        public Point OldPosition { get; set; }

        public Point Position
        {
            get => new Point(X, Y);
            set
            {
                X = (int)value.X;
                Y = (int)value.Y;
                OnPropertyChanged();
            }

        }

        private int _x;
        public int X
        {
            get => _x;
            set
            {
                value = (value / 10) * 10;
                if (_x == value) return;
                _x = value;
                OnPropertyChanged();
            }
        }

        private int _y;
        public int Y
        {
            get => _y;
            set
            {
                value = (value / 10) * 10;
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
            get => new Vector(Width,Height);
            set
            {
                Width = (int)value.X;
                Height = (int)value.Y;
                OnPropertyChanged();
            }
        }

        private int _minWidth;
        public int MinWidth
        {
            get => _minWidth;
            set
            {
                value = (value / 10) * 10;
                if (_minWidth == value) return;
                _minWidth = value;
                if (Width < _minWidth) Width = _minWidth;
                OnPropertyChanged();
            }
        }


        private int _minHeight;
        public int MinHeight
        {
            get => _minHeight;
            set
            {
                value = (value / 10) * 10;
                if (_minHeight == value) return;
                _minHeight = value;
                if (Height < _minHeight) Height = _minHeight;
                OnPropertyChanged();
            }
        }

        private int _width;
        public int Width
        {
            get => _width;
            set
            {
                value = (value / 10) * 10;
                if (value < MinWidth) value = MinWidth;
                if(_width == value) return;
                _width = value;
                OnPropertyChanged();
            }
        }

        private int _height;
        public int Height
        {
            get => _height;
            set
            {
                value = (value / 10) * 10;
                if (value < MinHeight) value = MinHeight;
                if (_height == value) return;
                _height = value;
                OnPropertyChanged();
            }
        }

        public int BorderWidth 
        {
            get
            {
                switch (OutOrientation)
                {
                    case Orientation.Bottom: return Width;
                    case Orientation.Right: return Width-10;
                    case Orientation.Top: return Width;
                    case Orientation.Left: return Width-10;
                    default: return Width;
                }
            }
        }

        public int BorderHeight
        {
            get
            {
                switch (OutOrientation)
                {
                    case Orientation.Bottom: return Height-10;
                    case Orientation.Right: return Height;
                    case Orientation.Top: return Height-10;
                    case Orientation.Left: return Height;
                    default: return Height;
                }
            }
        }

        public Point[] InputConnectionsPositions
        {
            get
            {
                var grouped = InputConnections.Select((c,i) => new{I = i, C = c }).GroupBy(g => g.C.EndConnectionOrientation);
                var result = new Point[InputConnections.Count];
                var height = BorderHeight;
                var width = BorderWidth;
                var deltaX = (OutOrientation == Orientation.Left) ? 10 : 0;
                var deltaY = (OutOrientation == Orientation.Top) ? 10 : 0;
                foreach (var gr in grouped)
                {
                    
                    switch (gr.Key)
                    {
                        case Orientation.Left:
                        {
                            var ordered = gr.OrderBy(g => g.C.Y).ToArray();
                            for (int i = 0, count = ordered.Length; i < count; i++)
                                result[ordered[i].I] = new Point(X + deltaX, Y + height * (i + 1) / (count + 1) + deltaY);
                        }
                        break;
                        case Orientation.Bottom:
                        {
                            var ordered = gr.OrderBy(g => g.C.X).ToArray();
                            for (int i = 0, count = ordered.Length; i < count; i++)
                                result[ordered[i].I] = new Point(X + width * (i + 1) / (count + 1) + deltaX, Y + height + deltaY);
                        }
                        break;
                        case Orientation.Right:
                        {
                            var ordered = gr.OrderBy(g => g.C.Y).ToArray();
                            for (int i = 0, count = ordered.Length; i < count; i++)
                                result[ordered[i].I] = new Point(X + width + deltaX, Y + height * (i + 1) / (count + 1) + deltaY);
                        }
                        break;
                        case Orientation.Top:
                        {
                            var ordered = gr.OrderBy(g => g.C.X).ToArray();
                            for (int i = 0, count = ordered.Length; i < count; i++)
                                result[ordered[i].I] = new Point(X + width * (i + 1) / (count + 1) + deltaX, Y + deltaY);
                        }
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
                    case Orientation.Right: return new Point(X + Width, Y + Height / 2);
                    case Orientation.Top: return new Point(X + Width / 2, Y);
                    case Orientation.Left: return new Point(X, Y + Height / 2);
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

        public bool IsLineConnectorRotate
        {
            get
            {
                switch (OutOrientation)
                {
                    case Orientation.Bottom: return true;
                    case Orientation.Right: return false;
                    case Orientation.Top: return true;
                    case Orientation.Left: return false;
                    default: return false;
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
            OutputConnections.CollectionChanged += (o, e) => OnOutputConnectionPositionChanged();
        }

     
    }
}
