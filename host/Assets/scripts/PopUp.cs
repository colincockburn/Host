using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    [SerializeField] private Text pop;

    [SerializeField] private string keyToPress = "RightArrow"; // Default key is right mouse click, can be changed

    private bool allowed;

    public string KeyToPress // Public property to set the key from outside the script
    {
        get { return keyToPress; }
        set { keyToPress = value; }
    }

    private void Start()
    {
        pop.gameObject.SetActive(false);
    }

    private void Update()
    {
         if (allowed && Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), keyToPress)))
        {
            pickUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pop.gameObject.SetActive(true);
            allowed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pop.gameObject.SetActive(false);
            allowed = false;
        }
    }

    private void pickUp()
    {
        Destroy(gameObject);
    }
}

