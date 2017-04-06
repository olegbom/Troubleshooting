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
    [ImplementPropertyChanged]
    public class NodeViewModel:INotifyPropertyChanged
    {

        public ObservableCollection<ConnectionViewModel> OutputConnectios { get; } = new ObservableCollection<ConnectionViewModel>();

        public ObservableCollection<ConnectionViewModel> InputConnections { get; } = new ObservableCollection<ConnectionViewModel>();

        public Point OutputConnectionPosition => new Point(X + Width, Y + Height / 2);

        
        public void OnOutputConnectionPositionChanged()
        {
            var connectionPosition = OutputConnectionPosition;
            foreach (var c in OutputConnectios)
                c.StartPoint = connectionPosition;
        }


        public Point[] InputConnectionsPositions => InputConnections.Select(
                (c, i) => new {
                    SourceY = c.SourceNode.Y,
                    P =  new Point(X, Y + Height * (i + 1) / (InputConnections.Count + 1))
                }).OrderBy(anon => anon.SourceY).Select(anon => anon.P).ToArray();
            

        public void OnInputConnectionsPositionsChanged()
        {
            var connectionsPositions = InputConnectionsPositions;
            for (var i = 0; i < InputConnections.Count; i++)
                InputConnections[i].EndPoint = connectionsPositions[i];
        }


        public string Text { get; set; }
        public bool EditMode { get; set; }
        public bool SelectMode { get; set; }
        public bool InvSelectMode => !SelectMode;
        public bool IntersectsMode { get; set; }

        public int InputsCount { get; set; } = 1;

        public Brush BackgroundFillBrush
        {
            get
            {
                if (SelectMode) return Brushes.DodgerBlue;
                return Brushes.White;
            }
        }
        
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
                foreach (var c in OutputConnectios) c.SinkNode.OnInputConnectionsPositionsChanged();
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
            OutputConnectios.CollectionChanged += (o, e) => OnOutputConnectionPositionChanged();
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
