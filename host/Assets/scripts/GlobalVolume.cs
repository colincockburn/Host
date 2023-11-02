using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolume : MonoBehaviour
{
    public HostMechanic hostMechanic;
    public ChromaticAberration chromaticAberration;
    public Volume globalVolume;
    // Start is called before the first frame update
    void Start()
    {
        globalVolume = GetComponent<Volume>();
        if (globalVolume != null)
        {
            // Try to get the ChromaticAberration effect from the Volume component
            if (globalVolume.profile.TryGet(out chromaticAberration))
            {
                // ChromaticAberration effect found
                Debug.Log("Chromatic Aberration effect found!");
            }
            else
            {
                // ChromaticAberration effect not found
                Debug.LogWarning("Chromatic Aberration effect not found!");
            }
        }
        hostMechanic = GameManager.instance.GetComponent<HostMechanic>();
    }

    // Update is called once per frame
    void Update()
    {
        chromaticAberration.intensity.value = hostMechanic.intensity;
        
    }
}
