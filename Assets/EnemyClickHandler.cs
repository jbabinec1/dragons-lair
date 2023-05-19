using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClickHandler : MonoBehaviour
{

    private attack attackScript;
    private MouseController mouseController;

    public GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        attackScript = FindObjectOfType<attack>();
        mouseController = FindObjectOfType<MouseController>();
        enemy = GameObject.Find("enemy_idle_01");
    }

    void OnMouseDown(){
        attackScript.SetSelectedEnemy(gameObject);
        mouseController.MoveTowardsEnemy(enemy);
      //  attackScript.ToggleAttack(); //Replace this with method that moves enemy towards player and attacks
    }
}
