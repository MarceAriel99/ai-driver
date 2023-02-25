using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject countdown;

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

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        ShowCountdown();
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
        ResumeGame();
    }

    public void ShowCountdown()
    {
        Debug.Log("Showing countdown");
        Cursor.visible = false;
        countdown.SetActive(true);
        countdown.GetComponent<Animator>().Play("Base Layer.Countdown", 0, 0f);
        countdown.SetActive(false);
    }
}
