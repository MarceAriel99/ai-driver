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
        SceneManager.LoadScene("GameScene");
        ResumeGame();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        //ShowCountdown(); //FIXME: this is not working
        Time.timeScale = 1;
    }

    public void ShowCountdown()
    {
        Debug.Log("Showing countdown");
        Cursor.visible = false;
        CountDown.SetActive(true);
        CountDown.GetComponent<Animator>().Play("Countdown");
        StartCoroutine(DisableCountdown());
    }

    IEnumerator DisableCountdown()
    {
        yield return new WaitForSeconds(CountdownClip.length-0.2f);
        CountDown.SetActive(false);
        Time.timeScale = 1;
    }
}
