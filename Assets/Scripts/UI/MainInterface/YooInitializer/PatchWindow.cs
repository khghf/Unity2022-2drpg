using GFW.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PatchWindow : MonoBehaviour
{
    /// <summary>
    /// 对话框封装类
    /// </summary>
    private class MessageBox
    {
        private GameObject _cloneObject;
        private Text _content;
        private Button _btnOK;
        private System.Action _clickOK;

        public bool ActiveSelf
        {
            get
            {
                return _cloneObject.activeSelf;
            }
        }

        public void Create(GameObject cloneObject)
        {
            _cloneObject = cloneObject;
            _content = cloneObject.transform.Find("txt_content").GetComponent<Text>();
            _btnOK = cloneObject.transform.Find("btn_ok").GetComponent<Button>();
            _btnOK.onClick.AddListener(OnClickYes);
        }
        public void Show(string content, System.Action clickOK)
        {
            _content.text = content;
            _clickOK = clickOK;
            _cloneObject.SetActive(true);
            _cloneObject.transform.SetAsLastSibling();
        }
        public void Hide()
        {
            _content.text = string.Empty;
            _clickOK = null;
            _cloneObject.SetActive(false);
        }
        private void OnClickYes()
        {
            _clickOK?.Invoke();
            Hide();
        }
    }
    private readonly List<MessageBox> _msgBoxList = new List<MessageBox>();

    // UGUI相关
    [SerializeField]
    private GameObject _messageBoxObj;
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private Text _tips;
    private void Awake()
    {
        EventMgr.Inst.AddListener<PatchEventDefine.InitializeFailed>(HandleEvent);
        EventMgr.Inst.AddListener<PatchEventDefine.PackageManifestUpdateFailed>(HandleEvent);
        EventMgr.Inst.AddListener<PatchEventDefine.PatchStepsChange>(HandleEvent);
        EventMgr.Inst.AddListener<PatchEventDefine.FoundUpdateFiles>(HandleEvent);
        EventMgr.Inst.AddListener<PatchEventDefine.DownloadUpdate>(HandleEvent);
        EventMgr.Inst.AddListener<PatchEventDefine.PackageVersionRequestFailed>(HandleEvent);
        EventMgr.Inst.AddListener<PatchEventDefine.WebFileDownloadFailed>(HandleEvent);
    }
    private void Start()
    {
       
    }

    private IEnumerator DelayAction(Action action,float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        action?.Invoke();
        StopCoroutine("DelayAction");
    }

    private void OnDestroy()
    {
        EventMgr.Inst.RemoveAllListenerFromTarget(this);
    }
    private void HandleEvent(GFW.Event.GfwEvent @event)
    {
        if (@event is PatchEventDefine.InitializeFailed)
        {
            System.Action callback = () =>
            {
                GfwEvent.Broadcast<PatchEventDefine.UserTryInitialize>();
            };
            ShowMessageBox($"Failed to initialize package !", callback);
        }
        else if (@event is PatchEventDefine.PatchStepsChange)
        {
            var msg = @event as PatchEventDefine.PatchStepsChange;
            _tips.gameObject.SetActive(true);
            _tips.text = msg.Tips;
            UnityEngine.Debug.Log(msg.Tips);
            if (msg.bOver) StartCoroutine(DelayAction(() => _tips.gameObject.SetActive(false), 1)); 
        }
        else if (@event is PatchEventDefine.FoundUpdateFiles)
        {
            var msg = @event as PatchEventDefine.FoundUpdateFiles;
            System.Action callback = () =>
            {
                GfwEvent.Broadcast<PatchEventDefine.UserBeginDownloadWebFiles>();
            };
            float sizeMB = msg.TotalSizeBytes / 1048576f;
            sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
            string totalSizeMB = sizeMB.ToString("f1");
            ShowMessageBox($"Found update patch files, Total count {msg.TotalCount} Total szie {totalSizeMB}MB", callback);
        }
        else if (@event is PatchEventDefine.DownloadUpdate)
        {
            var msg = @event as PatchEventDefine.DownloadUpdate;
            _slider.gameObject.SetActive(true);
            _slider.value = (float)msg.CurrentDownloadCount / msg.TotalDownloadCount;
            _slider.onValueChanged.AddListener((float x) => { 
                if (x>=1) StartCoroutine(DelayAction(() => _slider.gameObject.SetActive(false), 1));
            });
            string currentSizeMB = (msg.CurrentDownloadSizeBytes / 1048576f).ToString("f1");
            string totalSizeMB = (msg.TotalDownloadSizeBytes / 1048576f).ToString("f1");
            _tips.text = $"{msg.CurrentDownloadCount}/{msg.TotalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
        }
        else if (@event is PatchEventDefine.PackageVersionRequestFailed)
        {
            System.Action callback = () =>
            {
                GfwEvent.Broadcast<PatchEventDefine.UserTryRequestPackageVersion>();
            };
            ShowMessageBox($"Failed to request package version, please check the network status.", callback);
        }
        else if (@event is PatchEventDefine.PackageManifestUpdateFailed)
        {
            System.Action callback = () =>
            {
                GfwEvent.Broadcast<PatchEventDefine.UserTryUpdatePackageManifest>();
            };
            ShowMessageBox($"Failed to update patch manifest, please check the network status.", callback);
        }
        else if (@event is PatchEventDefine.WebFileDownloadFailed)
        {
            var msg = @event as PatchEventDefine.WebFileDownloadFailed;
            System.Action callback = () =>
            {
                GfwEvent.Broadcast<PatchEventDefine.UserTryDownloadWebFiles>();
            };
            ShowMessageBox($"Failed to download file : {msg.FileName}", callback);
        }
        else
        {
            throw new System.NotImplementedException($"{@event.GetType()}");
        }
    }


    /// <summary>
    /// 显示对话框
    /// </summary>
    private void ShowMessageBox(string content, System.Action ok)
    {
        // 尝试获取一个可用的对话框
        MessageBox msgBox = null;
        for (int i = 0; i < _msgBoxList.Count; i++)
        {
            var item = _msgBoxList[i];
            if (item.ActiveSelf == false)
            {
                msgBox = item;
                break;
            }
        }

        // 如果没有可用的对话框，则创建一个新的对话框
        if (msgBox == null)
        {
            msgBox = new MessageBox();
            var cloneObject = GameObject.Instantiate(_messageBoxObj, _messageBoxObj.transform.parent);
            msgBox.Create(cloneObject);
            _msgBoxList.Add(msgBox);
        }

        // 显示对话框
        msgBox.Show(content, ok);
    }
}
