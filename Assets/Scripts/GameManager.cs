using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject CountDown;
    public AnimationClip CountdownClip;

    public RaceTrainManager raceTrainManager;

    public void Start()
    {
        Cursor.visible = false;
    }

    public void Update()
    {
        // if P is pressed, then pause the game
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
            Cursor.visible = true;
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void RestartGame()
    {
        raceTrainManager.EndEpisodeForAllAgents();
        // SceneManager.LoadScene("GameScene");
        ResumeGame();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        StartCoroutine(ShowCountdown());
        // Time.timeScale = 1;
    }

    IEnumerator ShowCountdown()
    {
        Debug.Log("Showing countdown");
        Cursor.visible = false;
        CountDown.SetActive(true);
        CountDown.GetComponent<Animator>().Play("Countdown");
        // wait for the animation to finish using unscaled time
        yield return new WaitForSecondsRealtime(CountdownClip.length-0.2f);
        CountDown.SetActive(false);
        Time.timeScale = 1;
    }
}
