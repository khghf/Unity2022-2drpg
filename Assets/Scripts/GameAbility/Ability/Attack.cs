using GFW.GameAbilitySystem.NGameAbility;
public class Attack : GameAbility
{
    private BattleCharacter Character;

    private DamageCollider[] DamageCollider;
    public override void OnActive()
    {
        base.OnActive();
        Character.Fsm.ChangeState<CommonAttackState>();
        EnableCollider(true);
        Character.Fsm.CurState.OnExitHook+=() => { EndAbility(); };
    }

    public override void OnEnd()
    {
        base.OnEnd();
        EnableCollider(false);
    }

   /* public override void OnAdded()
    {
        base.OnAdded();
        Character=OwnerComponent.Owner.GetComponent<Character>();
        DamageCollider=OwnerComponent.Owner.GetComponentsInChildren<DamageCollider>();
    }*/

    private void EnableCollider(bool isEnable)
    {
        foreach(DamageCollider collider in DamageCollider)
        {
            collider?.Enable(isEnable);
        }
    }
}
