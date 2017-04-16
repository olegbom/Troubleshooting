using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PropertyChanged;
using Troubleshooting.Annotations;


namespace Troubleshooting.ViewModels
{
    [Serializable]
    [ImplementPropertyChanged]
    public class SelectRectangleViewModel: INotifyPropertyChanged
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point Point1 => new Point(Point2.X, 0);
        

        public Point Point2 { get; set; }


        public Point Point3 => new Point(0, Point2.Y);
        public bool Visible { get; set; }

        public Rect Rect() => new Rect(
            new Point(Math.Min(X, X+Point2.X) , Math.Min(Y,Y + Point2.Y )),
            new Size (Math.Abs(Point2.X), Math.Abs(Point2.Y)));

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
