using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // creed instance of this class that can be accessed by other scripts
    public static GameManager instance;
    private HostMechanic hostMechanic;
    public bool playerDead = false;
    // this object will be set to whatever is controlled by the player
    [SerializeField] public GameObject controlledObject;
    // this object will always be set to the main character object which disappears when the host mechanic is used.
    [SerializeField] public GameObject mainCharacterObject;
    public StartingPosition[] startingArray;
    public GameObject meleePrefab;
    public GameObject gunPrefab;
    public GameObject playerPrefab;
    public GameObject brutePrefab;
    public GameObject bomberPrefab;
    public float deathHeight;
    public float healthRegen;
    public float healthRegenTimerStart;
    public float healthRegenTimer;


    public struct StartingPosition
    {
        public Vector3 pos;
        public char type;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        hostMechanic = GetComponent<HostMechanic>();
        startingArray = new StartingPosition[]{
            new StartingPosition{pos =
                GameManager.instance.mainCharacterObject.
                transform.position, type = 'p'}
        };
        SaveStartingPositions();
    }

    // Update is called once per frame
    void Update()
    {
        CheckFallDeath();
        HealthRegen();
    }

    void SaveStartingPositions()
    {
        GameObject[] objectsInScene = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in objectsInScene)
        {
            if(obj.GetComponent<Combatants>() != null && obj != 
                GameManager.instance.mainCharacterObject)
            {
                StartingPosition pos = new StartingPosition();
                if(obj.GetComponent<MeleeGrunt>() != null)
                {
                    pos.type = 'm';
                }
                if(obj.GetComponent<gunGrunt>() != null){
                    pos.type = 'g';
                }
                if(obj.GetComponent<Brute>() != null){
                    pos.type = 'b';
                }
                if(obj.GetComponent<Bomber>() != null){
                    pos.type = 'x';
                }
                pos.pos = obj.transform.position;
                System.Array.Resize(ref startingArray, startingArray.Length 
                + 1);
                startingArray[startingArray.Length-1] = pos;
            }
        }
    }

    public void ReloadCheckpoint(){

        hostMechanic.hostCharge = hostMechanic.hostChargeMax;

        GameObject[] objectsInScene = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in objectsInScene)
        {
            if(obj.GetComponent<Combatants>() != null)
            {
                Destroy(obj);
            }
        }

        foreach (StartingPosition startingPosition in startingArray){

            Debug.Log(startingPosition.pos);
            if(startingPosition.type == 'p')
            {
                GameObject combatant = Instantiate(playerPrefab, startingPosition.pos, Quaternion.identity);
                GameManager.instance.mainCharacterObject = combatant;
                GameManager.instance.controlledObject = combatant;
            }
            else if (startingPosition.type == 'm')
            {
                GameObject combatant = Instantiate(meleePrefab, startingPosition.pos, Quaternion.identity);
            }
            else if (startingPosition.type == 'g')
            {
                GameObject combatant = Instantiate(gunPrefab, startingPosition.pos, Quaternion.identity);
            }
            else if (startingPosition.type == 'b')
            {
                GameObject combatant = Instantiate(brutePrefab, startingPosition.pos, Quaternion.identity);
            }
            else if (startingPosition.type == 'x')
            {
                GameObject combatant = Instantiate(bomberPrefab, startingPosition.pos, Quaternion.identity);
            }
        }

    }

    void CheckFallDeath(){
        if (GameManager.instance.controlledObject.transform.position.y < deathHeight)
        {
            ReloadCheckpoint();
        }
    }
    void HealthRegen()
    {
        GameObject player = GameManager.instance.mainCharacterObject;
        if (player.GetComponent<Combatants>().health < player.GetComponent<Combatants>().startHealth && healthRegenTimer < 0)
        {
            player.GetComponent<Combatants>().health += healthRegen;
            healthRegenTimer = healthRegenTimerStart;
        }
        healthRegenTimer -= Time.deltaTime;
    }
}
