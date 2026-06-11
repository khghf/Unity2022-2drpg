using GFW.Fsm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class ClearCacheBundle : FsmState
{
    public override void OnEnter()
    {
        base.OnEnter();
        PatchEventDefine.PatchStepsChange patchStepsChange = new PatchEventDefine.PatchStepsChange();
        patchStepsChange.Tips="清理未使用的缓存文件！";
        var packageName = (string)GetBlackboardValue("PackageName");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
        operation.Completed += Operation_Completed;
    }
    private void Operation_Completed(YooAsset.AsyncOperationBase obj)
    {
        ChangeState<StartGame>();
    }
}
