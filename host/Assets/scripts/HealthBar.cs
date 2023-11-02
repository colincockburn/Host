using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private static Combatants combatantScript;
    private float startHealth;
    private float health;

    void Start() {
        combatantScript = 
          GameManager.instance.controlledObject.GetComponent<Combatants>();
        startHealth = combatantScript.startHealth;    
        health = combatantScript.health;
        SetHealth(startHealth, health);
    }

    void Update() {
        combatantScript = GameManager.instance.controlledObject.GetComponent<Combatants>();
        startHealth = combatantScript.startHealth;
        health = combatantScript.health;
        SetHealth(startHealth, health);
    }

    public void SetHealth(float startHealth, float health) {
        slider.maxValue = startHealth;
        gradient.Evaluate(1f);
        
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

}
