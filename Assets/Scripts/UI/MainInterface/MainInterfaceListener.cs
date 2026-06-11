using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;

public class MainInterfaceListener : SOListener
{
    public override  void ReceviceNotification(SOEvent soEvent)
    {
        if(soEvent.name=="StartButtonClick")
        {
            GameMgr.Inst.StartGame();
            Debug.Log("StartButtonClick");
        }
        if (soEvent.name=="ExitButtonClick")
        {
            Debug.Log("ExitButtonClick");
            GameMgr.Inst.QuitGame();
        }
    }
}
