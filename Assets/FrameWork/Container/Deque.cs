using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GFW.Container
{
    /// <summary>
    /// 循环双向队列
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class Deque<TValue>
    {
        [SerializeField]
        private readonly List<TValue> _InnerList = new List<TValue>();

        private int _Front = 0;
        private int _Trail = 0;
        private int _Size = 0;
        private int _Capacity = 0;

        //队头索引
        public int Front => _Front;
        //队尾索引
        public int Trail => _Trail;
        //元素数量
        public int Size => _Size;
        //元素容量
        public int Capacity => _Capacity;

        public Deque(int capacity)
        {
            _Capacity = capacity;
            _InnerList = new List<TValue>(_Capacity);
        }

        public Deque(List<TValue> list)
        {
            _InnerList = list;
            _Size=_InnerList.Count;
            _Capacity =_InnerList.Capacity;
            _Front = 0;
            _Trail=_Size>0 ? _Size-1 : 0;
        }

        public bool IsFull()
        {
            return _Size>=_Capacity;
        }

        public bool IsEmpty()
        {
            return _Size==0;
        }

        /// <summary>
        /// 尾部入队
        /// </summary>
        /// <returns></returns>
        public void Push_Back(TValue element)
        {
            if (IsFull()) return;
            _Trail=(_Trail+1)%_Capacity;
            _InnerList[_Trail]=default(TValue);
            _InnerList[_Trail]=element;
            ++_Size;
        }

        /// <summary>
        /// 头部入队
        /// </summary>
        /// <returns></returns>
        public void Push_Front(TValue element)
        {
            if (IsFull()) return;
            _Front=(_Front+_Capacity-1)%_Capacity;
            _InnerList[_Front]=default(TValue);
            _InnerList[_Front]=element;
            ++_Size;
        }

        /// <summary>
        /// 获取队尾元素并出队
        /// </summary>
        /// <returns></returns>
        public TValue Pop_Back()
        {
            TValue res = default(TValue);
            if (IsEmpty()) return res;
            res= _InnerList[_Trail];
            _Trail=(_Trail+_Capacity-1)%_Capacity;
            --_Size;
            return res;
        }

        /// <summary>
        /// 获取队头元素并出队
        /// </summary>
        /// <returns></returns>
        public TValue Pop_Front()
        {
            TValue res = default(TValue);
            if (IsEmpty()) return res;
            res= _InnerList[_Front];
            _Front=(_Front+1)%_Capacity;
            --_Size;
            return res;
        }

        /// <summary>
        /// 获取队尾元素
        /// </summary>
        /// <returns></returns>
        public TValue Peek_Back()
        {
            TValue res = default(TValue);
            if (IsEmpty()) return res;
            res= _InnerList[_Trail];
            return res;
        }
        /// <summary>
        /// 获取队头元素
        /// </summary>
        /// <returns></returns>
        public TValue Peek_Front()
        {
            TValue res = default(TValue);
            if (IsEmpty()) return res;
            res= _InnerList[_Front];
            return res;
        }

        /// <summary>
        /// 根据_InnerList更新内部字段非编辑器内赋值的_InnerList不要使用
        /// </summary>
        public void Update()
        {
            _Size=_InnerList.Count;
            _Capacity =_InnerList.Capacity;
            _Front = 0;
            _Trail=_Size>0 ? _Size-1 : 0;
        }
    }
}


