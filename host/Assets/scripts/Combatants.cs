using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this script gets attached to every enemy and the player object and has all the shared functionality between combatant object. individual functionality like a I will be found on individual scripts for specific types of enemies and the player.
public class Combatants : MonoBehaviour
{
    private AudioManager audioManager;
    public float timeBetweenAttack;
    static float startTimeBetweenAttack = 0.4f;
    public float confusedStartTime = 1;
    public float confusedTimer;
    public bool confused = false;
    public bool takingDamage;
    public float damageTimer = 0;
    public float startDamageTimer = 0.2f;
    public bool meleeCooling;
    public Rigidbody2D rb;
    private BoxCollider2D coll;
    public float dirX;
    public Transform attackPosition; 
    public LayerMask whatIsEnemies;
    public float attackRange;
    public bool attemptMelee = false;
    public int damage = 34;
    public int normalDamage = 34;
    public int hostedDamage = 50;
    public float health = 100f;
    public int startHealth = 100;
    public float knockbackPower = 5;
    private Vector2 knockbackVelocity;
    public bool hosted;
    public bool isWallSliding;
    private float wallSlidingSpeed = 2f;
    private float wallSlideSpeedTimer;
    public float wallSlideSpeedTimerMax = 0.75f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    public Vector2 wallJumpingPower = new Vector2(8f, 16f);
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float airAcceleration = 1f;
    [SerializeField] private float moveSpeed = 7f;
    public float jumpForce =20f;
    private Animator animator;
    public bool dead = false;
    public bool doubleJumped;
    public bool movementRestricted;
    public bool attemptingCrouch;
    public bool crouching;
    public bool sliding;
    public float decellerationX;
    public float decellerationRate;
    public float slideVelocityX;
    public int comboCounter;
    public float meleeLunge;
    public bool upperCut;
    public  bool groundSlamFalling;
    public  bool groundSlamming;
    public bool attemptGroundSlam;
    public bool midAirMeleeing;
    public GameObject punchWavePrefab;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        attackPosition.localPosition = new Vector2(1,0);
        comboCounter = 0;
        audioManager = AudioManager.instance;
    }


    // Update is called once per frame
    void Update()
    {
        if (health <= 0){
            gameObject.transform.localScale = (knockbackVelocity.x < 0) ?
            new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            HandleDeath();
        }
        if (!dead){

        // check if the object is the one controlled by the player
        hosted = (GameManager.instance.controlledObject == gameObject) ?
         true : false;
        if (hosted){
            confusedTimer = confusedStartTime;
            // change layer to player layer and enemies to enemy layer
            gameObject.layer = 8;
            gameObject.tag="Player";
            damage = hostedDamage;

            whatIsEnemies = LayerMask.GetMask("enemy");
            // check if player is taking damage
            if (!takingDamage && !movementRestricted){
                if (!groundSlamming){
                    HostedMovement();
                }
                
                GroundSlam();
            } 
        } else {
            // if not controlled by enemy than set whatIsEnemies layer to player and object layer to enemy
            whatIsEnemies = LayerMask.GetMask("player");
            gameObject.layer = 7;
            damage = normalDamage;
            GroundSlam();
        }
        // move Melee area to the right side of object based on movement
        if (damageTimer <= 0){
            takingDamage = false;
            CheckForMelee();
        }
        SlideNCrouch();
        orientObject();
        DecrementTimers();
        }
        HandleAnimations();
    }


    // player controlled movement
    public void HostedMovement()
    {
        if (!crouching){
            dirX = Input.GetAxisRaw("Horizontal");
            if(timeBetweenAttack > 0 && !midAirMeleeing){
                dirX = 0;
            }
        }
        if (IsGrounded() && !takingDamage && !crouching && !groundSlamFalling){
            
            if (timeBetweenAttack < 0)
            {
                rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
            }
            else if (!attemptingCrouch)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        } else {
            if (!isWallJumping && !upperCut)
            {
                InAirMovement(dirX);
            }
        }

        if (IsGrounded() || isWallSliding)
        {
            doubleJumped = false;
            if (Input.GetButtonDown("Jump" ))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        } else {
            if (Input.GetButtonDown("Jump" ) && !doubleJumped)
            {
                doubleJumped = true;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        if(!(groundSlamFalling || groundSlamming)){
            WallSlide();
            WallJump();
        }

    }

    // applies acceleration so a fixed velocity and the air based on the 
    // direction given in the parameter.
    public void InAirMovement(float dir){

        // Calculate the acceleration vector in the x-axis direction
        float accelerationX = dir * airAcceleration;

        // Add the acceleration vector to the current velocity
        rb.velocity += new Vector2(accelerationX * Time.deltaTime, 0.0f);

        // Limit the velocity to the maximum speed
        if (rb.velocity.x > moveSpeed)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
        else if (rb.velocity.x < -moveSpeed)
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }
    }

    void SlideNCrouch()
    {
        if(Input.GetKey(KeyCode.S) && hosted)
        {
            attemptingCrouch = true;
            if(IsGrounded()){
                coll.size = new Vector2(1.7f,1.1f);
                if (!crouching && Mathf.Abs(rb.velocity.y) < 0.001) rb.position -= new Vector2(0f, 0.45f);
                crouching = true;
                if (Mathf.Abs(rb.velocity.x) > 0.5f){
                    if (!sliding) slideVelocityX = rb.velocity.x;
                    sliding = true;
                    decellerationX = (rb.velocity.x > 0) ? decellerationRate : -decellerationRate;
                    slideVelocityX -= decellerationX * Time.deltaTime;
                    rb.velocity = new Vector2(slideVelocityX, rb.velocity.y);
                }
                else
                {
                    sliding = false;
                }
            }
            else 
            {
                crouching = false;
                coll.size = new Vector2(1.3f,2.2f);
            }
        }
        else{
            if (crouching) rb.position += new Vector2(0f, 0.45f);
            attemptingCrouch = false;
            crouching = false;
            sliding = false;
            coll.size = new Vector2(1.3f,2.2f);
        }
    }



    // ran every frame and updates animation based on what the player is doing. this saves us from putting animation functionality inside non animation based functions. 
    public void HandleAnimations(){
        if (IsGrounded()){
            if (dirX != 0f){
                SetBool("running", true);
            }
            else {
                // sets idle active
                SetBool("running", false);
            }
        }
        if (crouching)
        {
            SetBool("crouching", true);
        }
        if(sliding)
        {
            SetBool("sliding", true);
        }
        if (rb.velocity.y > 0.2)
        {
            SetBool("jumping", true);
            if (doubleJumped && transform.position.x > -565f){
                SetBool("doubleJump", true);
            }
        }
        if (rb.velocity.y < 0 && !IsGrounded()){
            if (IsWalled()){
                SetBool("wall sliding", true);
            } else {
                SetBool("falling", true);
            }
        }
        if(timeBetweenAttack < startTimeBetweenAttack && 
                timeBetweenAttack > 0){ 
            if(IsGrounded()){
                if(comboCounter == 1)
                {
                    SetBool("melee", true); 
                }
                else if (comboCounter == 2)
                {
                    SetBool("melee2", true); 
                }
                else if (comboCounter == 3)
                {
                    SetBool("melee3", true); 
                }
            }
        }
        if(midAirMeleeing){
            SetBool("movingMelee", true); 
        }
        
        if (upperCut && timeBetweenAttack < startTimeBetweenAttack && 
            timeBetweenAttack > 0)
            {
                SetBool("upperCut", true);
            }
        if(takingDamage){
            SetBool("takeDamage", true);
        }

        if(GetComponent<Brute>() != null){
            if(GetComponent<Brute>().charging){
                SetBool("charge", true);
            }
            if (GetComponent<Brute>().preparingCharge){
                SetBool("preparingCharge", true);
            }
            
        }

        if (groundSlamFalling){
            SetBool("groundSlamFalling", true);
        }
        if (groundSlamming){
            SetBool("groundSlamming", true);
        }

        if(dead){
            SetBool("dead", true);
        }
    }

    public void SetBool(string parameterName, bool value)
    {
        // Get an array of all the bool parameters in the animator
        AnimatorControllerParameter[] parameters = animator.parameters;

        // Loop through each parameter and set it to false, except for the one we want to set
        foreach (AnimatorControllerParameter parameter in parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name != parameterName)
            {
                animator.SetBool(parameter.name, false);
            }
        }
        // Set the desired parameter to the desired value
        animator.SetBool(parameterName, value);
    }

    public void DecrementTimers()
    {
        confusedTimer -= Time.deltaTime;
        damageTimer -= Time.deltaTime;
        timeBetweenAttack -= Time.deltaTime;
        wallSlideSpeedTimer -= Time.deltaTime;
        if (timeBetweenAttack < 0){
            meleeCooling = false;
        }
        if(confusedTimer > 0){
            confused = true;
        } else {
            confused = false;
        }
        
    }


    public void CheckForMelee()
    {
        if (timeBetweenAttack <= 0) 
        {
            upperCut = false;
            if (hosted)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (!(Input.GetKey(KeyCode.S) && !IsGrounded()))
                        melee();
                        if(IsGrounded())
                        {
                            comboCounter++;
                            // reset combo or limit gunGrunt to 1st melee
                            if ((hosted && comboCounter >= 4) || !hosted && GetComponent<gunGrunt>() != null)
                            {
                                comboCounter = 0;
                            }
                        }
                }
                if (timeBetweenAttack <= 0 && comboCounter >= 3 || timeBetweenAttack < -1.4) 
                {
                    comboCounter = 0;
                }
            }
            else if(attemptMelee && !confused)
            {
                Debug.Log(timeBetweenAttack);
                comboCounter = 1;
                melee();
                attemptMelee = false;
            }
        }
    }

    public void melee(){
        
        if (!crouching){
            upperCut = false;
        }
        else
        {
            upperCut = true;
            int jumpDirX = (transform.localScale.x > 0) ? 1 : -1;
            rb.velocity = new Vector2(3 * jumpDirX, jumpForce);
        }
        if (IsGrounded() && !crouching)
        {
            rb.position += (rb.transform.localScale.x > 0) ? new Vector2(meleeLunge, 0) : new Vector2(meleeLunge * -1f, 0);
        }

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(
            attackPosition.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++){

            Vector2 knockbackVector = 
            (enemiesToDamage[i].GetComponent<Combatants>
            ().rb.position - rb.position); 
            knockbackVector.Normalize();

            if (upperCut){
                int knockbackVX = (knockbackVector.x > 0) ? 1 : -1;
                knockbackVector = new Vector2(knockbackVX * 3, jumpForce);
                enemiesToDamage[i].GetComponent<Combatants>().confusedTimer = 0.7f;
                timeBetweenAttack -= 2f;    
            } 
            else 
            {
                // create a vector that is the difference between attacker
                // turn vector into velocity for knockback
                knockbackVector = new Vector2(knockbackVector.x * knockbackPower, knockbackVector.y * knockbackPower + 5);
            }                
            enemiesToDamage[i].GetComponent<Combatants>().TakeDamage(damage, knockbackVector);
        }

        if (enemiesToDamage.Length > 0){
            if (comboCounter == 0){
                audioManager.Play("punchHit1");
            }
            else if (comboCounter == 1){
                audioManager.Play("punchHit2");
            }
            else if (comboCounter == 2){
                audioManager.Play("punchHit3");
            }
            if (!IsGrounded()){
                float kickback = (transform.localScale.x > 0) ? -5 : 5;
                rb.velocity += new Vector2(kickback, 10);
            }
        }
        if(!IsGrounded()){
            midAirMeleeing = true;
        }
        timeBetweenAttack = startTimeBetweenAttack;
    }   

    void MidAirMeleeOver(){
        midAirMeleeing = false;
    }

    void GroundSlam()
    {
        // check if ground slam attempt and is valid time to attempt
        if ((Input.GetKey(KeyCode.S) && !IsGrounded() && Input.GetKey(KeyCode.Mouse0) && !groundSlamFalling && !upperCut && hosted)|| attemptGroundSlam)
        {
            attemptGroundSlam = false;
            // trigger ground slam fall
            groundSlamFalling = true;
            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(
                    transform.position, 20, whatIsEnemies);
            Vector2 bestDirection = Vector2.zero;
            for (int i = 0; i < enemiesToDamage.Length; i++){
                    Debug.Log(enemiesToDamage[i].gameObject);
                    Vector2 vectorToEnemy = 
                    (enemiesToDamage[i].GetComponent<Combatants>
                    ().rb.position - rb.position);
                    bool notBlocked = false;
                    RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, vectorToEnemy, 200f);
                    for (int j = 0; j < hits.Length; j++)
                    {
                        if 
                        (hits[j].collider.gameObject.
                        GetComponent<Combatants>() == null)
                        {
                            break;
                        }
                        else if (j > 0)
                        {
                            notBlocked = true;
                            break;
                        }
                        
                    }
                    Debug.Log(Mathf.Abs(vectorToEnemy.x) / 
                        Mathf.Abs(vectorToEnemy.y));
                    if 
                        (Mathf.Abs(vectorToEnemy.x) / 
                        Mathf.Abs(vectorToEnemy.y) < 0.7f 
                        && vectorToEnemy.y < 0 && 
                        vectorToEnemy.magnitude > 3f && notBlocked && GetComponent<Brute>() == null)
                    {
                        if (bestDirection == Vector2.zero || Mathf.Abs(vectorToEnemy.y) / Mathf.Abs(vectorToEnemy.x) < Mathf.Abs(bestDirection.y) / Mathf.Abs(bestDirection.x))
                        {
                            bestDirection = vectorToEnemy;
                        }
                    }
            }
            if (bestDirection == Vector2.zero)
            {
                float xDifference = (transform.localScale.x > 0) ? 3 : -3;
                rb.velocity += new Vector2(xDifference, -10);
            }
            else
            {
                Debug.Log("ya ariana");
                bestDirection.Normalize();
                rb.velocity = new Vector2(bestDirection.x * 10f, bestDirection.y * 10f);
            }
        }

        if (groundSlamFalling){
            // trigger ground slam
            if (IsGrounded()){
                groundSlamFalling = false;
                groundSlamming = true;
                float groundSlamRange = (hosted) ? attackRange + 1 : attackRange * 1.5f;
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(
                    transform.position - new Vector3(0,1,0), groundSlamRange, whatIsEnemies);
                for (int i = 0; i < enemiesToDamage.Length; i++){
                    Vector2 knockbackVector = 
                    (enemiesToDamage[i].GetComponent<Combatants>
                    ().rb.position - rb.position ); 
                    knockbackVector.Normalize();
                    knockbackVector = new Vector2(knockbackVector.x * 
                      knockbackPower*1.5f, knockbackVector.y * knockbackPower
                      + 10);
                    enemiesToDamage[i].GetComponent<Combatants>
                      ().TakeDamage(damage, knockbackVector);
                }
            }
        }
    }


    // explained in the update function
    public void orientObject(){
        if (isWallJumping){
            gameObject.transform.localScale = new Vector3(wallJumpingDirection, 1, 1);          
        }
        else if (dirX > 0){
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (dirX < 0){
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        // if walled flip scale
        if (isWallSliding){
            gameObject.transform.localScale = 
                new Vector3(-1*gameObject.transform.localScale.x, 1, 1);
            wallCheck.localPosition = 
             new Vector3(-0.62f, 0, 0);
        } else {
            wallCheck.localPosition = 
             new Vector3(0.62f, 0, 0);
        }
    }

    // applies damage to player destroys if the players health is zero and gives knock back to the object.
    public void TakeDamage(int damage, Vector2 knockbackVelocity){
        rb.velocity = new Vector2(knockbackVelocity.x, knockbackVelocity.y);
        health -= damage;

    
          //Destroy(gameObject, 0.1f);
        takingDamage = true;
        damageTimer = startDamageTimer;
    }

    async Task HandleDeath()
    {
        dead = true;
        await Task.Delay(TimeSpan.FromSeconds(1));
        if (gameObject == GameManager.instance.mainCharacterObject){
            GameManager.instance.ReloadCheckpoint();
        } else {
            Destroy(gameObject); 
        }
    }
   
    public bool IsGrounded(){
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, 
                                 Vector2.down, .1f, jumpableGround);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.4f, wallLayer);
    }

    private void WallSlide()
    {
        if (isWallSliding){
            wallSlideSpeedTimer -= Time.deltaTime;
            if (wallSlideSpeedTimer < 0){
                wallSlidingSpeed = 7f;
            }
            else{
                wallSlidingSpeed = 2f;
            }
        }
        else{
            wallSlideSpeedTimer = wallSlideSpeedTimerMax;
        }
        if (IsWalled() && !IsGrounded() && dirX != 0f){
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding){
            isWallJumping = false;
            wallJumpingDirection = -dirX;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f){
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * 
            wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
        }

        // need a section here for animation that flips player direction and
        // plays jumping animation

        Invoke(nameof(StopWallJumping), wallJumpingDuration);
    }

    private void StopWallJumping()
    {
        isWallJumping =false;
    }

    // draws the red circle round the attack area so you can see where the melee area is
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
        Gizmos.DrawWireSphere(wallCheck.position, 0.75f);
        Gizmos.DrawWireSphere(transform.position - new Vector3(0,1,0), attackRange *1.5f);
    }

    void CreatePunchWave(){
        Instantiate(punchWavePrefab, transform.position, Quaternion.identity);
    }

    void EndGroundSlam(UnityEngine.AnimationEvent animationEvent){
        groundSlamming = false;
    }
    // Animation Events
    // These functions are called inside the animation files for sound effects


}

