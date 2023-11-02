using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private bool hasReachedCheckpoint = false;
    private Vector2 respawnPosition;

    // Called when a player reaches the checkpoint
    public void SetReached(Vector2 position)
    {
        hasReachedCheckpoint = true;
        respawnPosition = position;
    }

    // Called when the player needs to respawn
    public Vector2 GetRespawnPosition()
    {
        if (hasReachedCheckpoint)
        {
            return respawnPosition;
        }
        else
        {
            return transform.position; // respawn at the start if no checkpoint has been reached
        }
    }
    public bool HasReachedCheckpoint()
    {
        return hasReachedCheckpoint;
    }

}
    // Start is called before the first frame update
   /*
    private static Combatants combatantScript;
    public GameObject checkpoint;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
         combatantScript = GameManager.instance.controlledObject.GetComponent<Combatants>();
         
    }
   
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {

            combatantScript.UpdatePosition(transform.position);
        }
    }

   
}
*/
