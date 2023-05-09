using System.Collections.Generic;
using UnityEngine;

public class SpatialHashGrid
{
    private Dictionary<Vector2Int, List<Creep>> cells; // Dictionary to store cells and their corresponding creeps
    private float cellSize; // Size of each grid cell

    public SpatialHashGrid(float cellSize)
    {
        this.cellSize = cellSize;
        cells = new Dictionary<Vector2Int, List<Creep>>();
    }

    // Add a creep to the grid
    public void AddCreep(Creep creep)
    {
        Vector2Int cell = GetCell(creep.transform.position); // Get the cell position based on the creep's position
        if (!cells.TryGetValue(cell, out List<Creep> creeps))
        {
            creeps = new List<Creep>();
            cells[cell] = creeps;
        }

        creeps.Add(creep);
    }

    // Remove a creep from the grid
    public void RemoveCreep(Creep creep)
    {
        Vector2Int cell = GetCell(creep.transform.position); // Get the cell position based on the creep's position
        if (cells.TryGetValue(cell, out List<Creep> creeps))
        {
            creeps.Remove(creep);
            if (creeps.Count == 0)
            {
                cells.Remove(cell);
            }
        }
    }

    // Get the nearest creep to a given turret position within a specified range
    public Creep GetNearestCreep(Vector3 turretPosition, float range)
    {
        Creep nearestCreep = null;
        float nearestDistance = float.MaxValue;

        // Iterate through the adjacent cells around the turret
        foreach (var cell in GetAdjacentCells(turretPosition, range))
        {
            if (cells.TryGetValue(cell, out List<Creep> cellCreeps))
            {
                // Iterate through the creeps in the current cell
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

    // Get the cell position based on a given position
    private Vector2Int GetCell(Vector2 position)
    {
        int x = Mathf.FloorToInt(position.x / cellSize);
        int y = Mathf.FloorToInt(position.y / cellSize);
        return new Vector2Int(x, y);
    }

    // Get the adjacent cells around a given position within a specified range
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
