using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace GFW.Event
{
    /// <summary>
    /// 事件数据实体
    /// </summary>
    public class EventData
    {
        ///// <summary>
        ///// 事件名
        ///// </summary>
        //public string EventName;

        /// <summary>
        /// 自定义数据
        /// </summary>
        public GameObject CustomData;

        /// <summary>
        /// 延迟触发时间(秒)，小于或等于0为立即触发
        /// </summary>
        public float DelayTime;

        /// <summary>
        /// 事件创建时间(用于计算延迟触发)
        /// </summary>
        public float CreateTime { get; private set; }
        protected EventData( GameObject customData = null, float delayTriggerTime = 0)
        {
            CustomData = customData;
            DelayTime = delayTriggerTime;
            CreateTime = Time.time;
        }

        public static EventData Create(float delayTriggerTime = 0,GameObject customData = null)
        {

           return new EventData(customData, delayTriggerTime);
        }

        /// <summary>
        /// 判断是否到达触发时间
        /// </summary>
        public bool IsReadyToTrigger()
        {
            return Time.time - CreateTime >= DelayTime;
        }
    }

    public class GfwEvent
    {
        public int Id { get; internal set; }


        public EventData Payload { get; protected set; }

        public static T Create<T>()where T : GfwEvent,new()
        {
            T ret=new T();

            ret.Id=typeof(T).GetHashCode();
            ret.Payload=EventData.Create(0, null);
            return ret;
        }

        public static T Create<T>(float delayTriggerTime) where T : GfwEvent, new()
        {
            T ret = new T();

            ret.Id=typeof(T).GetHashCode();
            ret.Payload=EventData.Create(delayTriggerTime, null);

            return ret;
        }

        public static T Create<T>(float delayTriggerTime, GameObject customData) where T : GfwEvent, new()
        {
            T ret = new T();

            ret.Id=typeof(T).GetHashCode();
            ret.Payload=EventData.Create(delayTriggerTime, customData);

            return ret;
        }
        /// <summary>
        /// 调用该实例方法时请确保实例是通过GfwEvent.Create()创建的
        /// </summary>
        public void Broadcast()
        {
            EventMgr.Inst.Broadcast(this);
        }
        /// <summary>
        /// 调用该实例方法时请确保实例是通过GfwEvent.Create()创建的
        /// </summary>
        public void Broadcast_Safe()
        {
            EventMgr.Inst.Broadcast_Safe(this);
        }

        public static void Broadcast<T>()where T: GfwEvent,new()
        {
            EventMgr.Inst.Broadcast(Create<T>());
        }
        public static void Broadcast_Safe<T>() where T : GfwEvent, new()
        {
            EventMgr.Inst.Broadcast_Safe(Create<T>());
        }
    }
}

