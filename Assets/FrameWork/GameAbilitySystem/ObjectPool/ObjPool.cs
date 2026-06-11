using System;

namespace GFW.GameAbilitySystem.ObjectPool
{
    /// <summary>
    /// 普通C#类对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ObjPool<T>: ObjPoolBase<T> where T :IPoolItem
    {
        public ObjPool(Func<IPoolItem> itemCreator,int initialCount = 0, int maxCapacity = 0): base(itemCreator,maxCapacity)
        {
            Preload(initialCount);
        }

        public void Preload(int count)
        {
            lock (_LockObj)
            {
                for (int i = 0; i < count; i++)
                {
                    if (_MaxCapacity > 0 && TotalCount >= _MaxCapacity) break;
                    _FreeObjs.Add((T)ItemCreator());
                }
            }
        }
    }
}


