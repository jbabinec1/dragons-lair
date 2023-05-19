using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MouseController : MonoBehaviour
{
    public GameObject characterPrefab;
    private CharacterInfo character;
    public Animator characterAnimationSprite;
    public attack attackStatus;

    public GameObject enemy;
    //public Enemy enemy;

    public GameManager gameManager;

    private Pathfinder pathfinder;

    public float speed;

    private List<OverlayTile> path = new List<OverlayTile>();

    public OverlayTile startingTile;
    public OverlayTile activeTile;

    public List<OverlayTile> inRangeTiles = new List<OverlayTile>();

    public LayerMask collisionLayers;

    private float timeBlocked = 0f;
    public float maxTimeBlocked = 0.5f; // You can adjust this value as needed

    public float movementForce = 100f;
    
    public bool isTurnInProgress = true;

    public Color defaultHoverColor;
    public Color enemyHoverColor;

    public SpriteRenderer hoverIconRenderer;




 



    // Start is called before the first frame update
    void Start()
    {
        //character = characterPrefab.GetComponent<CharacterInfo>();
        pathfinder = new Pathfinder();
        gameManager = FindObjectOfType<GameManager>();
        
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
      //  Debug.Log("Mouse Screen Position: " + mouseScreenPosition);

        // Convert the screen position of the mouse to a world position and print it out
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
       // Debug.Log("Mouse World Position: " + mouseWorldPosition);

        // Print out the enemy's world position
       // Debug.Log("Enemy World Position: " + enemy.transform.position);


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
                    if (character.activeTile == null)
                    {
                        // Set startingTile to the tile that the player is currently on
                        character.startingTile = focusedOnTileFromPlayer?.collider.gameObject.GetComponent<OverlayTile>();

                        path = pathfinder.FindPath(character.startingTile, overlayTile);

                        character.activeTile = character.startingTile;

                    }
                    else if (character.activeTile != overlayTile)
                    {
                        path = pathfinder.FindPath(character.activeTile, overlayTile);
                        character.activeTile = overlayTile;
                    }
                } 
            } else if(focusedTileHit.Value.collider.gameObject == enemy)
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
    //OverlayTile enemyTile = /* Get the tile the enemy is on */

    // This will find a path to the enemy's tile.
    // Replace character.activeTile with the character's current tile.
    path = pathfinder.FindPath(character.activeTile, enemyObject.GetComponent<Enemy>().activeTile);

    // This will make the character start moving along the path on the next frame
    // because it triggers the path-following logic in your Update method.
  if (path.Count > 0)
    {
        MoveAlongPath();
    }
  //  else
   // {
   //     Debug.Log("No path to enemy found!");
  //  }


}



/*
public void MoveAlongPath()
{
    if (!isTurnInProgress && path != null && path.Count > 0)
    {
        isTurnInProgress = true;
    }

    if (isTurnInProgress)
    {
        if (path != null && path.Count > 0)
        {
            Vector2 newPosition = path[0].transform.position;
            Vector2 direction = newPosition - (Vector2)character.transform.position;
            direction.Normalize();

            Rigidbody2D rb = character.GetComponent<Rigidbody2D>();

            RaycastHit2D hit = Physics2D.Linecast(character.transform.position, newPosition);

            if (hit.collider != null && hit.collider.gameObject != character.gameObject)
            {
                timeBlocked += Time.deltaTime;
                if (timeBlocked >= maxTimeBlocked)
                {
                    path.Clear();
                    isTurnInProgress = false;
                    GameManager gameManager = FindObjectOfType<GameManager>();
                    gameManager.EndTurn();
                }
                return;
            }
            else
            {
                timeBlocked = 0;
            }

            rb.AddForce(direction * movementForce * Time.deltaTime);

            if ((character.transform.position - path[0].transform.position).magnitude < 0.1f)
            {
                rb.velocity = Vector2.zero;
                PositionCharacterOnTile(path[0]);
                path.RemoveAt(0);

                if (path.Count == 0)
                {
                    character.turnsTaken++;
                    isTurnInProgress = false;
                    GameManager gameManager = FindObjectOfType<GameManager>();
                    gameManager.EndTurn();
                }
                else
                {
                    character.stepsTaken++;
                }
            }
        }
    }
}*/






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

            //Turns vs Steps.. Turns are the total number of times the character reached destination. Steps is the total amount of steps taken.


            //character.stepsTaken = 0;
            //character.turnsTaken++;

        } 
       

    }


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