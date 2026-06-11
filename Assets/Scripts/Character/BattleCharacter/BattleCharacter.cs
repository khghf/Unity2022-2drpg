using GFW.Fsm;
using GFW.GameAbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗场景下角色
/// </summary>
public class BattleCharacter : Character
{
    
    private Coroutine _MoveToCoroutineHandle = null;

    [SerializeField]
    protected Fsm _Fsm = new Fsm();
    public Fsm Fsm => _Fsm;

    [HideInInspector]
    public GameAbilityComponent  AC = null;

    [HideInInspector]
    public CharacterAttribute _Attribute = null;


    [HideInInspector]
    public List<AbilityWarp> ActiveAbilities = null;

    [SerializeField]
    private CharacterFsmBlackboard _FsmBlackboard = null;

    [SerializeField]
    private LayerMask _AttackTargetLayer;
    public LayerMask AttackTargetLayer=>_AttackTargetLayer;

    private void Awake()
    {
        _Fsm.Owner=gameObject;
        _Fsm.SetBlackboard(_FsmBlackboard);
        _Fsm.AddState<IdleState>();
        _Fsm.AddState<RunState>();
        _Fsm.AddState<CommonAttackState>();
        _Fsm.SetEntryState<IdleState>();
        _Fsm.Run();
    }

    private void Start()
    {
        gameObject.layer=LayerMask.NameToLayer("Character");
        AC=gameObject.GetComponent<GameAbilityComponent >();
        if (AC==null) AC=gameObject.AddComponent<GameAbilityComponent >();
        var initlizer = GetComponent<BattleCharacterInitializer>();
        initlizer.Init(this);
        Destroy(initlizer);
    }

    private void Update()
    {
        _Fsm.Update();
    }

    public override void MoveTo(float posx, float posy)
    {
        if (_MoveToCoroutineHandle!=null) StopCoroutine(_MoveToCoroutineHandle);
        _MoveToCoroutineHandle=StartCoroutine(MoveToCoroutine(new Vector3(posx,posy)));
    }

   
    public void StopMoveTo()
    {
        if (_MoveToCoroutineHandle!=null) StopCoroutine(_MoveToCoroutineHandle);
        _MoveToCoroutineHandle=null;
    }
    private IEnumerator MoveToCoroutine(Vector3 pos)
    {
        //计算方向以及距离
        Vector3 dir = pos-WorldPos;
        float distance = dir.magnitude;
        dir=dir.normalized;

        //改变为奔跑状态
        _Fsm.ChangeState<RunState>();
        while (distance>0)
        {
            float movedDis = _Attribute.MoveSpeed.CurValue*Time.deltaTime;
            this.transform.position = this.transform.position+dir*movedDis;
            distance-=movedDis;
            //Debug.Log("Move"+movedDis);
            yield return null;
        }
        _MoveToCoroutineHandle=null;

        //改变为空闲状态
        _Fsm.ChangeState<IdleState>();
    }
    public Vector3 GetPivotPos()
    {
        return Pivot.transform.position;
    }
}