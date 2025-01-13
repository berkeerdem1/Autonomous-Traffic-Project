using System.Collections.Generic;
using UnityEngine;

public class SpatialHashingSystem : MonoBehaviour
{
    private Dictionary<Vector2Int, List<Transform>> grid = new Dictionary<Vector2Int, List<Transform>>();
    private float cellSize;

    public SpatialHashingSystem(float cellSize)
    {
        this.cellSize = cellSize;
    }

    private Vector2Int GetGridCell(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x / cellSize), Mathf.FloorToInt(position.z / cellSize));
    }

    public void AddPoint(Transform point)
    {
        Vector2Int cell = GetGridCell(point.position);
        if (!grid.ContainsKey(cell))
        {
            grid[cell] = new List<Transform>();
        }
        grid[cell].Add(point);
    }

    public Transform FindClosestPoint(Vector3 position)
    {
        Vector2Int cell = GetGridCell(position);
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int neighborCell = cell + new Vector2Int(x, y);
                if (grid.ContainsKey(neighborCell))
                {
                    foreach (Transform point in grid[neighborCell])
                    {
                        float distance = Vector3.Distance(position, point.position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closest = point;
                        }
                    }
                }
            }
        }

        return closest;
    }
}
