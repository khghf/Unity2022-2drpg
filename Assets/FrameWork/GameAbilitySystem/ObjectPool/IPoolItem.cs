using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.ObjectPool
{
    public interface IPoolItem
    {
        /// <summary>
        /// 创建一个对象池实例返回，仅限于对象池使用
        /// </summary>
        /// <returns></returns>
        public static IPoolItem CreatePoolItem() { return null; }
        /// <summary>
        /// 销毁时调用
        /// </summary>
        public virtual void OnPoolItemDestroy() 
        {
        
        }
        /// <summary>
        /// 从对象池中获取时调用
        /// </summary>
        public virtual void OnGetPoolItem() 
        {
        
        }
        /// <summary>
        /// 被回收到对象池时调用
        /// </summary>
        public virtual void OnRecyclePoolItem() 
        {
            
        }
    }
}
