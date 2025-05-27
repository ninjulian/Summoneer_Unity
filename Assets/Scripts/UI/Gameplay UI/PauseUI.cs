using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Serialization;
//using UnityEditor.Timeline.Actions;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [Header("References")]
    public GameObject PauseMenu;
    public GameObject SettingsMenu;
    public GameObject HelpMenu;

    [Header("Help Screen")]
    public GameObject ControlsPage;
    public GameObject StatsInformationPage;
    public GameObject HowToPlayPage;
    public GameObject HowToPlayPage1;
    public GameObject HowToPlayPage2;
    public GameObject HowToPlay2NextButton;



    public void SettingsButton()
    {
        //Open Settings Panel
        SettingsMenu.SetActive(!SettingsMenu.activeInHierarchy);
        PauseMenu.SetActive(!SettingsMenu.activeInHierarchy);
    }

    public void HelpButton()
    {
        //Shows Controls and info
        PauseMenu.SetActive(!PauseMenu.activeInHierarchy);
        HelpMenu.SetActive(!HelpMenu.activeInHierarchy);


    }

    public void ControlsButton()
    {
        HelpMenu.SetActive(!HelpMenu.activeInHierarchy);
        ControlsPage.SetActive(!ControlsPage.activeInHierarchy);

    }

    public void StatsInformationButton()
    {
        HelpMenu.SetActive(!HelpMenu.activeInHierarchy);
        StatsInformationPage.SetActive(!StatsInformationPage.activeInHierarchy);
    }

    public void HowToPlayButton()
    {
        HelpMenu.SetActive(!HelpMenu.activeInHierarchy);
        HowToPlayPage.SetActive(!HowToPlayPage.activeInHierarchy);
        HowToPlayPage1.SetActive(true);
    }

    public void MainMenuButton()
    {
        //Quit to Main Menu
        SceneManager.LoadScene(1);
    }

    public void QuitToDesktopButton()
    {
        //Close application
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void BackButton()
    {
        //Go back to previous page
        if (PauseMenu.activeInHierarchy)
        {
            PauseMenu.SetActive(!PauseMenu.activeInHierarchy);
        }

        if (SettingsMenu.activeInHierarchy)
        {
            PauseMenu.SetActive(true);
            SettingsMenu.SetActive(false);
        }



        // Help panel
        if (HelpMenu.activeInHierarchy)
        {
            HelpMenu.SetActive(false);
            PauseMenu.SetActive(true);
        }

        if (StatsInformationPage.activeInHierarchy)
        {
            HelpMenu.SetActive(true);
            StatsInformationPage.SetActive(false);
        }

        if (ControlsPage.activeInHierarchy)
        {
            HelpMenu.SetActive(true);
            ControlsPage.SetActive(false);
        }

        if (HowToPlayPage1.activeInHierarchy)
        {
            HelpMenu.SetActive(true);
            HowToPlayPage.SetActive(false);
        }
        else if  (HowToPlayPage2.activeInHierarchy)
        {
            HowToPlayPage2.SetActive(false);
            HowToPlayPage1.SetActive(true);
            HowToPlay2NextButton.SetActive(true);
        }
    }

    public void NextButton()
    {
        if (HowToPlayPage1.activeInHierarchy)
        {
            HowToPlayPage1.SetActive(false);
            HowToPlayPage2.SetActive(true);
            HowToPlay2NextButton.SetActive(false);
        }
     
    }
}
