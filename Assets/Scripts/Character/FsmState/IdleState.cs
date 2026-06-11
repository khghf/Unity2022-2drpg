using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : CharacterState
{
    public override void OnEnter()
    {
        base.OnEnter();
        SpineAnimationState.SetAnimation(0, CharacterFsmBlackboard.anims.Idle.name, true);
    }
}
