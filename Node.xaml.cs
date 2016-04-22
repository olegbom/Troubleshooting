using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Troubleshooting.Annotations;

namespace Troubleshooting
{
    /// <summary>
    /// Логика взаимодействия для Node.xaml
    /// </summary>
    public partial class Node : UserControl,INotifyPropertyChanged
    {

        public Connection ConnectionOut;
        public List<Connection> ConnectionsIn = new List<Connection>();

        private string _text = "Название";
        public string Text
        {
            get { return _text; }
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private bool _editMode;
        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                if (value == _editMode) return;
                _editMode = value;
                OnPropertyChanged(nameof(EditMode));
            }
        }
               

        public Point OldPosition { get; private set; }

        public void SaveOldPosition() => OldPosition = Position;

        public Point Position
        {
            get { return new Point(X, Y); }
            set { X = value.X; Y = value.Y; }
        }


        public double X
        {
            get { return Canvas.GetLeft(this); }
            set
            {
                value = ((int) (value/10))*10;
                if (X == value) return;
                Canvas.SetLeft(this,value);
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get { return Canvas.GetTop(this); }
            set
            {
                value = ((int)(value / 10)) * 10;
                if (Y == value) return;
                Canvas.SetTop(this, value);
                OnPropertyChanged();
            }
        }

        public Node()
        {
            InitializeComponent();
            OnPropertyChanged(nameof(Text));


        }


        public void SetMouseManipulation(Canvas parent)
        {
            #region Size Bottom Right
            {
                bool move = false;
                Point moveStart = new Point();
                double xStart = 0, yStart = 0;
                RectSizeBR.MouseDown += (s, e) =>
                {
                    parent.Focus();
                    Keyboard.Focus(parent);

                    moveStart = e.GetPosition(parent);
                    xStart = ActualWidth;
                    yStart = ActualHeight;
                    CaptureMouse();
                    move = true;
                    e.Handled = true;
                };
                parent.MouseMove += (s, e) =>
                {
                    if (move)
                    {
                        Point moveNow = e.GetPosition(parent);
                        var newWidth  = xStart + (moveNow.X - moveStart.X);
                        var newHeight = yStart + (moveNow.Y - moveStart.Y);
                        newWidth = ((int)(newWidth / 10)) * 10;
                        newHeight = ((int)(newHeight / 10)) * 10;
                        Width  = (newWidth  >= MinWidth ) ? newWidth  : MinWidth;
                        Height = (newHeight >= MinHeight) ? newHeight : MinHeight;
                        e.Handled = true;
                    }
                };
                parent.MouseUp += (s, e) =>
                {
                    
                    move = false;
                    ReleaseMouseCapture();
                };

            }
            #endregion

            #region 
            {
                bool move = false;
                Point moveStart = new Point();
                Connection connection = null;
                ConnectorOut.MouseDown += (s, e) =>
                {
                    parent.Focus();
                    Keyboard.Focus(parent);

                    moveStart = e.GetPosition(parent);
                    connection = new Connection();

                  
                    
                    
                    connection.X2 = moveStart.X;
                    connection.Y2 = moveStart.Y;
                    parent.Children.Add(connection);
                    CaptureMouse();
                    move = true;
                    e.Handled = true;
                };
                parent.MouseMove += (s, e) =>
                {
                    if (move)
                    {
                        Point moveNow = e.GetPosition(parent);
                        connection.X2 = moveNow.X;
                        connection.Y2 = moveNow.Y;
                        e.Handled = true;
                    }
                };
                parent.MouseUp += (s, e) =>
                {
                    move = false;
                    ReleaseMouseCapture();
                };
            }
            #endregion

            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MyNode_MouseEnter(object sender, MouseEventArgs e)
        {
            EditMode = true;
        }

        private void MyNode_MouseLeave(object sender, MouseEventArgs e)
        {
            EditMode = false;
        }


    }
}
