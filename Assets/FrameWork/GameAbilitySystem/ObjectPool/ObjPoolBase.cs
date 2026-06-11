using System;
using System.Collections.Generic;

namespace GFW.GameAbilitySystem.ObjectPool
{
    /// <summary>
    /// 通用对象池基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class ObjPoolBase<T>where T : IPoolItem
    {
        // 池容器：存储空闲对象
        protected readonly List<T> _FreeObjs = new List<T>();
        // 池容器：存储繁忙对象
        protected readonly List<T> _BusyObjs = new List<T>();
        // 最大容量(0表示无限制)
        protected readonly int _MaxCapacity;
        // 忙碌/空闲 < 0.5 触发缩容
        protected readonly float _ShrinkThreshold = 0.5f;
        // 线程安全锁
        protected readonly object _LockObj = new object();

        protected Func<IPoolItem> ItemCreator;


        protected int FreeCountInner => _FreeObjs.Count;
        protected int BusyCountInner => _BusyObjs.Count;
        protected int TotalCountInner => BusyCountInner+FreeCountInner;
        public int FreeCount
        {
            get { lock (_LockObj) return FreeCountInner; }
        }
        public int BusyCount
        {
            get { lock (_LockObj) return BusyCountInner; }
        }
        public int TotalCount
        {
            get { lock (_LockObj) return TotalCountInner; }
        }
        protected ObjPoolBase(Func<IPoolItem> itemCreator,int maxCapacity = 0)
        {
            _MaxCapacity = maxCapacity;
            ItemCreator=itemCreator;
           
        }

        public T Get() 
        {
            lock (_LockObj)
            {
                T obj;
                if (FreeCountInner > 0)
                {
                    var lastIdx = FreeCountInner - 1;
                    obj = _FreeObjs[lastIdx];
                    _FreeObjs.RemoveAt(lastIdx);
                    _BusyObjs.Add(obj);
                }
                else
                {
                    obj=(T)ItemCreator();
                    obj.OnGetPoolItem();
                    if (_MaxCapacity > 0 && TotalCountInner >= _MaxCapacity)
                    {
                        return obj;
                    }
                    else
                    {
                        _BusyObjs.Add(obj);
                    }
                }
                return obj;
            }
        }

        public void Recycle(T obj)
        {
            if (obj == null) return;

            lock (_LockObj)
            {
                if (_BusyObjs.Remove(obj))
                {
                    obj.OnRecyclePoolItem();
                    _FreeObjs.Add(obj);
                    AutoShrink();
                }
            }
        }

        protected virtual void AutoShrink()
        {
            bool needShrink = BusyCountInner * 1.0f < _ShrinkThreshold * FreeCountInner;

            if (needShrink)
            {
                int removeCount = FreeCountInner / 2;
                if (removeCount > 0)
                {
                    _FreeObjs.RemoveRange(removeCount, removeCount);
                }
            }
        }

        public void Clear()
        {
            lock (_LockObj)
            {
                foreach (var o in _FreeObjs)o.OnPoolItemDestroy();
                foreach (var o in _BusyObjs) o.OnPoolItemDestroy();
                _FreeObjs.Clear();
                _BusyObjs.Clear();
            }
        }
    }
}

