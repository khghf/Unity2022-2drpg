using GFW.Fsm;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : CharacterState
{
    //上一帧的位置的x值用于判断左右运动
    private float prePosX = 0f;
    public override void OnEnter()
    {
        base.OnEnter();
        SpineAnimationState.SetAnimation(0, CharacterFsmBlackboard.anims.Run.name, true);
        prePosX=CharacterFsmBlackboard.SpineObject.transform.position.x;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        float curPosX = CharacterFsmBlackboard.SpineObject.transform.position.x;
        float deltaPosX = curPosX-prePosX;
        //Debug.Log("deltaPosX:"+deltaPosX);

        if (deltaPosX==0) return;

        //角色朝向和运动方向相反，改变朝向
        if(deltaPosX*CurOrientation<0)
        {
            CurOrientation*=-1;
            SpineSkeleton.ScaleX*=-1;
            //Debug.Log("flip");
        }
        prePosX=curPosX;
    }
    public override void OnExit()
    {
        base.OnExit();
        OwnerFsm.Owner.GetComponent<BattleCharacter>().StopMoveTo();
    }
}
