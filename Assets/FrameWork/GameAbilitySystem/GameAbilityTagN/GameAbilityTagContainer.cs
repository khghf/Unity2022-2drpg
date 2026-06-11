using System.Collections.Generic;
using System.Linq;

namespace GFW.GameAbilitySystem.GameAbilityTagN
{
    //public enum AddResult
    //{
    //    AlreadyOwned,//已拥有，无需再次添加
    //    Success,//添加成功
    //    Failutr,//添加失败
    //}

    //public enum RemoveResult
    //{
    //    AlreadyRemoved,//没有，无需移除
    //    Success,//移除成功
    //    Failutr,//移除失败
    //}
    /// <summary>
    /// 标签容器用于存储对象所拥有的标签及父标签
    /// </summary>
    public class GameAbilityTagContainer
    {

        internal readonly HashSet<GameAbilityTag> _Tags = new HashSet<GameAbilityTag>();
        internal readonly HashSet<GameAbilityTag> _ParentTags = new HashSet<GameAbilityTag>();

        internal GameAbilityTagContainer() { }
        internal GameAbilityTagContainer(GameAbilityTag tag)
        {
            AddTag(tag);
        }

        public bool AddTag(GameAbilityTagContainer other)
        {
            if (other==null) return false;
            foreach (GameAbilityTag tag in other._Tags)
            {
                AddTag(tag);
            }
            return true;
        }

        public bool AddTag(GameAbilityTag tagToAdd)
        {
            if (_Tags.Contains(tagToAdd))
            {
                return false;
            }

            GameAbilityTagContainer parentTags = GameAbilityTagMgr.Inst.GetParentTags(tagToAdd);
            if (parentTags != null)
            {
                foreach (GameAbilityTag parentTag in parentTags._Tags)
                {
                    _ParentTags.Add(parentTag);
                }
            }

            if (_Tags.Add(tagToAdd)) return true;

            return false;
        }


        public bool RemoveTag(GameAbilityTagContainer tagsToRemove)
        {
            foreach (GameAbilityTag tagToRemove in tagsToRemove._Tags)
            {
                RemoveTag(tagToRemove);
            }
            return true;
        }

        public bool RemoveTag(GameAbilityTag tagToRemove)
        {
            if (!_Tags.Contains(tagToRemove))
            {
                return false;
            }
            GameAbilityTagContainer parentTags = GameAbilityTagMgr.Inst.GetParentTags(tagToRemove);
            if (parentTags != null)
            {
                foreach (GameAbilityTag parentTag in parentTags._Tags)
                {
                    _ParentTags.Remove(parentTag);
                }
            }
            if (_Tags.Remove(tagToRemove)) return true;
            return false;
        }



        /// <summary>
        /// 在_Tag和_ParentTag中检索是否存在指定标签
        /// [A.B].HasTag([A])       return true
        /// [A.B].HasTag([A.B])     return true
        /// [A].HasTag([A.B])       return false
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasTag(GameAbilityTag tag)
        {
            if (_Tags.Count<=0) return false;
            return _Tags.Contains(tag)||_ParentTags.Contains(tag);
        }

        /// <summary>
        /// 在_Tag中检索是否存在指定标签
        /// [A.B].HasTag([A])       return false
        /// [A.B].HasTag([A.B])     return true
        /// [A].HasTag([A.B])       return false
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasTagExact(GameAbilityTag tag)
        {
            if (_Tags.Count<=0) return false;
            return _Tags.Contains(tag)||_ParentTags.Contains(tag);
        }


        /// <summary>
        /// this._Tag、this._ParentTags与other._Tag是否存在任意交集
        /// this._Tag与other._Tag有交集           return true
        /// this._ParentTags与other._Tag有交集    return true
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HasTagAny(GameAbilityTagContainer other)
        {
            if (other==null) return false;
            if (_Tags.Count<=0||other._Tags.Count<=0) return false;
            var otherTags = other._Tags.ToArray();
            foreach (var tag in otherTags)
            {
                if (HasTag(tag)) return true;
            }
            return false;
        }

        /// <summary>
        /// this._Tag与other._Tag是否存在任意交集
        /// this._Tag与other._Tag有交集           return true
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HasTagAnyExcat(GameAbilityTagContainer other)
        {
            if (other==null) return false;
            if (_Tags.Count<=0||other._Tags.Count<=0) return false;
            var otherTags = other._Tags.ToArray();
            foreach (var tag in otherTags)
            {
                if (HasTagExact(tag)) return true;
            }
            return false;
        }

        /// <summary>
        /// this._Tag、this._ParentTags是other._Tag的父集
        /// this._Tag和this._ParentTags的并集包含other._Tag      return true
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HasTagAll(GameAbilityTagContainer other)
        {
            if (other==null) return false;
            if (_Tags.Count<=0||other._Tags.Count<=0) return false;
            var otherTags = other._Tags.ToArray();
            foreach (var tag in otherTags)
            {
                if (!HasTag(tag)) return false;
            }
            return true;
        }

        /// <summary>
        /// this._Tag是other._Tag的父集
        /// this._Tag包含other._Tag      return true
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HasTagAllExcat(GameAbilityTagContainer other)
        {
            if (other==null) return false;
            if (_Tags.Count<=0||other._Tags.Count<=0) return false;
            var otherTags = other._Tags.ToArray();
            foreach (var tag in otherTags)
            {
                if (!HasTagExact(tag)) return false;
            }
            return true;
        }

        public void Clear()
        {
            _Tags.Clear();
            _ParentTags.Clear();
        }

    }
}
