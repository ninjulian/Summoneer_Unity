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
            PlayerPrefs.SetInt("TutorialCompleted", 1); // 1 = true, 0 = false
            PlayerPrefs.Save(); // Save immediately

            // Game level
            SceneManager.LoadScene("The_Lab");
        }
    }
}
