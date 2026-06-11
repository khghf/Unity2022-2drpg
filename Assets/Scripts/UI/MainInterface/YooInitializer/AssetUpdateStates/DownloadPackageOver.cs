using GFW.Event;
using GFW.Fsm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadPackageOver : FsmState
{
    public override void OnEnter()
    {
        base.OnEnter();
        PatchEventDefine.PatchStepsChange patchStepsChange = GfwEvent.Create<PatchEventDefine.PatchStepsChange>();
        patchStepsChange.Tips="资源文件下载完毕！";
        patchStepsChange.bOver=true;
        patchStepsChange.Broadcast();
        ChangeState<ClearCacheBundle>();
        //ChangeState<FsmClearCacheBundle>();
    }
}
