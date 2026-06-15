using GFW.Container;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 寻路节点数据类
public class PathNode
{
    public int x;
    public int y;

    // G代价：从起点走到当前格子的实际消耗
    public float gCost;
    // H代价：从当前格子到终点的预估消耗（启发函数）
    public float hCost;
    // F代价：综合代价 F = G + H
    public float fCost => gCost + hCost;

    // 父节点：用于最终回溯路径
    public PathNode parent;

    public PathNode(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public static class AStarPathfinder
{
    

    /// <summary>
    /// 执行 A* 寻路
    /// </summary>
    /// <param name="gridManager">网格管理器实例</param>
    /// <param name="startPos">起点逻辑坐标 (row, col)</param>
    /// <param name="targetPos">终点逻辑坐标 (row, col)</param>
    /// <returns>可行走的路径点列表(不包含起点，包含终点或离终点最近的最后一步)</returns>
    public static List<Vector2Int> FindPath(GridMgr gridManager, Vector2Int startPos, Vector2Int targetPos)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        
        if (!IsValidPos(startPos) || !IsValidPos(targetPos)) return path;

        int startIndex = GridMgr.LogicCellXYToIndex(startPos.x, startPos.y);
        // 如果起点本身就不可达，直接返回空路径
        if (gridManager.LogicCells[startIndex].IsUnReachable) return path;

        // 开启列表与关闭列表
        PriorityQueue<PathNode> openList = new PriorityQueue<PathNode>((PathNode x, PathNode y) => { return x.fCost<y.fCost; });
        HashSet<int> closedList = new HashSet<int>();

        Dictionary<int, PathNode> allNodes = new Dictionary<int, PathNode>();

        // 初始化起点
        PathNode startNode = new PathNode(startPos.x, startPos.y)
        {
            gCost = 0,
            hCost = GetManhattanDistance(startPos, targetPos)
        };

        allNodes[startIndex] = startNode;
        openList.Enqueue(startNode);

        PathNode closestNode = startNode;
        // 定义上下左右四个移动方向 (dx: 行, dy: 列)
        int[] dx = { -1, 1, 0, 0 }; // 上，下
        int[] dy = { 0, 0, -1, 1 }; // 左，右

        while (openList.Count > 0)
        {
            PathNode currentNode = openList.Dequeue();
            
            closedList.Add(GridMgr.LogicCellXYToIndex(currentNode.x, currentNode.y));

            // 更新当前最近节点(如果当前节点的H值比记录的更小，说明离终点更近了)
            if (currentNode.hCost < closestNode.hCost)
            {
                closestNode = currentNode;
            }

            // 判断是否抵达终点
            if (currentNode.x == targetPos.x && currentNode.y == targetPos.y)
            {
                // 到达终点，生成并返回路径
                return RetracePath(startNode, currentNode);
            }

            //遍历四个相邻格子
            for (int i = 0; i < 4; i++)
            {
                int neighborX = currentNode.x + dx[i];
                int neighborY = currentNode.y + dy[i];
                Vector2Int neighborPos = new Vector2Int(neighborX, neighborY);

                // 越界检测
                if (!IsValidPos(neighborPos)) continue;

                int neighborIndex = GridMgr.LogicCellXYToIndex(neighborX, neighborY);

                // 如果已经在关闭列表中，忽略
                if (closedList.Contains(neighborIndex)) continue;

                // 读取逻辑网格数据，遇到障碍物(不可到达格子)直接忽略
                LogicGridCell cellData = gridManager.LogicCells[neighborIndex];
                if (cellData.IsUnReachable) continue;

                // 获取或创建邻居节点
                if (!allNodes.ContainsKey(neighborIndex))
                {
                    allNodes[neighborIndex] = new PathNode(neighborX, neighborY);
                }
                PathNode neighborNode = allNodes[neighborIndex];

                // 计算前往该邻居的G代价
                float tentativeGCost = cellData.Cost;

                bool isNeighborInOpenList = openList.Contains(neighborNode);

                // 如果是一条更优(G代价更小)的路径，或者该邻居还没有被评估过
                if (!isNeighborInOpenList || tentativeGCost < neighborNode.gCost)
                {
                    // 更新代价值和父节点指向
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = GetManhattanDistance(neighborPos, targetPos);
                    neighborNode.parent = currentNode;

                    // 如果不在开启列表中，则加入以便后续评估
                    if (!isNeighborInOpenList)
                    {
                        openList.Enqueue(neighborNode);
                    }
                }
            }
        }

        // 寻路失败判定：如果开启列表空了还没找到终点，说明终点死路不可达。
        // 根据需求，此时直接返回距离终点最近的可达路径 (closestNode)
        return RetracePath(startNode, closestNode);
    }

    /// <summary>
    /// 计算曼哈顿距离(因为只能上下左右移动，所以不用欧几里得直线距离，而是绝对值相加)
    /// </summary>
    private static float GetManhattanDistance(Vector2Int nodeA, Vector2Int nodeB)
    {
        return Mathf.Abs(nodeA.x - nodeB.x) + Mathf.Abs(nodeA.y - nodeB.y);
    }

    /// <summary>
    /// 校验坐标是否在逻辑网格边界内
    /// </summary>
    private static bool IsValidPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < GridMgr.LogicGridResolution &&
               pos.y >= 0 && pos.y < GridMgr.LogicGridResolution;
    }

    /// <summary>
    /// 回溯节点，生成最终路径序列
    /// </summary>
    private static List<Vector2Int> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        PathNode currentNode = endNode;

        // 从终点一路往回找父节点，直到回到起点
        while (currentNode != startNode && currentNode != null)
        {
            path.Add(new Vector2Int(currentNode.x, currentNode.y));
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }
}