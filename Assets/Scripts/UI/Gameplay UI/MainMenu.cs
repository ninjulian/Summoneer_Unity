using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
public class MainMenu : MonoBehaviour
{
    public Button startButton;


    private void Start()
    {
        // Check if the tutorial was completed
        bool tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;

        // Disable startButton if tutorial isn't completed
        startButton.interactable = tutorialCompleted;
    }

    public void StartButton()
    {   
        // Load up main level
        SceneManager.LoadScene("The_Lab");
    }

    public void SettingsButton()
    {
        Debug.Log("Opening Settigns");
    }

    public void TutorialButton()
    {   

        // Load up tutorial level
        SceneManager.LoadScene("Tutorial");
    }

    public void QuitGameButton()
    {   
        // Quits application
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
