using GFW.Event;
using GFW.Fsm;
using System;
using System.Collections;
using UnityEngine;
using YooAsset;
[Serializable]
public class YooAssetInitializer:MonoBehaviour
{
    private Fsm fsm=new Fsm();
    [SerializeField]
    private EPlayMode playMode;
    [SerializeField]
    private string DefaultPackageName = "DefaultPackage";
    [SerializeField]
    string AppVersion = "1.0";
    IEnumerator Start()
    {
        if (YooAssets.Initialized)StopCoroutine("Start");
        YooAssets.Initialize();

        fsm.Owner=gameObject;
        fsm.Blackboard.AddItem("PlayMode", playMode);
        fsm.Blackboard.AddItem("PackageName", DefaultPackageName);
        fsm.Blackboard.AddItem("AppVersion", AppVersion);

        fsm.AddState<InitPackage>();
        fsm.AddState<RequestPackageVersion>();
        fsm.AddState<UpdatePackageManifest>();
        fsm.AddState<CreateDownloader>();
        fsm.AddState<DownloadPackageFiles>();
        fsm.AddState<DownloadPackageOver>();
        fsm.AddState<ClearCacheBundle>();
        fsm.AddState<StartGame>();
        // 开始补丁更新流程
        var operation = new PatchOperation("DefaultPackage", playMode,fsm);
        YooAssets.StartOperation(operation);
        fsm.Blackboard.AddItem("PatchOperation", operation);
        yield return operation;

        // 设置默认的资源包
        var gamePackage = YooAssets.GetPackage("DefaultPackage");
        YooAssets.SetDefaultPackage(gamePackage);
    }
}

public class PatchOperation : GameAsyncOperation
{
    private enum ESteps
    {
        None,
        Update,
        Done,
    }
    private readonly Fsm fsm;
    private readonly string _PackageName;
    private ESteps _Steps = ESteps.None;

    public PatchOperation(string packageName, EPlayMode playMode,Fsm fsm)
    {
        _PackageName = packageName;
        this.fsm=fsm;
        // 注册监听事件
        EventMgr.Inst.AddListener<PatchEventDefine.UserTryInitialize>(OnHandleEventMessage);
        EventMgr.Inst.AddListener<PatchEventDefine.UserBeginDownloadWebFiles>(OnHandleEventMessage);
        EventMgr.Inst.AddListener<PatchEventDefine.UserTryRequestPackageVersion>(OnHandleEventMessage);
        EventMgr.Inst.AddListener<PatchEventDefine.UserTryUpdatePackageManifest>(OnHandleEventMessage);
        EventMgr.Inst.AddListener<PatchEventDefine.UserTryDownloadWebFiles>(OnHandleEventMessage);
    }
    protected override void OnStart()
    {
        _Steps = ESteps.Update;
        fsm.SetEntryState<InitPackage>();
        fsm.Run();
    }
    protected override void OnUpdate()
    {
        if (_Steps == ESteps.None || _Steps == ESteps.Done)
            return;

        if (_Steps == ESteps.Update)
        {
            fsm.Update();
        }
    }
    protected override void OnAbort()
    {
    }

    public void SetFinish()
    {
        _Steps = ESteps.Done;
        Status = EOperationStatus.Succeed;
        EventMgr.Inst.RemoveAllListenerFromTarget(this);
        Debug.Log($"Package {_PackageName} patch done !");
    }

    /// <summary>
    /// 接收事件
    /// </summary>
    private void OnHandleEventMessage(GFW.Event.GfwEvent @event)
    {
        if (@event is PatchEventDefine.UserTryInitialize)
        {
            fsm.ChangeState<InitPackage>();
        }
        else if (@event is PatchEventDefine.UserBeginDownloadWebFiles)
        {
            fsm.ChangeState<DownloadPackageFiles>();
        }
        else if (@event is PatchEventDefine.UserTryRequestPackageVersion)
        {
            fsm.ChangeState<RequestPackageVersion>();
        }
        else if (@event is PatchEventDefine.UserTryUpdatePackageManifest)
        {
            fsm.ChangeState<UpdatePackageManifest>();
        }
        else if (@event is PatchEventDefine.UserTryDownloadWebFiles)
        {
            fsm.ChangeState<CreateDownloader>();
        }
        else
        {
            throw new System.NotImplementedException($"{@event.GetType()}");
        }
    }
}
