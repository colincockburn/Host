using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bomb : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public float timer;
    private bool hasStarted;
    private bool hasExploded;
    public int damage;
    public float knockbackPower;
    public float attackRange;
    private Animator animator;
    public AudioManager audioManager;   
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        animator = GetComponent<Animator>();
        audioManager = AudioManager.instance;
    }

    void Update()
    {
        if (hasStarted){
            timer -= Time.deltaTime;
            if (timer < 0 && !hasExploded)
            {
                Explode();
            }
        }
        if(hasExploded)
        {
            rb.velocity = Vector2.zero;
        }
    }


    void Explode()
    {
        audioManager.Play("explosion");
        hasExploded = true;
        LayerMask whatIsCombatants = LayerMask.GetMask("enemy", "player");

        Collider2D[] objectsToDamage = 
          Physics2D.OverlapCircleAll(transform.position, attackRange, whatIsCombatants);
        for (int i = 0; i < objectsToDamage.Length; i++)
        {
            Vector2 knockbackVector = (objectsToDamage[i].transform.position - transform.position);
            knockbackVector.Normalize();
            knockbackVector = new Vector2(knockbackVector.x * 
                knockbackPower, knockbackVector.y * knockbackPower + 5);
            objectsToDamage[i].GetComponent<Combatants>().TakeDamage(damage, knockbackVector);
        }

        animator.SetBool("exploded", true);
        rb.angularVelocity = 0f;
        rb.rotation = 0f;


    }

    void DestroyBomb(UnityEngine.AnimationEvent animationEvent){
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        audioManager.Play("grenadeBounce");
        rb.velocity = new Vector2(rb.velocity.x / 1.5f, rb.velocity.y);
        hasStarted = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
