using GFW.Fsm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : FsmState
{
    public override void OnEnter()
    {
        base.OnEnter();
        GetBlackboardValue<PatchOperation>("PatchOperation").SetFinish();
    }
}
