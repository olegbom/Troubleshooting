using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Troubleshooting
{
    public class MyTable
    {
        public List<MyTable> Childrens { get; } = new List<MyTable>();

        public int TableDepth
        {
            get
            {
                if (Childrens.Any()) return Math.Max(Childrens[0].TableDepth, Childrens[1].TableDepth);
                return Level;
            }
        }

        public int Level { get; }
        public int[,] Workings { get; }
        public int[] Nums { get; }
        public int[] Ones { get; }
        public int[] Zeros { get; }
        public int[] W { get; }

        public MyTable(FunctionalDiagram diagram)
        {
            Level = 1;
            var count = diagram.Count;
            Workings = new int[count, count];
            Nums = new int[count];
            Ones = new int[count];
            Zeros = new int[count];
            W = new int[count];
            for (int i = 0; i < count; i++)
            {
                Nums[i] = i;
                foreach (var b in diagram) b.Working = true;
                diagram[i].ErrorInfluence();
                for (int j = 0; j < count; j++)
                    Workings[j, i] = diagram[j].Working ? 1 : 0;
            }
            ClackSums();
            if(count>1)
                Solve();
        }

        public MyTable(MyTable table, int[] nums)
        {
            Level = table.Level + 1;
            var count = nums.Length;
            Workings = new int[count,count];
            Ones = new int[count];
            Zeros = new int[count];
            Nums = new int[count];
            W = new int[count];
            for (int i = 0; i < count; i++)
            {
                Nums[i] = table.Nums[nums[i]];
                for (int j = 0; j < count; j++)
                    Workings[i, j] = table.Workings[nums[i], nums[j]];
            }
            ClackSums();
            if(count>1)
                Solve();
        }

        private void Solve()
        {
            var minWIndex = W.IndexOfMin();
            var nums0 = new List<int>();
            var nums1 = new List<int>();
            for (int j = 0; j < Nums.Length; j++)
            {
                if(Workings[minWIndex,j] == 0) nums0.Add(j); 
                else if(Workings[minWIndex,j] == 1) nums1.Add(j); 
            }

            Childrens.Add(new MyTable(this, nums0.ToArray()));
            Childrens.Add(new MyTable(this, nums1.ToArray()));
        }

        public void ClackSums()
        {
            for (int i = 0; i < Ones.Length; i++)
            {
                var ones = 0;
                var numLenght = Nums.Length;
                for (int j = 0; j < numLenght; j++)
                    ones += Workings[i, j];
                var zeros = numLenght - ones;
                Ones[i] = ones;
                Zeros[i] = zeros;
                W[i] = Math.Abs(zeros - ones);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Nums.Aggregate("    " ,(i,x) => $"{i}{x+1,4}"));
            sb.Append("\n");
            for (int i = 0; i < Nums.Length; i++)
            {
                string row= $"{(Nums[i]+1),4}";
                for (int j = 0; j < Nums.Length; j++)
                {
                    row += $"{Workings[i, j],4}";
                }
                row += $"{Ones[i],4}";
                row += $"{Zeros[i],4}";
                row += $"{W[i],4}\n";
                sb.Append(row);
            }

            return sb.ToString();
        }

        public Grid GetGrid()
        {
            Grid superGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top, 
                Margin = new Thickness(4,0,4,2)
            };
            Grid labelGrid = new Grid();
            
            TextBlock tb = new TextBlock
            {
                Text = $"Z{Nums[W.IndexOfMin()] + 1}",
                FontSize = 14,
                Margin = new Thickness(2,0,2,0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            labelGrid.Children.Add(tb);
          
            superGrid.Children.Add(labelGrid);

            if (Nums.Length > 1) // 
            {
                //var grid = GenerateInfluenceGrid();
                //superGrid.Children.Add(grid);
                Rectangle rect = new Rectangle
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                    StrokeThickness = 1,
                    SnapsToDevicePixels = true,
                };
                labelGrid.Children.Add(rect);

                Path path = new Path()
                {
                    SnapsToDevicePixels = true,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                PathGeometry geometry = new PathGeometry(new []
                {
                    new PathFigure(new Point(0,labelGrid.ActualWidth/2), new []
                    {
                        new LineSegment(new Point(-10,labelGrid.ActualWidth/2), true ),
                        new LineSegment(new Point(-10, labelGrid.ActualWidth), true ), 
                    } , false ),
                    new PathFigure(new Point(labelGrid.ActualHeight,labelGrid.ActualWidth/2), new []
                    {
                        new LineSegment(new Point(10,labelGrid.ActualWidth/2), true ),
                        new LineSegment(new Point(10, labelGrid.ActualWidth), true ),
                    } , false ),
                });
                path.Data = geometry;
                labelGrid.Children.Add(path);
                // labelGrid.SetBinding(Grid.VisibilityProperty,
                //   new Binding("LabelZVisibility") {ElementName = "MyWindow"});
            }
            else
            {
                Rectangle rect = new Rectangle
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                    StrokeThickness = 1,
                    SnapsToDevicePixels = true,
                    RadiusX = 5,
                    RadiusY = 5
                };
                labelGrid.Children.Add(rect);
            }



            return superGrid;
        }

        private Grid GenerateInfluenceGrid()
        {
            Grid grid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Top,
                
            };
            for (int i = 0, count = Nums.Length + 4; i < count; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0, count = Nums.Length + 2; i < count; i++)
                grid.RowDefinitions.Add(new RowDefinition());

            grid.AddChildren(new TextBlock
            {
                Text = "Zi",
                Width = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            }, 0, 0);


            grid.AddChildren(new TextBlock
            {
                Text = "Состояние блока Si",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }, 1, 0, Nums.Length);
            grid.AddChildren(new TextBlock
            {
                Text = "Число\nсостояний",
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5, 0, 5, 0)
            }, Nums.Length + 1, 0, 2);
            grid.AddChildren(new TextBlock
            {
                Text = "Wi",
                Width = 25,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            }, Nums.Length + 3, 0);

            grid.AddChildren(new TextBlock {Text = ""}, 0, 1);
            for (int i = 0; i < Nums.Length; i++)
            {
                grid.AddChildren(new TextBlock {Text = $"{Nums[i] + 1}", Width = 25, TextAlignment = TextAlignment.Center},
                    i + 1, 1);
            }
            grid.AddChildren(new TextBlock {Text = "0", HorizontalAlignment = HorizontalAlignment.Center}, Nums.Length + 1, 1);
            grid.AddChildren(new TextBlock {Text = "1", HorizontalAlignment = HorizontalAlignment.Center}, Nums.Length + 2, 1);
            grid.AddChildren(new TextBlock {Text = ""}, Nums.Length + 3, 1);

            for (int i = 0; i < Nums.Length; i++)
            {
                var color = (i == W.IndexOfMin()) ? Color.FromArgb(255, 173, 216, 230) : default(Color);
                grid.AddChildren(new TextBlock
                {
                    Text = $"Z{Nums[i] + 1}",
                    Margin = new Thickness(3, 0, 0, 0)
                }, 0, i + 2, 1, 1, color);
                for (int j = 0; j < Nums.Length; j++)
                {
                    grid.AddChildren(new TextBlock
                        {
                            Text = $"{Workings[i, j]}",
                            HorizontalAlignment = HorizontalAlignment.Center
                        }, j + 1, i + 2, 1, 1,
                        (Workings[i, j] == 1)
                            ? Color.Add(
                                Color.FromArgb(255, 245, 245, 220),
                                color)
                            : color);
                }
                grid.AddChildren(new TextBlock
                {
                    Text = $"{Zeros[i]}",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 3, 0)
                }, Nums.Length + 1, i + 2, 1, 1, color);
                grid.AddChildren(new TextBlock
                {
                    Text = $"{Ones[i]}",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 3, 0)
                }, Nums.Length + 2, i + 2, 1, 1, color);
                grid.AddChildren(new TextBlock
                {
                    Text = $"{W[i]}",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 3, 0)
                }, Nums.Length + 3, i + 2, 1, 1, color);
            }

            grid.SetBinding(Grid.VisibilityProperty,
                new Binding("GridsVisibility") { ElementName = "MyWindow" });

            return grid;
        }
    }
}
