using System;
using System.Collections.Generic;
using System.Linq;

namespace Troubleshooting
{
    public class FunctionalDiagram: List<Block>
    {
        public FunctionalDiagram(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Add(new Block(1));
            }
        }

        public void Connect(int a, params int[] b)
        {

            if (a > Count || b.Any(x => x>Count)) return;
            foreach (var i in b)
            {
                this[a-1].AddChildren(this[i-1]);
            }
            
        } 

        public int[,] GetSolveMatrix()
        {
            var result = new int[Count+3, Count];
            for (int i = 0; i < Count; i++) this[i].Number = i;


            for (int i = 0; i < Count; i++)
            {
                foreach (var b in this) b.Working = true;
                this[i].ErrorInfluence();
                for (int j = 0; j < Count; j++)
                    result[i, j] = this[j].Working?1:0;
            }

            for (int i = 0; i < Count; i++)
            {
                int sumOne = 0;
                for (int j = 0; j < Count; j++)
                {
                    sumOne += result[j, i];
                }
                var sumZero = Count - sumOne;
                result[Count, i] = sumZero;
                result[Count + 1, i] = sumOne;
                result[Count + 2, i] = Math.Abs(sumZero - sumOne);
            }
            return result;
        }



        public MyTable GetMyTable()
        {
            for (int i = 0; i < Count; i++) this[i].Number = i;
            return new MyTable(this);
        }


    }
}
