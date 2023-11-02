using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioPlayer : MonoBehaviour
{
    public AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Punch1()
    {
        audioManager.Play("punch1");
    }

    public void Punch2()
    {
        audioManager.Play("punch2");
    }

    public void Punch3()
    {
        audioManager.Play("punch3");
    }

    public void Landing()
    {
        audioManager.Play("landing");
    }

    public void MidAirKick()
    {
        audioManager.Play("midAirKick");
    }

    public void Footstep1()
    {
        audioManager.Play("footstep1");
    }

    public void Footstep2()
    {
        audioManager.Play("footstep2");
    }

    public void Jump()
    {
        audioManager.Play("jump");
    }

    public void DoubleJump()
    {
        audioManager.Play("doubleJump");
    }

    public void Explosion ()
    {
        audioManager.Play("explosion");
    }

    public void Gunshot()
    {
        audioManager.Play("gunshot");
    }

    public void BulletImpact()
    {
        audioManager.Play("bulletImpact");
    }

    public void BulletHit()
    {
        audioManager.Play("bulletHit");
    }

    public void GroundSlamSound()
    {
        audioManager.Play("groundSlam");
    }

    public void GroundSlamFalling()
    {
        audioManager.Play("groundSlamFalling");
    }

    public void Death()
    {
        audioManager.Play("death");
    }



}
