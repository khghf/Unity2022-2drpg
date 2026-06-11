using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonAttackState : CharacterState
{
    public override void OnEnter()
    {
        base.OnEnter();
        TrackEntry entry=SpineAnimationState.SetAnimation(0, CharacterFsmBlackboard.anims.CommonAttack.name, false);

        entry.Complete+=(TrackEntry trackEntry) => { ChangeState<IdleState>(); };
    }
}
