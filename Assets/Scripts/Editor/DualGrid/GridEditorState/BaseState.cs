#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace GridEditor
{
    public class BaseState : FsmStateEditor
    {
        protected DualGridEditor gridManagerEditor = null;
        protected GridMgr gridMgr = null;
        //protected SerializedObject serializedObject = null;
        public override void OnAdded()
        {
            base.OnAdded();
            gridManagerEditor=GetBlackboardValue<DualGridEditor>("GridManagerEditor");
            gridMgr=GetBlackboardValue<GridMgr>("GridMgr");
            //serializedObject=GetBlackboardValue<SerializedObject>("SerializedObject");
        }

        public override void OnEnter()
        {
            base.OnEnter();
            gridManagerEditor?.Repaint();
        }
    }
}
#endif

