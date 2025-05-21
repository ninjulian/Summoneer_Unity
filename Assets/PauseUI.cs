using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Serialization;

public class PauseUI : MonoBehaviour
{
    [Header("References")]
    public GameObject PauseMenu;
    public GameObject SettingsMenu;
    public GameObject ControlsScreen;


    public void SettingsButton()
    {
        //Open Settings Panel
    }

    public void HelpButton()
    {
        //Shows Controls and info
    }

    public void MainMenuButton()
    {
        //Quit to Main Menu
    }

    public void QuitToDesktopButton()
    {
        //Close application
    }

    public void BackButton()
    {
        //Go back to previous page
    }
}
