using GFW.Fsm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
/// <summary>
/// 请求资源包版本
/// </summary>
public class RequestPackageVersion : FsmState
{
    public override void OnEnter()
    {
        base.OnEnter();
        GameMgr.Inst.StartCoroutine(UpdatePackageVersion());
    }
    private IEnumerator UpdatePackageVersion()
    {
        var packageName = (string)GetBlackboardValue("PackageName");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.RequestPackageVersionAsync();
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
        }
        else
        {
           
            SetBlackboardValue("PackageVersion", operation.PackageVersion);
            ChangeState<UpdatePackageManifest>();
        }
    }
}
