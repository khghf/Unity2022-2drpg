using GFW.GameAbilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [SerializeField]
    private Collider2D damageCollider = null;


    private HashSet<Collider2D> triggeredColliders= new HashSet<Collider2D>();

    //在一次启用碰撞的过程中可能会对同一目标产生多次OnTriggerEnter2D
    //OnlyTriggerOnce可以控制对同一目标只产生一次OnTriggerEnter2D回调
    public bool OnlyTriggerOnce = true;
    private void Start()
    {
        Enable(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnlyTriggerOnce&&triggeredColliders.Contains(collision)) return;
        triggeredColliders.Add(collision);
        //onTriggerEnter?.Invoke(collision);
        OnHit(collision.gameObject);
    }

    private void OnHit(GameObject gameObject)
    {
        if (!(gameObject.CompareTag("Hero")||gameObject.CompareTag("Enemy"))) return;
        GameAbilityComponent  abilityComponent = gameObject.GetComponent<GameAbilityComponent>();

        //Type type=typeof(Hit);
        //AbilityHandle handle = abilityComponent.GetHandle(type);
        //handle.context.AbilityTrigger=this.gameObject.transform.root.gameObject;
        abilityComponent.ActiveGameAbility<Hit>();
    }


    public void Enable(bool isEnable)
    {
        if (damageCollider.enabled==isEnable) return;
        damageCollider.enabled = isEnable;
        if (isEnable==false) Reset();
    }
    public void Reset()
    {
        triggeredColliders.Clear();
    }
}
