using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Troubleshooting.Models
{
    [Serializable]
    public class DiagramEditorModel
    {
        public List<NodeModel> Nodes;
        public List<ConnectionModel> Connections;
    }
}
