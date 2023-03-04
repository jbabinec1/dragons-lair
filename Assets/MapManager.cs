using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public OverlayTile overlayTilePrefab;
    public GameObject overlayTilez; //Creating this if I want to attempt spawning tiles on server...
    public GameObject overlayContainer;
    public Dictionary<Vector2Int, OverlayTile> map;
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }

    // Update is called once per frame
    private void Awake()
    {

        if (_instance != null && _instance != this)
        {

            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }


    }

    
    void Start()
    {
      
        SpawnTiles();

    }




    //[Command]
    public void CmdSpawnTiles()
    {
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();

        map = new Dictionary<Vector2Int, OverlayTile>();

        BoundsInt bounds = tileMap.cellBounds;

        //Looping through all tiles
        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {

                    var tileLocation = new Vector3Int(x, y, z);
                    var tileKey = new Vector2Int(x, y);

                    if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(overlayTilez, overlayContainer.transform);
                        //var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);

                        //NetworkServer.Spawn(overlayTile);



                        var cellWorldLocation = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldLocation.x, cellWorldLocation.y, cellWorldLocation.z + 1);

                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = 10;//                                                                      //tileMap.GetComponent<TilemapRenderer>().sortingOrder;

                        var tileProps = overlayTile.GetComponent<OverlayTile>(); //using this for now because overlayTile as a gameobject doesn't have gridLocation property

                        tileProps.gridLocation = tileLocation;
                        map.Add(tileKey, tileProps);
                        //overlayTile.gridLocation = tileLocation;
                        //map.Add(tileKey, overlayTile);
                    }
                }
            }

        }
    }


    //Spawning tiles just on client side
    public void SpawnTiles()
    {
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();

        map = new Dictionary<Vector2Int, OverlayTile>();

        BoundsInt bounds = tileMap.cellBounds;

        //Looping through all tiles
        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {

                    var tileLocation = new Vector3Int(x, y, z);
                    var tileKey = new Vector2Int(x, y);

                    if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                        //CmdSpawnTiles();

                        var cellWorldLocation = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldLocation.x, cellWorldLocation.y, cellWorldLocation.z + 1);

                        // overlayTile.poop = overlayTile.transform.position;  //MAYBE??

                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = 10;//tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                        overlayTile.gridLocation = tileLocation;
                        map.Add(tileKey, overlayTile);
                    }
                }
            }

        }

    }






}
