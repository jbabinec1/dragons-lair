using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Tilemaps;


public class MouseController : MonoBehaviour
{
    public GameObject characterPrefab;
    private CharacterInfo character;
    public Animator characterAnimationSprite;
    public attack attackStatus;
    public attack attackScript;

    public GameObject enemy;
    //public Enemy enemy;

    public GameManager gameManager;

    public MouseController mouseController;

    public Pathfinder pathfinder;

    public float speed;

    private List<OverlayTile> path = new List<OverlayTile>();

    public OverlayTile startingTile;
    public OverlayTile activeTile;

    public List<OverlayTile> inRangeTiles = new List<OverlayTile>();

    public LayerMask collisionLayers;


    public float movementForce = 100f;
    
    public bool isTurnInProgress = true;

    public Color defaultHoverColor;
    public Color enemyHoverColor;

    public SpriteRenderer hoverIconRenderer;

    private float timeBlocked = 0f;
    public float maxTimeBlocked = 2f;
    public bool isBlocked = false;

    public  bool isAttacking = false;




 



    // Start is called before the first frame update
    void Start()
    {
        //test
       // var tileMap = gameObject.GetComponentInChildren<Tilemap>();
       // pathfinder = new Pathfinder(tileMap);

        //OG?
         pathfinder = new Pathfinder();
        
        
        // pathfinder = new Pathfinder();
        gameManager = FindObjectOfType<GameManager>();

        attackScript = FindObjectOfType<attack>();

        mouseController = FindObjectOfType<MouseController>();
        
        character = GameObject.Find("dragon_child").GetComponent<CharacterInfo>();
        characterAnimationSprite = GameObject.Find("dragon_child").GetComponent<Animator>();
        //enemy = GameObject.Find("enemy_idle_01").GetComponent<Enemy>();
        enemy = GameObject.Find("enemy_idle_01");

        hoverIconRenderer = GameObject.Find("Cursor").GetComponent<SpriteRenderer>();


    }


    // Update is called once per frame
    void Update()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

        var focusedTileHit = GetFocusedOnTile();

        //var focusedOnTileFromPlayer = GetFocusedOnTileFromPlayer();
        var focusedOnTileFromPlayer = GetTileUnderMouse();

        var isMoving = false;


        // Get the mouse position on the screen and print it out
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = 10.0f;

        // Convert the screen position of the mouse to a world position and print it out
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        if (focusedOnTileFromPlayer.HasValue)
        {
            character.startingTile = focusedOnTileFromPlayer.Value.collider.gameObject.GetComponent<OverlayTile>();

        }
        
        if (focusedTileHit != null && focusedTileHit.HasValue && !IsPointerOverUIObject())
        {
            OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();

            if (focusedTileHit.Value.collider.gameObject == enemy)
            {
                hoverIconRenderer.color = Color.red;
            }
            else
            {

                hoverIconRenderer.color = Color.yellow;

            } 

            if (overlayTile != null)
            {
                transform.position = overlayTile.transform.position;
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

                
                if (Input.GetMouseButtonDown(0) && overlayTile != null && gameManager.isPlayerTurn == true)
                {
                    List<OverlayTile> newPath = null;
                    if (character.activeTile == null)
                    {
                        // Set startingTile to the tile that the player is currently on
                        character.startingTile = focusedOnTileFromPlayer?.collider.gameObject.GetComponent<OverlayTile>();

                     // OG   path = pathfinder.FindPath(character.startingTile, overlayTile);
                         newPath = pathfinder.FindPath(character.startingTile, overlayTile);

                        character.activeTile = character.startingTile;

                    }
                    else if (character.activeTile != overlayTile)
                    {
                     //OG   path = pathfinder.FindPath(character.activeTile, overlayTile);
                        newPath = pathfinder.FindPath(character.activeTile, overlayTile);
                    //OG    character.activeTile = overlayTile;
                    }

                    //TEST
                    if (newPath != null && newPath.Count > 0)
                    {
                    path = newPath;
                    character.activeTile = overlayTile;
                    }
                } 
            } else if(focusedTileHit.Value.collider.gameObject == enemy) // If the mouse is over the enemy
                 {
                 transform.position = mouseWorldPosition;
                 gameObject.GetComponent<SpriteRenderer>().sortingOrder = enemy.GetComponent<SpriteRenderer>().sortingOrder + 1;
                }
        }

