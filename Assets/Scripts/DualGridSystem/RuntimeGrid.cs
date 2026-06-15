using UnityEngine;

public class RuntimeGrid : MonoBehaviour
{
    [Header("网格设置")]
    public int columns = GridMgr.LogicGridResolution;       // 多少列
    public int rows = GridMgr.LogicGridResolution;          // 多少行
    public float cellSize = GridMgr.GridCellSize;           // 每个格子的大小

    [Header("外观")]
    public Color gridColor = new Color(0f, 1f, 0f, 0.4f); // 半透明绿色

    // 缓存材质
    private Material lineMaterial;

    private void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;

            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private void OnRenderObject()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.LINES);
        GL.Color(gridColor);

        float width = columns * cellSize;
        float height = rows * cellSize;

        // 1. 画所有的横线
        for (int y = 0; y <= rows; y++)
        {
            float yPos = GridMgr.LogicGridTopLeft.y- y * cellSize;
            GL.Vertex3(-GridMgr.LogicGridTopLeft.x, yPos, 0);       // 起点
            GL.Vertex3(GridMgr.LogicGridTopLeft.x, yPos, 0);   // 终点
        }

        // 2. 画所有的竖线
        for (int x = 0; x <= columns; x++)
        {
            float xPos = GridMgr.LogicGridTopLeft.x+x * cellSize;
            GL.Vertex3(xPos, -GridMgr.LogicGridTopLeft.y, 0);       // 起点
            GL.Vertex3(xPos, GridMgr.LogicGridTopLeft.y, 0);  // 终点
        }

        GL.End();
        GL.PopMatrix();
    }
}