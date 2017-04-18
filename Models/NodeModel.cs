using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troubleshooting.ViewModels;

namespace Troubleshooting.Models
{
    [Serializable]
    public class NodeModel
    {
        public string Text;

        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int MinWidth;
        public int MinHeight;

        public Orientation OutOrientation;
    }
}
