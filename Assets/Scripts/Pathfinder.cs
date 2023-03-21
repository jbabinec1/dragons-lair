using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Pathfinder
{

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




}
