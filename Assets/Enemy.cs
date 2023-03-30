using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float visionAngle = 45f;
    public float visionDistance = 10f;
    public LayerMask playerLayer;

    private GameObject player;
    private bool isPlayerVisible = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        // Check if the player is visible
        CheckForPlayer();

        // If the player is visible, initiate combat
        if (isPlayerVisible)
        {
           // InitiateCombat();
        }
    }

  void CheckForPlayer()
{
    // Calculate the direction from the enemy to the player
    Vector2 directionToPlayer = player.transform.position - transform.position;

    // Calculate the angle between the enemy's forward vector and the direction to the player
    float angleToPlayer = Vector2.Angle(transform.up, directionToPlayer);

    // Check if the angle is within the vision cone
    if (angleToPlayer <= visionAngle / 2f)
    {
        // Check if the player is within the vision distance
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToPlayer.normalized, visionDistance, playerLayer);
        isPlayerVisible = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.tag == "Player" && !hit.collider.isTrigger)
            {
                //Debug.Log("Hit collider tag: " + hit.collider.tag);
                // The player is visible
                isPlayerVisible = true;
                break;
            }
        }

        // Draw a debug raycast to show the direction and length of the raycast
        Debug.DrawRay(transform.position, directionToPlayer.normalized * visionDistance, isPlayerVisible ? Color.green : Color.red);
    }
    else
    {
        // The player is not within the vision cone
        isPlayerVisible = false;
    }
}




    void InitiateCombat()
    {
        // Code to initiate turn-based combat with the player
        Debug.Log("Initiating combat!");
    }
}
