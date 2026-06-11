using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameAbilityTagN
{
    internal class TagTreeNode
    {
        //该节点对应的标签
        public GameAbilityTag Tag;
        //结点名称例：路径为A.B.C则名称为C
        public string Name;
        //父级标签
        public GameAbilityTagContainer ParentTags = null;


        public TagTreeNode Parent = null;
        public List<TagTreeNode> Children = null;

        public TagTreeNode(GameAbilityTag tag)
        {
            Tag = tag;
            string[] parts = tag.Name.Split(GameAbilityTagMgr.GetSplitChar());
            Name = parts.Last();
            Children=new List<TagTreeNode>();
        }
    }
}
