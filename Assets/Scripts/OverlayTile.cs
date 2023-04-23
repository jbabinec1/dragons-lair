using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class OverlayTile : MonoBehaviour
{
    [SerializeField] public int G;
    [SerializeField] public int H;

    public int F { get { return G + H; } }

    [SerializeField] public bool isBlocked;

    [SerializeField] public OverlayTile previous;

    [SerializeField] public Vector3Int gridLocation;

    [SerializeField] public Vector3 poop;

    // In range tiles highlighted when attack btn is pressed
    public bool activatedTile = false;


    // Update is called once per frame
    public void Update()
    {
        poop = transform.position;


      //  if (Input.GetMouseButtonDown(0))
      //  {
       //     HideTile();
      //  }
    }


    public void ShowTileAttack()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }


    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void HideTile()
    {

        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

    }


    public void HighlightRed()
    {
    GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void ResetHighlight()
    {
    GetComponent<SpriteRenderer>().color = Color.white;
    }


    
}
