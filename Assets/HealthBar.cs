using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthBar : MonoBehaviour
{
    public CharacterInfo character;
    Vector3 localScale;
    private GameObject dragonChild;

    private float foregroundWidth;

    private void Start()
    {
        dragonChild = GameObject.FindGameObjectWithTag("Player");

       // Get a reference to the CharacterInfo component on the dragon_child GameObject 
       character = dragonChild.GetComponent<CharacterInfo>();

       localScale = transform.localScale;
       
      

    }

    private void Update()
    {

     // Update the localScale of the foreground bar based on the dragon's health
    localScale.x = character.currentHealth;
    transform.localScale = localScale;

      
    
    }
}
