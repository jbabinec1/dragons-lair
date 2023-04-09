using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


[Serializable]
public class Pathfinder
{

    public Tilemap overlayTilemap;


    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end)
    {

        List<OverlayTile> openList = new List<OverlayTile>();

        List<OverlayTile> closedList = new List<OverlayTile>();

        openList.Add(start);

        while (openList.Count > 0)
        {

            OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();


            if (currentOverlayTile != null)
            {
                openList.Remove(currentOverlayTile);
                closedList.Add(currentOverlayTile);

            }

            if (currentOverlayTile == end)
            {
                return GetFinishedList(start, end);
            }

            var neighborTiles = GetNeighborTiles(currentOverlayTile);

            foreach (var neighbor in neighborTiles)
            {
                // 1 = jump height
                if (neighbor.isBlocked || closedList.Contains(neighbor) || Mathf.Abs(currentOverlayTile.gridLocation.z - neighbor.gridLocation.z) > 1)
                {
                    continue;
                }

                neighbor.G = GetManhattanDistance(start, neighbor);
                neighbor.H = GetManhattanDistance(end, neighbor);

                neighbor.previous = currentOverlayTile;

                // neighbor.poop = currentOverlayTile.transform.position;
                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
            }

        }

        return new List<OverlayTile>();
    }




    public List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>();

        OverlayTile currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }
        finishedList.Reverse(); //List was reversed upon looping through

        return finishedList;
    }

    public int GetManhattanDistance(OverlayTile start, OverlayTile neighbor)
    {
        return Mathf.Abs(start.gridLocation.x - neighbor.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbor.gridLocation.y);
    }

    public List<OverlayTile> GetNeighborTiles(OverlayTile currentOverlayTile)
    {
        var map = MapManager.Instance.map;

        List<OverlayTile> neighbors = new List<OverlayTile>();

        //Top neighbor
        Vector2Int locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y + 1);

        if (map.ContainsKey(locationToCheck))
        {
            neighbors.Add(map[locationToCheck]);
        }

        //Bottom neighbor
        locationToCheck = new Vector2Int(
           currentOverlayTile.gridLocation.x,
           currentOverlayTile.gridLocation.y - 1);

        if (map.ContainsKey(locationToCheck))
        {
            neighbors.Add(map[locationToCheck]);
        }

        //Right neighbor
        locationToCheck = new Vector2Int(
           currentOverlayTile.gridLocation.x + 1,
           currentOverlayTile.gridLocation.y);

        if (map.ContainsKey(locationToCheck))
        {
            neighbors.Add(map[locationToCheck]);
        }

        //Left neighbor
        locationToCheck = new Vector2Int(
           currentOverlayTile.gridLocation.x - 1,
           currentOverlayTile.gridLocation.y);

        if (map.ContainsKey(locationToCheck))
        {
            neighbors.Add(map[locationToCheck]);
        }

        return neighbors;
    }




public List<OverlayTile> GetAdjacentTiles(OverlayTile tile)
{
    List<OverlayTile> adjacentTiles = new List<OverlayTile>();

    int x = (int)tile.transform.position.x;
    int y = (int)tile.transform.position.y;

    // Check the tile to the left
    OverlayTile leftTile = GetTileAtPosition(x - 1, y);
    if (leftTile != null)
    {
        adjacentTiles.Add(leftTile);
    }

    // Check the tile to the right
    OverlayTile rightTile = GetTileAtPosition(x + 1, y);
    if (rightTile != null)
    {
        adjacentTiles.Add(rightTile);
    }

    // Check the tile above
    OverlayTile topTile = GetTileAtPosition(x, y + 1);
    if (topTile != null)
    {
        adjacentTiles.Add(topTile);
    }

    // Check the tile below
    OverlayTile bottomTile = GetTileAtPosition(x, y - 1);
    if (bottomTile != null)
    {
        adjacentTiles.Add(bottomTile);
    }

    return adjacentTiles;
}

// Returns the cell position of the given world position
private Vector3Int WorldToCellPosition(Vector3 worldPosition)
{
    return overlayTilemap.WorldToCell(worldPosition);
}


// Returns the tile at the given position
private OverlayTile GetTileAtPosition(int x, int y)
{
    Vector3 worldPosition = new Vector3(x, y, 0);
    Vector3Int cellPosition = WorldToCellPosition(worldPosition);
    Vector3 cellCenterWorld = overlayTilemap.GetCellCenterWorld(cellPosition);

    RaycastHit2D hit = Physics2D.Raycast(cellCenterWorld, Vector2.zero);

    if (hit.collider != null)
    {
        OverlayTile tile = hit.collider.GetComponent<OverlayTile>();
        if (tile != null)
        {
            return tile;
        }
    }

    return null;
}




public List<OverlayTile> GetTilesInRange(OverlayTile start, int range)
{
   var inRangeTiles = new List<OverlayTile>();
   int stepCount = 0;

   inRangeTiles.Add(start);

   var tilesFromPreviousStep = new List<OverlayTile>();
   tilesFromPreviousStep.Add(start);


   while(stepCount < range) {
    var surroundingTiles = new List<OverlayTile>();

    foreach(var item in tilesFromPreviousStep){
        surroundingTiles.AddRange(GetNeighborTiles(item));
    }

    inRangeTiles.AddRange(surroundingTiles);
    tilesFromPreviousStep = surroundingTiles.Distinct().ToList();
    stepCount++;
   }

  return inRangeTiles.Distinct().ToList();
   
}












}
