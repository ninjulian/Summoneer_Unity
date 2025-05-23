using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("The_Lab");
    }

    public void SettingsButton()
    {
        Debug.Log("Opening Settigns");
    }

    public void TutorialButton()
    {
        SceneManager.LoadScene("Tutorial Level");
    }

    public void QuitGameButton()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
