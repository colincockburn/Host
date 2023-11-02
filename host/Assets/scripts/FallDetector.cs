using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDetector : MonoBehaviour
{
    public GameObject fallDetector;
    private static Combatants combatantScript;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        combatantScript = GameManager.instance.controlledObject.GetComponent<Combatants>();
        // move fall detector to follow the character being played as
        fallDetector.transform.position = new Vector2(combatantScript.transform.position.x, fallDetector.transform.position.y);
    }

    // checks if player has collided with fall detector to end game
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            Destroy(combatantScript.gameObject, 0.1f);
        }
    }
}
