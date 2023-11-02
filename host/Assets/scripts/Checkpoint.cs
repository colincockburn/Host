using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public bool isActivated = false;
    public float activeDistance;

    void Update()
    {
        Vector3 path = 
        gameObject.transform.position - 
            GameManager.instance.controlledObject.transform.position;
        float distance = path.magnitude;
        if (distance < activeDistance)
        {
            isActivated = true;
            if (GameManager.instance.startingArray.Length > 0){
                GameManager.instance.startingArray[0].pos = 
                transform.position;
            }
        }
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activeDistance);
    }

}

