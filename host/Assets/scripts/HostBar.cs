using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public bool host;

    // Bar value variables
    public int maxValue = 100;
    public float depletionRate = 0.01f;

    private static HostMechanic hosted;
    public float barValue;


    void Start() {
        slider.maxValue = maxValue;
        slider.value = maxValue;
        barValue = slider.value;
        fill.color = gradient.Evaluate(1f);
    }

    void Update() {
        hosted = GameManager.instance.GetComponent<HostMechanic>();
        host = hosted.hosted;
        // Deplpete bar if hosting
        if (host){
            UseBar();
        } 
        // Regen bar when in main character
        else {
            NotUseBar();
        }
    }

    // When hosted, this causes the bar to deplete
    public void UseBar() {
        slider.value -= depletionRate;
        barValue = slider.value;
        fill.color = gradient.Evaluate(slider.normalizedValue);

        if (slider.value <= 0.0f) {
            hosted.LeaveHost();
        }
    }

    // When not hosting, bar regenerates
    public void NotUseBar() {
        slider.value = 100;
        barValue = slider.value;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
