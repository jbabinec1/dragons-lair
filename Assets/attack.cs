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



    // Start is called before the first frame update
    void Start()
    {
       // pathfinder = new Pathfinder();
       pathfinder = new Pathfinder();

       inRangeTiles = new List<OverlayTile>();
        
        character = GameObject.Find("dragon_child").GetComponent<CharacterInfo>();

        enemy = GameObject.Find("enemy_idle_01");
    }

    // Update is called once per frame
    void Update()
    {

        if (isAttackActive)
    {
       // HighlightAttackableTiles();
        // AttackEnemy();
    }
        
    }

    public void GetInRangeTiles() {
      
    if (isAttackActive) {
        foreach(var item in inRangeTiles) {
            item.HideTile();
        }
        return;
    }

    


    foreach(var item in inRangeTiles){
        item.HideTile();
    }

    if (character.startingTile != null) {
        inRangeTiles = pathfinder.GetTilesInRange(character.startingTile, 3);
    } else {
        inRangeTiles = new List<OverlayTile>();
    }

    if (character.activeTile != null) {
        inRangeTiles.AddRange(pathfinder.GetTilesInRange(character.activeTile, 1));
    }

    foreach(var item in inRangeTiles){

        item.ShowTile();
        
    }
    OverlayTile mouseTile = GetTileUnderMouse();
        Debug.Log(mouseTile);
}



 /*  public void GetInRangeTiles() {

    foreach(var item in inRangeTiles){
        item.HideTile();
    }

    if (character.startingTile != null) {
        inRangeTiles = pathfinder.GetTilesInRange(character.startingTile, 3);
    }
    else {
        inRangeTiles = new List<OverlayTile>();
    }

    if (character.activeTile != null) {
        inRangeTiles.AddRange(pathfinder.GetTilesInRange(character.activeTile, 1));
    }

    foreach(var item in inRangeTiles){
        item.ShowTile();
    }
} */



  /* public void ToggleAttack() {
    isAttackActive = !isAttackActive;
    if (!isAttackActive) {
        foreach(var item in inRangeTiles) {
            item.HideTile();
        }
    }
} */

public void ToggleAttack() {
    isAttackActive = !isAttackActive;
    if (isAttackActive) {
        // Disable overlay tiles while attack is active
        foreach(var item in inRangeTiles) {
            item.GetComponent<Collider2D>().isTrigger = true;
        }
    } else {
        // Enable overlay tiles again when attack is toggled off
        foreach(var item in inRangeTiles) {
            item.GetComponent<Collider2D>().isTrigger = false;
            item.HideTile();
        }
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


private void AttackEnemy()
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
