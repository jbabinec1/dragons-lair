using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour
{
    private Pathfinder pathfinder;
     public List<OverlayTile> inRangeTiles = new List<OverlayTile>();
     private CharacterInfo character;
     public bool isAttackActive = false;

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = new Pathfinder();
        
        character = GameObject.Find("dragon_child").GetComponent<CharacterInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


 public void GetInRangeTiles() {

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
}



   public void ToggleAttack() {
    isAttackActive = !isAttackActive;
    if (!isAttackActive) {
        foreach(var item in inRangeTiles) {
            item.HideTile();
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
