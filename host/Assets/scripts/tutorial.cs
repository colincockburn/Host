using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(GameManager.instance.controlledObject.transform.position.x < -565f)  
        {
        GameManager.instance.controlledObject.GetComponent<Combatants>().doubleJumped = true;
        }
    }
}
