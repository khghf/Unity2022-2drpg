using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GFW.Container;
using UnityEngine.Device;
namespace GFW.Event
{
    /// <summary>
    /// 事件系统
    /// </summary>
    public class EventMgr : Manager<EventMgr>
    {
        // 事件Id(typeof(T).GetHashCode()) -> 回调函数集合(Action<Event>)
        private readonly MultiDict<int, Action<GfwEvent>> _EventDict = new MultiDict<int, Action<GfwEvent>>();
        // 事件队列
        private readonly Queue<GfwEvent> _EventQueue = new Queue<GfwEvent>();
        // 线程锁
        private readonly object _QueueLock = new object();

        private void Update()
        {
            ProcessEventQueue();
        }

        private void OnDestroy()
        {
            ClearAllEvents();
        }

        /// <summary>
        /// 注册指定类型事件回调
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <param name="action">无参回调</param>
        public void AddListener<T>(Action<GfwEvent> action)where T : GfwEvent
        {
            if (action == null)
            {
                LogWarning("注册事件的回调！");
                return;
            }
            Type eventType=typeof(T);
            bool isAdded = _EventDict.Add(eventType.GetHashCode(), action);
            if (!isAdded )
            {
                LogWarning($"事件[{eventType.Name}]重复注册同一回调函数");
            }
        }

        /// <summary>
        /// 注销指定类型事件的指定回调
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="action">对应回调</param>
        public void RemoveListener<T>(Action<GfwEvent> action)
        {
            if (action == null)
            {
                LogWarning("注销事件回调为空！");
                return;
            }
            Type eventType = typeof(T);
            bool isRemoved = _EventDict.Remove(eventType.GetHashCode(), action);
            if (!isRemoved )
            {
                LogWarning($"事件[{eventType.Name}]未找到要注销的回调，注销失败！");
            }
        }

        /// <summary>
        /// 立即广播事件
        /// </summary>
        public void Broadcast<T>(T @event) where T : GfwEvent
        {
            if(@event==null)
            {
                LogWarning("广播的事件不能为空！");
                return;
            }

            if(@event.Payload!=null&&@event.Payload.DelayTime>0)
            {
                Broadcast_Safe(@event);
            }

            List<Action<GfwEvent>> observe = _EventDict.GetValues(@event.Id);
            if(observe==null)
            {
                LogInfo($"广播事件{typeof(T).Name}(Id:{@event.Id})没有任何监听者");
            }
            foreach(Action<GfwEvent> action in observe)
            {
                action?.Invoke(@event);
            }
        }
       



        /// <summary>
        /// 广播事件线程安全
        /// </summary>
        public void Broadcast_Safe<T>(T @event) where T : GfwEvent
        {
            if (@event==null)
            {
                LogWarning("广播的事件不能为空！");
                return;
            }
            lock(_QueueLock)
            {
                _EventQueue.Enqueue(@event);
            }
        }
        internal void Broadcast_Safe(int eventId)
        {
            if (!_EventDict.ContainsKey(eventId)) return;
            GfwEvent @event=new GfwEvent();
            @event.Id=eventId;
            lock (_QueueLock)
            {
                _EventQueue.Enqueue(@event);
            }
        }
        /// <summary>
        /// 处理事件队列(Update中执行，保证所有事件在主线程)
        /// </summary>
        private void ProcessEventQueue()
        {
            if (_EventQueue.Count == 0) return;

            lock (_QueueLock)
            {
                var tempQueue = new Queue<GfwEvent>(_EventQueue);
                _EventQueue.Clear();

                foreach (var @event in tempQueue)
                {
                    if (@event.Payload.IsReadyToTrigger())
                    {
                        // 到达触发时间，执行事件
                        InvokeEvent(@event);
                    }
                    else
                    {
                        // 未到时间，重新加入队列
                        _EventQueue.Enqueue(@event);
                    }
                }
            }
        }

        /// <summary>
        /// 执行事件回调方法
        /// </summary>
        private void InvokeEvent(GfwEvent @event)
        {
            var callbacks = _EventDict.GetValues(@event.Id);
            if (callbacks.Count == 0&& EnableDebugLog)
            {
                LogWarning($"事件[{@event.GetType().Name}]没有注册任何回调，触发失败！");
                return;
            }

            // 遍历回调副本，避免遍历中注销导致的集合修改异常
            foreach (var callback in callbacks)
            {
                try
                {
                    callback?.Invoke(@event);
                }
                catch (Exception e)
                {
                    LogError($"事件[{@event.GetType().Name}]回调执行失败：{e.Message}\n{e.StackTrace}");
                }
            }

            if (EnableDebugLog)
            {
                LogInfo($"事件[{@event.GetType().Name}]触发成功，共执行[{callbacks.Count}]个回调！");
            }
        }

        /// <summary>
        /// 注销指定事件的所有回调
        /// </summary>
        public void RemoveAllListener<T>()where T : GfwEvent
        {
            Type eventType=typeof(T);
            _EventDict.RemoveKey(eventType.GetHashCode());
            if (EnableDebugLog)
            {
                LogInfo($"事件[{eventType.Name}]的所有回调已注销！");
            }
        }

        /// <summary>
        /// 批量注销指定对象的所有事件
        /// </summary>
        public void RemoveAllListenerFromTarget(object target)
        {
            if (target == null)
            {
                LogWarning("注销的目标对象不能为空！");
                return;
            }
            // 移除所有目标为该对象的回调
            _EventDict.RemoveWhere(action =>
            {
                if (action == null) return true;
                return action.Target == target;
            });
            if (EnableDebugLog)
            {
                LogInfo($"目标对象[{target.GetType().Name}]的所有事件回调已注销！");
            }
        }

        /// <summary>
        /// 清空所有事件和事件队列
        /// </summary>
        public void ClearAllEvents()
        {
            _EventDict.Clear();
            lock (_QueueLock)
            {
                _EventQueue.Clear();
            }
            LogInfo("所有事件已清空，事件队列已重置！");
        }
    }
}


