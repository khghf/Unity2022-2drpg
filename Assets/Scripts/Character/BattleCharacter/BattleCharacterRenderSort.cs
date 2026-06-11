using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 通过改变z值来决定角色的渲染顺序
/// </summary>
public class BattleCharacterRenderSort : MonoBehaviour
{
    public Transform rootTransform;
    public Transform pivot;
    [HideInInspector]
    public Vector3 newPos=Vector3.zero;

    private void Update()
    {
        newPos=rootTransform.position;
        newPos.z=pivot.position.y;
        rootTransform.position=newPos;
    }
}
