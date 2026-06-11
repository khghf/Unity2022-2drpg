using GFW.GameAbilitySystem.ObjectPool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameAbilityTagN
{
    /// <summary>
    /// 标签用于标记游戏对象的状态
    /// </summary>
    public class GameAbilityTag : IEquatable<GameAbilityTag>,IPoolItem
    {
        public GameAbilityTag() { }
        internal GameAbilityTag(string name)
        {
           _Name =name;
           CacheHashCode();
        }
        

        public static GameAbilityTag Create(string tag)
        {
            GameAbilityTag abilityTag = null;
            if (string.IsNullOrWhiteSpace(tag))
            {
                AbilityLog.LogError("标签名不能为空");
                return abilityTag;
            }
            abilityTag= GameAbilityTagMgr.Inst._TagPool.Get();
            abilityTag._Name =tag;
            abilityTag.CacheHashCode();
            return abilityTag;
        }


        public static IPoolItem CreatePoolItem()
        {
            return new GameAbilityTag();
        }
        public virtual void OnRecyclePoolItem()
        {
            Reset();
        }
        private void Reset()
        {
            _Name=string.Empty;
            _CachedHashCode=int.MinValue;
            _HasCachedHashCode=false;
        }

        private void CacheHashCode()
        {
            _CachedHashCode=GetHashCode();
            _HasCachedHashCode=true;
        }
        public override int GetHashCode()
        {
            if (_HasCachedHashCode) return _CachedHashCode;
            return _Name.GetHashCode();
        }
        public static bool operator ==(GameAbilityTag a, GameAbilityTag b) => a.Equals(b);
        public static bool operator !=(GameAbilityTag a, GameAbilityTag b) => !a.Equals(b);
        public bool Equals(GameAbilityTag other)
        {
            if (ReferenceEquals(other, null)||ReferenceEquals(this, null))
            {
                AbilityLog.LogWarning("[Tag]空引用比较");
                return false;
            }
            if (ReferenceEquals(other, this)) return true;
            return ((GameAbilityTag)other)._CachedHashCode==this._CachedHashCode;
        }
        public override bool Equals(object obj)
        {
            return Equals((GameAbilityTag)obj);
        }

        public new string ToString()
        {
            return _Name;
        }


        //Tag的名称不是某个路径下单的某个结点而是整条路径比如:A.B.C
        private string _Name = string.Empty;

        //缓存的字符哈希码
        private int _CachedHashCode;
        private bool _HasCachedHashCode = false;
        public string Name => _Name;
        public int CachedHashCode => _CachedHashCode;
    }
}
