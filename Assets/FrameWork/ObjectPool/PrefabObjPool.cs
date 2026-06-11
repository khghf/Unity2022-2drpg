using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 预制体对象池
/// </summary>
/// <typeparam name="T"></typeparam>
public class PrefabObjPool<T> : ObjPoolBase<T> where T : MonoBehaviour
{
    // 对象预制体
    private readonly T _Prefab;
    // 父物体(用于层级管理)
    private readonly Transform _ParentTransform;

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="prefab">要复用的对象预制体</param>
    /// <param name="parent">对象回收后的父节点(可选)</param>
    /// <param name="initialCount">初始预加载数量</param>
    /// <param name="maxCapacity">池最大容量(0=无限制)</param>
    public PrefabObjPool(T prefab, Transform parent = null, int initialCount = 0, int maxCapacity = 0) : base(maxCapacity)
    {
        _Prefab = prefab;
        _ParentTransform = parent;
        Preload(initialCount);
    }

    protected override T CreateNew()
    {
        var obj = GameObject.Instantiate(_Prefab, _ParentTransform);
        obj.gameObject.SetActive(false);
        return obj;
    }

    protected override void DestroyItem(T obj)
    {
        if (obj != null) GameObject.Destroy(obj.gameObject);
    }

    public T Get(Vector3 position= default, Quaternion rotation= default)
    {
        var obj = Get();
        if (obj != null)
        {
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.gameObject.SetActive(true);
        }
        return obj;
    }
    public void Preload(int count)
    {
        lock (_LockObj)
        {
            for (int i = 0; i < count; i++)
            {
                if (_MaxCapacity > 0 && TotalCountInner >= _MaxCapacity) break;
                _FreeObjs.Add(CreateNew());
            }
        }
    }
}