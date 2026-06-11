using GFW.GameAbilitySystem;
using GFW.Gameplay;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class BattlePlayerControl : GameCharacterController
{
    [Serializable]
    class DragIndicator
    {
        [Header("拖拽指示线设置")]
        [SerializeField]
        private LineRenderer dragLineIndicator = null; //拖拽指示线
        public float indicatorLineWidth = 0.1f;         //指示线宽度

        [Header("拖拽地标指示")]
        [SerializeField]
        private GameObject dragStartIndicator = null;  //拖拽地面指示-起点
        [SerializeField]
        private GameObject dragEndIndicator = null;    //拖拽地面指示-终点

        private Collider2D[] HoveredObjs = new Collider2D[5];

        public LayerMask DefaultHoverableLayer;
        public void Init()
        {
            //初始化拖拽指示线
            dragLineIndicator.positionCount = 2;
            dragLineIndicator.startWidth = indicatorLineWidth;
            dragLineIndicator.endWidth = indicatorLineWidth;
            dragLineIndicator.enabled = false;
            dragEndIndicator.GetComponent<SpriteRenderer>().material.SetFloat("_BreathAmount", 0);
        }


        public Collider2D GetClosetHoverdObj(Vector2 point,LayerMask layerMask)
        {
            int count = Physics2D.OverlapPointNonAlloc(point, HoveredObjs, DefaultHoverableLayer);
            Collider2D res= GetClosetObj(point, in HoveredObjs, count);
            return res;
        }

        private Collider2D GetClosetObj(Vector2 point, in Collider2D[] collider2Ds, int count)
        {
            Collider2D closest = null;
            float minDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider2D col = collider2Ds[i];
                if (col == null) continue;
                BattleCharacter character = col.gameObject.GetComponent<BattleCharacter>();
                float dist = Vector2.Distance(point, character.GetPivotPos());
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = col;
                }
            }

            return closest;
        }

        /// <summary>
        /// 设置拖拽指示器起点
        /// </summary>
        /// <param name="pos"></param>
        public void SetDragIndicatorStartPos(Vector2 pos)
        {
            dragStartIndicator.transform.position = pos;
            dragLineIndicator.SetPosition(0, pos);
        }
        /// <summary>
        /// 设置拖拽知识器终点
        /// </summary>
        /// <param name="pos"></param>
        public void SetDragIndicatorEndPos(Vector2 pos)
        {
            dragEndIndicator.transform.position = pos;
            dragLineIndicator.SetPosition(1, pos);
        }
        /// <summary>
        /// 设置拖拽指示器起点和终点
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SetDragIndicatorStartAndEndPos(Vector2 start,Vector2 end)
        {
            dragStartIndicator.transform.position = start;
            dragEndIndicator.transform.position = end;
            dragLineIndicator.SetPosition(0, start);
            dragLineIndicator.SetPosition(1, end);
        }
        /// <summary>
        /// 设置拖拽指示器起点的父对象
        /// </summary>
        /// <param name="parent"></param>
        public void SetDragIndicatorStartParent(Transform parent)
        {
            dragStartIndicator.transform.SetParent(parent, false);
        }
        /// <summary>
        /// 设置拖拽指示器终点的父对象
        /// </summary>
        /// <param name="parent"></param>
        public void SetDragIndicatorEndParent(Transform parent)
        {
            dragEndIndicator.transform.SetParent(parent, false);
        }
        /// <summary>
        /// 控制拖拽指示器起点的显隐
        /// </summary>
        /// <param name="enable"></param>
        public void EnableDragIndicatorStart(bool enable)
        {
            dragStartIndicator.SetActive(enable);
        }
        /// <summary>
        /// 控制拖拽指示器终点的显隐
        /// </summary>
        /// <param name="enable"></param>
        public void EnableDragIndicatorEnd(bool enable)
        {
            dragEndIndicator.SetActive(enable);
        }

        public void OnDragStart()
        {
            dragStartIndicator.SetActive(true);
            dragLineIndicator.enabled=true;
            dragEndIndicator.SetActive(true);
        }
        public void OnDragEnd()
        {
            //dragStartIndicator.SetActive(false);
            dragLineIndicator.enabled=false;
            dragEndIndicator.SetActive(false);
        }
    }


    public InputContext inputContext;
    private InputAction clickAction;

    private Coroutine dragCoroutineHandle = null;   //拖拽协程
    private bool isDragging=false;
    private GameObject selectedTarget=null;         //选中目标
    private GameObject preSelectedTarget=null;      //前一个选中目标
    private GameObject drageTarget=null;            //拖拽目标

    private BattleCharacter drageCharacter = null;        //拖拽目标的Character组件

    private string drageTargetTag = "Hero";         //拖拽目标需要具备的tag


    private Vector3 pressPos;                       //拖拽起点
    private PlayerHUD playerHUD;                    //玩家头显

    private Camera mainCamera = null;               //主相机

    [SerializeField]
    private DragIndicator indicator;

    private void Awake()
    {
        inputContext=new InputContext();
        clickAction=inputContext.BattlePlayerController.Click;
        clickAction.started+=OnTouchStarted;
        clickAction.canceled+=OnTouchCancaled;
        mainCamera=Camera.main;
    }

    private void Start()
    {
        playerHUD=(PlayerHUD)Hud;
        playerHUD.OnSkillButtonClicked+=(AbilityWarp abilityWarp) => 
        {
            selectedTarget.GetComponent<GameAbilityComponent>().ActiveGameAbility(abilityWarp.Ability.GetType());
        };
        //初始化拖拽指示线

        indicator?.Init();
       
    }
    private void OnEnable()
    {
        inputContext.BattlePlayerController.Enable();
    }

    private void OnDisable()
    {
        inputContext.BattlePlayerController.Disable();
    }

    private void Update()
    {
        if (!isDragging&&clickAction.IsPressed()&&Input.mousePosition!=pressPos&&drageTarget!=null)
        {
            dragCoroutineHandle=StartCoroutine(DragCoroutine());
        }
    }

    private void OnTouchStarted(InputAction.CallbackContext callbackContext)
    {
        //Debug.Log("OnTouchStarted");
        pressPos=Input.mousePosition;
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,1000,indicator.DefaultHoverableLayer);
        if (hit.collider != null)
        {
            preSelectedTarget=selectedTarget;
            selectedTarget =hit.collider.gameObject;
            drageTarget = selectedTarget;
            if (preSelectedTarget!=selectedTarget)
            {
                if (!selectedTarget.CompareTag(drageTargetTag)) return;
                drageCharacter=selectedTarget.GetComponent<BattleCharacter>();
                playerHUD.OnControlledHeroChanged((Hero)drageCharacter);

                indicator?.EnableDragIndicatorStart(true);
                indicator?.SetDragIndicatorStartParent(drageCharacter.Pivot.transform);
            }
        }
    }

    
    private void OnTouchCancaled(InputAction.CallbackContext callbackContext)
    {
        //Debug.Log("OnTouchCancaled");

        if (dragCoroutineHandle!=null)
        {
            StopCoroutine(dragCoroutineHandle);
            dragCoroutineHandle=null;
            OnDraggingStop();
        }
        isDragging=false;
        indicator?.EnableDragIndicatorEnd(false);
    }

    IEnumerator DragCoroutine()
    {
        OnDraggingStart();
        while (isDragging)
        {
            OnDragging();
            yield return null;
        }
    }

    private void OnDraggingStart()
    {
        //绘制拖拽线条
        if (drageTarget != null && drageTarget.CompareTag(drageTargetTag))
        {
            indicator?.OnDragStart();
            indicator?.SetDragIndicatorStartPos(drageCharacter.GetPivotPos());
        }
        isDragging =true;
        //Debug.Log("OnDraggingStart");
    }

    private void OnDragging()
    {
        //Debug.Log("OnDragging");
        if (isDragging && drageTarget != null)
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            indicator?.SetDragIndicatorStartPos(drageCharacter.GetPivotPos());
            Collider2D hoveredobj = indicator?.GetClosetHoverdObj(mouseWorldPos, drageCharacter.AttackTargetLayer);
            if (hoveredobj!=null)
            {
                mouseWorldPos=hoveredobj.gameObject.GetComponent<BattleCharacter>().GetPivotPos();
            }
            indicator?.SetDragIndicatorEndPos(mouseWorldPos);
        }
    }

    private void OnDraggingStop()
    {
        //Debug.Log("OnDraggingStop");
        indicator?.OnDragEnd();
        if (drageTarget==null) return;
        drageTarget.GetComponent<BattleCharacter>().MoveTo(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        drageTarget=null;
    }

    private void OnDestroy()
    {
        inputContext?.BattlePlayerController.Disable();
    }
}
