namespace Troubleshooting.Models
{
    public class ConnectionModel
    {
        public NodeModel SourceNode { get; }
        public NodeModel SinkNode { get; set; }

        public ConnectionModel(NodeModel source)
        {
            SourceNode = source;
        }

    }
}
