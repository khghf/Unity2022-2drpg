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
    public string curScene = "StartLevel";
    /// <summary>
    /// 玩家角色所处的逻辑网格行列
    /// </summary>
    public Vector2Int playerPos;
    /// <summary>
    /// 玩家角色预制体名称
    /// </summary>
    public string playerPrefabName = "PlayerCharacter";
}
