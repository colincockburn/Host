using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource soundPlayer;
    public LevelTransition levelTransition;

    public void playGame()
    {
        soundPlayer.Play();
        levelTransition.FadeToNextLevel();
    }

    public void endGame()
    {
        soundPlayer.Play();
        Application.Quit();
    }
}
