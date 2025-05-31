using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MainMenu : MonoBehaviour
{
    public GameObject startButton;
    public TMP_Text highscoreText;


    private void Start()
    {
        // Check if the tutorial was completed
        bool tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;

        // Disable startButton if tutorial isn't completed
        startButton.SetActive(tutorialCompleted);

        // Load and display the saved high wave
        int highestWave = PlayerPrefs.GetInt("HighestWave", 0);
        highscoreText.text = "Highest Wave: " + highestWave;
    }

    public void StartButton()
    {   
        // Load up main level
        SceneManager.LoadScene("Final Map");
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
