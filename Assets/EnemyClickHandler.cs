using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClickHandler : MonoBehaviour
{

    private attack attackScript;

    // Start is called before the first frame update
    void Start()
    {
        attackScript = FindObjectOfType<attack>();
    }

    void OnMouseDown(){
        attackScript.SetSelectedEnemy(gameObject);
        attackScript.ToggleAttack();
    }
}
