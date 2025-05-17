using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Xml.Serialization;

public class SummlingManagerUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject summlingUI;
    public GameObject replaceSummling;
    public GameObject summonConfirmationTab;
    public TMP_Text summonCost;
    private bool selectedReplacement = false;

    [Tooltip("Summon Button")]
    public GameObject summonButton;

    [Header("Summling Party")]
    [Tooltip("The Panel that shows your Summling Party")]
    public GameObject summlingPartyPanel;

    [Header("Summling Party Locations")]
    [Tooltip("The parents and location of where the Party will be")]
    public GameObject SummonPos;
    public GameObject ManagementPos;

    [Header("Class References")]
    public SummlingManager manager;

    [Header("Page Management")]
    public List<GameObject> pages = new List<GameObject>();
    public int currentPageIndex = 0;

    public void Awake()
    {
        UpdateCost();
        UpdatePartyPanelLocation(SummonPos);
    }

    private void UpdatePartyPanelLocation(GameObject newParent)
    {
        if (summlingPartyPanel == null || newParent == null) return;

        summlingPartyPanel.SetActive(true);

        // Reparent while maintaining world position
        summlingPartyPanel.transform.SetParent(newParent.transform, true);
        
        // Reset local position and rotation
        summlingPartyPanel.transform.localPosition = Vector3.zero;
        summlingPartyPanel.transform.localRotation = Quaternion.identity;

        // Optional: Reset scale if needed
        // summlingPartyPanel.transform.localScale = Vector3.one;
    }

    private void CheckPage()
    {
        // Set initial position based on starting index
        if (currentPageIndex == 0)
        {
            Debug.Log("Changing Parent to SummonPOS");
            UpdatePartyPanelLocation(SummonPos);
        }
        else if (currentPageIndex == 1)
        {
            Debug.Log("Changing Parent to ManagamentPOS");
            UpdatePartyPanelLocation(ManagementPos);
        }
    }

    private void UpdateCost()
    {
        float cost = manager.GetSummonCost();
        summonCost.text = cost.ToString();
    }

    public void SummonButton()
    {   
        manager.UpdateButtonInteractivity();
        if (manager.canSummon)
        {
            UpdateCost();
            Debug.Log("OPening confirmation page");

            summonConfirmationTab.SetActive(!summonConfirmationTab.activeInHierarchy);
            replaceSummling.SetActive(manager.isReplacing);


            manager.GenerateSummling();
        }

    }

    public void ReplaceSummling()
    {
        if (manager.isReplacing)
        {
            replaceSummling.SetActive(!replaceSummling.activeInHierarchy);
            summonButton.SetActive(summonButton.activeInHierarchy);
        }


        //else
        //{
        //    replaceSummling.SetActive(!replaceSummling.activeInHierarchy);
        //    summonButton.SetActive(!summonButton.activeInHierarchy);
        //}

    }

    public void NotReplacingSummling()
    {
        replaceSummling.SetActive(!replaceSummling.activeInHierarchy);
        summonButton.SetActive(!summonButton.activeInHierarchy);
        manager.DeclineSummon();
    }

    public void ConfirmSummon()
    {
        

        // Prevent confirmation if in replacement mode but no slot selected
        if (manager.isReplacing && !selectedReplacement)
        {
            
           // Debug.LogError("Cannot confirm - Please select a slot to replace first!");
            //return;
        }
        else if (manager.isReplacing && selectedReplacement)
        {
            // Always close confirmation tab first
            summonConfirmationTab.SetActive(false);

            // Handle replacement flow
            manager.ConfirmReplacement();
            summonButton.SetActive(true);
            selectedReplacement = false;
            // Update UI elements
            replaceSummling.SetActive(manager.isReplacing);
        }
        else
        {
            // Always close confirmation tab first
            summonConfirmationTab.SetActive(false);

            // Handle normal confirmation
            manager.ConfirmSummon();
            // Update UI elements
           // replaceSummling.SetActive(manager.isReplacing);
        }

        
    }

    public void CancelSummon()
    {
        //Close YesNo, take away SE
        manager.DeclineSummon();
        summonConfirmationTab.SetActive(!summonConfirmationTab.activeInHierarchy);
        summonButton.SetActive(!manager.isReplacing);
        selectedReplacement = false;
    }

    public void SelectSummling()
    {
        //Get Summling Data
    }

    public void TransmuteButton()
    {
        //Gain SE delete effects and delete Summling
        //transmute selected true
    }

    public void MergeButton()
    {
        //Combine both Summlings
        //Merge selected true
    }

    public void HoverIcon()
    {
        //Show description of Summling
    }

    public void HoverTransmute()
    {
        //Explain how to transmute
    }

    public void HoverMerge()
    {
        //Explain how to merge
    }

    public void IconButtons(int slotIndex)
    {
        manager.SelectReplacementSlot(slotIndex);
        selectedReplacement = true;
    }

    // Modified navigation methods
    public void NextPage()
    {
        if (currentPageIndex >= pages.Count - 1) return;

        SetPageActive(currentPageIndex, false);

        

        currentPageIndex++;
        SetPageActive(currentPageIndex, true);
        CheckPage();
        CleanupPageState();
    }

    public void PreviousPage()
    {
        SetPageActive(currentPageIndex, false);
        currentPageIndex = (currentPageIndex - 1 + pages.Count) % pages.Count;

        CheckPage();

        SetPageActive(currentPageIndex, true);
        CleanupPageState();
    }

    private void SetPageActive(int index, bool state)
    {
        if (index >= 0 && index < pages.Count)
        {
            pages[index].SetActive(state);
        }


    }

    private void CleanupPageState()
    {
        // Common cleanup for all page changes
        summonConfirmationTab.SetActive(false);
        manager.DeclineSummon();
        selectedReplacement = false;
    }

    // Initialization (call in Start/Awake)
    private void InitializePages()
    {
        // Deactivate all pages first
        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }
        CheckPage();
        // Activate starting page
        SetPageActive(currentPageIndex, true);
    }

}
