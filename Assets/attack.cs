using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System.Linq;




public class attack : MonoBehaviour
{
    
     private List<OverlayTile> inRangeTiles;
     private CharacterInfo character;
     public bool isAttackActive = false;
     public GameObject enemy;
     public bool enemyInRange = false;

     public GameManager gameManager;
     public Pathfinder pathfinder;

    
    public bool attackReady = false; // this determines if enemy is detected in range when attack btn is first press

    public bool attackQueued = false; // queue before attack is performed

    public int queTurnNumber = 0;

    public bool attackInProgress = false;

    public GameObject selectedEnemy;

    bool isAttacking = false;

    public MouseController mouseController;

    public float attackRange = 2f;

    MapManager mapManager = MapManager.Instance;


    // Start is called before the first frame update
    void Start()
    {
       
       pathfinder = new Pathfinder();

       inRangeTiles = new List<OverlayTile>();
        
        character = GameObject.Find("dragon_child").GetComponent<CharacterInfo>();

        enemy = GameObject.Find("enemy_idle_01");

        gameManager = FindObjectOfType<GameManager>();

        mouseController = FindObjectOfType<MouseController>();

    }

    // Update is called once per frame
    void Update()
    {

     
        
    }
    


    public void GetInRangeTiles() {
    if (isAttackActive && !enemyInRange) {
        foreach(var item in inRangeTiles) {
            item.HideTile();
        }
        return;
    }
    
    if (character.startingTile != null) {
        inRangeTiles = pathfinder.GetTilesInRange(character.startingTile, 3);
    } else {
        inRangeTiles = new List<OverlayTile>();
    }

    if (character.activeTile != null) {
        inRangeTiles.AddRange(pathfinder.GetTilesInRange(character.activeTile, 1));
    }
    enemyInRange = false;
    foreach(var item in inRangeTiles){

        item.ShowTile();
        item.activatedTile = true;

        // Check if the enemy is standing on the tile
       if (Vector2.Distance(item.transform.position, enemy.transform.position) <= 0.5f) {
        item.ShowTileAttack();
        enemyInRange = true;

        Debug.Log("Enemy is in range");
    } else {
        Debug.Log("Enemy is not in range");
    }
        
    }
  //  OverlayTile mouseTile = GetTileUnderMouse();
       // Debug.Log(mouseTile);
}


public void CollisionAttack() {
    if (IsTileNextToEnemy(character.activeTile)) {
        Debug.Log("Attack");
        enemy.GetComponent<Enemy>().TakeDamage(.1f);
        //attackReady = false;
        gameManager.EndTurn();
    } else {
        Debug.Log("Attack not ready. Not in range sonion.");
    }
}

public void ToggleAttack() 
{
    GetInRangeTiles();

    if (!isAttackActive)

    {
        isAttackActive = true;

        if (enemyInRange)
        {
            attackQueued = true;
            queTurnNumber += 1;
        }
        else
        {

            attackQueued = false;
            attackReady = false;
        }
    }
    else
    {
        if (enemyInRange == true && queTurnNumber == 1)
        {
            PerformPunchAttack();
            attackQueued = false;
            ClearAttackTiles();

            // Set attackReady to true here
            attackReady = true;
        }
        isAttackActive = false;
        isAttacking = false;
    }

    // Reset queTurnNumber to 0 when attack is toggled off
    if (!isAttackActive)
    {
        queTurnNumber = 0;
    }
}


public bool IsTileNextToEnemy(OverlayTile activeTile)
{
    Vector2Int enemyTilePosition = new Vector2Int(enemy.GetComponent<Enemy>().activeTile.gridLocation.x, enemy.GetComponent<Enemy>().activeTile.gridLocation.y);
    Vector2Int activeTilePosition = new Vector2Int(character.activeTile.gridLocation.x, character.activeTile.gridLocation.y);

    int diffX = Mathf.Abs(activeTilePosition.x - enemyTilePosition.x);
    int diffY = Mathf.Abs(activeTilePosition.y - enemyTilePosition.y);

    // Check if the tile is adjacent to the enemy tile
    if((diffX == 1 && diffY == 0) || (diffX == 0 && diffY == 1))
    {
        return true;
    }

    return false;
}



void OnTriggerEnter2D(Collider2D other) {
    if(other.gameObject.CompareTag("Enemy")) {
        enemyInRange = true;
        enemy = other.gameObject; // the enemy that entered your attack range
    }
}

void OnTriggerExit2D(Collider2D other) {
    if(other.gameObject.CompareTag("Enemy")) {
        enemyInRange = false;
        enemy = null; // no enemy in range
    }
}

/*void OnDrawGizmosSelected() {
    if (character != null) {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(character.transform.position, attackRange);
    }
}*/




    private void ClearAttackTiles()
    {
        foreach (var item in inRangeTiles)
        {
            item.HideTile();
        }
        
    } 

    public void SetSelectedEnemy(GameObject enemy)
{
    selectedEnemy = enemy;
}



 public void PerformPunchAttack() {
        if(attackReady == true) {
            Debug.Log("Punch attack");
            enemy.GetComponent<Enemy>().TakeDamage(.1f);
            attackReady = false;
            gameManager.EndTurn();
        } if(!enemyInRange) {
            Debug.Log("Attack not ready. Not in range.");
        } if(attackQueued == true && !attackReady) {
            Debug.Log("Attack not ready. Attack is in queue.");
        }
        
    }



private OverlayTile GetTileUnderMouse()
{

    // Check if mouse is over the attack button
    if (IsPointerOverUIObject())
    {
        return null;
    }

    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3Int cellPosition = pathfinder.WorldToCellPosition(mouseWorldPosition);
    Vector3 cellCenterWorld = pathfinder.overlayTilemap.GetCellCenterWorld(cellPosition);

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

private bool IsPointerOverUIObject()
{
    PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
    eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

    // Check if the pointer is over any UI element other than the Attack button
    return results.Any(result => result.gameObject.tag != "AttackButton");
}


private void HighlightAttackableTiles()
{
    OverlayTile mouseTile = GetTileUnderMouse();
    if (mouseTile != null && inRangeTiles.Contains(mouseTile))
    {
        foreach (var tile in inRangeTiles)
        {
            if (tile == mouseTile)
            {
                tile.HighlightRed();
            }
            else
            {
                tile.ResetHighlight();
            }
        }
    }
    else
    {
        foreach (var tile in inRangeTiles)
        {
            tile.ResetHighlight();
        }
    }
}


public void AttackEnemy()
{
    if (Input.GetMouseButtonDown(0))
    {
        OverlayTile mouseTile = GetTileUnderMouse();
        if (mouseTile != null && inRangeTiles.Contains(mouseTile))
        {
            // EnemyController enemy = mouseTile.GetComponentInChildren<EnemyController>();
            if (attackReady && enemy != null)
            {
                Debug.Log("Attack");
               // character.GetComponent<CharacterAnimation>().SetTrigger("attack");
               // enemy.TakeDamage(1);
            }
        }
    }
}


  /*  public void AttackEnemy() {

        if (inRangeTiles.Contains(activeTile)) {
            Debug.Log("Attack");
            characterAnimationSprite.SetTrigger("attack");
            enemy.GetComponent<EnemyController>().TakeDamage(1);
        }
    }*/
}
