using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SOListener : MonoBehaviour
{
    [SerializeField]
    protected List<SOEvent> ListenEvents = null;

    private void OnEnable()
    {
        StartListening();
    }
    private void OnDisable()
    {
        EndListening();
    }

    public abstract void ReceviceNotification(SOEvent soEvent);
    

    public void StartListening()
    {
        if (ListenEvents==null) return;
        foreach (SOEvent soEvent in ListenEvents)
        {
            soEvent.AddListener(this);
        }
    }
    public void EndListening()
    {
        if(ListenEvents==null) return;
        foreach (SOEvent soEvent in ListenEvents)
        {
            soEvent.RemoveListener(this);
        }
    }
}
