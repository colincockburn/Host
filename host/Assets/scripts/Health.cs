using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health :  MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int StartHealth;
    public int currHealth { get; private set; }

    private void Awake()
    {
        currHealth=StartHealth;
    }
    public void TakeDamage(int dmg)
    {
        currHealth= Mathf.Clamp(currHealth -dmg,0,StartHealth);
        if (currHealth>0)
        {
           
        }
       
    }

    private void Update()
    {
      
    }
}