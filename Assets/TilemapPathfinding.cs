using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node
{
    public Vector3 position;
    public List<Node> neighbors;

    public Node(Vector3 position)
    {
        this.position = position;
        this.neighbors = new List<Node>();
    }

    public void AddNeighbor(Node node)
    {
        this.neighbors.Add(node);
    }
}

public class TilemapPathfinding : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase[] unwalkableTiles;

    private Dictionary<Vector3Int, Node> nodes = new Dictionary<Vector3Int, Node>();

    private void Awake()
    {
        // Iterate over all tiles in the tilemap
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            // If the tile is not walkable, skip it
            if (!tilemap.HasTile(localPlace))
            {
                continue;
            }

            var tile = tilemap.GetTile(localPlace);
            // If the tile is not walkable, skip it
            if (Array.Exists(unwalkableTiles, t => t == tile))
            {
                continue;
            }

            // Add a new node to the dictionary
            var worldPlace = tilemap.CellToWorld(localPlace);
            nodes.Add(localPlace, new Node(worldPlace));
        }

        // Iterate over all nodes and connect them to their neighbors
        foreach (var node in nodes)
        {
            var neighbors = GetNeighbors(node.Key);

            foreach (var neighbor in neighbors)
            {
                node.Value.neighbors.Add(nodes[neighbor]);
            }
        }
    }

    private List<Vector3Int> GetNeighbors(Vector3Int position)
    {
        var neighbors = new List<Vector3Int>();

        neighbors.Add(new Vector3Int(position.x - 1, position.y, position.z));
        neighbors.Add(new Vector3Int(position.x + 1, position.y, position.z));
        neighbors.Add(new Vector3Int(position.x, position.y - 1, position.z));
        neighbors.Add(new Vector3Int(position.x, position.y + 1, position.z));

        return neighbors;
    }
}
