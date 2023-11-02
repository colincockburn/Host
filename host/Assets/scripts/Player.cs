using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private bool playerDead = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    

    // unity automatically run this function if this object is destroyed.
    private void OnDestroy()
    {

        GameManager.instance.playerDead = true;

    }


}
