using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public LevelTransition levelTransition;
    public float activeDistance;

    void Update()
    {
        Vector3 path = 
        gameObject.transform.position - 
            GameManager.instance.controlledObject.transform.position;
        float distance = path.magnitude;
        if (distance < activeDistance)
        {
            levelTransition.FadeToNextLevel();
        }
    }
}