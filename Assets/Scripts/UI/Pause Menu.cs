using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private GameObject MenuPage;
    [SerializeField] private GameObject SettingsPage;   


    void ResumeButton()
    {
        Time.timeScale = 1f;
    }
    void SettingButton()
    {

    }

    void MainMenuButton()
    {

    }

    void QuitButton()
    {

    }

    void BackButton()
    {

    }

    

}
