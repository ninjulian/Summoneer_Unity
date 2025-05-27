using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyButton : MonoBehaviour
{

    [SerializeField] private string urlToOpen;

    public void OpenURL()
    {
        Application.OpenURL(urlToOpen);
    }

    //public void PasteURL(string newURL)
    //{
    //    urlToOpen = newURL;
    //}
}
