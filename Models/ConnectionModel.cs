using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Troubleshooting.ViewModels;

namespace Troubleshooting.Models
{
    [Serializable]
    public class ConnectionModel
    {
        public NodeModel SourceNode;
        public NodeModel SinkNode;
    }
}
