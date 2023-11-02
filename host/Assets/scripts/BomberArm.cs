using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberArm : MonoBehaviour
{
    public GameObject bomber;
    public bool throwForward;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetIdle(UnityEngine.AnimationEvent animationEvent){
        SetBool("throwBomb", false);
    }

    public void HandleAnimations()
    {
        throwForward = Mathf.Sign(bomber.transform.localScale.x) == 
                Mathf.Sign(bomber.GetComponent<Bomber>
                ().target.x - 
                bomber.transform.position.x);
            if (throwForward)
            {
                SetBool("throwBomb", true);
            }
            else
            {
                SetBool("throwBombBackwards", true);
            }  
    }

    public void SetBool(string parameterName, bool value)
    {
        // Get an array of all the bool parameters in the animator
        AnimatorControllerParameter[] parameters = bomber.GetComponent<Bomber>().armAnimator.parameters;

        // Loop through each parameter and set it to false, except for the one we want to set
        foreach (AnimatorControllerParameter parameter in parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name != parameterName)
            {
                bomber.GetComponent<Bomber>().armAnimator.SetBool(parameter.name, false);
            }
        }
        // Set the desired parameter to the desired value
        bomber.GetComponent<Bomber>().armAnimator.SetBool(parameterName, value);
    }

}
