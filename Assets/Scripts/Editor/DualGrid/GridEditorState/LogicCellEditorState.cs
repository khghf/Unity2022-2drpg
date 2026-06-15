#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace GridEditor
{
    public class LogicCellEditorState :BaseState
    {
        private Vector2Int selectedLogicCell = new Vector2Int(-1, -1);
        private SerializedProperty logicCellsProp = null;
        private bool isCtrl_UKeyPressed = false;
        private Color selectedHighLight = new Color(0, 0, 0, 0.6f);
        private Color unReachableColor = new Color(1, 0, 0, 1);
        public override void OnSceneGUI()
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);


            for(int i = 0;i<gridMgr.LogicCells.Length;++i)
            {
                var cell = gridMgr.LogicCells[i];
                if (cell.IsUnReachable)
                {
                    DualGridEditor.HilightLogicCell(GridMgr.LogicCellIndexToXY(i), unReachableColor);
                }
            }
            DualGridEditor.HilightLogicCell(selectedLogicCell.x, selectedLogicCell.y, selectedHighLight);

            Event e = Event.current;
            if (e.isKey && e.keyCode == KeyCode.U)
            {
                if (e.type == EventType.KeyDown)
                {
                    if (e.control)
                    {
                        e.Use();
                        isCtrl_UKeyPressed = true;
                    }
                }
                else if (e.type == EventType.KeyUp)
                {
                    isCtrl_UKeyPressed = false;
                }
            }
            if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                Plane plane = new Plane(Vector3.forward, Vector3.zero);

                if (plane.Raycast(ray, out float distance))
                {
                    Vector3 worldPos = ray.GetPoint(distance);
                    Vector2Int logicPos = GridMgr.WorldPosToLogicCellXY(worldPos);

                    if (logicPos.x >= 0 && logicPos.x < GridMgr.LogicGridResolution && logicPos.y >= 0 && logicPos.y < GridMgr.LogicGridResolution)
                    {
                        selectedLogicCell = logicPos;

                        HandleCtrl_U_Mouse();
                        gridManagerEditor?.Repaint();
                        SceneView.lastActiveSceneView?.Repaint();
                    }
                }
            }
            
        }

        private void HandleCtrl_U_Mouse()
        {
            if (isCtrl_UKeyPressed)
            {
                Event e = Event.current;
                int cellIndex = GridMgr.LogicCellXYToIndex(selectedLogicCell.x, selectedLogicCell.y);
                if (cellIndex >= 0 && cellIndex < gridMgr.LogicCells.Length)
                {
                    LogicGridCell logicGridCell = gridMgr.LogicCells[cellIndex];
                    if (logicGridCell != null)
                    {
                        Undo.RecordObject(gridMgr, "Change Cell Reachability");
                        if (e.button == 0) // 左键可达
                        {
                            logicGridCell.IsUnReachable = false;
                        }
                        else if (e.button == 1) // 右键不可达
                        {
                            logicGridCell.IsUnReachable = true;
                        }
                        EditorUtility.SetDirty(gridMgr);
                        e.Use();
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(15);
            EditorGUILayout.LabelField("逻辑网格编辑模式", EditorStyles.boldLabel);

            if (selectedLogicCell.x == -1 || selectedLogicCell.y == -1)
            {
                EditorGUILayout.HelpBox("请在Scene视图中【点击】一个绿色格子以编辑其数据。", MessageType.Info);
            }
            else
            {
                int row = selectedLogicCell.x;
                int col = selectedLogicCell.y;
                EditorGUILayout.HelpBox($"当前选中逻辑格子: 第 {row} 行, 第 {col} 列", MessageType.None);

                int cellIndex = GridMgr.LogicCellXYToIndex(row, col);

                if (logicCellsProp==null) logicCellsProp = gridManagerEditor.serializedObject.FindProperty("LogicCells");
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
                        gridManagerEditor.serializedObject.ApplyModifiedProperties();
                    }
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.HelpBox("无法获取网格数据：LogicCells 数组未初始化或索引越界。", MessageType.Warning);
                }
            }


            EditorGUILayout.Space();
        }
    }
}

#endif