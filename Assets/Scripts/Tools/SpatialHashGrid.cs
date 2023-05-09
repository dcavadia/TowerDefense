using System.Collections.Generic;
using UnityEngine;

public class SpatialHashGrid
{
    private Dictionary<Vector2Int, List<Creep>> cells;
    private float cellSize;

    public SpatialHashGrid(float cellSize)
    {
        this.cellSize = cellSize;
        cells = new Dictionary<Vector2Int, List<Creep>>();
    }

    public void AddCreep(Creep creep)
    {
        Vector2Int cell = GetCell(creep.transform.position);
        if (!cells.TryGetValue(cell, out List<Creep> creeps))
        {
            creeps = new List<Creep>();
            cells[cell] = creeps;
        }

        creeps.Add(creep);
    }

    public void RemoveCreep(Creep creep)
    {
        Vector2Int cell = GetCell(creep.transform.position);
        if (cells.TryGetValue(cell, out List<Creep> creeps))
        {
            creeps.Remove(creep);
            if (creeps.Count == 0)
            {
                cells.Remove(cell);
            }
        }
    }

    public Creep GetNearestCreep(Vector3 turretPosition, float range)
    {
        Creep nearestCreep = null;
        float nearestDistance = float.MaxValue;

        foreach (var cell in GetAdjacentCells(turretPosition, range))
        {
            if (cells.TryGetValue(cell, out List<Creep> cellCreeps))
            {
                foreach (var creep in cellCreeps)
                {
                    float distance = Vector3.Distance(turretPosition, creep.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestCreep = creep;
                        nearestDistance = distance;
                    }
                }
            }
        }

        return nearestCreep;
    }

    private Vector2Int GetCell(Vector2 position)
    {
        int x = Mathf.FloorToInt(position.x / cellSize);
        int y = Mathf.FloorToInt(position.y / cellSize);
        return new Vector2Int(x, y);
    }

    // Provide adjacent cells query
    private IEnumerable<Vector2Int> GetAdjacentCells(Vector3 position, float range)
    {
        int minX = Mathf.FloorToInt((position.x - range) / cellSize);
        int maxX = Mathf.FloorToInt((position.x + range) / cellSize);
        int minY = Mathf.FloorToInt((position.y - range) / cellSize);
        int maxY = Mathf.FloorToInt((position.y + range) / cellSize);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                yield return new Vector2Int(x, y);
            }
        }
    }
}