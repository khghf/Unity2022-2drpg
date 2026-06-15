
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace GridEditor
{
    public class PaintState :BaseState
    {
        SerializedProperty layersProp = null;
        SerializedProperty tilesProp = null;
        SerializedProperty nameProp = null;
        public override void OnSceneGUI()
        {
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);
            
            GridLayer selectedLayer = gridMgr.GetSelectedLayer();
            if (selectedLayer == null) return;

            if (selectedLayer.Tiles.Length != 16)
            {
                if (e.type == EventType.MouseDown && e.button == 0)
                    Debug.LogWarning("当前选择的瓦片不满足16SetTiles标准，请设置好后再涂抹");
                return;
            }
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
                        Undo.RecordObject(gridMgr, "Paint Grid");
                        gridMgr.SetLogicCellMark(logicPos.x, logicPos.y, !isErasing);
                        e.Use(); // 消耗掉该事件
                    }
                }
            }
        }
        public override void OnInspectorGUI()
        {
            GUILayout.Space(15);

            EditorGUILayout.LabelField("涂抹模式", EditorStyles.boldLabel);

            if (layersProp==null) layersProp = gridManagerEditor.serializedObject.FindProperty("GridLayers");

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
                    gridMgr.GridLayers[i].LayerIndex= i;
                    if (gridMgr.GridLayers[i].Tiles.Length>0) gridMgr.GridLayers[i].Name= gridMgr.GridLayers[i].Tiles[0].name;
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
            gridManagerEditor.serializedObject.ApplyModifiedProperties();


            EditorGUILayout.LabelField("选择图层刷子:", EditorStyles.boldLabel);
            if (GUILayout.Button($"取消选择", GUILayout.Height(30)))
            {
                gridMgr.SelectedLayerIndex = -1;
            }



            foreach (var gridLayer in gridMgr.GridLayers)
            {
                bool isSelected = (gridMgr.SelectedLayerIndex == gridLayer.LayerIndex);
                GUI.backgroundColor=isSelected ? Color.green : Color.white;
                if (GUILayout.Button($"{gridLayer.Name}", GUILayout.Height(30)))
                {
                    gridMgr.SelectedLayerIndex = gridLayer.LayerIndex;
                }
                GUI.backgroundColor = Color.white;
            }
            if (GUILayout.Button($"重新生成初始地图", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("重新生成初始地图", "确定要重新生成初始地图吗，这会清除所有数据包括瓦片布局,逻辑标记？", "确定", "取消"))
                {
                    EditorApplication.delayCall += () =>
                    {
                        if (gridMgr != null) gridMgr.ReGeneratorTileMap();
                    };
                }
            }

            if (gridMgr.SelectedLayerIndex!=-1)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("全覆盖-基础层"))
                {
                    if (EditorUtility.DisplayDialog("全覆盖-基础层", "确定要覆盖吗？", "确定", "取消"))
                    {
                        foreach (var logicGrid in gridMgr.LogicCells)
                        {
                            logicGrid.Mark=-gridMgr.SelectedLayerIndex-1;
                        }

                        for (int i = 0; i<gridMgr.LogicCells.Length; ++i)
                        {
                            Vector2Int xy = GridMgr.LogicCellIndexToXY(i);
                            gridMgr.UpdateShowCell(xy.x, xy.y);
                        }
                    }
                }
                if (GUILayout.Button("全覆盖-覆盖层"))
                {
                    if (EditorUtility.DisplayDialog("全覆盖-覆盖层", "确定要覆盖吗？", "确定", "取消"))
                    {
                        foreach (var logicGrid in gridMgr.LogicCells)
                        {
                            logicGrid.Mark=gridMgr.SelectedLayerIndex+1;
                        }


                        for (int i = 0; i<gridMgr.LogicCells.Length; ++i)
                        {
                            Vector2Int xy = GridMgr.LogicCellIndexToXY(i);
                            gridMgr.UpdateShowCell(xy.x, xy.y);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}

#endif