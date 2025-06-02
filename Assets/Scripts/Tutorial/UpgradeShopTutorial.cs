using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShopTutorial : MonoBehaviour
{
    //public GameObject upgradeUI;
    public UIManager uiManager;
    //public UpgradeManager upgradeManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            UpgradeManager.Instance.GenerateUpgrades(1);
            uiManager.ToggleUpgradeUI();
            Debug.Log("OPENING SHOP");
        }

    }

    public void ShowInstruction(GameObject uiPanel)
    {
        uiPanel.SetActive(true);
    }

    public void HideInstruction(GameObject uiPanel)
    {
        uiPanel.SetActive(false);
    }

}
