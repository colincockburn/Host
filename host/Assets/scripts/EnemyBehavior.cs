using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private Combatants combatants;
    private Vector2 playerPosition;
    public bool playerDetected;
    public Vector2 vectorToPlayer;
    public float distanceToPlayer;
    public float detectionDistance = 12f;
    public float stopAdvancingDistance;


    // Start is called before the first frame update
    void Start()
    {
        combatants = GetComponent<Combatants>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!combatants.hosted)
        {
            CheckPlayerDetected();
        }
        if(combatants.hosted || combatants.confused) playerDetected = false;
    }

    public void CheckPlayerDetected(){
        GameObject player = GameManager.instance.controlledObject;
        playerPosition = player.transform.position;
        vectorToPlayer = player.transform.position - 
            gameObject.transform.position;
        distanceToPlayer = vectorToPlayer.magnitude;

       RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, playerPosition);

        if (hits.Length >= 2){
            Collider2D collider = hits[1].collider;

            if (distanceToPlayer < detectionDistance && 
                !GameManager.instance.controlledObject.
                GetComponent<Combatants>().dead && !combatants.confused && 
                collider is BoxCollider2D && !combatants.hosted)
            {
                playerDetected = true;
            } else {
                playerDetected = false;
            }
        }
    }


    public bool WillFallOffEdge(float dir)
    {

        List<Vector3> positions = new List<Vector3>();

        GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();
        Vector3 smallest = new Vector3 (10000, 10000, 10000);
        foreach (GameObject possibleEdgeObject in gameObjects)
        {
            if (possibleEdgeObject.layer == 9)
            {
                Vector3 vectorToEdge = possibleEdgeObject.transform.position 
                                       - gameObject.transform.position;
                if(smallest != null){
                if (vectorToEdge.magnitude < smallest.magnitude){
                    smallest = vectorToEdge;
                }}
                if (vectorToEdge.magnitude < stopAdvancingDistance &&
                    Mathf.Sign(vectorToEdge.x) == Mathf.Sign(dir)){
                    return true;
                }
            }
        }
        return false;
    }

}

