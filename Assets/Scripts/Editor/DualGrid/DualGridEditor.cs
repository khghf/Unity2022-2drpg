#if UNITY_EDITOR

using Codice.Client.Common;
using NUnit.Framework;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;
namespace GridEditor
{
    [CustomEditor(typeof(GridMgr))]
    public class DualGridEditor : Editor
    {
        public static bool isEditMode = false;
        private static DualGridEditor inst;
        private static GridMgr gridMgr;
        private static FsmEditor fsmEditor;
        private static DualGridSettings globSettings;
        private void OnEnable()
        {
            inst=this;
            gridMgr =target as GridMgr;

           
           
            Undo.undoRedoPerformed += OnUndoRedo;
            Debug.Log("OnEnable");
        }
        private void OnDisable()
        {
            
            Undo.undoRedoPerformed -= OnUndoRedo;
            Debug.Log("OnDisable");
        }
        private void OnUndoRedo()
        {
            if (gridMgr == null) return;
            for (int x = 0; x < GridMgr.LogicGridResolution; ++x)
            {
                for (int y = 0; y < GridMgr.LogicGridResolution; ++y)
                {
                    gridMgr.UpdateShowCell(x, y);
                }
            }
            Repaint();
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Grid Editor/切换双网格编辑模式 %g")]
        public static void ToggleEditMode()
        {
            isEditMode = !isEditMode;
            SceneView.duringSceneGui -= OnDuringSceneGui;
            if (!Application.isEditor) return;

            if (isEditMode)
            {
                SceneView.duringSceneGui += OnDuringSceneGui;
                gridMgr = Object.FindObjectOfType<GridMgr>();
                if (gridMgr == null)
                {
                    GameObject go = new GameObject("GridDataSystem");
                    go.transform.position = Vector3.zero;
                    gridMgr = go.AddComponent<GridMgr>();
                    gridMgr.Init();
                }
                else
                {
                    gridMgr.transform.position = Vector3.zero;
                }
                Selection.activeObject = gridMgr;

                if (fsmEditor==null)
                {
                    fsmEditor=new FsmEditor();

                    fsmEditor.Blackboard.AddItem("GridManagerEditor", inst);
                    fsmEditor.Blackboard.AddItem("GridMgr", gridMgr);
                    fsmEditor.AddState<DefaultState>();
                    fsmEditor.AddState<PaintState>();
                    fsmEditor.AddState<LogicCellEditorState>();
                    fsmEditor.SetEntryState<DefaultState>();
                    fsmEditor.Run();

                    globSettings=fsmEditor.AddState<DualGridSettings>();
                    fsmEditor.RemoveState<DualGridSettings>();
                }


                fsmEditor?.ChangeState<DefaultState>();
                Debug.Log("<b>[Grid Editor]</b> 已开启双网格编辑模式。\n<b>操作说明：</b>双击左键选物体拖拽，双击空白处恢复涂抹，右键选格子编辑数据！");
            }
            else
            {
                Debug.Log("<b>[Grid Editor]</b> 已关闭网格编辑模式。");

                //fsmEditor.Blackboard.RemoveItem("GridManagerEditor");
                //fsmEditor.Blackboard.RemoveItem("GridMgr");
                //fsmEditor.Blackboard.RemoveItem("SerializedObject");
                fsmEditor=null;
                globSettings=null;
            }

            //if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.Repaint();
        }
        private static void OnDuringSceneGui(SceneView sceneView)
        {
            
            if (!isEditMode) return;
            globSettings?.OnSceneGUI();
            // 获取当前事件
            Event e = Event.current;

            // 1. 锁定 Scene 视图的焦点，防止按键被其它面板抢占
            //int controlID = GUIUtility.GetControlID(FocusType.Passive);
            //HandleUtility.AddDefaultControl(controlID);

            // 2. 监听纯字母按键 (不带 Ctrl / Alt)
            if (e != null && e.isKey && !e.control && !e.alt)
            {
                if (e.keyCode == KeyCode.B)
                {
                    // 确保只在按下时触发一次状态切换
                    if (e.type == EventType.KeyDown)
                    {
                        Selection.activeObject=gridMgr;
                        fsmEditor?.ChangeState<PaintState>();
                    }
                    // 无论 KeyDown 还是 KeyUp 都将事件吃掉，防止向下传递
                    e.Use();
                }
                else if (e.keyCode == KeyCode.L)
                {
                    if (e.type == EventType.KeyDown)
                    {
                        Selection.activeObject=gridMgr;
                        fsmEditor?.ChangeState<LogicCellEditorState>();
                    }
                    e.Use();
                }
                else if(e.keyCode == KeyCode.G)
                {
                    if (e.type == EventType.KeyDown)
                    {
                        Selection.activeObject=gridMgr;
                        fsmEditor?.ChangeState<DefaultState>();
                    }
                    e.Use();
                }
            }

            fsmEditor?.OnSceneGUI();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            globSettings?.OnInspectorGUI();
            fsmEditor?.OnInspectorGUI();

            if (GUI.changed) SceneView.RepaintAll();
        }

        /// <summary>
        /// 绘制网格
        /// </summary>
        /// <param name="topLeft">绘制起点</param>
        /// <param name="worldGridSize">网格单元世界大小</param>
        /// <param name="GridResolution">网格单元数量GridResolution*GridResolution</param>
        public static void DrawGridLines(Vector3 topLeft, float worldGridSize, int GridResolution)
        {
            float gridSideLength = worldGridSize * GridResolution;
            for (int i = 0; i <= GridResolution; ++i)
            {
                float offset = i * worldGridSize;
                Handles.DrawLine(topLeft + new Vector3(offset, 0, 0), topLeft + new Vector3(offset, -gridSideLength, 0));
                Handles.DrawLine(topLeft + new Vector3(0, -offset, 0), topLeft + new Vector3(gridSideLength, -offset, 0));
            }
        }
        public static void DrawLogicMark()
        {
            Vector3 pos = Vector3.zero;
            float gridCellHalfSize = GridMgr.GridCellSize/2f;
            pos.z=20;
            for (int row = 0; row<GridMgr.LogicGridResolution; ++row)
            {
                pos.y=GridMgr.LogicGridTopLeft.y-gridCellHalfSize-row*GridMgr.GridCellSize;
                for (int col = 0; col <GridMgr.LogicGridResolution; ++col)
                {
                    pos.x=GridMgr.LogicGridTopLeft.x+gridCellHalfSize+col*GridMgr.GridCellSize;
                    Handles.Label(pos, $"{gridMgr.GetLogicCellMark(row, col)}");
                }
            }
        }
        public static void DrawShowTileIndex()
        {
            Vector3 pos = Vector3.zero;
            float gridCellHalfSize = GridMgr.GridCellSize/2f;
            pos.z=20;
            for (int row = 0; row<GridMgr.ShowGridResolution; ++row)
            {
                pos.y=GridMgr.ShowGridTopLeft.y-gridCellHalfSize-row*GridMgr.GridCellSize;
                for (int col = 0; col <GridMgr.ShowGridResolution; ++col)
                {
                    pos.x=GridMgr.ShowGridTopLeft.x+gridCellHalfSize+col*GridMgr.GridCellSize;
                    Handles.Label(pos, $"{gridMgr.ShowCells[GridMgr.ShowCellXYToIndex(row, col)].TileIndex}");
                }
            }
        }

        public static void HilightLogicCell(Vector2Int pos, Color color)
        {
            HilightLogicCell(pos.x,pos.y,color);    
        }

        public static void HilightLogicCell(int logicCellX, int logicCellY,Color color)
        {
            if (logicCellX<0||logicCellX>=GridMgr.LogicGridResolution) return;
            if (logicCellY<0||logicCellY>=GridMgr.LogicGridResolution) return;
            int row = logicCellX;
            int col = logicCellY;

            float leftX = GridMgr.LogicGridTopLeft.x + col * GridMgr.GridCellSize;
            float topY = GridMgr.LogicGridTopLeft.y - row * GridMgr.GridCellSize;

            Vector3[] verts = new Vector3[]
            {
            new Vector3(leftX, topY - GridMgr.GridCellSize, 0),
            new Vector3(leftX, topY, 0),
            new Vector3(leftX + GridMgr.GridCellSize, topY, 0),
            new Vector3(leftX + GridMgr.GridCellSize, topY - GridMgr.GridCellSize, 0)
            };

            Handles.color = color;
            Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 0f, 0f, 1f), Color.red);
        }


       
    }
}
#endif