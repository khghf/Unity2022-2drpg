using GFW.GameAbilitySystem.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.TimerMgrN
{
    /// <summary>
    /// 定时器实体
    /// </summary>
    internal class Timer:IPoolItem
    {
        public Timer() { }

        public Timer(float delay, float period, bool TriggerNextFrame)
        {
            Reset(delay, period, TriggerNextFrame);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay">延迟指定时间后触发，小于0表示延迟时间无限的定时器</param>
        /// <param name="period">小于0：在延迟时间内每帧都触发，等于0：延迟完成后触发一次，大于0：在延迟时间内周期触发</param>
        /// <param name="TriggerNextFrame">对于周期性触发定时器，指定其是否在下一帧立即触发一次</param>
        public void Reset(float delay=0f, float period=0f, bool TriggerNextFrame=false)
        {
            Handle=TimerHandle.Invalid;
            Delay = delay;
            Period = period<0?-1:(period==0?0:period);

            if(IsInfinite)
            {
                NextTriggerRemainTime=float.MaxValue;
            }
            else
            {
                NextTriggerRemainTime=delay;
            }

            if (IsPeriod)
            {
                if (TriggerNextFrame) NextTriggerRemainTime = 0f;
                else NextTriggerRemainTime =Period>0 ? Period : 0;
            }

            CurElapsedTimeInexact=0f;
            OnPeriodTrigger = null;
            OnDelayEnd=null;
            IsPaused = false;
        }



        public static IPoolItem CreatePoolItem()
        {
            return new Timer();
        }

        public virtual void OnPoolItemDestroy() { }
        public virtual void OnRecyclePoolItem() 
        { 
            Reset();
        }



        // 唯一ID，由管理器分配
        public TimerHandle Handle;

        /// <summary>
        /// 延迟指定时间后触发，小于0表示延迟时间无限的定时器(一般用于永久的周期触发定时器)
        /// </summary>
        public float Delay;
        /// <summary>
        /// 小于0：在延迟时间内每帧都触发，等于0：延迟完成后触发一次，大于0：在延迟时间内周期触发
        /// </summary>
        public float Period;

        public bool IsPaused;
        /// <summary>
        /// 距离下一次触发的时间
        /// </summary>
        public float NextTriggerRemainTime;

        /// <summary>
        /// 定时器采用帧驱动和优先队列方案来实现
        /// 但为了满足改变定时器延迟时间、周期性触发的要求，我们需要记录当前定时器已经延迟的时间，
        /// 同时为了利用优先队列带来的优势，我把定时器已经延迟的时间拆成了两个变量ElapsedTimeInexact和TimerMgr
        /// 的 ElapseTimeSinceLastAddOrRemoveTimer,当我们移除因延迟时间结束或者添加新的定时器时我们会遍历 
        /// 所有的定时器并将  ElapseTimeSinceLastAddOrRemoveTimer 的值加到 ElapsedTimeInexact 并重置
        /// ElapseTimeSinceLastAddOrRemoveTimer 
        /// </summary>
        public float CurElapsedTimeInexact = 0f;

        /// <summary>
        /// 周期触发回调
        /// </summary>
        public Action OnPeriodTrigger;
        /// <summary>
        /// 延迟结束时回调
        /// </summary>
        public Action OnDelayEnd;

        /// <summary>
        /// 是否为周期性定时器
        /// </summary>
        public bool IsPeriod => Period!=0f;
        /// <summary>
        /// 是否为永久定时期
        /// </summary>
        public bool IsInfinite=>Delay<0f;
    }
}
