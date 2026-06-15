using Cinemachine;
using GFW.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapPlayerController : GameCharacterController
{
    PlayerCharacter PlayerCharacter;
    InputContext inputContext;
    public CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float scaleZoomSensitive = 0.01f;
    private void OnEnable()
    {
        inputContext.MapPlayerController.Enable();
    }
    private void OnDisable()
    {
        inputContext.MapPlayerController.Disable();

    }
    private void Awake()
    {
        inputContext=new InputContext();

    }
    private void Start()
    {
        inputContext.MapPlayerController.Click.started+=OnClick;
        inputContext.MapPlayerController.ScaleZoom.performed+=OnScaleZoom;
    }


    private void OnClick(InputAction.CallbackContext callbackContext)
    {
        Camera camera=GameScene.Inst.GameCamera;
        Vector3 screenPos = Input.mousePosition;
        Vector3 worldPos=camera.ScreenToWorldPoint(screenPos);
        if(IsPointerOverUI(screenPos))
        {

        }
        else
        {
            var logicCellPos=GridMgr.WorldPosToLogicCellXY(worldPos);

            PlayerCharacter.MoveTo(logicCellPos);
        }
    }
    private void OnScaleZoom(InputAction.CallbackContext callbackContext)
    {
        Camera camera=GameScene.Inst.GameCamera;
        var lens = virtualCamera.m_Lens;
        float scrollValue = callbackContext.ReadValue<Vector2>().y;
        float targetZoom  = lens.OrthographicSize-scrollValue*scaleZoomSensitive;
        targetZoom = Mathf.Clamp(targetZoom, 5, 10);
        lens.OrthographicSize = targetZoom;
        virtualCamera.m_Lens = lens;
        Debug.Log($"targetZoom{targetZoom}");
    }
    protected override void OnControlledPawnChanged()
    {
        base.OnControlledPawnChanged();
        PlayerCharacter=target as PlayerCharacter;
    }

    /// <summary>
    /// 移动到指定的逻辑网格位置
    /// </summary>
    /// <param name="pos">行列</param>
    public void MoveTo(Vector2Int pos)
    {
        MoveTo(pos.x, pos.y);
    }
    /// <summary>
    /// 移动到指定的逻辑网格位置
    /// </summary>
    /// <param name="posx">行</param>
    /// <param name="posy">列</param>
    public void MoveTo(int posx,int posy)
    {
        PlayerCharacter.MoveTo(posx, posy);
    }


    /// <summary>
    /// 判断是否点击在 UI 上
    /// </summary>
    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}
