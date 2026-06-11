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
            if (_Items.ContainsKey(key)) return;
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
        public GameObject Owner { get; set; }
        private Dictionary<int, FsmState> _States = new Dictionary<int, FsmState>();

        private FsmState _CurState = null;
        public FsmState CurState=>_CurState;
        //控制状态的更新，不影响状态的转换
        private bool _IsRunning=false;
        private bool _IsFirstRun=true;

        private BlackBoard _Blackboard=new BlackBoard();
        public BlackBoard Blackboard=>_Blackboard;
        //黑板用于给状态提供额外的信息
        [SerializeField]
        private object _BlackboardInternal = null;
        public object BlackboardInternal => _BlackboardInternal;


        public Fsm() { }

        public Fsm(GameObject owner)
        {
            Owner=owner;
        }

        public void Update()
        {
            if(_IsRunning)
            {
                _CurState?.OnUpdate();
            }
        }

        /// <summary>
        /// 设置入口状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        public void SetEntryState<T>()where T:FsmState
        {
            Type type = typeof(T);
            int id=type.GetHashCode();
            if(!_States.ContainsKey(id))
            {
                Debug.LogWarning($"设置入口状态失败，状态机内没有{type.Name}状态");
                return;
            }
            _CurState= _States[id];
        }

        /// <summary>
        /// 运行状态机每帧调用当前状态的更新函数
        /// </summary>
        public void Run()
        {
            _IsRunning=true;
            if (_IsFirstRun)
            {
                _IsFirstRun=false;
                if(_CurState==null)
                {
                    Debug.LogWarning($"运行状态机前必须设置入口状态(请调用SetEntryState)");
                    return;
                }
                _CurState.OnEnter();
            }
        }

        /// <summary>
        /// 暂停状态机不再每帧调用状态的更新函数
        /// 可以正常切换状态,OnEnter、OnExit不受影响
        /// </summary>
        public void Stop()
        {
            _IsRunning=false;
        }

        public void SetBlackboard(object blackboard)
        {
            _BlackboardInternal = blackboard;
        }

        public Fsm AddState<T>(T state)where T : FsmState
        {
            int id = typeof(T).GetHashCode();
            if (_States.ContainsKey(id))
            {
                Debug.LogWarning("[Fsm]重复添加状态");
                return this;
            }
            state.OwnerFsm = this;
            _States[id] = state;
            return this;
        }

        public T AddState<T>()where T : FsmState, new() 
        {
            T state = new T();
            AddState(state);
            return state;
        }

        public void ChangeState<T>()where T:FsmState
        {
            Type type = typeof(T);
            int id = type.GetHashCode();    
            if (!_States.ContainsKey(id))
            {
                Debug.LogWarning($"[Fsm]状态改变失败状态机内没有{type.Name}");
            }
            ChangeState(id);
        }

        private void ChangeState(int id)
        {
            _CurState?.OnExit();
            _CurState=_States[id];
            _CurState.OnEnter();
        }
        
        public FsmState GetState<T>()where T:FsmState
        {
            FsmState state = null;
            _States.TryGetValue(typeof(T).GetHashCode(), out state);
            return state;
        }
    }
}

