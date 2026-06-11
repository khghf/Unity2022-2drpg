using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SONotifier : MonoBehaviour
{
    [SerializeField]
    private SOEvent SOEvent;
    public void Notify()
    {
        if (SOEvent==null) return;
        var listeners=SOEvent.SOListeners;
        if (listeners==null) return;
        foreach (var listener in listeners)
        {
            listener.ReceviceNotification(SOEvent);
        }
    }
}
