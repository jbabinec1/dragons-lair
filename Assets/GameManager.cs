using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int turnNumber = 1;
    public bool isPlayerTurn = true;
    public bool isEnemyTurn = false;

    // Other code for your GameManager goes here...

    public void EndTurn()
    {
        if (isPlayerTurn)
        {
            // Player turn is over, start the enemy turn
            isPlayerTurn = false;
            isEnemyTurn = true;
            Debug.Log("End of Player Turn");
        }
        else
        {
            // Enemy turn is over, start the player turn
            isPlayerTurn = true;
            turnNumber++;
            Debug.Log("End of Enemy Turn");
        }
    }
}
