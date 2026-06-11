using GFW;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class LogicGridCell
{
    /// <summary>
    /// 标记所属的层，负数为基础层(土地)，正数为覆盖层(草地),注意：在设置标记时其值始终会小1(负数时)或大1(正数时)
    /// </summary>
    public int Mark = 0; 
    public int GridLayerIndex
    {  
        get
        {
            if (Mark==0) return -1;
            else
            {
                return Math.Abs(Mark)-1;
            }
        }
    }
    /// <summary>
    /// 从十字相邻格子移动到改格子时的消耗代价
    /// </summary>
    public float Cost = 1;
    /// <summary>
    /// 该格子是否可到达
    /// </summary>
    public bool IsUnReachable = false;
}

[Serializable]
public class GridLayer
{
    public GridLayer(string name,int layerIndex)
    {
        Name=name;
        LayerIndex=layerIndex;
    }
    /// <summary>
    /// 网格层名称
    /// </summary>
    public string Name;
    /// <summary>
    /// 该层对应的瓦片索引
    /// </summary>
    public int LayerIndex = -1;
    public Sprite[] Tiles;

}

/// <summary>
/// 网格管理器关卡唯一而非全局唯一
/// </summary>
public class GridMgr : Singleton<GridMgr>
{
    [Tooltip("像素与Unity世界单位的转换比例(需与背景Sprite设置一致，默认100)")]
    public static float pixelsPerUnit = 100f;

    Sprite DefaultTile = null;

    /// <summary>
    /// 单元格大小128 像素
    /// </summary>
    public static float CellSizePixels = 128;

    /// <summary>
    /// 显示单元格数量32*32
    /// </summary>
    public static int ShowGridResolution = 33;

    /// <summary>
    /// 逻辑单元格数量33*33
    /// </summary>
    public static int LogicGridResolution = 32;

    /// <summary>
    /// 单元格的世界大小
    /// </summary>
    public static float GridCellSize => CellSizePixels / pixelsPerUnit;

    public static float HalfGridCellSize => GridCellSize/2;
    /// <summary>
    /// 显示网格左上角世界位置
    /// </summary>
    public static Vector3 ShowGridTopLeft
    {
        get
        {
            float halfSize = ShowGridResolution * GridCellSize / 2.0f;
            return new Vector3(-halfSize, halfSize, 0);
        }
    }

    /// <summary>
    /// 逻辑网格左上角世界位置
    /// </summary>
    public static Vector3 LogicGridTopLeft
    {
        get
        {
            float halfSize = LogicGridResolution * GridCellSize / 2.0f;
            return new Vector3(-halfSize, halfSize, 0);
        }
    }

    // 当前选中的网格层
    public int SelectedLayerIndex = -1;
    /// <summary>
    /// 瓦片名字-网格层
    /// </summary>
    public List<GridLayer> GridLayers = new List<GridLayer>();
    /// <summary>
    /// 瓦片名字-显示网格
    /// </summary>
    public ShowGridCell[] ShowCells =new ShowGridCell[GridMgr.ShowGridResolution*GridMgr.ShowGridResolution];

    /// <summary>
    /// 瓦片名字-逻辑网格
    /// </summary>
    public LogicGridCell[] LogicCells = new LogicGridCell[GridMgr.LogicGridResolution*GridMgr.LogicGridResolution];

    /// <summary>
    /// 用于映射至正确的瓦片索引
    /// </summary>
    public static readonly Dictionary<int, int> TileMap = new Dictionary<int, int> {
        {0,12},{1,15},{2,8}, {3,9},
        {4,0 },{5,11},{6,14},{7,7},
        {8,13},{9,4},{10,1},{11,10},
        {12,3},{13,2},{14,5},{15,6}
    };

    private void Start()
    {
        //DefaultShowTex=null;
    }

    public void Init()
    {
        ReGeneratorTileMap();
    }


