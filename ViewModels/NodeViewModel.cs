﻿using System;
using System.ComponentModel;
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

      

        public ConnectionViewModel ConnectionOut { get; set; }
      
        //[DependsOn(nameof(X), nameof(Width), nameof(Y), nameof(Height))]
        public Point ConnectorOutPos => new Point(X + Width, Y + Height / 2);

        //[DependsOn(nameof(X), nameof(Y), nameof(Height))]
        public Point ConnectorInPos => new Point(X, Y + Height / 2);
        
        public string Text { get; set; }
        public bool EditMode { get; set; }
        public bool PreSelectMode { get; set; }
        public bool SelectMode { get; set; }
        public bool InvSelectMode => !SelectMode;
        public bool IntersectsMode { get; set; }

        //[DependsOn(nameof(PreSelectMode), nameof(SelectMode))]
        public Brush BackgroundFillBrush
        {
            get
            {
                if (SelectMode) return Brushes.DodgerBlue;
                if (PreSelectMode) return Brushes.LightSkyBlue;
                return Brushes.White;
            }
        }

        


        public Point OldPosition { get; set; }

        public Point Position
        {
            get { return new Point(X, Y); }
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
            get { return _x; }
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
            get { return _y; }
            set
            {
                value = Math.Round(value / 10) * 10;
                if (_y == value) return;
                _y = value;
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
