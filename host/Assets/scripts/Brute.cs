using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : MonoBehaviour
{
    public float chargeTimerStart;
    public  float chargeCoolDownTimer;
    public float chargeCoolDownTimerStart;
    public float chargeTimer;
    public float chargeSpeed;
    private Combatants combatants;
    private EnemyBehavior enemyBehavior;
    public  bool charging;
    public int chargingDir;
    public float AIMoveSpeed;
    public bool groundSlamAttempted;
    public bool preparingCharge;
    public bool hostCheckForGroundSlam;
    public bool chargeOverGroundSlam;

    void Start()
    {
        combatants = GetComponent<Combatants>();
        enemyBehavior = GetComponent<EnemyBehavior>(); 
        chargeTimer = chargeTimerStart;
    }

    // Update is called once per frame
    void Update()
    {
        combatants.movementRestricted = (preparingCharge || charging) ? true : false;   

        if(! combatants.hosted && !combatants.dead){
            AIControlled();
        } else {
            chargeCoolDownTimer -= Time.deltaTime;
        
            if (Input.GetKey(KeyCode.Mouse1) && chargeCoolDownTimer < 0) 
            {

                StartCoroutine(PrepareCharge());
            }
        }
        if (charging && !combatants.dead){
            Charge(chargingDir);
            chargeTimer -= Time.deltaTime;
            if (chargeTimer < 0) {
                chargeTimer = chargeTimerStart;
                charging = false;

                chargeCoolDownTimer = chargeCoolDownTimerStart;
            }
        }
        if (combatants.IsGrounded())
        {
            combatants.attemptGroundSlam = false;
        }
    }

    private void LateUpdate() {
        if(!combatants.hosted && hostCheckForGroundSlam){
            combatants.rb.velocity = new Vector2(combatants.dirX*3, combatants.jumpForce);
            StartCoroutine(GroundSlamWithDelay());
            groundSlamAttempted = true;
        }
        hostCheckForGroundSlam = false;
    }



    public void AIControlled(){
        // checks the enemy behavior script to see if the enemy has detected the player
        if (enemyBehavior.playerDetected && !combatants.confused)
        {
            chargeCoolDownTimer -= Time.deltaTime;
            if (chargeCoolDownTimer+1 < 0 && chargeOverGroundSlam) 
            {
                StartCoroutine(PrepareCharge());
                groundSlamAttempted = false;
                chargeOverGroundSlam = false;

            }
            else if (chargeCoolDownTimer < 0 && !chargeOverGroundSlam && enemyBehavior.vectorToPlayer.magnitude < 2 && combatants.IsGrounded() && !groundSlamAttempted)
            {
                hostCheckForGroundSlam = true;
                chargeCoolDownTimer = chargeCoolDownTimerStart;
                chargeOverGroundSlam = true;
            }
            // move towards the player if they too far away to attack
            if (Mathf.Abs(enemyBehavior.vectorToPlayer.x) > 
                enemyBehavior.stopAdvancingDistance && !combatants.movementRestricted)
            {
                if (enemyBehavior.vectorToPlayer.x > 0){
                    combatants.dirX = 1f;
                } else {
                    combatants.dirX = -1f;
                }
            // if you are close enough to attack then melee instead of move
            } else if(combatants.IsGrounded() && GameManager.instance.controlledObject.GetComponent<Combatants>().IsGrounded()){
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
        if (combatants.damageTimer <= 0){
            if(combatants.IsGrounded()){
                combatants.rb.velocity = new Vector2(combatants.dirX*AIMoveSpeed,
                combatants.rb.velocity.y);
            } else {
                combatants.InAirMovement(combatants.dirX);
            }
        }


    }

    void Charge(int dirX){

        if(combatants.IsGrounded()){
            combatants.rb.velocity = new Vector2 (dirX * chargeSpeed, combatants.rb.velocity.y);
            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(
            combatants.attackPosition.position, combatants.attackRange,
            combatants.whatIsEnemies);

            if (enemiesToDamage.Length != 0 && !combatants.meleeCooling){
                float knockIncrease = Mathf.Abs(combatants.rb.velocity.x) * 1.5f;
                combatants.knockbackPower += knockIncrease;
                combatants.melee();
                combatants.knockbackPower -= knockIncrease;
                combatants.meleeCooling = true;
                chargeTimer = 0.3f;
                foreach(Collider2D collider in enemiesToDamage){
                    collider.GetComponent<Combatants>().confusedTimer = 0.5f;
                }
            }
        }
    }

    IEnumerator GroundSlamWithDelay()
    {
        
        yield return new WaitForSeconds(0.25f);
        combatants.attemptGroundSlam = true;
    }

    IEnumerator PrepareCharge()
    {
        preparingCharge = true;
        yield return new WaitForSeconds(0.5f);
        chargingDir = ((transform.localScale.x > 0)) ? 1 : -1;
        charging = true;
        preparingCharge = false;
        
    }
}

