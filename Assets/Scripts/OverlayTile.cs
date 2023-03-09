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


    // Update is called once per frame
    public void Update()
    {
        poop = transform.position;


        if (Input.GetMouseButtonDown(0))
        {
            HideTile();
        }
    }


    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void HideTile()
    {

        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

    }

    
}
