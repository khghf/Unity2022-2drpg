using GFW.Fsm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmEditor : Fsm
{
    FsmStateEditor  _curState;
    protected override void OnChangeState(FsmState state)
    {
        _curState=state as FsmStateEditor;
    }
    public  void OnSceneGUI() { _curState?.OnSceneGUI();  }
    public  void OnInspectorGUI() { _curState?.OnInspectorGUI(); }
}
