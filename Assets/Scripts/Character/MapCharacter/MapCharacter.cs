using DG.Tweening;
using GFW.Fsm;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
/// <summary>
/// 普通地图场景下的角色
/// </summary>
public class MapCharacter : Character
{
    public Vector2Int logicCellpos => GridMgr.WorldPosToLogicCellXY(WorldPos);
    /// <summary>
    /// 人物移动速度(走过一个格子所需要的时间)
    /// </summary>
    public float moveSpeed = 3;
    Tween moveTween = null;
    /// <summary>
    /// 移动到指定的逻辑网格
    /// </summary>
    /// <param name="posx">逻辑网格行</param>
    /// <param name="posy">逻辑网格列</param>
    public override void MoveTo(float posx, float posy)
    {
        List<Vector2Int> path = AStarPathfinder.FindPath(GridMgr.Inst, logicCellpos, new Vector2Int((int)posx, (int)posy));

        if (path != null && path.Count > 0)
        {
            moveTween?.Kill();

            Sequence sequence = DOTween.Sequence();
            Vector3 lastPos = transform.position;
            Vector2Int currentGrid = logicCellpos;

            for (int i = 0; i < path.Count; i++)
            {
                Vector2Int targetGrid = path[i];
                if (targetGrid == currentGrid) continue;

                Vector3 targetWorldPos = GridMgr.LogicCellXYToWorldPos(targetGrid);
                float distance = Vector3.Distance(lastPos, targetWorldPos);
                float duration = distance / (GridMgr.GridCellSize*moveSpeed);

                // 添加移动动画
                sequence.Append(transform.DOMove(targetWorldPos, duration).SetEase(Ease.Linear));

                // 捕获当前格子索引
                int capturedIndex = i;
                Vector2Int capturedGrid = targetGrid;

                // 添加到达回调
                sequence.AppendCallback(() => {
                    OnReachGrid(capturedGrid);
                    Debug.Log($"到达格子: {capturedGrid}, 索引: {capturedIndex}");
                });

                lastPos = targetWorldPos;
                currentGrid = targetGrid;
            }

            moveTween = sequence;
            moveTween.OnComplete(() => {
                moveTween?.Kill();
                moveTween = null;
                Debug.Log("路径移动完成");
            });
        }
        else
        {
            Debug.LogWarning($"寻路失败: 从 {logicCellpos} 到 ({posx}, {posy})");
        }
    }

    // 到达格子的事件
    private void OnReachGrid(Vector2Int gridPos)
    {
        Debug.Log($"到达格子: {gridPos}");
        // 这里可以触发其他逻辑，如触发陷阱、获取物品等
    }
   
    /// <summary>
    /// 设置位置到指定的逻辑网格
    /// </summary>
    /// <param name="pos">逻辑网格行列</param>
    public void SetPosTo(Vector2Int pos)
    {
        SetPosTo(pos.x, pos.y);
    }
    /// <summary>
    /// 设置位置到指定的逻辑网格
    /// </summary>
    /// <param name="x">行</param>
    /// <param name="y">列</param>
    public void SetPosTo(int x,int y)
    {
        WorldPos=GridMgr.LogicCellXYToWorldPos((int)x, (int)y);
    }
}
