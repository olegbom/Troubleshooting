using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Troubleshooting
{
    public static class Helper
    {
        // Returns last index of the value that is the minimum.
        public static int IndexOfMin(this IEnumerable<int> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int minValue = int.MaxValue;
            int minIndex = -1;
            int index = -1;

            foreach (int num in source)
            {
                index++;

                if (num < minValue)
                {
                    minValue = num;
                    minIndex = index;
                }
            }

            if (index == -1)
                throw new InvalidOperationException("Sequence was empty");

            return minIndex;
        }
        public static int IndexOfMax(this IEnumerable<int> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int maxValue = int.MinValue;
            int maxIndex = -1;
            int index = -1;

            foreach (int num in source)
            {
                index++;
                if (num > maxValue)
                {
                    maxValue = num;
                    maxIndex = index;
                }
            }

            if (index == -1)
                throw new InvalidOperationException("Sequence was empty");

            return maxIndex;
        }

        public static void AddChildren(this Grid grid, UIElement element, int column, int row, int columnSnap = 1, int rowSnap = 1, Color color = default(Color))
        {
            Rectangle rect = new Rectangle
            {
                Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                Fill = new SolidColorBrush(color),
                Margin = new Thickness(-0.5)
            };

            Grid.SetColumn(rect, column);
            Grid.SetRow(rect, row);

            Grid.SetColumn(rect, column);
            Grid.SetRow(rect, row);
            if (columnSnap > 1) Grid.SetColumnSpan(rect, columnSnap);
            if (rowSnap > 1) Grid.SetRowSpan(rect, rowSnap);
            rect.SnapsToDevicePixels = true;
            grid.Children.Add(rect);

            Grid.SetColumn(element,column);
            Grid.SetRow(element,row);
            
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
            if (columnSnap > 1) Grid.SetColumnSpan(element, columnSnap);
            if (rowSnap > 1) Grid.SetRowSpan(element, rowSnap);

            grid.Children.Add(element);

        }

        public static void AddTextBlock(this Grid grid, string text, int column, int row, int columnSnap = 1, int rowSnap = 1)
        {
            grid.AddChildren(new TextBlock { Text = text }, column, row, columnSnap, rowSnap);
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = child;

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            return FindParent<T>(parentObject);
        }
    }
}
