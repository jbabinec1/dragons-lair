using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public OverlayTile activeTile;
    public OverlayTile startingTile;

     public int stepsTaken;
     public int turnsTaken;

     public float maxHealth = 1;
     public float currentHealth = 1;

        void Start()
    {
      
        stepsTaken = 0;
        turnsTaken = 0;
    }
}
