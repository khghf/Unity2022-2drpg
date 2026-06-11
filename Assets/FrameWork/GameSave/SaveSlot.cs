using UnityEngine;
using System;
using System.Collections.Generic;



public class SaveSlot<T>where T : GameSaveData
{
    public T Data = null;

    public SaveSlot(T data)
    {
        this.Data = data;
    }
}