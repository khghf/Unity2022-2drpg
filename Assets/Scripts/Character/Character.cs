using GFW.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : GamePawn
{
    public string Name = "DefaultCharacterName";
    //角色的中心点，角色的移动都以此为基础
    public GameObject Pivot = null;

    public Vector3 WorldPos
    {
        get
        {
            return Pivot.transform.position;
        }
        set
        {
            gameObject.transform.position=gameObject.transform.position+(value-Pivot.transform.position);
        }
    }
    public void MoveTo(Vector2 pos)
    {
        MoveTo(pos.x, pos.y);
    }
    public abstract void MoveTo(float posx, float posy);
}
