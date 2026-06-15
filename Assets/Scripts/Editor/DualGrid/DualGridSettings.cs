#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace GridEditor
{
    public class DualGridSettings : BaseState
    {
        public bool isDrawGrid = true;
        public bool isDrawLogicGrid = true;
        public bool isDrawShowGrid = false;

        public bool isDrawNum = false;
        public bool isDrawLogicMark = true;
        public bool isDrawShowTileIndex = true;

        static Color LogicGirdCellColor = new Color(0.0f, 1f, 0.0f, 1f);
        static Color ShowGirdColor = new Color(1f, 0f, 0f, 1f);
        public override void OnSceneGUI()
        {
            if (isDrawGrid)
            {
                Handles.color = LogicGirdCellColor;
                if (isDrawLogicGrid) DualGridEditor.DrawGridLines(GridMgr.LogicGridTopLeft, GridMgr.GridCellSize, GridMgr.LogicGridResolution);
                Handles.color = ShowGirdColor;
                if (isDrawShowGrid) DualGridEditor.DrawGridLines(GridMgr.ShowGridTopLeft, GridMgr.GridCellSize, GridMgr.ShowGridResolution);
            }

            if (isDrawNum)
            {
                if (isDrawLogicMark) DualGridEditor.DrawLogicMark();
                if (isDrawShowTileIndex) DualGridEditor.DrawShowTileIndex();
            }

        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("全局设置", EditorStyles.boldLabel);
            isDrawGrid = EditorGUILayout.BeginToggleGroup("绘制网格", isDrawGrid);

            isDrawLogicGrid = EditorGUILayout.Toggle("绘制逻辑网格", isDrawLogicGrid);
            isDrawShowGrid = EditorGUILayout.Toggle("绘制显示网格", isDrawShowGrid);

            EditorGUILayout.EndToggleGroup();


            isDrawNum= EditorGUILayout.BeginToggleGroup("绘制数字", isDrawNum);

            isDrawLogicMark=EditorGUILayout.Toggle("绘制逻辑网格单元标记数字", isDrawLogicMark);
            isDrawShowTileIndex=EditorGUILayout.Toggle("绘制显示网格单元瓦片索引", isDrawShowTileIndex);

            EditorGUILayout.EndToggleGroup();
        }
    }
}
#endif