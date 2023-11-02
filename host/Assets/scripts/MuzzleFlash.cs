using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    
    void DestroyFlash(UnityEngine.AnimationEvent animationEvent){
        Destroy(gameObject);
    }
}
