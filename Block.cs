using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup.Localizer;

namespace Troubleshooting
{
    public class Block
    {
        public List<Block> Childrens = new List<Block>();
        public List<Block> Parents = new List<Block>();
       

        public int Number;
        public bool Working; 

        public Block(int num)
        {
            Number = num;
        }

        public void AddChildren(Block block)
        {
            if(!Childrens.Contains(block)) Childrens.Add(block);
            if(!block.Parents.Contains(this)) block.Parents.Add(this);
        }


        public void ErrorInfluence()
        {
            if (Working == false) return;
            Working = false;
            foreach (var b in Childrens) b.ErrorInfluence();
        }


        public List<Block> AllCildrens()
        {
            var result = new List<Block>();
            foreach (var block in Childrens)
            {
                result.Add(block);
                var grandChildrens = block.AllCildrens();
                foreach (var grandChildren in grandChildrens.Where(grandChildren => !result.Contains(grandChildren)))
                {
                    result.Add(grandChildren);
                }
            }
            return result;
        }

    }
}
