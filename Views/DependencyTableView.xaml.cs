using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Troubleshooting.ViewModels;

namespace Troubleshooting.Views
{
    /// <summary>
    /// Логика взаимодействия для DependencyTableView.xaml
    /// </summary>
    public partial class DependencyTableView : Window
    {
        public DependencyTableViewModel ViewModel { get; set; }

        public DependencyTableView(DependencyTableViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            for (int i = 0, count = viewModel.Count; i < count; i++)
                DependencyDiagram.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0, count = viewModel.TableDepth+1; i < count; i++)
                DependencyDiagram.RowDefinitions.Add(new RowDefinition());

            AddMyTableToMetaGrid(viewModel,0,0);
            CreateDependencyTable(ViewModel);
        }

        public void AddMyTableToMetaGrid(DependencyTableViewModel myTable, int column, int row)
        {
            Button button = LabelGrid(myTable);
            DependencyDiagram.AddChildren(button, column, row, myTable.Count, 1);
            if (myTable.Childrens.Any())
            {
                AddMyTableToMetaGrid(myTable.Childrens[0], column, row + 1);
                AddMyTableToMetaGrid(myTable.Childrens[1], column + myTable.Childrens[0].Count, row + 1);
            }
        }

        public Button LabelGrid(DependencyTableViewModel myTable)
        {

            var minW = myTable.W.IndexOfMin();
            Button button = new Button
            {
                Content = $"Z{myTable.Nodes[minW].Zindex + 1}",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
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
            

            return button;
        }



        private void CreateDependencyTable(DependencyTableViewModel vm)
        {
            DependencyTable.ColumnDefinitions.Clear();
            DependencyTable.RowDefinitions.Clear();
            DependencyTable.Children.Clear();

            for (int i = 0, count = vm.Count + 4; i < count; i++)
                DependencyTable.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            for (int i = 0, count = vm.Count + 2; i < count; i++)
                DependencyTable.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            DependencyTable.AddChildren(new TextBlock
            {
                Text = "Zi",
                Width = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            }, 0, 0);


            DependencyTable.AddChildren(new TextBlock
            {
                Text = "Состояние блока Si",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }, 1, 0, vm.Count);
            DependencyTable.AddChildren(new TextBlock
            {
                Text = "Число\nсостояний",
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5, 0, 5, 0)
            }, vm.Count + 1, 0, 2);
            DependencyTable.AddChildren(new TextBlock
            {
                Text = "Wi",
                Width = 25,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            }, vm.Count + 3, 0);

            DependencyTable.AddChildren(new TextBlock { Text = "" }, 0, 1);
            for (int i = 0; i < vm.Count; i++)
            {
                
                var zindexTextBlock = new TextBlock
                {
                    Text = $"{vm.Nodes[i].Zindex + 1}",
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
                DependencyTable.AddChildren(zindexTextBlock, i + 1, 1);
            }
            DependencyTable.AddChildren(new TextBlock
            {
                Text = "0",
                HorizontalAlignment = HorizontalAlignment.Center
            }, vm.Count + 1, 1);
            DependencyTable.AddChildren(new TextBlock
            {
                Text = "1",
                HorizontalAlignment = HorizontalAlignment.Center
            }, vm.Count + 2, 1);
            DependencyTable.AddChildren(new TextBlock { Text = "" }, vm.Count + 3, 1);

            var wIntexOfMin = vm.W.IndexOfMin();
            for (int i = 0; i < vm.Count; i++)
            {
                var color = i == wIntexOfMin ? Color.FromArgb(255, 173, 216, 230) : default(Color);

                var zindexTextBlock = new TextBlock
                {
                    Text = $"Z{vm.Nodes[i].Zindex + 1}",
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

                DependencyTable.AddChildren(zindexTextBlock, 0, i + 2, 1, 1, color);


                for (int j = 0; j < vm.Count; j++)
                {
                    var dependTextBlock = new TextBlock
                    {
                        Text = $"{vm.IsWorkMatrix[i, j]}",
                        MinWidth = 20,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Background = new SolidColorBrush(vm.IsWorkMatrix[i, j] == 1
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

                    DependencyTable.AddChildren(dependTextBlock, j + 1, i + 2);
                }
                DependencyTable.AddChildren(new TextBlock
                {
                    Text = $"{vm.Zeros[i]}",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 3, 0)
                }, vm.Count + 1, i + 2, 1, 1, color);
                DependencyTable.AddChildren(new TextBlock
                {
                    Text = $"{vm.Ones[i]}",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 3, 0)
                }, vm.Count + 2, i + 2, 1, 1, color);
                DependencyTable.AddChildren(new TextBlock
                {
                    Text = $"{vm.W[i]}",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 3, 0)
                }, vm.Count + 3, i + 2, 1, 1, color);
            }
        }
    }
}
