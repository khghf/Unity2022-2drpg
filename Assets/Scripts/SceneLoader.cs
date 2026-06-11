using GFW.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
public class SceneLoader
{
    /// <summary>
    /// 场景过渡类型，根据此枚举对目标场景执行不同的初始化逻辑
    /// </summary>
    enum TransitionType
    {
        None,
        UIToMap,    //从UI场景到Map场景
        MapToBattle,//从地图场景到战斗场景
        BattleToMap,//从战斗场景到地图场景
        MapToUI,    //从地图场景到UI场景
        MapToMap,   //从地图场景到地图场景
    }

    private static readonly string UIScene = "UIScene";
    private static readonly string MapScene = "MapScene";
    private static readonly string BattleScene = "BattleScene";
    private static readonly string TransitionLevelName = "TransitionLevel";
    private static readonly string TransitionLevelWithGroupName = $"{UIScene}_{TransitionLevelName}";

    /// <summary>
    /// 异步加载地图场景，加载期间会进入过渡场景等待,通过YooAsset(GroupAndName)寻址
    /// </summary>
    /// <param name="name"></param>
    public static void LoadMapSceneAsync(string name)
    {
        string path = MapScene + "_" + name;
        LoadScene(path, GetTransitionType(GameMgr.SceneType.MapScene));
    }

    /// <summary>
    /// 异步加载战斗场景，加载期间会进入过渡场景等待,通过YooAsset(GroupAndName)寻址
    /// </summary>
    /// <param name="name"></param>
    public static void LoadBattleSceneAsync(string name)
    {
        string path = BattleScene + "_" + name;

        LoadScene(path, GetTransitionType(GameMgr.SceneType.BattleScene));
    }

    private static void LoadScene(string path, TransitionType transitionType)
    {
        GameMgr.Inst.StartCoroutine(LoadSceneRoutine(path, transitionType));
    }

    private static IEnumerator LoadSceneRoutine(string path, TransitionType transitionType)
    {
        //加载过渡场景
        SceneHandle transition = YooAssets.LoadSceneAsync(TransitionLevelWithGroupName, LoadSceneMode.Additive);
        yield return transition;
        TransitionLevel transitionLevel = GetRootGameObjectFromScene(transition.SceneObject, "TransitionUI").GetComponent<TransitionLevel>();

        // 播放淡入动画
        transitionLevel.PlayBGFadeInAnim();
        yield return new WaitForSeconds(transitionLevel.FadeInTime);

        // 卸载当前场景
        Scene curScene = SceneManager.GetActiveScene();
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(curScene);

        // 加载目标场景
        SceneHandle target = YooAssets.LoadSceneAsync(path, LoadSceneMode.Additive, LocalPhysicsMode.None, true);
        
        //强制等待2秒
        yield return new WaitForSeconds(2);

        // 等待当前场景卸载完成
        while (!unloadOp.isDone) yield return null;

        //取消目标场景挂起继续加载
        target.UnSuspend();
        yield return target;

        if (target.Status == EOperationStatus.Succeed)
        {
            InitTargetScene(target.SceneObject,transitionType);
            // 播放淡出动画
            transitionLevel.PlayBGFadeOutAnim();
            yield return new WaitForSeconds(transitionLevel.FadeOutTime);
            target.ActivateScene();
            SceneManager.SetActiveScene(target.SceneObject);

            if (transition.IsValid)transition.UnloadAsync();
        }
    }

    public static GameObject GetRootGameObjectFromScene(in Scene scene,string objName)
    {
        var sceneGameObjects=scene.GetRootGameObjects();

        foreach (var gameObject in sceneGameObjects)
        {
            if(gameObject.name==objName)return gameObject;
        }
        return null;
    }

    /// <summary>
    /// 初始化目标场景
    /// </summary>
    /// <param name="curScene">当前场景</param>
    /// <param name="targetScene">要过度到的场景</param>
    /// <param name="transitionType">过渡类型</param>
    private static void InitTargetScene(Scene targetScene, TransitionType transitionType)
    {
        //从主界面到地图场景
        if(transitionType== TransitionType.UIToMap)
        {
            SaveData saveData= SaveMgr.GetSaveData(0);
            GameScene gameScene = GameScene.Inst;

            AssetHandle assetHandle= YooAssets.LoadAssetSync<GameObject>($"Prefab_{saveData.playerPrefabName}");
            GameObject playerGO= assetHandle.InstantiateSync();

            PlayerCharacter playerCharacter = playerGO.GetComponent<PlayerCharacter>();
            //gameScene.Init(new MapPlayerController(), playerCharacter, null);

            playerCharacter.SetPosTo(saveData.playerPos);
        }
        //从地图场景到地图场景
        else if(transitionType==TransitionType.MapToMap)
        {

        }

    }

    private static TransitionType GetTransitionType(GameMgr.SceneType targetType)
    {
        if(GameMgr.Inst.curSceneType==GameMgr.SceneType.UIScene)
        {
            if (targetType==GameMgr.SceneType.MapScene) return TransitionType.UIToMap;
        }
        if (GameMgr.Inst.curSceneType==GameMgr.SceneType.MapScene)
        {
            if(targetType==GameMgr.SceneType.UIScene)return TransitionType.MapToUI;
            if (targetType==GameMgr.SceneType.BattleScene) return TransitionType.MapToBattle;
        }
        if (GameMgr.Inst.curSceneType==GameMgr.SceneType.BattleScene)
        {
            if (targetType==GameMgr.SceneType.MapScene) return TransitionType.BattleToMap;
        }
        return TransitionType.None;
    }

}
