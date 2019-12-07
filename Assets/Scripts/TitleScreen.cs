using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{

    /** This is the name of the first room. */
    public string firstRoom;

    /** This is the name of the options page. */
    public string optionsPage;



    /**
     * This is the method to start the game, meaning go to the first room.
     */
    public void StartGame()
    {
        SceneManager.LoadScene(firstRoom);
    }

    /**
     * This is the method to show the help screen.
     */
    public void Help()
    {
        SceneManager.LoadScene(optionsPage);
    }

    /**
     * This is the method to exit the program.
     */
    public void ExitGame()
    {
        Application.Quit();
    }

}
