using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GFW.Fsm
{
    public class FsmState
    {
        //所属状态机
        public Fsm OwnerFsm;


        public event Action OnExitHook;
        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnExit()
        {
            OnExitHook?.Invoke();
        }

        public virtual void OnAdded()
        {

        }

        /// <summary>
        /// 改变到另一个状态
        /// </summary>
        /// <typeparam name="T">另一个状态类</typeparam>
        public virtual void ChangeState<T>()where T : FsmState
        {
            OwnerFsm?.ChangeState<T>();
        }

        public object GetBlackboardValue(string key)
        {
            return OwnerFsm.Blackboard.GetItem(key);
        }
        public T GetBlackboardValue<T>(string key)
        {
            return OwnerFsm.Blackboard.GetItem<T>(key);
        }

        public void SetBlackboardValue(string key,object value)
        {
            OwnerFsm.Blackboard.AddItem(key,value);
        }

        public T AddState<T>()where T: FsmState, new()
        {
            return OwnerFsm.AddState<T>();
        }
    }
}

