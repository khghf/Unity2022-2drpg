using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayerController : GameCharacterController
{
    PlayerCharacter playerCharacter = null;


    private void Start()
    {
        playerCharacter=Target as PlayerCharacter;

        if(playerCharacter==null)
        {
            Debug.LogError("[MapPlayerController]:未正确设置玩家角色(PlayerCharacter)");
        }
    }

    /// <summary>
    /// 移动到指定的逻辑网格位置
    /// </summary>
    /// <param name="pos">行列</param>
    public void MoveTo(Vector2Int pos)
    {
        MoveTo(pos.x, pos.y);
    }
    /// <summary>
    /// 移动到指定的逻辑网格位置
    /// </summary>
    /// <param name="posx">行</param>
    /// <param name="posy">列</param>
    public void MoveTo(int posx,int posy)
    {
        playerCharacter.MoveTo(posx, posy);
    }
}
