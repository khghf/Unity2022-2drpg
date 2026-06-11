using GFW.GameAbilitySystem.ObjectPool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameAbilityTagN
{
    /// <summary>
    /// 标签管理器，存储着定义的所有标签，将标签构建成树。
    /// 内部存有的字典可以快速检索、匹配标签但只向TagContainer提供接口
    /// </summary>
    public class GameAbilityTagMgr
    {
        private static GameAbilityTagMgr _Instacne=null;
        public static GameAbilityTagMgr Inst
        {
            get 
            { 
                if(_Instacne==null)
                {
                    _Instacne=new GameAbilityTagMgr();
                }
                return _Instacne; 
            }
        }





        //根节点
        private TagTreeNode _TagTreeRoot = new TagTreeNode(new GameAbilityTag("TagTreeRoot"));

        //Tag=>TagTreeNode用于通过Tag快速检索树节点
        private Dictionary<GameAbilityTag, TagTreeNode> _TagToTreeNode = new Dictionary<GameAbilityTag, TagTreeNode>(100);

        internal ObjPool<GameAbilityTag> _TagPool = new ObjPool<GameAbilityTag>(()=>new GameAbilityTag(),100);

        private GameAbilityTagMgr() { }
        ~GameAbilityTagMgr()
        {
            _TagPool.Clear();
        }
        public GameAbilityTag RegisterTag(string tag)
        {
            string[] parts = tag.Split(GetSplitChar());

            if (parts.Length<=0) return null;

            TagTreeNode parentNode = _TagTreeRoot;
            GameAbilityTag newTag = null;
            int index = 0;
            while (index<parts.Length)
            {
                TagTreeNode found = parentNode.Children.Find((TagTreeNode node) => { return node.Name==parts[index]; });
                if (found==null)
                {
                    string newTagName = string.Join(GetSplitChar(), parts.Take(index+1));
                    newTag = GameAbilityTag.Create(newTagName);
                    found=new TagTreeNode(newTag);
                    found.Parent=parentNode;
                    found.ParentTags=new GameAbilityTagContainer(parentNode.Tag);
                    found.ParentTags.AddTag(parentNode.ParentTags);
                    parentNode.Children.Add(found);
                    _TagToTreeNode.Add(newTag, found);
                }
                parentNode=found;
                ++index;
            }
            return newTag;
        }

        internal GameAbilityTagContainer GetParentTags(GameAbilityTag tag)
        {
            GameAbilityTagContainer res = null;
            if (_TagToTreeNode.TryGetValue(tag, out var node))
            {
                res=node.ParentTags;
            }
            return res;
        }

        internal void PrintTree()
        {
            string path = "";
            PrintTree(_TagTreeRoot, path);
        }
        private void PrintTree(TagTreeNode node, string path)
        {
            if (node==null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(path))
            {
                path = path+GetSplitChar()+node.Name;
            }
            else
            {
                path=node.Name;
            }
            if (node.Children.Count>0)
            {
                foreach (TagTreeNode child in node.Children)
                {
                    PrintTree(child, path);
                }
            }
            else
            {
                AbilityLog.LogInfo(path);
                path=string.Empty;
            }

        }
        internal static char GetSplitChar() { return '.'; }
    }
}
