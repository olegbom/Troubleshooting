using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Troubleshooting.Annotations;
using Troubleshooting.ViewModel;

namespace Troubleshooting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>


        
        private double _scale = 1;
        public double Scale
        {
            get { return _scale; }
            set
            {
                if (_scale == value) return;
                //if ((value < 0.3 && _scale >= 0.3) ||
                //    (value >= 0.3 && _scale < 0.3))
                {
                    OnPropertyChanged(nameof(LabelZVisibility));
                    OnPropertyChanged(nameof(GridsVisibility));
                }
                _scale = value;
                OnPropertyChanged(nameof(Scale));
                
            }
        }

        public Visibility LabelZVisibility => Scale <= 0.5 ? Visibility.Visible : Visibility.Hidden;
        public Visibility GridsVisibility => Scale > 0.5 ? Visibility.Visible : Visibility.Hidden;

        private double _x;
        public double X
        {
            get { return _x; }
            set
            {
                if (_x == value) return;
                _x = value;
                OnPropertyChanged(nameof(X));
            }
        }

        private double _y;
        public double Y
        {
            get { return _y; }
            set
            {
                if (_y == value) return;
                _y = value;
                OnPropertyChanged(nameof(Y));
            }
        }



        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            FunctionalDiagram diagram = new FunctionalDiagram(26);
            DiagramEditor diagramEditor = new DiagramEditor();
            diagramEditor.ShowDialog();
            diagram.Connect(1,2,3);
            diagram.Connect(2,4);
            diagram.Connect(3,5);
            diagram.Connect(4,6,7);
            diagram.Connect(5,6,7);
            diagram.Connect(6,9,10);
            diagram.Connect(7,8,22);
            diagram.Connect(8,11,12);
            diagram.Connect(9,13);
            diagram.Connect(10,14);
            diagram.Connect(11,15);
            diagram.Connect(12,16);
            diagram.Connect(13,17);
            diagram.Connect(14,17);
            diagram.Connect(15,18);
            diagram.Connect(16,18);
            diagram.Connect(17,19);
            diagram.Connect(18,20);
            diagram.Connect(19,21);
            diagram.Connect(20,21);
            diagram.Connect(21,23);
            diagram.Connect(22,23);
            diagram.Connect(23,24);
            diagram.Connect(24,25);
            diagram.Connect(25,26);

            
            var result = diagram.GetMyTable();
            Console.WriteLine(result.ToString());
            AddMyTableToCanvas(result);


        }

        public void AddMyTableToCanvas(MyTable myTable, double left = 0, double top = 0)
        {
            
            Grid metaGrid = new Grid();
            for (int i = 0, count = myTable.Nums.Length; i < count; i++)
                metaGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0, count = myTable.TableDepth; i < count; i++)
                metaGrid.RowDefinitions.Add(new RowDefinition());

            AddMyTableToMetaGrid(metaGrid, myTable, 0, 0);
            Canvas1.Children.Add(metaGrid);
        }

        public void AddMyTableToMetaGrid(Grid metaGrid, MyTable myTable, int column, int row)
        {
            Grid grid = myTable.GetGrid();
            metaGrid.AddChildren(grid, column, row, myTable.Nums.Length, 1, default(Color), false);
            if (myTable.Childrens.Any())
            {
                AddMyTableToMetaGrid(metaGrid, myTable.Childrens[0], column, row + 1);
                AddMyTableToMetaGrid(metaGrid, myTable.Childrens[1], column + myTable.Childrens[0].Nums.Length, row + 1);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private Point _coordMouseDown;
        private double _oldX, _oldY;
 
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                _coordMouseDown = e.GetPosition(this);
                _oldX = X;
                _oldY = Y;
            }
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                var coordMouse = e.GetPosition(this);
                X = _oldX + (coordMouse.X - _coordMouseDown.X);
                Y = _oldY + (coordMouse.Y - _coordMouseDown.Y);
            }
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private void Window_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            
            double mpl = 1 + 0.1 * e.Delta / 120;
            Scale *= mpl;
            Point mousePos = e.GetPosition(this);
            double mX = mousePos.X;
            double mY = mousePos.Y;
            X = mX - (mX - X)*mpl;
            Y = mY - (mY - Y)*mpl;
        }
    }
}