    public void ReGeneratorTileMap()
    {
        if (DefaultTile==null)
        {
            Texture2D tex= new Texture2D((int)CellSizePixels, (int)CellSizePixels);
            Color[] pixels = new Color[(int)CellSizePixels * (int)CellSizePixels];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.black;
            tex.SetPixels(pixels);
            tex.Apply();
            DefaultTile=Sprite.Create(tex, new Rect(0, 0, CellSizePixels, CellSizePixels), new Vector2(0.5f, 0.5f));
        }
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            GameObject child = transform.GetChild(i).gameObject;

#if UNITY_EDITOR
            UnityEditor.Undo.DestroyObjectImmediate(child);
#else
    DestroyImmediate(child);
#endif
        }

        for (int i = 0; i<LogicCells.Length;++i)
        {
            LogicCells[i]=null;
            LogicCells[i]=new LogicGridCell();
        }

        Vector3 pos = Vector3.zero;
        float gridCellHalfSize = GridCellSize/2f;
        pos.z=100;
        Func<int, int, Vector3, int,string, ShowGridCell> spawn = (r, c, p, idx,prefix) => {
            GameObject gameObject = new GameObject($"{prefix}{r}*{c}");
            gameObject.isStatic = true;
            gameObject.transform.parent = transform;
            gameObject.transform.position = p;

            ShowGridCell showGridCell = gameObject.AddComponent<ShowGridCell>();
            showGridCell.Renderer = gameObject.AddComponent<SpriteRenderer>();
            showGridCell.Renderer.sprite = DefaultTile;

            return showGridCell;
        };
        for (int row = 0; row<ShowGridResolution; ++row)
        {
            pos.y=ShowGridTopLeft.y-gridCellHalfSize-row*GridCellSize;
            for (int col = 0; col <ShowGridResolution; ++col)
            {
                pos.x=ShowGridTopLeft.x+gridCellHalfSize+col*GridCellSize;

                int cellIndex = ShowCellXYToIndex(row, col);

                ShowCells[cellIndex]=spawn(row,col,pos,cellIndex,"Tile");
            }
        }
    }


    #region 坐标转换函数
    /// 世界位置到显示网格单元行列
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector2Int WorldPosToShowCellXY(Vector3 pos)
    {
        Vector2Int res = new Vector2Int(-1, -1);
        res.x = (int)((ShowGridTopLeft.y-pos.y)/GridCellSize);
        res.y = (int)((pos.x-ShowGridTopLeft.x)/GridCellSize);
        return res;
    }

    /// <summary>
    /// 世界位置到显示网格单元数组索引
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static int WorldPosToShowCellIndex(Vector3 pos)
    {
        Vector2Int posxy = WorldPosToShowCellXY(pos);
        return ShowCellXYToIndex(posxy.x, posxy.y);
    }

    /// <summary>
    /// 显示网格单元行列到数组索引
    /// </summary>
    /// <param name="x">行</param>
    /// <param name="y">列</param>
    /// <returns></returns>
    public static int ShowCellXYToIndex(int x, int y)
    {
        if (x<0) return -1;
        if (y<0) return -1;
        if (x>=ShowGridResolution) return -1;
        if (y>=ShowGridResolution) return -1;
        return x*ShowGridResolution+y;
    }
    /// <summary>
    /// 显示网格单元数组索引到行列
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static Vector2Int ShowCellIndexToXY(int index)
    {
        Vector2Int vector2Int = new Vector2Int(-1, -1);
        vector2Int.x=index/ShowGridResolution;
        vector2Int.y=index%ShowGridResolution;
        return vector2Int;
    }


    /// <summary>
    /// 世界位置到逻辑网格单元数组索引
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static int WorldPosToLogicCellIndex(Vector3 pos)
    {
        Vector2Int posxy = WorldPosToLogicCellXY(pos);
        return LogicCellXYToIndex(posxy.x, posxy.y);
    }

    /// <summary>
    /// 世界位置到逻辑网格单元行列
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector2Int WorldPosToLogicCellXY(Vector3 pos)
    {
        Vector2Int res = new Vector2Int(-1, -1);
        res.x = (int)((LogicGridTopLeft.y-pos.y)/GridCellSize);
        res.y = (int)((pos.x-LogicGridTopLeft.x)/GridCellSize);
        return res;
    }

    public static Vector3 LogicCellXYToWorldPos(Vector2Int pos)
    {
        return LogicCellXYToWorldPos(pos.x, pos.y);
    }

    public static Vector3 LogicCellXYToWorldPos(int row,int col)
    {
        Vector3 res=new Vector3(0,0,0);

        res.x=LogicGridTopLeft.x+HalfGridCellSize+col*GridCellSize;
        res.y=LogicGridTopLeft.y+HalfGridCellSize+row*GridCellSize;

        return res;
    }

    /// <summary>
    /// 逻辑网格单元行列到数组索引
    /// </summary>
    /// <param name="x">行</param>
    /// <param name="y">列</param>
    /// <returns></returns>
    public static int LogicCellXYToIndex(int x, int y)
    {
        if (x<0) return -1;
        if (y<0) return -1;
        if (x>=LogicGridResolution) return -1;
        if (y>=LogicGridResolution) return -1;
        return x*LogicGridResolution+y;
    }

    /// <summary>
    /// 逻辑网格单元数组索引到行列
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static Vector2Int LogicCellIndexToXY(int index)
    {
        Vector2Int vector2Int = new Vector2Int(-1, -1);
        vector2Int.x=index/LogicGridResolution;
        vector2Int.y=index%LogicGridResolution;
        return vector2Int;
    }

    /// <summary>
    /// 获取被指定格子所覆盖的其它4个格子
    /// </summary>
    /// <param name="row">行</param>
    /// <param name="col">列</param>
    /// <returns></returns>
    public static List<Vector2Int> GetOverlyedShowCellXY(int row, int col)
    {
        int ux = row;
        int bx = row+1;
        int ly = col;
        int ry = col+1;
        List<Vector2Int> cells = new List<Vector2Int>();

        cells.Add(new Vector2Int(ux, ly));
        cells.Add(new Vector2Int(ux, ry));
        cells.Add(new Vector2Int(bx, ly));
        cells.Add(new Vector2Int(bx, ry));

        return cells;
    }
    /// <summary>
    /// 获取被指定格子所覆盖的其它4个格子
    /// </summary>
    /// <param name="row">行</param>
    /// <param name="col">列</param>
    /// <returns></returns>
    public static List<Vector2Int> GetOverlyedLogicCellXY(int row, int col)
    {
        int ux = row-1;
        int bx = row;
        int ly = col-1;
        int ry = col;
        List<Vector2Int> cells = new List<Vector2Int>();
        cells.Add(new Vector2Int(ux, ly));
        cells.Add(new Vector2Int(ux, ry));
        cells.Add(new Vector2Int(bx, ly));
        cells.Add(new Vector2Int(bx, ry));
        return cells;
    }
    #endregion

    /// <summary>

    #region 网格层相关函数
    public bool HasLayer(string layerName)
    {
        foreach (var layer in GridLayers)
        {
            if (layer.Name.Equals(layerName)) return true;
        }
        return false;
    }
    public bool HasLayer(int layerIndex)
    {
        if (GridLayers.Count<0) return false;
        if (layerIndex>=0&&layerIndex<GridLayers.Count) return true;
        return false;
    }
    public GridLayer GetGridLayer(string layerName)
    {
        GridLayer gridLayer = null;
        foreach (var layer in GridLayers)
        {
            if (layer.Name.Equals(layerName))
            {
                gridLayer=layer;
            }
        }
        return gridLayer;
    }

    public GridLayer GetGridLayer(int layerIndex)
    {
        if (HasLayer(layerIndex)) return GridLayers[layerIndex];
        return null;
    }

    public GridLayer GetSelectedLayer()
    {
        return GetGridLayer(SelectedLayerIndex);
    }
    public GridLayer AddGridLayer(string layerName)
    {
        GridLayer gridLayer = null;
        gridLayer= GetGridLayer(layerName);
        if (gridLayer==null)
        {
            gridLayer=new GridLayer(layerName, GridLayers.Count);
            GridLayers.Add(gridLayer);
        }
        return gridLayer;

    }

    public void RemoveLayer(string layerName)
    {
        GridLayer gridLayer = GetGridLayer(layerName);
        if (gridLayer!=null)
        {
            GridLayers.RemoveAt(gridLayer.LayerIndex);
        }
    }

    public int GetLogicCellMark(int gridx, int gridy)
    {
        GridLayer gridLayer = GetGridLayer(SelectedLayerIndex);
        if (gridLayer==null) return 0;
        int gridCellIndex = LogicCellXYToIndex(gridx, gridy);

        if (gridCellIndex<0||gridCellIndex>=LogicCells.Length) return 0;

        return LogicCells[gridCellIndex].Mark;
    }
    /// <summary>
    /// 设置逻辑网格标志
    /// </summary>
    /// <param name="layerIndex">网格标志最终值会小1(负数即mark==false)/大1(正数即mark==true)</param>
    /// <param name="gridx"></param>
    /// <param name="gridy"></param>
    /// <param name="mark"></param>
    /// <returns></returns>
    public bool SetLogicCellMark(int gridx, int gridy, bool mark)
    {
        GridLayer gridLayer = GetGridLayer(SelectedLayerIndex);
        if (gridLayer==null) return false;

        int gridCellIndex = LogicCellXYToIndex(gridx, gridy);
        if (gridCellIndex<0||gridCellIndex>=LogicCells.Length) return false;

        LogicCells[gridCellIndex].Mark=mark==false ? -SelectedLayerIndex-1 : SelectedLayerIndex+1;
        UpdateShowCell(gridx, gridy);
        

        return true;
    }

    /// <summary>
    /// 获取当前选中网格层的指定瓦片，该函数用于编辑时，运行时请用另一个重载函数
    /// </summary>
    /// <param name="tileIndex"></param>
    /// <returns></returns>
    public Sprite GetTile(int tileIndex)
    {
       return GetTile(SelectedLayerIndex,tileIndex);
    }

    public Sprite GetTile(int layerIndex,int tileIndex)
    {
        Sprite sprite = DefaultTile;

        GridLayer gridLayer = GetGridLayer(layerIndex);
        if(gridLayer!=null)
        {
            sprite=gridLayer.Tiles[tileIndex];
        }
        return sprite;
    }

    #endregion
    public  void UpdateShowCell(int logicCellX, int logicCellY)
    {
        List<Vector2Int> showCellXYs = GridMgr.GetOverlyedShowCellXY(logicCellX, logicCellY);
        foreach (var showCellXY in showCellXYs)
        {
            int tileIndex = 0;
            List<Vector2Int> logicCellPos = GridMgr.GetOverlyedLogicCellXY(showCellXY.x, showCellXY.y);
            for (int i = 0; i<logicCellPos.Count; ++i)
            {
                int logicMark = GetLogicCellMark(logicCellPos[i].x, logicCellPos[i].y);
                int one = 1;
                tileIndex=logicMark>0 ? tileIndex|one<<i : tileIndex;
            }
            tileIndex=GridMgr.TileMap[tileIndex];
            int showCellIndex = GridMgr.ShowCellXYToIndex(showCellXY.x, showCellXY.y);
            if (showCellIndex<0) continue;
            ShowCells[showCellIndex].Renderer.sprite=GetTile(tileIndex);
            ShowCells[showCellIndex].TileIndex = tileIndex;
        }
    }
}