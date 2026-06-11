using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
namespace GFW
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;
        private static bool _isApplicationQuiting = false;

        public static T Inst
        {
            get
            {
                //程序退出调用GameObject的销毁函数时可能会引用到已销毁的单例而触发在OnDestroy中创建对象的错误
                if (_isApplicationQuiting) return _instance;
                if (_instance == null)
                {
                    _instance=FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        Type type = typeof(T);
                        GameObject gameobj = new GameObject(type.Name);
                        _instance = gameobj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
        protected Singleton() { }
        private void OnApplicationQuit()
        {
            _isApplicationQuiting=true;
        }
    }
}


