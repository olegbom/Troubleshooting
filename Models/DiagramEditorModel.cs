using System.Collections.ObjectModel;

namespace Troubleshooting.Models
{
    public class DiagramEditorModel
    {
        public ObservableCollection<NodeModel> Nodes { get; } = new ObservableCollection<NodeModel>();
        
        public DiagramEditorModel()
        {
            
        }
    }
}
