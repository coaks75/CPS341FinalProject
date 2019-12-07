using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{

    /** This field is for checking if the game is paused. */
    public static bool gameIsPaused = false;

    /** Tha pause menu. */
    public GameObject pauseMenu;

    /** The title screen name. */
    public string titleScreen;



    private void Start()
    {
        Resume();
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Debug.Log("Pausing?");
                Pause();
            }
        }

    }

    /**
     * A getter for the game is paused. 
     */
    public bool getGameIsPaused()
    {
        return gameIsPaused;
    }

    /**
     * A method to resume the game.
     */
     public void Resume()
    {

        PlayerController.mouseLook.SetCursorLock(true);

        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        gameIsPaused = false;
    }


    /**
     * A method to pause the game.
     */
    public void Pause()
    {
        PlayerController.mouseLook.SetCursorLock(false);

        pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
        gameIsPaused = true;
        PlayerController.mouseLook.lockCursor = false;
    }


    /**
     * A method for the help button
     */
    public void Help()
    {
        Debug.Log("Loading help options");
    }


    /**
     * A method to quit the game
     */
    public void QuitGame()
    {
        Debug.Log("Going to title screen");
        SceneManager.LoadScene(titleScreen);
    }


}
