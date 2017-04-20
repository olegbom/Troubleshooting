using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PropertyChanged;
using Troubleshooting.Annotations;

namespace Troubleshooting.ViewModels
{
    [ImplementPropertyChanged]
    public class DependencyTableViewModel: INotifyPropertyChanged
    {

        public List<DependencyTableViewModel> Childrens { get; } = new List<DependencyTableViewModel>();

        public int TableDepth
        {
            get
            {
                if (Childrens.Any()) return Math.Max(Childrens[0].TableDepth, Childrens[1].TableDepth);
                return Level;
            }
        }
        public int Count { get; }
        public int Level { get; }

        public int[,] IsWorkMatrix { get; }
        public NodeViewModel[] Nodes { get; }
        public int[] Ones { get; }
        public int[] Zeros { get; }
        public int[] W { get; }

        public string TableDescription { get; set; }

        public DependencyTableViewModel(DiagramEditorViewModel diagramEditorVm)
        {
            TableDescription = "Таблица зависимостей";
            Count = diagramEditorVm.Nodes.Count;
            IsWorkMatrix = new int[Count, Count];
            Nodes = new NodeViewModel[Count];
            Ones = new int[Count];
            Zeros = new int[Count];
            W = new int[Count];

            for (int i = 0; i < Count; i++)
            {
                Nodes[i] = diagramEditorVm.Nodes[i];
            }
            Nodes = Nodes.OrderBy(x => x.Zindex).ToArray();

            for (int i = 0; i < Count; i++)
            {
                foreach (var b in Nodes) b.IsWork = true;
                Nodes[i].ErrorInfluence();
                for (int j = 0; j < Count; j++)
                    IsWorkMatrix[j, i] = Nodes[j].IsWork ? 1 : 0;
            }
            ClackSums();
            if (Count > 1)
                Solve();
        }

        public DependencyTableViewModel(DependencyTableViewModel table, NodeViewModel[] nodes, int isWork)
        {

            TableDescription = $"Таблица зависимостей при Z{table.Nodes[table.W.IndexOfMin()].Zindex +1} = {isWork}";
            Level = table.Level + 1;
            Count = nodes.Length;
            IsWorkMatrix = new int[Count, Count];
            Ones = new int[Count];
            Zeros = new int[Count];
            Nodes = nodes;
            W = new int[Count];
            var parentNodesList = table.Nodes.ToList();
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < Count; j++)
                    IsWorkMatrix[i, j] = table.IsWorkMatrix
                        [parentNodesList.IndexOf(Nodes[i]), parentNodesList.IndexOf(Nodes[j])];
            }
            ClackSums();
            if (Count > 1)
                Solve();
        }



        public void ClackSums()
        {
            for (int i = 0; i < Ones.Length; i++)
            {
                var ones = 0;
                for (int j = 0; j < Count; j++)
                    ones += IsWorkMatrix[i, j];
                var zeros = Count - ones;
                Ones[i] = ones;
                Zeros[i] = zeros;
                W[i] = Math.Abs(zeros - ones);
            }
        }

        private void Solve()
        {
            var minWIndex = W.IndexOfMin();
            var nodes0 = new List<NodeViewModel>();
            var nodes1 = new List<NodeViewModel>();
            for (int j = 0; j < Count; j++)
            {
                if (IsWorkMatrix[minWIndex, j] == 0) nodes0.Add(Nodes[j]);
                else if (IsWorkMatrix[minWIndex, j] == 1) nodes1.Add(Nodes[j]);
            }

            Childrens.Add(new DependencyTableViewModel(this, nodes0.ToArray(), 0));
            Childrens.Add(new DependencyTableViewModel(this, nodes1.ToArray(), 1));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
