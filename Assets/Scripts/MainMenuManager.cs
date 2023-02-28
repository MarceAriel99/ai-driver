using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        // use RestartGame() from GameManager.cs
        FindObjectOfType<GameManager>().RestartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
