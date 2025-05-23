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
    private TMP_Text mergeText;
    public Button transmuteButton;
    private TMP_Text transmuteText;

    public GameObject confirmMTButton;
    public TMP_Text confirmMTText;
    public TMP_Text managerDescriptionText;

    private List<int> selectedIndices = new List<int>();
    //private bool isMergeMode = false;
    //private bool isTransmuteMode = false;


    public void Awake()
    {
        mergeText = mergeButton.GetComponentInChildren<TMP_Text>();
        transmuteText = transmuteButton.GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        UpdateCost();
        UpdatePartyPanelLocation(SummonPos);
    }

    //Changes the location and parent of the Party Panel UI so ease of use and referencing
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

    // Page function, for future implementation of other pages
    private void CheckPage()
    {
        // Set initial position based on starting index
        if (currentPageIndex == 0)
        {
            //Debug.Log("Changing Parent to SummonPOS");
            UpdatePartyPanelLocation(SummonPos);
        }
        else if (currentPageIndex == 1)
        {
            //Debug.Log("Changing Parent to ManagamentPOS");
            UpdatePartyPanelLocation(ManagementPos);
        }
    }

    //Updates the cost to Summon Summlings
    private void UpdateCost()
    {
        float cost = manager.GetSummonCost();
        summonCost.text = cost.ToString();
    }

    // Summon Button 
    public void SummonButton()
    {
        manager.GenerateSummling();
        
        manager.UpdateButtonInteractivity();
        if (manager.canSummon)
        {
           
            manager.summonCountInWave += 1;
            UpdateCost();
            Debug.Log("OPening confirmation page");

            summonConfirmationTab.SetActive(true); //Awlays shows confirmation tab

            replaceSummling.SetActive(manager.isReplacing);


        }
        else
        {
            
            Debug.Log("Not enough Soul Essence!");
                // You could add visual feedback here (like shaking the button or showing a message)
                
            
        }

    }

    // Replaces the Summling selected by the newly Summoned Summling
    public void ReplaceSummling()
    {
        if (manager.isReplacing)
        {
            replaceSummling.SetActive(true);
            summonButton.SetActive(summonButton.activeInHierarchy);
        }

        //else
        //{
        //    replaceSummling.SetActive(!replaceSummling.activeInHierarchy);
        //    summonButton.SetActive(!summonButton.activeInHierarchy);
        //}

    }

    // Cancel Button on replacement screen
    public void NotReplacingSummling()
    {
        replaceSummling.SetActive(!replaceSummling.activeInHierarchy);
        summonButton.SetActive(!summonButton.activeInHierarchy);
        manager.DeclineSummon();
    }

    // Confirmation Button with the Summon Button
    public void ConfirmSummon()
    {
        if (manager.isReplacing && !selectedReplacement)
        {
            return; // Require slot selection
        }

        // Close confirmation tab
        summonConfirmationTab.SetActive(false);

        if (manager.isReplacing)
        {
            manager.ConfirmReplacement();
            replaceSummling.SetActive(false); // Explicitly hide replacement UI
        }
        else
        {
            manager.ConfirmSummon();
        }

        // Reset UI
        foreach (GameObject border in selectBorder)
        {
            border.SetActive(false);
        }
        selectedReplacement = false;
    }

    // If the Player decides to not accept the Summoned Summling
    public void CancelSummon()
    {
        // Closes confirmation page and takes away SE
        manager.DeclineSummon();
        summonConfirmationTab.SetActive(!summonConfirmationTab.activeInHierarchy);
        summonButton.SetActive(!manager.isReplacing);
        selectedReplacement = false;

        foreach (GameObject border in selectBorder)
        {
            border.SetActive(false);
        }
    }

    //public void SelectSummling()
    //{
    //    //Get Summling Data
    //}

    // Confirm Button in Manager page (Confirm Merge and Transmutation)
    public void ConfirmMTButton()
    {   
        transmuteText.text = "Transmute Summling";
        mergeText.text = "Merge Summling";


        if (manager.isMerging)
        {
            PerformMerge();
        }
        else if (manager.isTransmuting)
        {
            PerformTransmute();
        }

        ClearSelected();
        
        //Turns off confirm Button
        confirmMTButton.SetActive(false);

        // Updates Party Panel UI
        manager.UpdatePartyUI();
    }

    public void ClearSelected()
    {
        // Clear selections
        foreach (var index in selectedIndices)
        {
            selectBorder[index].SetActive(false);
        }
        selectedIndices.Clear();
    }

    // Transmutation Button (sells Summling for SE and XP)
    public void TransmuteButton()
    {
        if (!manager.isTransmuting)
        {
            // Activate Transmute mode
            transmuteText.text = "Cancel";
            mergeText.text = "Merge Summlings"; // Reset Merge button text

            manager.isTransmuting = true;
            manager.isMerging = false;

            manager.UpdateButtonInteractivity();
            ClearSelected();
            //confirmMTButton.SetActive(true);
            //managerDescriptionText.text = "Select a Summling to transmute";
        }
        else if (manager.isTransmuting)
        {
            // Deactivate Transmute mode
            transmuteText.text = "Transmute Summlings";

            manager.isTransmuting = false;
            manager.UpdateButtonInteractivity();
            ClearSelected();
            //confirmMTButton.SetActive(false);
            managerDescriptionText.text = "Select a Summling to transmute";
        }
    }

    public void MergeButton()
    {
        if (!manager.isMerging)
        {
            // Activate Merge mode
            mergeText.text = "Cancel";
            transmuteText.text = "Transmute Summlings"; // Reset Transmute button text

            manager.isMerging = true;
            manager.isTransmuting = false;

            manager.UpdateButtonInteractivity();
            ClearSelected();
            selectedIndices.Clear();
            //confirmMTButton.SetActive(true);
            managerDescriptionText.text = "Select two identical Summlings to merge";
        }
        else if (manager.isMerging)
        {
            // Deactivate Merge mode
            mergeText.text = "Merge Summlings";

            manager.isMerging = false;
            manager.UpdateButtonInteractivity();
            ClearSelected();
            selectedIndices.Clear();
            //confirmMTButton.SetActive(false);
            managerDescriptionText.text = "Select two identical Summlings to merge";
        }
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
            // Deselect if already selected
            selectedIndices.Remove(slotIndex);
            selectBorder[slotIndex].SetActive(false);
        }
        else
        {
            // Enforce 2-selection limit with "rolling" selection
            if (selectedIndices.Count >= 2)
            {
                // Remove oldest selection (first in list)
                int firstIndex = selectedIndices[0];
                selectedIndices.RemoveAt(0);
                selectBorder[firstIndex].SetActive(false);
            }

            // Add new selection
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

            if (stats1.summlingName == stats2.summlingName && stats1.mark == stats2.mark)
            {
                confirmMTButton.SetActive(true);
                confirmMTText.text = "Merge the pair?";
                return;
            }
            else if (stats1.mark == Mark.Pre || stats2.mark == Mark.Pre)
            {
                confirmMTText.text = "Cannot Merge with a Mark : Pre";
            }
            else
            {
                confirmMTText.text = "Select a matching Summling pair";
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
