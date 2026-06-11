using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SOEvent : ScriptableObject
{
    protected List<SOListener>Listeners = new List<SOListener>();

    public List<SOListener> SOListeners=>Listeners;
    public void AddListener(SOListener listener)
    {
        if (listener==null) return;
        Listeners.Add(listener);
    }
    public void RemoveListener(SOListener listener)
    {
        if (listener==null) return;
        Listeners.Remove(listener);
    }
}

