using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : MonoBehaviour
{
    private Combatants combatants;
    private Rigidbody2D rb;
    public float minThrowDistance = 5f;
    private EnemyBehavior enemyBehavior;
    public GameObject bombPrefab;
    private Vector2 direction;
    private Vector3 mousePosition;
    private Vector2 bombPosition;
    public Vector2 target;
    private float angle;
    public Transform bombTransform;
    public float timeBetweenThrows = 0.5f;
    public float timeBetweenThrowsAI = 1.25f;
    private float throwTimer;
    public GameObject throwingArm;
    public Animator armAnimator;
    public float velocity;
    
    public float AIMoveSpeed;


    // Start is called before the first frame update
    void Start()
    {
        combatants = GetComponent<Combatants>();
        enemyBehavior = GetComponent<EnemyBehavior>();
        rb = GetComponent<Rigidbody2D>();
        armAnimator = throwingArm.GetComponent<Animator>();
        throwTimer = 0;
        throwTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        bombPosition = new Vector2(bombTransform.position.x, bombTransform.position.y);
        if(!combatants.dead){
            if(!combatants.hosted ){
                AIControlled();
            } else {
                Hosted();
            }
        }
        throwTimer -= Time.deltaTime;
    }


    void ThrowBomb()
    {   
        direction = target - bombPosition;
        float distance = direction.magnitude;
        float gravity = Physics2D.gravity.magnitude;
        switch (distance){
            case float d when d < 2:
            {
                velocity = 5;
                break;
            }
            case float d when d < 4 && d >= 2:
            {
                velocity = 8;
                break;
            }
            case float d when d < 10 && d >= 4:
            {
                velocity = 12;
                break;
            }
            case float d when d >= 10:
            {
                velocity = 15;
                break;
            }
        }

        if (Mathf.Abs(direction.y) / Mathf.Abs(direction.x) > 1.2)
        {
            angle = Mathf.Atan2(direction.y, direction.x) 
            * Mathf.Rad2Deg;
        } else {
            angle = Mathf.Atan((Mathf.Pow(velocity, 2) - Mathf.Sqrt(Mathf.Pow(velocity, 4) - gravity * (gravity * distance * distance + 2 * direction.y * velocity * velocity))) / (gravity * distance));
            angle = Mathf.Rad2Deg * angle;
        };

        if (direction.x < 0 && !(Mathf.Abs(direction.y) / Mathf.Abs(direction.x) > 1.2))
        {
            angle = 180 - angle;
        }

        if (direction.x < 0 && angle < 90)
        {
            angle = 180 - angle;
            Debug.Log("switched");
        }
        if (float.IsNaN(angle)){
            if (direction.x > 0){
                angle = 0;
            } else {
                angle = 180;
            }
        }

        GameObject bomb = Instantiate(bombPrefab, 
            bombTransform.position, 
        Quaternion.AngleAxis(angle, Vector3.forward));
        // this sets the speed of the throw
        bomb.GetComponent<bomb>().speed = velocity;
    }

    void AIControlled()
    {
        if (enemyBehavior.playerDetected && !combatants.confused){
            throwTimer -= Time.deltaTime;
            // this needs to change for target
            target = GameManager.instance.controlledObject.transform.position;

                RaycastHit2D[] hits = Physics2D.RaycastAll(target, Vector2.down);

                for (int i = 0; i < hits.Length; i++)
                {
                    Collider2D collider = hits[i].collider;

                    // Check if the collided object is on the "Ground" layer
                    if (collider.gameObject.layer == 
                        LayerMask.NameToLayer("Ground"))
                    {

                        float targetAdjustment = (target.x - rb.position.x > 
                            0) ? -2 : 2;
                        targetAdjustment += 
                            GameManager.instance.controlledObject
                            .GetComponent<Combatants>().rb.velocity.x / 2;
                        target = new Vector2(target.x + targetAdjustment, 
                            hits[i].point.y);
                        break;
                    }
                }

            // if far away use bombs
            if (Mathf.Abs((target - rb.position).magnitude) > minThrowDistance){
                combatants.dirX = 0;
                gameObject.transform.localScale = (direction.x > 0) ?
                new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
                if (enemyBehavior.playerDetected){
                    if (throwTimer < 0){

                        throwingArm.GetComponent<BomberArm>
                        ().HandleAnimations();
                        StartCoroutine(ThrowBombWithDelay(0.2f));

                        throwTimer = timeBetweenThrowsAI;
                    }
                }
            } 
            else 
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
                } else if(GameManager.instance.controlledObject.GetComponent<Combatants>().IsGrounded()){
                    combatants.dirX = 0;
                    combatants.attemptMelee = true;
                }
                // if will fall off edge cancle movment
                if (enemyBehavior.WillFallOffEdge(combatants.dirX)){
                    combatants.dirX = 0;
                }
            }
        } 
                // if player not detected don't move
        else 
        {
          combatants.dirX = 0;
        }
        if (combatants.damageTimer <= 0){
            if(combatants.IsGrounded()){
                rb.velocity = new Vector2(combatants.dirX*AIMoveSpeed,
                rb.velocity.y);
            } else {
                combatants.InAirMovement(combatants.dirX);
            }
        }
    }

    void Hosted()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
        target = new Vector2(mousePosition.x, mousePosition.y);

        if (Input.GetKeyDown(KeyCode.Mouse1) && throwTimer < 0)
        {
            throwingArm.GetComponent<BomberArm>().HandleAnimations();
            if (throwingArm.GetComponent<BomberArm>().throwForward)
            {
                StartCoroutine(ThrowBombWithDelay(0.2f));
            }
            else{
                StartCoroutine(ThrowBombWithDelay(0.1f));
            }
        }
    }

    IEnumerator ThrowBombWithDelay(float throwingDelay)
    {
        yield return new WaitForSeconds(throwingDelay);
        throwTimer = (combatants.hosted) 
            ? timeBetweenThrows : timeBetweenThrowsAI;
        ThrowBomb();
    }
}


