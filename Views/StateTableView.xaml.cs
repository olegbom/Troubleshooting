using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Troubleshooting.ViewModels;

namespace Troubleshooting.Views
{
    /// <summary>
    /// Логика взаимодействия для StateTableView.xaml
    /// </summary>
    public partial class StateTableView : Window
    {
        public StateTableViewModel ViewModel { get; set; }

        private static readonly FontFamily TimesNewRoman = new FontFamily("Times New Roman");

        public StateTableView(StateTableViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            for (int i = 0, count = viewModel.Count; i < count; i++)
                DependencyDiagram.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto});
            for (int i = 0, count = viewModel.TableDepth+1; i < count; i++)
                DependencyDiagram.RowDefinitions.Add(new RowDefinition{ Height = GridLength.Auto });

            AddMyTableToMetaGrid(viewModel,0,0,2);
            CreateDependencyTable(ViewModel);
        }

        public void AddMyTableToMetaGrid(StateTableViewModel myTable, int column, int row, int isWork)
        {
            Grid grid = LabelGrid(myTable, isWork);
            DependencyDiagram.AddChildren(grid, column, row, myTable.Count, 1, default(Color), false);
            
            if (myTable.Childrens.Any())
            {
                AddMyTableToMetaGrid(myTable.Childrens[0], column, row + 1, 0);
                AddMyTableToMetaGrid(myTable.Childrens[1], column + myTable.Childrens[0].Count, row + 1, 1);
            }
        }

        public Grid LabelGrid(StateTableViewModel myTable, int isWork)
        {
            Grid myGrid = new Grid();

            myGrid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(20)});
            myGrid.RowDefinitions.Add(new RowDefinition(){Height = GridLength.Auto});

            myGrid.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(1,GridUnitType.Star)});
            myGrid.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(1,GridUnitType.Star) });

            var minW = myTable.W.IndexOfMin();
            Button button = new Button
            {
                Content = $"Z{myTable.Nodes[minW].Zindex + 1}",
                FontFamily = TimesNewRoman,
                FontSize = 14,
                MinWidth = 28,
                Margin= new Thickness(2,0,2,0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
            };
            button.MouseEnter += (o, e) =>
            {
                myTable.Nodes[minW].ParentMode = true;
                myTable.Nodes[minW].ChildMode = true;
            };
            button.MouseLeave += (o, e) =>
            {
                myTable.Nodes[minW].ParentMode = false;
                myTable.Nodes[minW].ChildMode = false;
            };

            if (myTable.Count > 1)
            {
                button.Click += (o, e) =>
                {
                    CreateDependencyTable(myTable);
                };
            }
            
            myGrid.AddChildren(button,0,1,2,1,default(Color),false);

            if (isWork < 2)
            {
                Path path = new Path
                {
                    Data = new PathGeometry(new[]
                    {
                        new PathFigure(new Point(7, 0), new[]
                        {
                            new LineSegment(new Point(7, 20), true),
                            new LineSegment(new Point(14, 10), false),
                            new LineSegment(new Point(7, 20), true),
                            new LineSegment(new Point(0, 10), true),
                        }, false),
                    }),
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    
                };
                TextBlock textBlock = new TextBlock()
                {
                    Text = isWork.ToString(),
                    Margin = new Thickness(5, 0, 5, 0),
                    FontFamily = TimesNewRoman,
                    FontSize = 14,
                };

                if (isWork == 0)
                {
                    myGrid.AddChildren(path, 1, 0, 1, 1, default(Color), false);
                    path.HorizontalAlignment = HorizontalAlignment.Left;
                    myGrid.AddChildren(textBlock, 0, 0, 1, 1, default(Color), false);
                    textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                }
                if (isWork == 1)
                {
                    myGrid.AddChildren(path, 0, 0, 1, 1, default(Color), false);
                    path.HorizontalAlignment = HorizontalAlignment.Right;
                    myGrid.AddChildren(textBlock, 1, 0, 1, 1, default(Color), false);
                    textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                }
            }
          
            

            return myGrid;
        }



        private void CreateDependencyTable(StateTableViewModel vm)
        {
            foreach (var node in ViewModel.Nodes)
            {
                node.WarSmokeMode = true;
            }

            foreach (var node in vm.Nodes)
            {
                node.WarSmokeMode = false;
            }

            DependencyTable.ColumnDefinitions.Clear();
            DependencyTable.RowDefinitions.Clear();
            DependencyTable.Children.Clear();

            for (int i = 0, count = vm.Count + 4; i < count; i++)
                DependencyTable.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            for (int i = 0, count = vm.Count + 3; i < count; i++)
                DependencyTable.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            DependencyTable.AddChildren(new TextBlock
            {
                Text = vm.TableDescription,
                FontFamily = TimesNewRoman,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            }, 0, 0, vm.Count + 4, 1, default(Color), false);

            DependencyTable.AddChildren(new TextBlock
            {
                Text = "Zi",
                FontFamily = TimesNewRoman,
                FontSize = 14,
                Width = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            }, 0, 1);


            DependencyTable.AddChildren(new TextBlock
            {
                Text = "Состояние блока Si",
                FontSize = 14,
                FontFamily = TimesNewRoman,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }, 1, 1, vm.Count);
            DependencyTable.AddChildren(new TextBlock
            {
                Text = "Число\nсостояний",
                FontFamily = TimesNewRoman,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5, 0, 5, 0)
            }, vm.Count + 1, 1, 2);
            DependencyTable.AddChildren(new TextBlock
            {
                Text = "Wi",
                FontFamily = TimesNewRoman,
                FontSize = 14,
                Width = 25,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            }, vm.Count + 3, 1);

            DependencyTable.AddChildren(new TextBlock { Text = "" }, 0, 1);
            for (int i = 0; i < vm.Count; i++)
            {
                
                var zindexTextBlock = new TextBlock
                {
                    Text = $"{vm.Nodes[i].Zindex + 1}",
                    FontFamily = TimesNewRoman,
                    FontSize = 14,
                    MinWidth = 25,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Center
                };
                int zindexI = i;
                zindexTextBlock.MouseEnter += (o, e) =>
                {
                    vm.Nodes[zindexI].ParentMode = true;
                    vm.Nodes[zindexI].ChildMode = true;
                };
                zindexTextBlock.MouseLeave += (o, e) =>
                {
                    vm.Nodes[zindexI].ParentMode = false;
                    vm.Nodes[zindexI].ChildMode = false;
                };
                DependencyTable.AddChildren(zindexTextBlock, i + 1, 2);
            }
            DependencyTable.AddChildren(new TextBlock
            {
                Text = "0",
                FontFamily = TimesNewRoman,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center, 
                VerticalAlignment = VerticalAlignment.Center
            }, vm.Count + 1, 2);
            DependencyTable.AddChildren(new TextBlock
            {
                Text = "1",
                FontFamily = TimesNewRoman,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }, vm.Count + 2, 2);
            DependencyTable.AddChildren(new TextBlock { Text = "" }, vm.Count + 3, 2);

            var wIntexOfMin = vm.W.IndexOfMin();
            for (int i = 0; i < vm.Count; i++)
            {
                var color = i == wIntexOfMin ? Color.FromArgb(255, 173, 216, 230) : default(Color);

                var zindexTextBlock = new TextBlock
                {
                    Text = $"Z{vm.Nodes[i].Zindex + 1}",
                    FontSize = 14,
                    FontFamily = TimesNewRoman,
                    Margin = new Thickness(3, 0, 0, 0)
                };
                int zindexI = i;
                zindexTextBlock.MouseEnter += (o, e) =>
                {
                    vm.Nodes[zindexI].ParentMode = true;
                    vm.Nodes[zindexI].ChildMode = true;
                };
                zindexTextBlock.MouseLeave += (o, e) =>
                {
                    vm.Nodes[zindexI].ParentMode = false;
                    vm.Nodes[zindexI].ChildMode = false;
                };

                DependencyTable.AddChildren(zindexTextBlock, 0, i + 3, 1, 1, color);


                for (int j = 0; j < vm.Count; j++)
                {
                    var dependTextBlock = new TextBlock
                    {
                        Text = $"{vm.IsWorkMatrix[i, j]}",
                        FontFamily = TimesNewRoman,
                        FontSize = 14,
                        TextAlignment = TextAlignment.Center,
                        MinWidth = 25,
                        Margin = new Thickness(1,0.5,0.5,1),
                        Background = new SolidColorBrush( vm.IsWorkMatrix[i, j] == 1
                            ? Color.Add(Color.FromArgb(255, 245, 245, 220), color) : color)
                    };

                    if (vm.IsWorkMatrix[i, j] == 0)
                    {
                        var i1 = i;
                        var j1 = j;
                        dependTextBlock.MouseEnter += (o, e) =>
                        {
                            vm.Nodes[j1].ParentMode = true;
                            vm.Nodes[i1].ChildMode = true;
                        };
                        dependTextBlock.MouseLeave += (o, e) =>
                        {
                            vm.Nodes[j1].ParentMode = false;
                            vm.Nodes[i1].ChildMode = false;
                        };
                    }

                    DependencyTable.AddChildren(dependTextBlock, j + 1, i + 3);
                }
                DependencyTable.AddChildren(new TextBlock
                {
                    Text = $"{vm.Zeros[i]}",
                    FontFamily = TimesNewRoman,
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 3, 0)
                }, vm.Count + 1, i + 3, 1, 1, color);
                DependencyTable.AddChildren(new TextBlock
                {
                    Text = $"{vm.Ones[i]}",
                    FontFamily = TimesNewRoman,
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 3, 0)
                }, vm.Count + 2, i + 3, 1, 1, color);
                DependencyTable.AddChildren(new TextBlock
                {
                    Text = $"{vm.W[i]}",
                    FontSize = 14,
                    FontFamily = TimesNewRoman,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 3, 0)
                }, vm.Count + 3, i + 3, 1, 1, color);
            }
        }

        private void DependencyTableView_OnClosed(object sender, EventArgs e)
        {
            foreach (var node in ViewModel.Nodes)
            {
                node.WarSmokeMode = false;
            }
        }

     

        private void SaveToPictureDependencyDiagram_OnClick(object sender, RoutedEventArgs e)
        {
            DependencyDiagram.SaveAsPictureViaDialog();
            
        }

        private void CopyAsPictureDependencyDiagram_OnClick(object sender, RoutedEventArgs e)
        {
            DependencyDiagram.ToClipboardAsPicture();
        }

        private void SaveToPictureDependencyTable_OnClick(object sender, RoutedEventArgs e)
        {
            DependencyTable.SaveAsPictureViaDialog();
        }
        
        private void CopyAsPictureDependencyTable_OnClick(object sender, RoutedEventArgs e)
        {
            DependencyTable.ToClipboardAsPicture();
        }
    }
}