        if (path != null && path.Count > 0)
        {
            isMoving = true;

            if (isMoving)
            {
                characterAnimationSprite.SetBool("isWalking", true);
                MoveAlongPath();
            }
        }
       /* else if(!enemy.isPlayerVisible){
            gameManager.isPlayerTurn == true;
        }*/
        else
        {
            characterAnimationSprite.SetBool("isWalking", false);
            isMoving = false;
        }
    }




    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2 = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }


        return null;
    }

    
    public RaycastHit2D? GetTileUnderMouse()
   {
    Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

    if (hit.collider != null)
    {
        OverlayTile tile = hit.collider.gameObject.GetComponent<OverlayTile>();
        if (tile != null)
        {
            return hit;
        }
    }

    return null;
}


    
    private void PositionCharacterOnTile(OverlayTile tile)
    {
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);

        character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

        character.activeTile = tile;

        
         character.startingTile = tile;

     
    }



    public void MoveTowardsEnemy(GameObject enemyObject)
   {
    path = pathfinder.FindPath(character.activeTile, enemyObject.GetComponent<Enemy>().activeTile);

    if (path.Count > 0)
    {
       MoveAlongPath();
    }
    else
    {
        Debug.Log("No path to enemy found!");
    }


}


/* Test
public void MoveTowardsEnemy(GameObject enemyObject)
{
    // Get the enemy's current tile
    var enemy = enemyObject.GetComponent<Enemy>();
    OverlayTile enemyTile = enemy.activeTile;

    // Get the tiles adjacent to the enemy
    var adjacentTiles = pathfinder.GetAdjacentTiles(enemyTile);
    
    // Find the tile that is immediately below the enemy
    OverlayTile tileInFrontOfEnemy = null;
    foreach (var tile in adjacentTiles)
    {
        // Check if the tile's y-coordinate is below the enemy's y-coordinate
        if (tile.transform.position.y < enemy.transform.position.y)
        {
            tileInFrontOfEnemy = tile;
            break;
        }
    }

    if(tileInFrontOfEnemy != null)
    {
        path = pathfinder.FindPath(character.activeTile, tileInFrontOfEnemy);

        if (path.Count > 0)
        {
            MoveAlongPath();
        }
        else
        {
            Debug.Log("No path to enemy found!");
        }
    }
    else
    {
        Debug.Log("No valid tile in front of enemy found!");
    }
}
*/



public void MoveAlongPath()
{
    var step = speed * Time.deltaTime;
    var zIndex = path[0].transform.position.z;
    Vector2 currentPosition = character.transform.position;
    Vector2 targetPosition = path[0].transform.position;
    Vector2 direction = (targetPosition - currentPosition).normalized;

    float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
    RaycastHit2D hit = Physics2D.Raycast(currentPosition, direction, distanceToTarget, collisionLayers);

    if(hit.collider != null && hit.collider.gameObject.CompareTag("Enemy")) {
        //Debug.Log("Detected enemy in path. Attacking...");
        if (!isAttacking) {
            Debug.Log("Toggling attack...");
            attackScript.CollisionAttack(); // Default attack with collision
            isAttacking = true;
        }
        timeBlocked += Time.deltaTime;
        if (timeBlocked >= maxTimeBlocked) {
            Debug.Log("Trying to end turn...");
            isAttacking = false;
            path.Clear();
            gameManager.EndTurn();
            return;
        }
    }
    else {
        isAttacking = false;
        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);
        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);
        timeBlocked = 0f;
    }
    if ((character.transform.position - path[0].transform.position).magnitude < 0.0001f) {         
        PositionCharacterOnTile(path[0]);
        path.RemoveAt(0);
        if(path.Count == 0 ) { 
            //character.turnsTaken++;
            gameManager.EndTurn();
        } else {
            character.stepsTaken++;
        }
    }
}



 

/* OG
    public void MoveAlongPath()
    {
      
        var step = speed * Time.deltaTime;

        var zIndex = path[0].transform.position.z;

        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);

        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

         GameManager gameManager = FindObjectOfType<GameManager>();

        if ((character.transform.position - path[0].transform.position).magnitude < 0.0001f)
        {         
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);

            if(path.Count == 0 ) { 
               // GetInRangeTiles();
                character.turnsTaken++;
                gameManager.EndTurn();
            }

            else {
                character.stepsTaken++;
            }

        } 
       

    } */


    //  Get the tile the player is currently on
    public RaycastHit2D? GetFocusedOnTileFromPlayer()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(character.transform.position, Vector2.zero);
        // Debug.Log(hits);
        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    //  Check if mouse is over button so we can prevent movement 
    private bool IsPointerOverUIObject()
{
    PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
    {
        position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
    };
    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
    return results.Count > 0;
}



   public void GetInRangeTiles() {

      foreach(var item in inRangeTiles){
        item.HideTile();
    }

    inRangeTiles = pathfinder.GetTilesInRange(character.startingTile, 1);

    foreach(var item in inRangeTiles){
        item.ShowTile();
    }

   }





}