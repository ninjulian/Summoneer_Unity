using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLoadGame : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            // Set PlayerPrefs to mark tutorial as completed
            // 1 = true, 0 = false
            PlayerPrefs.SetInt("TutorialCompleted", 1); 

            // Save immediately
            PlayerPrefs.Save();
            SceneManager.LoadScene("Final Map");
        }
    }
}
