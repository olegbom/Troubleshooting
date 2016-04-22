using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Troubleshooting.Annotations;

namespace Troubleshooting
{
    /// <summary>
    /// Логика взаимодействия для Connection.xaml
    /// </summary>
    public partial class Connection : UserControl, INotifyPropertyChanged
    {

       
        public double X1{
            get{
                return MyLine.X1;
            }
            set{
                if (value == MyLine.X1) return;
                MyLine.X1 = value;
                OnPropertyChanged();
            }
        }

        public double X2
        {
            get
            {
                return MyLine.X2;
            }
            set
            {
                if (value == MyLine.X2) return;
                MyLine.X2 = value;
                OnPropertyChanged();
            }
        }


        public double Y1
        {
            get
            {
                return MyLine.Y1;
            }
            set
            {
                if (value == MyLine.Y1) return;
                MyLine.Y1 = value;
                OnPropertyChanged();
            }
        }

        public double Y2
        {
            get
            {
                return MyLine.Y2;
            }
            set
            {
                if (value == MyLine.Y2) return;
                MyLine.Y2 = value;
                OnPropertyChanged();
            }
        }



        public Connection()
        {
            InitializeComponent();
            

        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
