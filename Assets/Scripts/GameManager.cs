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

    public Cinemachine.CinemachineVirtualCamera playerCamera;

    public Vector3 cameraStartPosition;
    public Quaternion cameraStartRotation;

    public RaceTrainManager raceTrainManager;

    public void Start()
    {
        Cursor.visible = false;
        GetCameraStartPositionAndRotation();
        Time.timeScale = 0;
        AudioListener.pause = true;
        // wait for 1 second before showing the countdown

        StartCoroutine(ShowCountdown(2));
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
        // mute the audio
        AudioListener.pause = true;
        pauseMenu.SetActive(true);
    }

    public void RestartGame()
    {
        raceTrainManager.EndEpisodeForAllAgents();
        Debug.Log("Moving camera back to start position at " + cameraStartPosition + " and rotation " + cameraStartRotation);
        playerCamera.ForceCameraPosition(cameraStartPosition, cameraStartRotation);
        ResumeGame();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Debug.Log("Resuming game");
        StartCoroutine(ShowCountdown());
    }

    IEnumerator ShowCountdown(int waitSeconds = 0)
    {
        if (waitSeconds > 0) {yield return new WaitForSecondsRealtime(waitSeconds);}
        Debug.Log("Showing countdown");
        Cursor.visible = false;
        CountDown.SetActive(true);
        CountDown.GetComponent<Animator>().Play("Countdown");
        // wait for the animation to finish using unscaled time
        yield return new WaitForSecondsRealtime(CountdownClip.length-0.2f);
        CountDown.SetActive(false);
        Time.timeScale = 1;
        // unmute the audio
        AudioListener.pause = false;
    }

    public void GetCameraStartPositionAndRotation()
    {
        cameraStartPosition = playerCamera.transform.position;
        cameraStartRotation = playerCamera.transform.rotation;
    }
}
