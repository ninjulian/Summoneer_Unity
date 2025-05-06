using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject systemUI;
    [SerializeField] private GameObject playerStatUI;
    [SerializeField] private GameObject upgradeUI;


    void Update()
    {
        if (playerStatUI.activeSelf && upgradeUI.activeSelf)
        {
            // Prioritize one and disable the other
            upgradeUI.SetActive(false);
        }
    }


    public void closeSystemUI()
    {
        systemUI.SetActive(false);
    }

    public void openUpgradeUI()
    {
        SetExclusiveUI(upgradeUI);
    }

    public void closeUpgradeUI() 
    {
        SetExclusiveUI(playerStatUI);
    }

    private void SetExclusiveUI(GameObject uiToEnable)
    {
        playerStatUI.SetActive(uiToEnable == playerStatUI);
        upgradeUI.SetActive(uiToEnable == upgradeUI);
    }

}
