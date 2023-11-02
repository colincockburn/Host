using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public AudioSource source;
    public static AudioManager instance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake() {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
    }

    public void Play(string name)
    {
        foreach(Sound s in sounds)
        {
            if(s.name == name)
            {
                source.PlayOneShot(s.clip);
                return;
            }
        }
        Debug.Log("sound not found");
    }
}
