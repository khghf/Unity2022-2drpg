#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GridEditor
{
    public class DefaultState : BaseState
    {
        private int lastHotControl;

        // 缓存被选中物体的初始位置
        private Dictionary<int, Vector3> startPositions = new Dictionary<int, Vector3>();

        public override void OnEnter()
        {
            base.OnEnter();
            // 状态激活时，监听 Unity 的选择变化事件
            Selection.selectionChanged += OnSelectionChanged;
            UpdateStartPositions();
        }

        public override void OnExit()
        {
            base.OnExit();
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            // 当玩家点选、框选改变了选中物体时，更新它们的初始位置缓存
            UpdateStartPositions();
        }

        private void UpdateStartPositions()
        {
            startPositions.Clear();
            foreach (Transform t in Selection.transforms)
            {
                if (t != null)
                {
                    startPositions[t.GetInstanceID()] = t.position;
                }
            }
        }

        public override void OnSceneGUI()
        {
            int currentHotControl = GUIUtility.hotControl;

            if (lastHotControl != 0 && currentHotControl == 0)
            {
                bool hasMoved = false;

                // 检查选中的物体中，是否有任何一个发生了实际位移
                foreach (Transform t in Selection.transforms)
                {
                    if (t != null && startPositions.TryGetValue(t.GetInstanceID(), out Vector3 startPos))
                    {
                        if (Vector3.Distance(t.position, startPos) > 0.001f)
                        {
                            hasMoved = true;
                            break; // 只要有一个物体动了，就执行整体吸附
                        }
                    }
                }

                if (hasMoved)
                {
                    SnapSelectedObjects(GridMgr.LogicGridTopLeft, GridMgr.GridCellSize);

                    // 吸附完成后，更新初始位置为最新的吸附位置
                    // 防止原地点击或者其它操作重复触发吸附
                    UpdateStartPositions();
                }
            }

            lastHotControl = currentHotControl;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("默认模式", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("按下G键进入绘制模式", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("按下L键进入逻辑网格编辑模式", EditorStyles.boldLabel);

        }

        private void SnapSelectedObjects(Vector3 gridTopLeft, float cellSize)
        {
            if (Selection.transforms.Length == 0) return;

            foreach (Transform t in Selection.transforms)
            {
                if (gridMgr != null && t == gridMgr.transform) continue;

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
    }
}
#endif