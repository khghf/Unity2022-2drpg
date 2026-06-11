using GFW.GameAbilitySystem.ObjectPool;
using GFW.Container;
using System.Collections.Generic;
namespace GFW.GameAbilitySystem.TimerMgrN
{
    /// <summary>
    /// 定时器管理器
    /// </summary>
    public class TimerMgr
    {
        private static TimerMgr _Instance = null;
        public static TimerMgr Inst
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new TimerMgr();
                }
                return _Instance;
            }
        }


        private static long _IdCounter = 0; // 0为无效句柄
        public static readonly TimerHandle InvalidHandle = new TimerHandle(_IdCounter);
        public float GlobalTimeScale { get; set; } = 1f;

        /// <summary>
        /// 自上次从定时器字典添加或移除定时器所逝去的时间
        /// </summary>
        private float ElapsedTimeSinceLastAddOrRemoveTimer = 0f;
        
        // 优先队列：按下次触发时间排序
        private readonly PriorityQueue<Timer> _pq = new PriorityQueue<Timer>((Timer x,Timer y) => { return x.NextTriggerRemainTime<y.NextTriggerRemainTime; });
        // 定时器字典(用于通过Handle快速查找)
        private readonly Dictionary<TimerHandle, Timer> _Timers = new Dictionary<TimerHandle, Timer>();

        private ObjPool<Timer> _TimerPool = new ObjPool<Timer>(()=> Timer.CreatePoolItem(), 50);

        public void Update(float deltaTime)
        {
            float scaledDelta = deltaTime * GlobalTimeScale;
            ElapsedTimeSinceLastAddOrRemoveTimer+=scaledDelta;
            List<Timer> triggeredTimerThisFrame = new List<Timer>();
            while (_pq.Count > 0)
            {
                // 获取最近的要触发的定时器
                if (!_pq.TryPeek(out var timer))break;

                if (!timer.IsPaused) timer.NextTriggerRemainTime-=scaledDelta;

                bool hasPeriodTimerTrigger = false;
                //检查是否周期计时器触发
                if (IsTimerPeriodTrigger(timer))
                {
                    // 时间到，触发回调
                    _pq.Dequeue();
                    OnTimerPeriodTrigger(ref timer);
                    hasPeriodTimerTrigger=true;
                }

                //检查是否有计时器延迟结束
                if (IsTimerDelayEnd(timer))
                {
                    if(!hasPeriodTimerTrigger) _pq.Dequeue();
                    OnTimerDelayEnd(ref timer);
                }

                if (hasPeriodTimerTrigger) triggeredTimerThisFrame.Add(timer);
                else break;
            }

            foreach(var enTimer in triggeredTimerThisFrame)
            {
                if (!enTimer.Handle.IsValid()) continue;
                _pq.Enqueue(enTimer);
            }
        }

        /// <summary>
        /// 遍历所有的Timer更新已延迟时间
        /// </summary>
        /// <param name="excludeTimer">被排除更新的Timer</param>
        private void UpdateTimersElapsedTime(Timer excludeTimer=null)
        {
            if (ElapsedTimeSinceLastAddOrRemoveTimer==0f) return;
            foreach (var timer in _Timers.Values)
            {
                if (excludeTimer!=null&&timer.Handle.Equals(excludeTimer.Handle)) continue;
                timer.CurElapsedTimeInexact+=ElapsedTimeSinceLastAddOrRemoveTimer;
            }
            ElapsedTimeSinceLastAddOrRemoveTimer=0f;
        }


        /// <summary>
        /// 创建一个定时器
        /// <param name="delay">delay:延迟指定时间后触发，小于0表示延迟时间无限的定时器</param>
        /// <param name="period">period:小于0：在延迟时间内每帧都触发，等于0：延迟完成后触发一次，大于0：在延迟时间内周期性触发</param>
        /// <param name="TriggerNextFrame">TriggerNextFrame:对于周期性触发定时器，指定其是否在下一帧立即触发一次</param>
        /// </summary>
        /// <returns></returns>
        public TimerHandle CreateTimer(float delay, float period = 0f, bool TriggerNextFrame = false)
        {
            var timer = _TimerPool.Get();
            timer.Reset(delay, period, TriggerNextFrame);
            timer.Handle=CreateTimerHandle();

            return AddTimer(timer);
        }

        private TimerHandle CreateTimerHandle()
        {
            return new TimerHandle(++_IdCounter);
        }

        /// <summary>
        /// 检查timer是否延迟结束
        /// </summary>
        /// <param name="timer">要检查的Timer</param>
        /// <returns></returns>
        private bool IsTimerDelayEnd(in Timer timer)
        {
            if(timer.IsInfinite)return false;
            return timer.CurElapsedTimeInexact+ElapsedTimeSinceLastAddOrRemoveTimer>=timer.Delay;
        }

        private bool IsTimerPeriodTrigger(in Timer timer)
        {
            if(!timer.IsPeriod)return false;
            return timer.NextTriggerRemainTime<=0f;
        }
        private void OnTimerDelayEnd(ref Timer timer)
        {
            timer.OnDelayEnd?.Invoke();
            //延迟结束移除定时器
            RemoveTimerFromDic(timer);
            _TimerPool.Recycle(timer);
        }

        private void OnTimerPeriodTrigger(ref Timer timer)
        {
            timer.OnPeriodTrigger?.Invoke();
            timer.NextTriggerRemainTime=timer.Period+timer.NextTriggerRemainTime;
        }
        private TimerHandle AddTimer(Timer timer)
        {
            UpdateTimersElapsedTime();
            _Timers[timer.Handle] = timer;
            _pq.Enqueue(timer);
            return timer.Handle;
        }

        

        

        internal Timer GetTimer(TimerHandle handle)
        {
            if (handle.IsValid() && _Timers.TryGetValue(handle, out var timer))
                return timer;
            return null;
        }

        private bool HasTimer(in Timer timer)
        {
            if(timer==null)return false;
            return HasTimer(timer);
        }
        public bool HasTimer(in TimerHandle handle)
        {
            if(!handle.IsValid())return false;
            return _Timers.ContainsKey(handle);
        }

        internal bool PauseTimer(TimerHandle handle)
        {
            if (!handle.IsValid()) return false;
            Timer timer = GetTimer(handle);
            timer.IsPaused = true;
            UpdateTimersElapsedTime(timer);
            return true;
        }
        internal bool ResumeTimer(TimerHandle handle)
        {
            if (!handle.IsValid()) return false;
            Timer timer = GetTimer(handle);
            UpdateTimersElapsedTime(timer);
            timer.IsPaused = false;
            return true;
        }

        /// <summary>
        /// 更改定时器的延迟时间
        /// </summary>
        /// <param name="handle">定时器句柄</param>
        /// <param name="newDelay">新的延迟时间</param>
        /// <param name="resetTime">是否重置该计时与时间相关的变量包括：下一次触发剩余时间、当前已延迟的时间</param>
        public void SetTimerDelay(TimerHandle handle, float newDelay,bool resetTime=false)
        {
            var timer = GetTimer(handle);
            if (timer != null)
            {
                timer.Delay = newDelay;
                if (resetTime)
                {
                    UpdateTimersElapsedTime();
                    timer.CurElapsedTimeInexact=0f;
                    timer.NextTriggerRemainTime=0f;
                }

                if (IsTimerDelayEnd(timer))
                {
                    OnTimerDelayEnd(ref timer);
                    return;
                }

                if(resetTime)
                {
                    DequeueSpecifiedTimer(timer);
                    _pq.Enqueue(timer);
                }
            }
        }
        /// <summary>
        /// 更新定时器的触发周期
        /// </summary>
        /// <param name="handle">定时器句柄</param>
        /// <param name="newPeriod">新的周期</param>
        /// <param name="resetNextTriggerTime">是否重置当前周期已经过去的时间</param>
        public void SetTimerPeriod(TimerHandle handle, float newPeriod,bool resetNextTriggerTime=false)
        {
            var timer = GetTimer(handle);
            if (timer != null)
            {
                timer.Period = newPeriod;
                if (resetNextTriggerTime)
                {
                    DequeueSpecifiedTimer(timer);
                    _pq.Enqueue(timer);
                }
            }
        }

        /// <summary>
        /// 从字典和优先队列中移除对应定时器并回收到定时器对象池中
        /// </summary>
        /// <param name="handle">定时器句柄</param>
        /// <param name="isTriggerDelayEndCallback">成功移除后是否触发延迟结束回调</param>
        /// <returns></returns>
        internal bool RemoveSpecifiedTimer(TimerHandle handle,bool isTriggerDelayEndCallback)
        {
            Timer timer = GetTimer(handle);
            if (timer == null) return false;
            if(RemoveTimerFromDic(timer))
            {
                DequeueSpecifiedTimer(timer);
                timer.OnDelayEnd?.Invoke();
                _TimerPool.Recycle(timer);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从字典中移除对应定时器
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        private bool RemoveTimerFromDic(Timer timer)
        {
            if (_Timers.Remove(timer.Handle))
            {
                UpdateTimersElapsedTime();
                return true;
            }
            return false;
        }


        /// <summary>
        /// 将指定的timer从优先队列中移除，该操作会先将所有
        /// 队列的元素出队然后其它timer入队
        /// </summary>
        /// <param name="specifiedtimer"></param>
        private void DequeueSpecifiedTimer(Timer specifiedtimer)
        {
            if (!HasTimer(specifiedtimer)) return;
            List<Timer>copy= new List<Timer>(_Timers.Count);

            while(_pq.Count>0)
            {
                Timer timer = _pq.Dequeue();
                if (timer.Handle.Equals(specifiedtimer.Handle)) continue;
                copy.Add(timer);
            }
            foreach(var timer in copy)
            {
                _pq.Enqueue(timer);
            }
        }

        public void Clear()
        {
            _Timers.Clear();
           _pq.Clear();
            _TimerPool.Clear();
        }
    }
}
