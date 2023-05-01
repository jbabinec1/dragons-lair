using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public CharacterInfo character;
    Vector3 localScale;
    private GameObject enemy;

    private float foregroundWidth;


    // Start is called before the first frame update
    void Start()
    {

        enemy = GameObject.Find("enemy_idle_01");

       // Get a reference to the CharacterInfo component on the dragon_child GameObject 
       //enemy = enemy.GetComponent<Enemy>();

       localScale = transform.localScale;
        
         
    }

    // Update is called once per frame
    void Update()
    {

    localScale.x = enemy.GetComponent<Enemy>().currentHealth;
    transform.localScale = localScale;    

        
    }
}
