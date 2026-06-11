using System;
using System.Collections.Generic;
using System.Linq;
namespace GFW.Container
{
    /// <summary>
    /// 泛型多值字典(一个Key对应多个Value，自动去重，O(1)操作)
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class MultiDict<TKey, TValue>
    {
        // 底层存储：Key -> 不重复的Value集合
        private readonly Dictionary<TKey, HashSet<TValue>> _InnerDict;

        // 所有Value的总数
        public int ValueCount => _InnerDict.Sum(pair => pair.Value.Count);
        // 所有Key的数量
        public int KeyCount => _InnerDict.Count;
        // 所有Key集合
        public ICollection<TKey> Keys => _InnerDict.Keys;

        public MultiDict()
        {
            _InnerDict = new Dictionary<TKey, HashSet<TValue>>();
        }

        public MultiDict(int capacity)
        {
            _InnerDict = new Dictionary<TKey, HashSet<TValue>>(capacity);
        }

        /// <summary>
        /// 添加键值对
        /// </summary>
        public bool Add(TKey key, TValue value)
        {
            if (!_InnerDict.TryGetValue(key, out var valueSet))
            {
                valueSet = new HashSet<TValue>();
                _InnerDict[key] = valueSet;
            }
            return valueSet.Add(value);
        }

        /// <summary>
        /// 移除指定键的指定值
        /// </summary>
        public bool Remove(TKey key, TValue value)
        {
            if (!_InnerDict.TryGetValue(key, out var valueSet)) return false;
            var isRemoved = valueSet.Remove(value);
            // 移除后若Value集合为空，自动删除Key，避免空节点
            if (valueSet.Count == 0) _InnerDict.Remove(key);
            return isRemoved;
        }

        /// <summary>
        /// 移除整个Key及其所有Value
        /// </summary>
        public bool RemoveKey(TKey key)
        {
            return _InnerDict.Remove(key);
        }

        /// <summary>
        /// 获取指定Key的所有Value的副本、不存在则返回一个空值
        /// </summary>
        public List<TValue> GetValues(TKey key)
        {
            if (_InnerDict.TryGetValue(key, out var valueSet)) return new List<TValue>(valueSet);
            return null;
        }

        public bool  TryGetValues(TKey key, out List<TValue> outValues)
        {
            outValues=null;
            if (_InnerDict.TryGetValue(key, out var valueSet))
            {
                outValues=new List<TValue>(valueSet);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否包含指定键值对
        /// </summary>
        public bool Contains(TKey key, TValue value)
        {
            return _InnerDict.TryGetValue(key, out var valueSet) && valueSet.Contains(value);
        }

        /// <summary>
        /// 判断是否包含指定Key
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            return _InnerDict.ContainsKey(key);
        }

        /// <summary>
        /// 清空所有键值对
        /// </summary>
        public void Clear()
        {
            _InnerDict.Clear();
        }

        /// <summary>
        /// 批量移除指定条件的Value
        /// </summary>
        public void RemoveWhere(Predicate<TValue> predicate)
        {
            var keysToRemove = new List<TKey>();
            foreach (var pair in _InnerDict)
            {
                pair.Value.RemoveWhere(predicate);

                if (pair.Value.Count == 0) keysToRemove.Add(pair.Key);
            }
            // 移除空Key
            foreach (var key in keysToRemove) _InnerDict.Remove(key);
        }
    }
}
