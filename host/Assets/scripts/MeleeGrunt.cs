using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MeleeGrunt : MonoBehaviour
{
    // this gets variables from the combatants and the enemy behavior air scripts
    private Combatants combatants;
    private EnemyBehavior enemyBehavior;
    private Rigidbody2D rb;
    public float AIMoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
        // get the components from the other scripts on the object. again this just lets us access variables from the other scripts on the same object. the components work like objects
        combatants = GetComponent<Combatants>();
        enemyBehavior = GetComponent<EnemyBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if(! combatants.hosted && !combatants.dead){
            AIControlled();
        } else {
            combatants.confused = true;
        }
    }


    // a I control for melee grunt. 
    public void AIControlled(){
        // checks the enemy behavior script to see if the enemy has detected the player
        if (enemyBehavior.playerDetected && !combatants.confused)
        {
            // move towards the player if they too far away to attack
            if (Mathf.Abs(enemyBehavior.vectorToPlayer.x) > 
                enemyBehavior.stopAdvancingDistance)
            {
                if (enemyBehavior.vectorToPlayer.x > 0){
                    combatants.dirX = 1f;
                } else {
                    combatants.dirX = -1f;
                }
            // if you are close enough to attack then melee instead of move
            } else  if(GameManager.instance.controlledObject.GetComponent<Combatants>().IsGrounded()){
                combatants.dirX = 0;
                combatants.attemptMelee = true;
            }
            // if will fall off edge cancle movment
            if (enemyBehavior.WillFallOffEdge(combatants.dirX)){
                combatants.dirX = 0;
            }
        // if player not detected don't move
        } else {
          combatants.dirX = 0;
        }
        if (combatants.damageTimer <= 0 && combatants.timeBetweenAttack <= 0){
            if(combatants.IsGrounded()){
                rb.velocity = new Vector2(combatants.dirX*AIMoveSpeed,
                rb.velocity.y);
            } else {
                combatants.InAirMovement(combatants.dirX);
            }
        }
    }

}
