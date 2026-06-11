using GFW.Event;
using GFW.Fsm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class CreateDownloader : FsmState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Create();
    }
    void Create()
    {
        var packageName = (string)GetBlackboardValue("PackageName");
        var package = YooAssets.GetPackage(packageName);
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        SetBlackboardValue("Downloader", downloader);

        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("Not found any download files !");
            ChangeState<StartGame>();
        }
        else
        {
            // 发现新更新文件后，挂起流程系统，弹出提示窗口
            // 注意：开发者需要在下载前检测磁盘空间不足
            PatchEventDefine.FoundUpdateFiles foundUpdateFiles = GfwEvent.Create<PatchEventDefine.FoundUpdateFiles>();
            foundUpdateFiles.TotalCount=downloader.TotalDownloadCount;
            foundUpdateFiles.TotalSizeBytes=downloader.TotalDownloadBytes;
            foundUpdateFiles.Broadcast();
        }
    }
}
