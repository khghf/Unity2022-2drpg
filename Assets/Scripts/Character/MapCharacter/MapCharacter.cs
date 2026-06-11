using GFW.Fsm;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
/// <summary>
/// 普通地图场景下的角色
/// </summary>
public class MapCharacter : Character
{
    public Vector2Int pos=new Vector2Int(0,0);
    /// <summary>
    /// 人物移动速度(走过一个格子所需要的时间)
    /// </summary>
    public float moveSpeed = 1;
    private Coroutine _moveToCoroutineHandle =null;

    public override void MoveTo(float posx, float posy)
    {
        List<Vector2Int>path= AStarPathfinder.FindPath(GridMgr.Inst, pos, new Vector2Int((int)posx, (int)posy));
        _moveToCoroutineHandle=StartCoroutine(MoveToCoroutine(path));
    }
    private IEnumerator MoveToCoroutine(List<Vector2Int> path)
    {

        foreach(var pathitem in path)
        {
            Vector3 targetPos=GridMgr.LogicCellXYToWorldPos(pathitem);
            Vector3 curPos = WorldPos;
            curPos.z=0;
            Vector3 dir = targetPos-curPos;
            float distance = dir.magnitude;
            while (distance>0)
            {
                float movedDis = moveSpeed*Time.deltaTime;
                this.transform.position = this.transform.position+dir*movedDis;
                distance-=movedDis;
                //Debug.Log("Move"+movedDis);
                yield return null;
            }
            _moveToCoroutineHandle=null;
        }
    }
    public void SetPosTo(Vector2Int pos)
    {
        SetPosTo(pos.x, pos.y);
    }
    public void SetPosTo(int x,int y)
    {
        WorldPos=GridMgr.LogicCellXYToWorldPos((int)x, (int)y);
    }
}
