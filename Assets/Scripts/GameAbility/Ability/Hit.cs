using GFW.GameAbilitySystem.NGameAbility;
using UnityEngine;

public class Hit : GameAbility
{
    public override void OnActive()
    {
        base.OnActive();
        GameObject own =(GameObject) _Context.OwningGAC.Owner;
        Debug.Log($"{own.name}受到了攻击");
        EndAbility();
    }
}
