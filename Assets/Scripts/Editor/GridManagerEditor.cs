using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.Image;

[CustomEditor(typeof(GridMgr))]
public class GridManagerEditor : Editor
{
    private static GridManagerEditor instance;
    public static bool isEditMode = false;
    public static bool isSnapEnabled = true;

    public static bool isDrawGrid = true;
    public static bool isDrawLogicGrid = true;
    public static bool isDrawShowGrid = false;

    public static bool isDrawNum = false;
    public static bool isDrawLogicMark = true;
    public static bool isDrawShowTileIndex = true;

    private static GridMgr currentManager;

    private static Vector2Int SelectedLogicCell = new Vector2Int(-1, -1);

    private static int lastHotControl = 0;

    static Color LogicGirdCellColor = new Color(0.0f, 1f, 0.0f, 1f);
    static Color ShowGirdColor = new Color(1f, 0f, 0f, 1f);



    private void OnEnable()
    {
        instance = this;
        isSnapEnabled = EditorPrefs.GetBool("GridEditor_AutoSnap", true);
        currentManager = (GridMgr)target;
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnDisable()
    {
        instance = null;
        Undo.undoRedoPerformed -= OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        if (currentManager == null) return;

        for (int x = 0; x < GridMgr.LogicGridResolution; ++x)
        {
            for (int y = 0; y < GridMgr.LogicGridResolution; ++y)
            {
                currentManager.UpdateShowCell(x, y);
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
            currentManager = Object.FindObjectOfType<GridMgr>();
            if (currentManager == null)
            {
                GameObject go = new GameObject("GridDataSystem");
                go.transform.position = Vector3.zero;
                currentManager = go.AddComponent<GridMgr>();
                currentManager.Init();
            }
            else
            {
                currentManager.transform.position = Vector3.zero;
            }
            Selection.activeObject = currentManager;
            Debug.Log("<b>[Grid Editor]</b> 已开启双网格编辑模式。\n<b>操作说明：</b>双击左键选物体拖拽，双击空白处恢复涂抹，右键选格子编辑数据！");
        }
        else
        {
            SelectedLogicCell=new Vector2Int(-1, -1);
            Debug.Log("<b>[Grid Editor]</b> 已关闭网格编辑模式。");
        }

        if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.Repaint();
    }

    private static void OnDuringSceneGui(SceneView sceneView)
    {
        if (!isEditMode) return;

        if (currentManager == null)
        {
            currentManager = Object.FindObjectOfType<GridMgr>();
            if (currentManager == null) return;
        }

        Vector3 origin = currentManager.transform.position;
        HilightLogicCell(SelectedLogicCell.x,SelectedLogicCell.y);
        if (isDrawGrid)
        {
            Handles.color = LogicGirdCellColor;
            if (isDrawLogicGrid) DrawGridLines(GridMgr.LogicGridTopLeft, GridMgr.GridCellSize, GridMgr.LogicGridResolution);
            Handles.color = ShowGirdColor;
            if (isDrawShowGrid) DrawGridLines(GridMgr.ShowGridTopLeft, GridMgr.GridCellSize, GridMgr.ShowGridResolution);
        }

        if (isDrawNum)
        {
            if (isDrawLogicMark) DrawLogicMark();
            if (isDrawShowTileIndex) DrawShowTileIndex();
        }

        // =========================================================
        // 拖拽吸附逻辑
        // =========================================================
        int currentHotControl = GUIUtility.hotControl;
        if (lastHotControl != 0 && currentHotControl == 0)
        {
            if (isSnapEnabled)
            {
                SnapSelectedObjects(GridMgr.LogicGridTopLeft, GridMgr.GridCellSize);
            }
        }
        lastHotControl = currentHotControl;

        HandleMouseInput();
    }

    private static void HandleMouseInput()
    {
        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(controlID);

        // 1. 右键单击：高亮编辑逻辑网格
        if (e.type == EventType.MouseDown && e.button == 1)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Plane plane = new Plane(Vector3.forward, Vector3.zero);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 worldPos = ray.GetPoint(distance);
                Vector2Int logicPos = GridMgr.WorldPosToLogicCellXY(worldPos);

                if (logicPos.x >= 0 && logicPos.x < GridMgr.LogicGridResolution && logicPos.y >= 0 && logicPos.y < GridMgr.LogicGridResolution)
                {
                    SelectedLogicCell = logicPos;
                    if (instance != null) instance.Repaint();
                    if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.Repaint();
                    return;
                }
            }
        }
        // 左键双击：选择场景物体

        if (e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 2)
        {
            GameObject pickedObject = HandleUtility.PickGameObject(e.mousePosition, false);

            // 如果点到了场景里的普通物体
            if (pickedObject != null && pickedObject != currentManager.gameObject)
            {

                if(pickedObject.TryGetComponent<ShowGridCell>(out _))
                {
                    // 如果点到了空白处，重新选中网格管理器，恢复涂抹模式
                    Selection.activeGameObject = currentManager.gameObject;
                    e.Use();
                    if (instance != null) instance.Repaint();
                    if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.Repaint();
                    return;
                }

                Selection.activeGameObject = pickedObject;
                e.Use();
                if (instance != null) instance.Repaint();
                if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.Repaint();
                return;
            }
        }

      
        // 检查鼠标是否在操控柄（Move工具的箭头）上，或者正在拖动操控柄
        bool isHoveringHandle = HandleUtility.nearestControl != controlID;
        bool isDraggingHandle = GUIUtility.hotControl != 0;

        if (isHoveringHandle || isDraggingHandle)
        {
            return; // 将控制权完全还给 Unity 的移动工具，不执行涂抹
        }

        // 如果用户当前选中的不是网格管理器
        // 强制暂停涂抹，直到他重新双击空白处
        if (Selection.activeGameObject != currentManager.gameObject)
        {
            return;
        }

        
        GridLayer selectedLayer = currentManager.GetSelectedLayer();
        if (selectedLayer == null) return;

        if (selectedLayer.Tiles.Length != 16)
        {
            if (e.type == EventType.MouseDown && e.button == 0)
                Debug.LogWarning("当前选择的瓦片不满足16SetTiles标准，请设置好后再涂抹");
            return;
        }

        // 检测左键拖抹
        if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0 && !e.alt)
        {
            bool isErasing = e.control;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Plane plane = new Plane(Vector3.forward, Vector3.zero);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 worldPos = ray.GetPoint(distance);

                Vector2Int logicPos = GridMgr.WorldPosToLogicCellXY(worldPos);

                if (logicPos.x >= 0 && logicPos.x < GridMgr.LogicGridResolution && logicPos.y >= 0 && logicPos.y < GridMgr.LogicGridResolution)
                {
                    Undo.RecordObject(currentManager, "Paint Grid");
                    currentManager.SetLogicCellMark(logicPos.x, logicPos.y, !isErasing);
                    e.Use(); // 消耗掉该事件
                }
            }
        }
    }


    /// <summary>
    /// 绘制网格
    /// </summary>
    /// <param name="topLeft">绘制起点</param>
    /// <param name="worldGridSize">网格单元世界大小</param>
    /// <param name="GridResolution">网格单元数量GridResolution*GridResolution</param>
    private static void DrawGridLines(Vector3 topLeft, float worldGridSize, int GridResolution)
    {
        float gridSideLength = worldGridSize * GridResolution;
        for (int i = 0; i <= GridResolution; ++i)
        {
            float offset = i * worldGridSize;
            Handles.DrawLine(topLeft + new Vector3(offset, 0, 0), topLeft + new Vector3(offset, -gridSideLength, 0));
            Handles.DrawLine(topLeft + new Vector3(0, -offset, 0), topLeft + new Vector3(gridSideLength, -offset, 0));
        }
    }
    private static void DrawLogicMark()
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
                Handles.Label(pos, $"{currentManager.GetLogicCellMark(row, col)}");
            }
        }
    }
    private static void DrawShowTileIndex()
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
                Handles.Label(pos, $"{currentManager.ShowCells[GridMgr.ShowCellXYToIndex(row, col)].TileIndex}");
            }
        }
    }

    private static void HilightLogicCell(int logicCellX,int logicCellY)
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

        Handles.color = new Color(0f, 1f, 0f, 0.4f);
        Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 0f, 0f, 1f), Color.red);
    }


    private static void SnapSelectedObjects(Vector3 gridTopLeft, float cellSize)
    {
        if (Selection.transforms.Length == 0) return;

        foreach (Transform t in Selection.transforms)
        {
            if (currentManager != null && t == currentManager.transform) continue;

            Vector3 originalPos = t.position;
            Vector3 newPos = originalPos;

            SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Vector3 minPos = sr.bounds.min;
                float snapMinX = Mathf.Round((minPos.x - gridTopLeft.x) / cellSize) * cellSize + gridTopLeft.x;
                float snapMinY = Mathf.Round((minPos.y - gridTopLeft.y) / cellSize) * cellSize + gridTopLeft.y;
                newPos.x += (snapMinX - minPos.x);
                newPos.y += (snapMinY - minPos.y);
            }
            else
            {
                float halfCell = cellSize / 2f;
                float snapX = Mathf.Round((originalPos.x - gridTopLeft.x - halfCell) / cellSize) * cellSize + gridTopLeft.x + halfCell;
                float snapY = Mathf.Round((originalPos.y - gridTopLeft.y - halfCell) / cellSize) * cellSize + gridTopLeft.y + halfCell;
                newPos.x = snapX;
                newPos.y = snapY;
            }

            if (Vector3.Distance(originalPos, newPos) > 0.001f)
            {
                Undo.RecordObject(t, "Snap to Grid");
                t.position = newPos;
            }
        }
    }


    SerializedProperty logicCellsProp = null;
    SerializedProperty layersProp = null;
    SerializedProperty tilesProp = null;
    SerializedProperty nameProp = null;
    public override void OnInspectorGUI()
    {
        if (currentManager == null) currentManager = (GridMgr)target;

        serializedObject.Update();



        GUILayout.Space(15);
        EditorGUILayout.LabelField("逻辑网格数据编辑(Logic Cell Data)", EditorStyles.boldLabel);

        if (SelectedLogicCell.x == -1 || SelectedLogicCell.y == -1)
        {
            EditorGUILayout.HelpBox("请在 Scene 视图中【右击】一个绿色格子以编辑其数据。", MessageType.Info);
        }
        else
        {
            int row = SelectedLogicCell.x;
            int col = SelectedLogicCell.y;
            EditorGUILayout.HelpBox($"当前选中逻辑格子: 第 {row} 行, 第 {col} 列", MessageType.None);

            int cellIndex = GridMgr.LogicCellXYToIndex(row, col);

            if (logicCellsProp==null) logicCellsProp = serializedObject.FindProperty("LogicCells");
            if (logicCellsProp != null && cellIndex >= 0 && cellIndex < logicCellsProp.arraySize)
            {
                SerializedProperty cellProp = logicCellsProp.GetArrayElementAtIndex(cellIndex);

                EditorGUILayout.BeginVertical("box");
                EditorGUI.BeginChangeCheck();

                SerializedProperty childProp = cellProp.Copy();
                SerializedProperty endProp = cellProp.GetEndProperty();

                bool enterChildren = true;
                while (childProp.NextVisible(enterChildren) && !SerializedProperty.EqualContents(childProp, endProp))
                {
                    EditorGUILayout.PropertyField(childProp, true);
                    enterChildren = false;
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();

                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox("无法获取网格数据：LogicCells 数组未初始化或索引越界。", MessageType.Warning);
            }
        }


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("网格层配置 (Tiles)", EditorStyles.boldLabel);

        if (layersProp==null) layersProp = serializedObject.FindProperty("GridLayers");

        for (int i = 0; i < layersProp.arraySize; i++)
        {
            SerializedProperty layerProp = layersProp.GetArrayElementAtIndex(i);
            if (tilesProp==null) tilesProp = layerProp.FindPropertyRelative("Tiles");
            if (nameProp==null) nameProp = layerProp.FindPropertyRelative("Name");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(nameProp);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(tilesProp, true);

            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log($"检测到第 {i} 层 ({nameProp.stringValue}) 的 Tiles 发生了改变！");
                currentManager.GridLayers[i].LayerIndex= i;
                currentManager.GridLayers[i].Name= currentManager.GridLayers[i].Tiles[0].name;
            }
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                layersProp.DeleteArrayElementAtIndex(i);
                break; // 删除后直接跳出，防止索引越界
            }
            EditorGUILayout.EndVertical();
        }
        if (GUILayout.Button("+ 添加图层"))
        {
            layersProp.arraySize++;
        }
        serializedObject.ApplyModifiedProperties();


        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("16-Set双网格编辑器", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();


        isDrawGrid= EditorGUILayout.BeginToggleGroup("绘制网格", isDrawGrid);

        isDrawLogicGrid = EditorGUILayout.Toggle("绘制逻辑网格", isDrawLogicGrid);
        isDrawShowGrid = EditorGUILayout.Toggle("绘制显示网格", isDrawShowGrid);

        EditorGUILayout.EndToggleGroup();


        isDrawNum= EditorGUILayout.BeginToggleGroup("绘制数字", isDrawNum);

        isDrawLogicMark=EditorGUILayout.Toggle("绘制逻辑网格单元标记数字", isDrawLogicMark);
        isDrawShowTileIndex=EditorGUILayout.Toggle("绘制显示网格单元瓦片索引", isDrawShowTileIndex);

        EditorGUILayout.EndToggleGroup();



        if (EditorGUI.EndChangeCheck())
        {

            EditorUtility.SetDirty(currentManager);
        }

        if (!isEditMode)
        {
            EditorGUILayout.HelpBox("按下 Ctrl + G (或 %g) 开启自动拼接网格绘制模式。", MessageType.Warning);
            return;
        }

        if (currentManager.GridLayers == null || currentManager.GridLayers.Count == 0)
        {
            EditorGUILayout.HelpBox("请先在上方网格层", MessageType.Error);
            return;
        }

        EditorGUILayout.LabelField("选择图层刷子:", EditorStyles.boldLabel);
        if (GUILayout.Button($"取消选择", GUILayout.Height(30)))
        {
            currentManager.SelectedLayerIndex = -1;
        }

        foreach (var gridLayer in currentManager.GridLayers)
        {
            bool isSelected = (currentManager.SelectedLayerIndex == gridLayer.LayerIndex);
            GUI.backgroundColor=isSelected ? Color.green : Color.white;
            if (GUILayout.Button($"{gridLayer.Name}", GUILayout.Height(30)))
            {
                currentManager.SelectedLayerIndex = gridLayer.LayerIndex;
            }
            GUI.backgroundColor = Color.white;
        }
        if (GUILayout.Button($"重新生成初始地图", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("重新生成初始地图", "确定要重新生成初始地图吗，这会清除所有数据包括瓦片布局,逻辑标记？", "确定", "取消"))
            {
                EditorApplication.delayCall += () =>
                {
                    if (currentManager != null) currentManager.ReGeneratorTileMap();
                };
            }
        }

        if (currentManager.SelectedLayerIndex!=-1)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("全覆盖-基础层"))
            {
                if (EditorUtility.DisplayDialog("全覆盖-基础层", "确定要覆盖吗？", "确定", "取消"))
                {
                    foreach (var logicGrid in currentManager.LogicCells)
                    {
                        logicGrid.Mark=-currentManager.SelectedLayerIndex-1;
                    }

                    for (int i = 0; i<currentManager.LogicCells.Length; ++i)
                    {
                        Vector2Int xy = GridMgr.LogicCellIndexToXY(i);
                        currentManager.UpdateShowCell(xy.x, xy.y);
                    }
                }
            }
            if (GUILayout.Button("全覆盖-覆盖层"))
            {
                if (EditorUtility.DisplayDialog("全覆盖-覆盖层", "确定要覆盖吗？", "确定", "取消"))
                {
                    foreach (var logicGrid in currentManager.LogicCells)
                    {
                        logicGrid.Mark=currentManager.SelectedLayerIndex+1;
                    }


                    for (int i = 0; i<currentManager.LogicCells.Length; ++i)
                    {
                        Vector2Int xy = GridMgr.LogicCellIndexToXY(i);
                        currentManager.UpdateShowCell(xy.x, xy.y);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }


        if (GUI.changed) SceneView.RepaintAll();
    }
}