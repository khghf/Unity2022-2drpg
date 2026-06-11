using GFW.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class PatchEventDefine
{
    /// <summary>
    /// 补丁包初始化失败
    /// </summary>
    public class InitializeFailed : GFW.Event.GfwEvent
    {
      
    }

    /// <summary>
    /// 资源清单更新失败
    /// </summary>
    public class PackageManifestUpdateFailed : GFW.Event.GfwEvent
    {

    }

    /// <summary>
    /// 补丁流程步骤改变
    /// </summary>
    public class PatchStepsChange : GFW.Event.GfwEvent
    {
        public string Tips;
        public bool bOver = false;
    }

    /// <summary>
    /// 发现更新文件
    /// </summary>
    public class FoundUpdateFiles : GFW.Event.GfwEvent
    {
        public int TotalCount;
        public long TotalSizeBytes;
    }

    /// <summary>
    /// 下载进度更新
    /// </summary>
    public class DownloadUpdate : GFW.Event.GfwEvent
    {
        public int TotalDownloadCount;
        public int CurrentDownloadCount;
        public long TotalDownloadSizeBytes;
        public long CurrentDownloadSizeBytes;
    }

    /// <summary>
    /// 资源版本请求失败
    /// </summary>
    public class PackageVersionRequestFailed : GFW.Event.GfwEvent
    {

    }

   

    /// <summary>
    /// 网络文件下载失败
    /// </summary>
    public class WebFileDownloadFailed : GFW.Event.GfwEvent
    {
        public string FileName;
        public string Error;

    }







    /// <summary>
    /// 用户尝试再次初始化资源包
    /// </summary>
    public class UserTryInitialize : GFW.Event.GfwEvent
    {
       
    }

    /// <summary>
    /// 用户开始下载网络文件
    /// </summary>
    public class UserBeginDownloadWebFiles : GFW.Event.GfwEvent
    {
      
    }

    /// <summary>
    /// 用户尝试再次请求资源版本
    /// </summary>
    public class UserTryRequestPackageVersion : GFW.Event.GfwEvent
    {
       
    }

    /// <summary>
    /// 用户尝试再次更新补丁清单
    /// </summary>
    public class UserTryUpdatePackageManifest : GFW.Event.GfwEvent
    {
       
    }

    /// <summary>
    /// 用户尝试再次下载网络文件
    /// </summary>
    public class UserTryDownloadWebFiles : GFW.Event.GfwEvent
    {

    }
}