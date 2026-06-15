using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GFW
{
    /// <summary>
    /// 管理器基类，不要实现Awake(空函数也不行)，请用Init()来替代Awake()
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Manager<T> : Singleton<T> where T : Manager<T>
    {
        public bool EnableDebugLog = true;
        protected Type _Type=typeof(T);
        /// <summary>
        /// 该函数在Awake中执行所以不要实现Awake函数
        /// </summary>
        protected virtual void Init()
        {
            //场景切换时若新的场景中挂载了相同的管理器单例则销毁新的管理器防止出现多个"单例",
            //前提是需要把Awake的逻辑移到Init中实现且不能实现Awake函数即使是放个空函数也不行
            if (_instance!=null)
            {
                GameObject.Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(Inst);
        }
        private void Awake()
        {
            Init();
        }

        protected void LogInfo(string msg)
        {
            if (EnableDebugLog) Debug.Log($"[{_Type.Name}] {msg}");
        }
        protected void LogWarning(string msg)
        {
            if (EnableDebugLog) Debug.LogWarning($"[{_Type.Name}] {msg}");
        }
        protected void LogError(string msg)
        {
            if (EnableDebugLog) Debug.LogError($"[{_Type.Name}] {msg}");
        }
    }
}


