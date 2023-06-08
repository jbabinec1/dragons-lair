using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    public LayerMask collisionLayers;

    private float timeBlocked = 0f;
    public float maxTimeBlocked = 0.5f;

    public float maxHealth = 1;
    public float currentHealth = 1;
  

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GameObject.Find("enemy_idle_01");

        gameManager = FindObjectOfType<GameManager>();

        //test
        //var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        //pathfinder = new Pathfinder(tileMap);

        //OG
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

    if (path.Count > 0)
    {
        // Calculate the direction of movement
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = path[0].transform.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        Debug.DrawRay(currentPosition, direction * step, Color.red);

        // Perform a CircleCast to check for collisions
        float colliderRadius = GetComponent<CircleCollider2D>().radius;
        RaycastHit2D hit = Physics2D.CircleCast(currentPosition, colliderRadius, direction, step, collisionLayers);

        // Move the enemy if there's no collision
        if (hit.collider == null)
        {
            transform.position = Vector2.MoveTowards(currentPosition, targetPosition, step);
            transform.position = new Vector3(transform.position.x, transform.position.y, path[0].transform.position.z);
            timeBlocked = 0f;
        }
        else
            {
            timeBlocked += Time.deltaTime;
            if (timeBlocked >= maxTimeBlocked)
            {
                path.Clear();
                GameManager gameManager = FindObjectOfType<GameManager>();
                gameManager.EndTurn();
            }
            return;
        }

        if ((transform.position - path[0].transform.position).magnitude < 0.0001f)
        {
            PositionEnemyOnTile(path[0]);
            path.RemoveAt(0);
            nodesMoved++;
        }
    }
} 

    
    /*
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
    } */

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




    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        //Destroy(gameObject);
        Debug.Log("Enemy died");
    }




}


