using System;
using System.Collections.Generic;
using UnityEngine;
namespace GFW.Fsm
{
    public class BlackBoard
    {
        private Dictionary<string, object> _Items= new Dictionary<string, object>();
        public Dictionary<string, object> Items => _Items;

        public void AddItem(string key, object value)
        {
            //if (_Items.ContainsKey(key)) return;
            _Items[key] = value;
        }

        public void RemoveItem(string key)
        {
            _Items.Remove(key);
        }

        public object GetItem(string key) { return _Items[key]; }
        public T GetItem<T>(string key) { return  (T)_Items[key]; }
    }


    public class Fsm 
    {
        public GameObject owner { get; set; }
        private Dictionary<int, FsmState> _states = new Dictionary<int, FsmState>();

        private FsmState _curState = null;
        public FsmState CurState=>_curState;
        //控制状态的更新，不影响状态的转换
        private bool _isRunning=false;
        private bool _isFirstRun=true;

        //黑板用于给状态提供额外的信息
        private BlackBoard _blackboard=new BlackBoard();
        public BlackBoard Blackboard=>_blackboard;

        public Fsm() { }

        public Fsm(GameObject owner)
        {
            this.owner=owner;
        }

        public void Update()
        {
            if(_isRunning)
            {
                _curState?.OnUpdate();
            }
        }

        /// <summary>
        /// 设置入口状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        public void SetEntryState<T>()where T:FsmState
        {
            Type type = typeof(T);
            int id = type.GetHashCode();
            if (!_states.ContainsKey(id))
            {
                Debug.LogWarning($"设置入口状态失败，状态机内没有{type.Name}状态");
                return;
            }
            //ChangeState<T>();
            _curState= _states[id];
        }

        /// <summary>
        /// 运行状态机每帧调用当前状态的更新函数
        /// </summary>
        public void Run()
        {
            _isRunning=true;
            if (_isFirstRun)
            {
                _isFirstRun=false;
                if(_curState==null)
                {
                    Debug.LogWarning($"运行状态机前必须设置入口状态(请调用SetEntryState)");
                    return;
                }
                _curState.OnEnter();
            }
        }

        /// <summary>
        /// 暂停状态机不再每帧调用状态的更新函数
        /// 可以正常切换状态,OnEnter、OnExit不受影响
        /// </summary>
        public void Stop()
        {
            _isRunning=false;
        }

        //public void SetBlackboard(object blackboard)
        //{
        //    _blackboardInternal = blackboard;
        //}

        public T AddState<T>()where T : FsmState, new() 
        {
            T state = new T();
            AddState(state);
            return state;
        }
        public Fsm AddState<T>(T state) where T : FsmState
        {
            int id = typeof(T).GetHashCode();
            if (_states.ContainsKey(id))
            {
                Debug.LogWarning("[Fsm]重复添加状态");
                return this;
            }
            state.OwnerFsm = this;
            _states[id] = state;
            OnAddState(state);
            return this;
        }

        public void RemoveState<T>()where T:FsmState
        {
            int id = typeof(T).GetHashCode();
            if (!_states.ContainsKey(id))return ;
            if (_curState==_states[id])
            {
                Debug.LogWarning("不能移除正在运行的状态");
                return;
            }
            _states.Remove(id);
        }
        public void RemoveAllState()
        {
            Stop();
            foreach(var state in _states.Values)
            {
                OnRemoveState(state);
            }
            _states.Clear();
        }

        public void ChangeState<T>() where T : FsmState
        {
            Type type = typeof(T);
            int id = type.GetHashCode();
            if (!_states.ContainsKey(id))
            {
                Debug.LogWarning($"[Fsm]状态改变失败状态机内没有{type.Name}");
            }
            ChangeState(id);
        }
        private void ChangeState(int id)
        {
            if (_curState==_states[id]) return;
            _curState?.OnExit();
            _curState=_states[id];
            OnChangeState(_curState);
            _curState.OnEnter();
        }

        public FsmState GetState<T>()where T:FsmState
        {
            FsmState state = null;
            _states.TryGetValue(typeof(T).GetHashCode(), out state);
            return state;
        }

        protected virtual void OnAddState(FsmState state)
        {
            state?.OnAdded();
        }
        protected virtual void OnRemoveState(FsmState state)
        {

        }
        protected virtual void OnChangeState(FsmState state)
        {

        }
    }
}

