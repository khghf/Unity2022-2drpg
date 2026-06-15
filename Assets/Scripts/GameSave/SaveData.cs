using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData : GameSaveData
{
    /// <summary>
    /// 当前场景名称
    /// </summary>
    public string curScene = GameMgr.Inst.defaultScene;
    /// <summary>
    /// 玩家角色所处的逻辑网格行列
    /// </summary>
    public Vector2Int playerPos=new Vector2Int(GridMgr.LogicGridResolution/2, GridMgr.LogicGridResolution/2);
    /// <summary>
    /// 玩家角色预制体名称
    /// </summary>
    public string playerPrefabName = "PlayerCharacter";


}
