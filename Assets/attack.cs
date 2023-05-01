using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System.Linq;




public class attack : MonoBehaviour
{
    private Pathfinder pathfinder;
    private List<OverlayTile> inRangeTiles;
     private CharacterInfo character;
     public bool isAttackActive = false;
     public GameObject enemy;
     public bool enemyInRange = false;

     public GameManager gameManager;

    
    public bool attackReady = false; // this determines if enemy is detected in range when attack btn is first pressed

    public bool attackQueued = false; 

    public int queTurnNumber = 0;


    // Start is called before the first frame update
    void Start()
    {
       // pathfinder = new Pathfinder();
       pathfinder = new Pathfinder();

       inRangeTiles = new List<OverlayTile>();
        
        character = GameObject.Find("dragon_child").GetComponent<CharacterInfo>();

        enemy = GameObject.Find("enemy_idle_01");

        gameManager = FindObjectOfType<GameManager>();

        //gameManger = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (isAttackActive)
    {
       // HighlightAttackableTiles();
      // AttackEnemy();
       // UpdateAttackTiles();
    }
        
    }
    

    public void GetInRangeTiles() {
      
    if (isAttackActive && !enemyInRange) {
        foreach(var item in inRangeTiles) {
            item.HideTile();
        }
        return;
    }

    /* I don't think I actually need this
    foreach(var item in inRangeTiles){
        item.HideTile();
    }*/

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

        //(Vector2.Distance(item.transform.position, enemy.transform.position) < 0.1f) 

        // Check if the enemy is standing on the tile
       if (Vector2.Distance(item.transform.position, enemy.transform.position) <= 0.5f) {
        item.ShowTileAttack();
        enemyInRange = true;

        Debug.Log("Enemy is in range");
    } 
        
    }
  //  OverlayTile mouseTile = GetTileUnderMouse();
       // Debug.Log(mouseTile);
}



/*
public void ToggleAttack() {
    isAttackActive = !isAttackActive;
    if (isAttackActive) {

        // Disable overlay tiles while attack is active (to prevent movement in scene when clicking attack button overlayed on screen)
        foreach(var item in inRangeTiles) {
            item.GetComponent<Collider2D>().isTrigger = true;
        }
    } else {
        // Enable overlay tiles again when attack is toggled off
        foreach(var item in inRangeTiles) {
            item.GetComponent<Collider2D>().isTrigger = false;
            item.HideTile();
            enemyInRange = false;
        }
    }
} */



// Trying to get this darn thing to work
/*
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
        }
    }
    else
    {
        if (enemyInRange == true && queTurnNumber == 2)
        {
            PerformPunchAttack();
            attackQueued = false;
            ClearAttackTiles();
        }
        isAttackActive = false;
    }

    if (queTurnNumber == 2) {
        attackReady = true;
        queTurnNumber = 0;
    } else {
        attackReady = false;}
}
*/

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
    }

    // Reset queTurnNumber to 0 when attack is toggled off
    if (!isAttackActive)
    {
        queTurnNumber = 0;
    }
}



    private void ClearAttackTiles()
    {
        foreach (var item in inRangeTiles)
        {
            item.HideTile();
        }
        
    } 






/*
     private void UpdateAttackTiles()
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
                    foreach(var item in inRangeTiles) {
            item.HideTile();
        }
                }
            }
        }
        else
        {
            foreach (var tile in inRangeTiles)
            {
                tile.HideTile();
            }
        }
    }
*/



 public void PerformPunchAttack() {
        if(attackReady == true) {
            Debug.Log("Punch attack");
            enemy.GetComponent<Enemy>().TakeDamage(.1f);
            attackReady = false;
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
            if (enemy != null)
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
