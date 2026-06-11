using System;
using System.Collections.Generic;
using UnityEngine;
namespace GFW.ObjectPool
{
    /// <summary>
    /// 普通C#类对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjPool<T>: ObjPoolBase<T> where T : new ()
    {
        public ObjPool(int initialCount = 0, int maxCapacity = 0): base(maxCapacity)
        {
            Preload(initialCount);
        }

        protected override T CreateNew() => new T();
        protected override void DestroyItem(T obj) { }

        public void Preload(int count)
        {
            lock (_LockObj)
            {
                for (int i = 0; i < count; i++)
                {
                    if (_MaxCapacity > 0 && TotalCount >= _MaxCapacity) break;
                    _FreeObjs.Add(CreateNew());
                }
            }
        }
    }
}


