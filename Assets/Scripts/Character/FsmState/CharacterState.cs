using GFW.Fsm;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : FsmState
{
    protected SkeletonAnimation SkeletonAnimation=null;
    protected Spine.AnimationState SpineAnimationState;
    protected Spine.Skeleton SpineSkeleton;

    protected CharacterFsmBlackboard CharacterFsmBlackboard = null;
    //角色当前朝向，-1为向左，1为向右
    protected int CurOrientation = 1;
    public override void OnEnter()
    {
        base.OnEnter();
        //if(CharacterFsmBlackboard==null) CharacterFsmBlackboard=(CharacterFsmBlackboard)OwnerFsm.BlackboardInternal;
        if (SkeletonAnimation==null)
        {
            SkeletonAnimation=CharacterFsmBlackboard.SpineObject.GetComponent<SkeletonAnimation>();
            if (SkeletonAnimation==null)
            {
                Debug.LogError("获取SkeletonAnimation失败请检查角色状态机的黑板值是否设置正确");
                return;
            }
            SpineAnimationState=SkeletonAnimation.AnimationState;
            SpineSkeleton=SkeletonAnimation.Skeleton;
        }
    }
}
