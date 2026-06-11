using GFW.Event;
using GFW.Fsm;
using System.Collections;
using YooAsset;

public class DownloadPackageFiles : FsmState
{
    public override void OnEnter()
    {
        base.OnEnter();
        PatchEventDefine.PatchStepsChange patchStepsChange = GfwEvent.Create<PatchEventDefine.PatchStepsChange>();
        patchStepsChange.Tips="开始下载资源文件！";
        patchStepsChange.Broadcast();
        GameMgr.Inst.StartCoroutine(BeginDownload());
    }

    private IEnumerator BeginDownload()
    {
        var downloader = (ResourceDownloaderOperation)GetBlackboardValue("Downloader");
        downloader.DownloadErrorCallback = DownloadErrorCallback;
        downloader.DownloadUpdateCallback = DownloadUpdateCallback;
        downloader.BeginDownload();
        yield return downloader;

        // 检测下载结果
        if (downloader.Status != EOperationStatus.Succeed)
            yield break;

        ChangeState<DownloadPackageOver>();
    }

    private void DownloadErrorCallback(DownloadErrorData data)
    {
        PatchEventDefine.WebFileDownloadFailed webFileDownloadFailed=new PatchEventDefine.WebFileDownloadFailed();
        webFileDownloadFailed.FileName=data.FileName;
        webFileDownloadFailed.Error=data.ErrorInfo;
    }
    private void DownloadUpdateCallback(DownloadUpdateData data)
    {
        PatchEventDefine.DownloadUpdate downloadUpdate= GfwEvent.Create<PatchEventDefine.DownloadUpdate>();
        downloadUpdate.TotalDownloadCount = data.TotalDownloadCount;
        downloadUpdate.CurrentDownloadCount = data.CurrentDownloadCount;
        downloadUpdate.TotalDownloadSizeBytes = data.TotalDownloadBytes;
        downloadUpdate.CurrentDownloadSizeBytes = data.CurrentDownloadBytes;
        downloadUpdate.Broadcast();
    }

}
