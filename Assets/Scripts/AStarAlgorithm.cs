using System.Collections.Generic;
using UnityEngine;

public static class AStarAlgorithm
{
    public static List<Grid.Tile> FindPath(Grid grid, Grid.Tile startTile, Grid.Tile endTile
    )
    {
        var openSet = new List<Grid.Tile> { startTile };
        var closedSet = new List<Grid.Tile>();

        var gCost = new Dictionary<Grid.Tile, int>();
        var hCost = new Dictionary<Grid.Tile, int>();
        var parent = new Dictionary<Grid.Tile, Grid.Tile>();

        gCost[startTile] = 0;
        hCost[startTile] = GetDistance(startTile, endTile);

        while (openSet.Count > 0)
        {
            var currentTile = openSet[0];
            for (var i = 1; i < openSet.Count; i++)
            {
                var currentFCost = gCost[currentTile] + hCost[currentTile];
                var newTileFCost = gCost[openSet[i]] + hCost[openSet[i]];

                if (newTileFCost < currentFCost ||
                    (newTileFCost == currentFCost && hCost[openSet[i]] < hCost[currentTile])) currentTile = openSet[i];
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if (currentTile == endTile) return RetracePath(startTile, endTile, parent);

            foreach (var neighbour in grid.GetNeighbours(currentTile))
            {
                if (closedSet.Contains(neighbour) || neighbour.occupied)
                    continue;

                var newMovementCostToNeighbour = gCost[currentTile] + GetDistance(currentTile, neighbour);
                if (!gCost.ContainsKey(neighbour) || newMovementCostToNeighbour < gCost[neighbour])
                {
                    gCost[neighbour] = newMovementCostToNeighbour;
                    hCost[neighbour] = GetDistance(neighbour, endTile);
                    parent[neighbour] = currentTile;

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    private static int GetDistance(Grid.Tile a, Grid.Tile b)
    {
        var dstX = Mathf.Abs(a.x - b.x);
        var dstY = Mathf.Abs(a.y - b.y);
        return dstX + dstY;
    }

    private static List<Grid.Tile> RetracePath(Grid.Tile startTile, Grid.Tile endTile,
        Dictionary<Grid.Tile, Grid.Tile> parent)
    {
        var path = new List<Grid.Tile>();
        var currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = parent[currentTile];
        }

        path.Reverse();
        return path;
    }
}