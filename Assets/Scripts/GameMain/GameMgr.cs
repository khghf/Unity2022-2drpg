using DG.Tweening;
using GFW;
using GFW.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public class GameMgr : Manager<GameMgr>
{
    /// <summary>
    /// 场景类型
    /// </summary>
    public enum SceneType
    {
        UIScene,
        MapScene,
        BattleScene
    }
    /// <summary>
    /// 当前所处场景类型
    /// </summary>
    public SceneType curSceneType= SceneType.UIScene;
    /// <summary>
    /// 游戏是否开始(点击开始游戏后一直为true,目前)
    /// </summary>
    bool isGameStarted = false;
    /// <summary>
    /// 当前游戏场景
    /// </summary>
    public GameScene curGameScene = null;
    /// <summary>
    /// 游戏默认开始场景
    /// </summary>
    public string defaultScene = "StartLevel";

    protected override void Init()
    {
        base.Init();
        DOTween.Init(true);

    }
  
    private void OnApplicationQuit()
    {
        if (isGameStarted)
        {
            SaveData curSaveData=SaveMgr.GetCurSaveData();
            curSaveData.curScene=SceneManager.GetActiveScene().name;
            curSaveData.playerPos=(GameScene.Inst.Pawn as PlayerCharacter).logicCellpos;
            SaveMgr.SaveGameData();
        }
    }

    public void StartGame()
    {
        if (isGameStarted) return;
        isGameStarted=true;
        SaveData saveData= SaveMgr.LoadGameData();
        GameLoader.LoadGame(saveData);
        //LogInfo($"Glod:{saveData.Gold}");
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public IEnumerator DelayAction(Action action,float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        action?.Invoke();
    }
    private class GameLoader
    {
        public static void LoadGame(SaveData saveData)
        {
            if(saveData==null)
            {
                Debug.LogError("当前存档不可用，加载游戏失败");
                return;
            }
            SceneLoader.LoadMapSceneAsync(saveData.curScene);
        }
    }
}



