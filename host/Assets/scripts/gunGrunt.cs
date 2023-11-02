using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunGrunt : MonoBehaviour
{
    private Combatants combatants;
    public MuzzleFlash flashPrefab;
    public AudioManager audioManager;
    private EnemyBehavior behavior;
    public float attackDelay;
    private float attackDelayTimer;
    public Transform arm;
    private float armAngle;
    private Vector3 mousePosition;
    private Vector3 target;
    private Vector3 aimDir;
    public GameObject bulletPrefab;
    public Transform barrel;
    private Vector3 barrelPosition;
    public float timeBetweenShots = 0.5f;
    public float timeBetweenShotsAI = 1.25f;
    public float shotTimer;


    // Start is called before the first frame update
    void Start()
    {
        combatants = GetComponent<Combatants>();
        behavior = GetComponent<EnemyBehavior>();
        shotTimer = 0;
        attackDelayTimer = 0;
        audioManager = AudioManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(!combatants.hosted && !combatants.confused && !combatants.dead){
            AIControlled();
        } else if (combatants.hosted){
            Hosted();
        }
        shotTimer -= Time.deltaTime;
    }

    void PointArm(){

        aimDir = target - arm.position;
        armAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg + 90;
        arm.rotation = Quaternion.AngleAxis(armAngle, Vector3.forward);

    }

    void FireGun()
    {   
        Debug.Log("shot fired");
        if (shotTimer < 0)
        {
            audioManager.Play("gunshot");
            barrelPosition = barrel.position;
            GameObject bullet = Instantiate(bulletPrefab, barrelPosition, 
            Quaternion.AngleAxis(armAngle-90, Vector3.forward));
            bullet.GetComponent<bulletBehavior>().shooter = gameObject;
            shotTimer = (combatants.hosted) 
              ? timeBetweenShots : timeBetweenShotsAI;
            MuzzleFlash muzzleFlash = Instantiate(flashPrefab, barrelPosition, 
            Quaternion.AngleAxis(armAngle + 90, Vector3.forward));
            muzzleFlash.transform.SetParent(barrel.transform);
        }

    }

    void AIControlled()
    {
        attackDelayTimer -= Time.deltaTime;
        combatants.dirX = 0;
        gameObject.transform.localScale = (aimDir.x > 0) ?
         new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        if (behavior.playerDetected){
            target =
              GameManager.instance.controlledObject.transform.position;
              target += new Vector3(0, 0.5f, 0);
            PointArm();

            if (attackDelayTimer < 0){
                if (behavior.distanceToPlayer < 1 ){
                    combatants.attemptMelee = true;
                } else {
                    FireGun();
                }
                attackDelayTimer = attackDelay;
            }
        } else {
            target = new Vector3(0,-1000, 0);
        }
        

    }

    void Hosted(){
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
        target = mousePosition;
        PointArm();
        if (Input.GetKeyDown(KeyCode.Mouse1)) 
        {
            FireGun();
        }
    }




}
