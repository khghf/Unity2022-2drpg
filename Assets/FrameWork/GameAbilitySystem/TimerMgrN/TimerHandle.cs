using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.TimerMgrN
{
    /// <summary>
    /// 定时器句柄
    /// </summary>
    public struct TimerHandle : IEquatable<TimerHandle>
    {
        public static TimerHandle Invalid=>TimerMgr.InvalidHandle;

        public readonly long Id;

        internal TimerHandle(long id)
        {
            Id = id;
        }

        /// <summary>
        /// 为对应的定时器设置延迟结束回调
        /// </summary>
        /// <param name="callback"></param>
        public bool SetDelayEndCallback(Action callback)
        {
            var timer = TimerMgr.Inst.GetTimer(this);
            if (timer!=null)
            {
                timer.OnDelayEnd=callback;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 为对应的定时器设置周期触发回调
        /// </summary>
        /// <param name="callback"></param>
        public bool SetPeriodCallback(Action callback)
        {
            var timer = TimerMgr.Inst.GetTimer(this);
            if (timer!=null)
            {
                timer.OnPeriodTrigger=callback;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 移除对应的定时器
        /// <param name="isTriggerDelayEndCallback">isTriggerDelayEndCallback：成功移除定时器后是否触发延迟结束回调 </param>
        /// </summary>
        public bool RemoveTimer(bool isTriggerDelayEndCallback=false)
        {
            return TimerMgr.Inst.RemoveSpecifiedTimer(this, isTriggerDelayEndCallback);
        }

        /// <summary>
        /// 暂停对应的定时器，该操作会遍历所有定时器同步已延迟的时间
        /// </summary>
        /// <see cref ="TimerMgr.UpdateTimersElapsedTime"></ref>
        /// <returns></returns>
        public bool PauseTimer()
        {
            return TimerMgr.Inst.PauseTimer(this);
        }

        /// <summary>
        /// 恢复对应的定时器，该操作会遍历所有定时器同步已延迟的时间
        /// </summary>
        /// <see cref ="TimerMgr.UpdateTimersElapsedTime"></ref>
        /// <returns></returns>
        public bool ResumeTimer()
        {
            return TimerMgr.Inst.ResumeTimer(this);
        }


        public bool IsValid() => this != Invalid;

        public bool Equals(TimerHandle other) => Id == other.Id;
        public override bool Equals(object obj) => obj is TimerHandle handle && Equals(handle);
        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(TimerHandle left, TimerHandle right) => left.Id == right.Id;
        public static bool operator !=(TimerHandle left, TimerHandle right) => left.Id != right.Id;
    }
}
