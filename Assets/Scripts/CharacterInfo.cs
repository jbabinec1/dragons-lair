using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public OverlayTile activeTile;
    public OverlayTile startingTile;

     public int stepsTaken;
     public int turnsTaken;

        void Start()
    {
      
        stepsTaken = 0;
        turnsTaken = 0;
    }
}
