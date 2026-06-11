using System.Collections.Generic;
using UnityEngine;
namespace GFW.ObjectPool
{
    public class ObjPoolMgr : Manager<ObjPoolMgr>
    {
        private readonly Dictionary<string, object> _PoolDict = new Dictionary<string, object>();
        // 所有预制体池的父节点
        [SerializeField] private Transform _PoolRoot;
        /// <summary>
        /// 获取或创建预制体对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="prefab">预制体</param>
        /// <param name="initialCount">初始预加载数量</param>
        /// <param name="maxCapacity">最大容量</param>
        /// <returns>对应的对象池</returns>
        public PrefabObjPool<T> GetOrCreatePrefabPool<T>(T prefab, int initialCount = 0, int maxCapacity = 0) where T : MonoBehaviour
        {
            string key = prefab.name + typeof(T).FullName;
            Debug.Log("GetOrCreatePrefabPool:PoolName:"+key);
            if (_PoolDict.TryGetValue(key, out object poolObj) && poolObj is PrefabObjPool<T> pool)
            {
                return pool;
            }

            // 创建池的父节点
            Transform poolParent = new GameObject($"Pool_{prefab.name}").transform;
            poolParent.SetParent(_PoolRoot ?? transform);

            // 创建新池并缓存
            PrefabObjPool<T> newPool = new PrefabObjPool<T>(prefab, poolParent, initialCount, maxCapacity);
            _PoolDict[key] = newPool;
            return newPool;
        }


        /// <summary>
        /// 获取或创建普通类对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="initialCount">初始预加载数量</param>
        /// <param name="maxCapacity">最大容量</param>
        /// <returns>对应的对象池</returns>
        public ObjPool<T> GetOrCreatePool<T>(int initialCount = 0, int maxCapacity = 0)where T : new()
        {
            string key = typeof(T).FullName;
            Debug.Log("GetOrCreatePool:PoolName:"+key);
            if (_PoolDict.TryGetValue(key, out object poolObj) && poolObj is ObjPool<T> pool)
            {
                return pool;
            }

            ObjPool<T> newPool = new ObjPool<T>(initialCount, maxCapacity);
            _PoolDict[key] = newPool;
            return newPool;
        }

        /// <summary>
        /// 清空指定类型的预制体对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="prefab">预制体</param>
        public void ClearPrefabPool<T>(T prefab) where T : MonoBehaviour
        {
            string key = prefab.name + typeof(T).FullName;
            if (_PoolDict.TryGetValue(key, out object poolObj) && poolObj is PrefabObjPool<T> pool)
            {
                Debug.Log("ClearPrefabPool:PoolName:"+key);
                pool.Clear();
                _PoolDict.Remove(key);
            }
        }

        /// <summary>
        /// 清空指定类型的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="prefab">预制体</param>
        public void ClearPool<T>()where T : new()
        {
            string key = typeof(T).FullName;
            if (_PoolDict.TryGetValue(key, out object poolObj) && poolObj is ObjPool<T> pool)
            {
                Debug.Log("ClearPool:PoolName:"+key);
                pool.Clear();
                _PoolDict.Remove(key);
            }
        }

        /// <summary>
        /// 清空所有对象池
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var poolObj in _PoolDict.Values)
            {
                if (poolObj is ObjPoolBase<object> pool)
                {
                    pool.Clear();
                }
            }
            _PoolDict.Clear();
        }
        private void OnDestroy()
        {
            ClearAllPools();
            if (_PoolRoot != null && _PoolRoot.parent == transform)
            {
                Destroy(_PoolRoot.gameObject);
            }
        }
    }
}


