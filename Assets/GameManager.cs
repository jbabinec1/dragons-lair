using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int turnNumber = 1;
    public bool isPlayerTurn = true;
    public bool isEnemyTurn = false;
    public GameObject enemy;

    public bool isPlayerVisible = false;

    public static OverlayTile enemyStartingTile;

void Start() 
{
    enemy = GameObject.FindGameObjectWithTag("Enemy");

    // ... code to find the enemy game object ...
    Collider2D[] colliders = Physics2D.OverlapPointAll(enemy.transform.position);
    foreach (Collider2D collider in colliders)
    {
        OverlayTile tile = collider.GetComponent<OverlayTile>();
        if (tile != null)
        {
            Debug.Log("Enemy starting tile: " + enemyStartingTile);
            enemyStartingTile = tile;
            break;
        }
    }

   // Debug.Log(enemyStartingTile);
}



    public void EndTurn()
    {
        if (isPlayerTurn && isPlayerVisible)
        {
            // Player turn is over, start the enemy turn
            isPlayerTurn = false;
           // Debug.Log("End of Player Turn");
        }
        else
        {
            // Enemy turn is over, start the player turn
            isPlayerTurn = true;
            turnNumber++;
           // Debug.Log("End of Enemy Turn");
        }
    }


}
