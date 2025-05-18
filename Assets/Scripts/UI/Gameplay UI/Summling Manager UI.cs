using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


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
    public GameObject[] selectBorder;

    [Header("Summling Party Locations")]
    [Tooltip("The parents and location of where the Party will be")]
    public GameObject SummonPos;
    public GameObject ManagementPos;

    [Header("Class References")]
    public SummlingManager manager;

    [Header("Page Management")]
    public List<GameObject> pages = new List<GameObject>();
    public int currentPageIndex = 0;

    [Header("Manager UI")]
    public Button mergeButton;
    public Button transmuteButton;
    public GameObject confirmMTButton;
    public TMP_Text confirmMTText;
    public TMP_Text managerDescriptionText;

    private List<int> selectedIndices = new List<int>();
    private bool isMergeMode = false;
    private bool isTransmuteMode = false;


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

        foreach (GameObject border in selectBorder)
        {
            border.SetActive(false);
        }


    }

    public void CancelSummon()
    {
        //Close YesNo, take away SE
        manager.DeclineSummon();
        summonConfirmationTab.SetActive(!summonConfirmationTab.activeInHierarchy);
        summonButton.SetActive(!manager.isReplacing);
        selectedReplacement = false;

        foreach (GameObject border in selectBorder)
        {
            border.SetActive(false);
        }
    }

    public void SelectSummling()
    {
        //Get Summling Data
    }

    public void ConfirmMTButton()
    {
        if (isMergeMode)
        {
            PerformMerge();
        }
        else if (isTransmuteMode)
        {
            PerformTransmute();
        }

        // Clear selections
        foreach (var index in selectedIndices)
        {
            selectBorder[index].SetActive(false);
        }
        selectedIndices.Clear();
        confirmMTButton.SetActive(false);
        manager.UpdatePartyUI();
    }


    public void TransmuteButton()
    {
        manager.isTransmuting = true;
        isTransmuteMode = true;
        manager.UpdateButtonInteractivity();
        isMergeMode = false;
        selectedIndices.Clear();
        
        manager.isMerging = false;
        confirmMTButton.SetActive(true);
        managerDescriptionText.text = "Select a Summling to transmute";
    }

    public void MergeButton()
    {
        isMergeMode = true;
        manager.isMerging = true;
        manager.UpdateButtonInteractivity();
        isTransmuteMode = false;
        selectedIndices.Clear();
        
        manager.isTransmuting = false;
        confirmMTButton.SetActive(true);
        managerDescriptionText.text = "Select two identical Summlings to merge";
    }

    public void HoverTransmute()
    {
        //Explain how to transmute
        managerDescriptionText.text = "Sell your Summling for Soul Essence and XP";
    }

    public void ManagerHover()
    {
        managerDescriptionText.text = "Merge or Transmute your Summlings";
    }

    public void HoverMerge()
    {
        //Explain how to merge
        managerDescriptionText.text = "Choose two of the same Summlings and merge them to increase";
    }

    public void IconButtons(int slotIndex)
    {
        if (manager.isMerging)
        {
            HandleMergeSelection(slotIndex);
            selectBorder[slotIndex].SetActive(true);
        }
        else if (manager.isTransmuting)
        {
            HandleTransmuteSelection(slotIndex);
            selectBorder[slotIndex].SetActive(true);
        }
        else if (manager.isReplacing)
        {
            // Deactivate all borders first
            foreach (GameObject border in selectBorder)
            {
                border.SetActive(false);
            }

            // Activate only the selected border
            selectBorder[slotIndex].SetActive(true);
            manager.SelectReplacementSlot(slotIndex);
            selectedReplacement = true;
        }
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

        //Manager reset
        manager.isMerging = false;
        manager.isTransmuting = false;
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

    private void HandleMergeSelection(int slotIndex)
    {
        if (selectedIndices.Contains(slotIndex))
        {
            // Deselect
            selectedIndices.Remove(slotIndex);
            selectBorder[slotIndex].SetActive(false);
        }
        else
        {
            if (selectedIndices.Count >= 2) return;
            selectedIndices.Add(slotIndex);
            selectBorder[slotIndex].SetActive(true);
        }

        UpdateMergeConfirmation();
    }

    private void HandleTransmuteSelection(int slotIndex)
    {
        // Clear previous selection
        foreach (var index in selectedIndices)
        {
            selectBorder[index].SetActive(false);
        }
        selectedIndices.Clear();

        // Set new selection
        selectedIndices.Add(slotIndex);
        selectBorder[slotIndex].SetActive(true);
        UpdateTransmuteConfirmation();
    }

    private void UpdateMergeConfirmation()
    {
        if (selectedIndices.Count == 2)
        {
            GameObject s1 = manager.summlingsOwned[selectedIndices[0]];
            GameObject s2 = manager.summlingsOwned[selectedIndices[1]];

            SummlingStats stats1 = s1.GetComponent<SummlingStats>();
            SummlingStats stats2 = s2.GetComponent<SummlingStats>();

            if (stats1.specie == stats2.specie && stats1.mark == stats2.mark)
            {
                confirmMTButton.SetActive(true);
                confirmMTText.text = $"Merge {stats1.summlingName} and {stats2.summlingName}?";
                return;
            }
        }
        confirmMTButton.SetActive(false);
    }

    private void UpdateTransmuteConfirmation()
    {
        if (selectedIndices.Count == 1)
        {
            GameObject s = manager.summlingsOwned[selectedIndices[0]];
            SummlingStats stats = s.GetComponent<SummlingStats>();
            confirmMTButton.SetActive(true);
            confirmMTText.text = $"Transmute {stats.summlingName} for {stats.GetTransmuteValue()} SE?";
        }
        else
        {
            confirmMTButton.SetActive(false);
        }
    }


    // Replace PerformMerge and PerformTransmute in SummlingManagerUI.cs
    private void PerformMerge()
    {
        if (selectedIndices.Count != 2) return;
        manager.MergeSummlings(selectedIndices[0], selectedIndices[1]);
    }

    private void PerformTransmute()
    {
        if (selectedIndices.Count != 1) return;
        manager.TransmuteSummling(selectedIndices[0]);
    }

}
