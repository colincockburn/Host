using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehavior : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    private BoxCollider2D coll;
    public float knockbackPower;
    public int damage;
    private Rigidbody2D rb;
    public GameObject shooter;
    public AudioManager audioManager;
    public Animator animator;
    public float rotationAmount;
    private Collider2D hitCollider;
    private Collider2D collision;
    public float bulletHitDelay = 0.1f;
    private RaycastHit2D hit;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        Destroy(gameObject, lifetime);
        audioManager = AudioManager.instance;
        animator = GetComponent<Animator>();
        
        
    }

    void Update()
    {
        CheckBulletHit();
    }



    void CheckBulletHit()
    {
        if (collision == null){
            float raycastDistance = 0.8f; // Set the distance of the raycast

            // Calculate the raycast direction in local space (positive x-axis)
            Vector2 raycastDirection = transform.TransformDirection(Vector2.right);

            hit = Physics2D.Raycast(transform.position, raycastDirection, raycastDistance);
            if (hit.collider != null){
                if (hit.collider.gameObject != shooter && hit.collider.gameObject.layer != shooter.layer && (hit.collider.GetComponent<Combatants>() != null && !hit.collider.GetComponent<Combatants>().dead || hit.collider.GetComponent<Combatants>() == null)){
                    collision = hit.collider;
                    StartCoroutine(BulletHitWithDelay());
                }
            }
        }
    }

    IEnumerator BulletHitWithDelay()
    {
        yield return new WaitForSeconds(bulletHitDelay);
        BulletHit();
    }



    void BulletHit()
    {
        // Get a reference to the collided object
        GameObject collidedObject = collision.gameObject;
        Combatants combatants = collidedObject.GetComponent<Combatants>();
        if (combatants != null)
        {
            collidedObject.GetComponent<Combatants>().health -= damage;
            audioManager.Play("bulletHit");
            animator.SetBool("blood", true);
            // transform.localPosition += new Vector3(1.5f, 0f, 0f);
            transform.Rotate(0f, 0f, rotationAmount);
            rb.velocity = Vector2.zero;
            rb.transform.position= new Vector2(hit.transform.position.x, rb.transform.position.y );
        }
        else
        {
            audioManager.Play("bulletImpact");
            animator.SetBool("impact", true);
            rb.velocity = Vector2.zero; 
            transform.position = hit.point;
        }
    
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
