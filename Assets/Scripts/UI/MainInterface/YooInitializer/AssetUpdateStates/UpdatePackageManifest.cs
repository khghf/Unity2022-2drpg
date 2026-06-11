using GFW.Fsm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
/// <summary>
/// 更新资源清单
/// </summary>
public class UpdatePackageManifest : FsmState
{
    public override void OnEnter()
    {
        base.OnEnter();
        GameMgr.Inst.StartCoroutine(UpdateManifest());
        
    }
    private IEnumerator UpdateManifest()
    {
        var packageName = (string)GetBlackboardValue("PackageName");
        var packageVersion = (string)GetBlackboardValue("PackageVersion");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.UpdatePackageManifestAsync(packageVersion);
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            GFW.Event.GfwEvent.Broadcast<PatchEventDefine.PackageManifestUpdateFailed>();
            yield break;
        }
        else
        {
            //ChangeState<FsmCreateDownloader>();
            ChangeState<CreateDownloader>();
        }
    }
}
