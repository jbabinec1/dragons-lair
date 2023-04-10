using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Pathfinder pathfinder;
    private List<OverlayTile> path;

    public float visionAngle = 45f;
    public float visionDistance = 10f;
    public LayerMask playerLayer;

    private GameObject player;
    public GameObject enemy;
    public bool isPlayerVisible = false;
    public int actionPoints;
    public int nodesMoved;
    public OverlayTile startingTile;
    public OverlayTile activeTile;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GameObject.Find("enemy_idle_01");

        gameManager = FindObjectOfType<GameManager>();

    
        pathfinder = new Pathfinder();
        actionPoints = 2;
        nodesMoved = 0;

        startingTile = GameManager.enemyStartingTile;
        activeTile = GameManager.enemyStartingTile;

    }

    // Update is called once per frame
    void Update()
    {
    
        // Check if it's the enemy's turn
        GameManager gameManager = FindObjectOfType<GameManager>();

         var focusedOnTileFromPlayer = GetFocusedOnTileFromPlayer();

         CheckForPlayer();

          if (focusedOnTileFromPlayer.HasValue)
        {
            startingTile = focusedOnTileFromPlayer.Value.collider.gameObject.GetComponent<OverlayTile>();

        }
     //  if (focusedOnTileFromPlayer != null && focusedOnTileFromPlayer.HasValue) {
       
        // Check if the player is visible
        CheckForPlayer();
        if (isPlayerVisible && gameManager.isPlayerTurn == false)
        {
            if(activeTile == null){
            // Move the enemy towards the player's current tile
            path = pathfinder.FindPath(activeTile, player.GetComponent<CharacterInfo>().activeTile);

            activeTile = startingTile;
            }

            else if(activeTile != player.GetComponent<CharacterInfo>().activeTile) {
                 path = pathfinder.FindPath(activeTile, player.GetComponent<CharacterInfo>().activeTile);
                 activeTile = player.GetComponent<CharacterInfo>().activeTile;
            }

            // Move the enemy along the path
            if (path != null && path.Count > 0 && nodesMoved < actionPoints)
            {
                MoveAlongPath();
            }
            else
            {
                // End the enemy's turn if it has used up all its action points or has reached the player's tile
                nodesMoved = 0;
                gameManager.EndTurn();
            }
        } else if(!isPlayerVisible) {
            gameManager.EndTurn();
        }

    }
    

    public void MoveAlongPath()
    {
        var step = 2f * Time.deltaTime;

        var zIndex = path[0].transform.position.z;

        transform.position = Vector2.MoveTowards(transform.position, path[0].transform.position, step);

        transform.position = new Vector3(transform.position.x, transform.position.y, zIndex);

        if ((transform.position - path[0].transform.position).magnitude < 0.0001f)
        {
            PositionEnemyOnTile(path[0]);
            path.RemoveAt(0);
            nodesMoved++;
        }
    }

    private void PositionEnemyOnTile(OverlayTile tile)
    {
        transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);

        GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

        activeTile = tile;
    }


      void CheckForPlayer()
{
    // Calculate the direction from the enemy to the player
    Vector2 directionToPlayer = player.transform.position - transform.position;

    // Calculate the angle between the enemy's forward vector and the direction to the player
    float angleToPlayer = Vector2.Angle(transform.up, directionToPlayer);


    // Check if the angle is within the vision cone
    if (angleToPlayer <= visionAngle / 2f)
    {
        // Check if the player is within the vision distance
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToPlayer.normalized, visionDistance, playerLayer);
        isPlayerVisible = false;
        gameManager.isPlayerVisible = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.tag == "Player" && !hit.collider.isTrigger)
            {
                // The player is visible
                isPlayerVisible = true;
                gameManager.isPlayerVisible = true;
                break;
            }
        }

        // Draw a debug raycast to show the direction and length of the raycast
        Debug.DrawRay(transform.position, directionToPlayer.normalized * visionDistance, isPlayerVisible ? Color.green : Color.red);
    }
    else
    {
        // The player is not within the vision cone
        isPlayerVisible = false;
        gameManager.isPlayerVisible = false;
    }
}



    public RaycastHit2D? GetFocusedOnTileFromPlayer()
{
    RaycastHit2D[] hits = Physics2D.RaycastAll(enemy.transform.position, Vector2.zero);
    // Debug.Log(hits);
    if (hits.Length > 0)
    {
        OverlayTile tile = hits.OrderByDescending(i => i.collider.transform.position.z).First().collider.gameObject.GetComponent<OverlayTile>();
        if (tile != null)
        {
            startingTile = tile;
            activeTile = tile;
        }
        return hits.OrderByDescending(i => i.collider.transform.position.z).First();
    }

    return null;
}



}


