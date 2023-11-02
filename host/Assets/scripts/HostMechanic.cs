using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class HostMechanic : MonoBehaviour
{
    public AudioManager audioManager;
    public GameObject closestToMouse;
    public Text hostChargeText;
    public int hostCharge = 3;
    public int hostChargeMax = 3;
    private float hostChargeTimer;
    private float hostChargeTime = 10f;

    public bool hosted = false;
    //represents enemies confused by body switch
    public bool confused = false;
    private Vector2 hostedVelocity;
    public float barValue;
    public HostBar hostBar;
    private bool hostSelection = false;
    public int hostDistance;
    private Vector3 mousePos;
    private Vector3 worldMousePos;
    private LineRenderer lineRenderer;
    private bool validHost;
    private PauseMenu pauseMenuScript;
    public bool leavingHost;
    public float intensity = 0f;



    void Start(){
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        pauseMenuScript = GameManager.instance.GetComponent<PauseMenu>();
        hostChargeTimer = hostChargeTime;
        audioManager = AudioManager.instance;

    }

    void Update(){
        hosted = (GameManager.instance.controlledObject != GameManager.instance.mainCharacterObject) ? true : false;
        barValue = hostBar.barValue;
        // Only host if Host Bar is full
        if (barValue == 100 && hostCharge > 0) {
            Host();
        }
        CheckForceLeaveHost();
        if (!pauseMenuScript.isPaused)
        {
            Time.timeScale = (hostSelection) ? 0.33f : 1f;
            intensity = (hostSelection) ? 0.75f : 0f;
        }

        HostChargeController();
    }
    public void Host(){
        closestToMouse = GetClosestObjectToMouse();
        if (Input.GetKey(KeyCode.W) && !leavingHost)
        {
            HostSelection();
        }
        else if(hostSelection && validHost)
        {
            audioManager.Play("host");
            hostCharge -= 1;
            GameManager.instance.controlledObject = closestToMouse;
            GameManager.instance.mainCharacterObject.SetActive(false);
            Combatants[] objectsInScene = FindObjectsOfType<Combatants>();
            foreach (Combatants obj in objectsInScene)
            {
                StartCoroutine(StallConfused(obj));
            }
        }
        // not sure why this is here
        else if (hostSelection && !validHost){
        }
        if(!Input.GetKey(KeyCode.W) || closestToMouse == null){
            lineRenderer.enabled = false;
            hostSelection = false;
        }
    }

    void HostSelection()
    {
        hostSelection = true;

        if(closestToMouse != null && hostCharge > 0){
            lineRenderer.enabled = true;

            RaycastHit2D[] hits = 
                Physics2D.LinecastAll(GameManager.instance.controlledObject.
                transform.position, closestToMouse.transform.position);
            Collider2D collider = hits[1].collider;

            lineRenderer.SetPositions(new Vector3[] { 
                GameManager.instance.controlledObject.transform.position, 
                hits[1].point });
            lineRenderer.material = new 
                Material(Shader.Find("Sprites/Default"));

            if (collider is BoxCollider2D){
                lineRenderer.startColor = Color.green;
                lineRenderer.endColor = Color.green;
                validHost = true;
            } else {
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;
                validHost = false;
            }
        }
    }

    IEnumerator StallConfused(Combatants obj)
    {
        float randomWait = Random.Range(0f, 0.2f);
        yield return new WaitForSeconds(randomWait); // Wait for 2 seconds
        obj.confusedTimer = obj.confusedStartTime;   
    }

    public void CheckForceLeaveHost(){
        if (hosted && Input.GetKeyDown(KeyCode.W) || GameManager.instance.controlledObject.GetComponent<Combatants>().dead){
            leavingHost = true;
            LeaveHost();
        }
        if (leavingHost){
            if (!Input.GetKey(KeyCode.W))
            {
                leavingHost = false;
            }
        }

    }

    public void LeaveHost(){
        // set men character back to active, move it to current hosts 
        // position, and add upwards velocity to main character
        // maybe clean this up later with variables
        if (!GameManager.instance.mainCharacterObject.activeSelf){
            GameManager.instance.controlledObject.GetComponent<Combatants>
                ().confusedTimer =
            GameManager.instance.controlledObject.GetComponent<Combatants>
                ().confusedStartTime;
            GameManager.instance.mainCharacterObject.SetActive(true);
            GameManager.instance.mainCharacterObject.transform.position = 
            GameManager.instance.controlledObject.transform.position;
            hostedVelocity = 
                GameManager.instance.controlledObject.GetComponent<Combatants>
                ().rb.velocity;
            GameManager.instance.mainCharacterObject.GetComponent<Rigidbody2D>  
                ().velocity = new Vector2(hostedVelocity.x, 
                hostedVelocity.y + 10);
            GameManager.instance.controlledObject =  
            GameManager.instance.mainCharacterObject;
        }

    }

    
    private GameObject GetClosestObjectToMouse()
    {
    mousePos = Input.mousePosition;
    mousePos.z = Camera.main.transform.position.z;

    Collider2D[] objectsInScene = FindObjectsOfType<Collider2D>();
    GameObject closestObject = null;
    float closestDistance = Mathf.Infinity;

    foreach (Collider2D obj in objectsInScene)
    {
        Vector3 objectPos = obj.transform.position;
        worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        float distance = Vector3.Distance(objectPos, 
        worldMousePos);
        Vector3 playerToObj = objectPos -
            GameManager.instance.controlledObject.transform.position;

        if (distance < closestDistance && obj.gameObject.layer == 7 && playerToObj.magnitude < hostDistance)
        {
            closestDistance = distance;
            closestObject = obj.gameObject;
        }
    }

    return closestObject;
    }

    void HostChargeController()
    {
        if (hostCharge < hostChargeMax)
        {
            hostChargeTimer -= Time.deltaTime;
            if (hostChargeTimer <= 0)
            {
                hostChargeTimer = hostChargeTime;
                hostCharge++;
            }
        }
        hostChargeText.text = hostCharge.ToString();
    }
}

