using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.ProBuilder.MeshOperations;



public class SummlingManager : MonoBehaviour
{
    [Header("Configuration")]
    private int maxSlots = 5;
    public int baseSummonCost = 100;
    public float waveCostFactor = 10f;
    public float summonCountFactor = 5f;
    public int currentWave = 1;


    //NEw additions

    [System.Serializable]
    public class SummlignList
    {
        public Specie specie;
        public List<GameObject> prefabs;
    }

    [Header("Species Prefabs")]
    public List<SummlignList> summlingPrefabList = new List<SummlignList>();

    [Header("References")]
    public PlayerStats player;
    private WaveManager waveManager;
    private UpgradeManager upgradeManager;


    [Header("Spawning")]
    public Transform summlingSpawnPoint;
    private bool isPartyFull = false;
    public bool canSummon;
    public bool keepSummon;

    //public List<GameObject> summlingPrefabs;
    public List<GameObject> summlingsOwned = new List<GameObject>();

    // In SummlingManager.cs
    [Header("Species Weights")]
    public int perSpecieWeight = 20;
    public float summonCostMultiplier = 5f;

    [Header("UI References")]
    public Image previewImage;
    public Image previewBorder;
    public TMP_Text previewStats;
    public Image[] icons;
    public Image[] iconBorder;
    public Button[] iconButtons;
    public Sprite defaultIcon;

    //Information UI
    public GameObject[] highlightPreview;
    public TMP_Text[] highlightText;

    private Dictionary<Specie, int> ownedSpeciesCount = new();

    //Replacement Methods
    public int pendingReplacementIndex = -1;
    public bool isReplacing = false;
    public bool isMerging = false;
    public bool isTransmuting = false;

    private GameObject currentPendingSummon;
    private int summonCountInWave = 0;

    //Summongin Functions

    public void Start()
    {
        UpdateButtonInteractivity();
        waveManager = GetComponent<WaveManager>();
        upgradeManager = GetComponent<UpgradeManager>();
    }

    public void GenerateSummling()
    {
        if (player.soulEssence >= GetSummonCost())
        {
            canSummon = true;
            if (summlingsOwned.Count >= maxSlots)
            {
                isReplacing = true;
                UpdateButtonInteractivity();
            }

            player.SpendSoulEssence(GetSummonCost());

            // Calculate species weights
            Dictionary<Specie, int> weights = new();
            foreach (Specie s in Enum.GetValues(typeof(Specie)))
            {
                weights[s] = 1; // Base weight
                if (ownedSpeciesCount.ContainsKey(s))
                    weights[s] += ownedSpeciesCount[s] * perSpecieWeight;
            }

            // Select species
            Specie selectedSpecies = WeightedRandom(weights);

            // Select mark
            Mark mark = GetRandomMark();

            // In SummlingManager.cs - Modified GenerateSummling excerpt
            currentPendingSummon = Instantiate(GetPrefabBySpecies(selectedSpecies), summlingSpawnPoint.position, summlingSpawnPoint.rotation);
            SummlingStats stats = currentPendingSummon.GetComponent<SummlingStats>();
            stats.mark = mark;
            // Apply mark multipliers only to specified stats
            float markMultiplier = mark switch
            {
                Mark.Newborn => 1f,
                Mark.Child => 1.2f,
                Mark.Pre => 1.5f,
                _ => 1f
            };
            stats.ApplyMarkMultiplier(markMultiplier);
            UpdatePreviewUI(stats);

            

        }
        else
        {
            canSummon = false;
            Debug.Log("Not enough Soul Essence");
            return;
        }
    }

    //Calculation for Summons
    private Specie WeightedRandom(Dictionary<Specie, int> weights)
    {
        int total = weights.Values.Sum();
        int random = UnityEngine.Random.Range(0, total);

        foreach (var kvp in weights)
        {
            if (random < kvp.Value) return kvp.Key;
            random -= kvp.Value;
        }
        return Specie.Aquatic;
    }

    private Mark GetRandomMark()
    {
        float[] weights = { 75f, 20f, 5f }; // Newborn, Child, Pre
        float total = weights.Sum();
        float random = UnityEngine.Random.Range(0, total);

        for (int i = 0; i < weights.Length; i++)
        {
            if (random < weights[i]) return (Mark)i;
            random -= weights[i];
        }
        return Mark.Newborn;
    }

    public float GetSummonCost()
    {
        return Mathf.Floor(baseSummonCost + (currentWave * summonCostMultiplier));
    }

    //UI Update Functions
    private void UpdatePreviewUI(SummlingStats stats)
    {
        previewImage.sprite = stats.icon;

        switch (stats.mark)
        {
            case Mark.Newborn:
                previewBorder.color = Color.grey;
                break;

            case Mark.Child:
                previewBorder.color = Color.blue;
                break;

            case Mark.Pre:
                previewBorder.color = Color.red;
                break;
        }
        previewStats.text = $"Species: {stats.specie}\nMark: {stats.mark}\n";

        foreach (var mod in stats.effects)
        {
            previewStats.text += $"{mod.statType}: {mod.value} {(mod.isPercentage ? "%" : "")}\n";
        }
    }

    public void UpdatePartyUI()
    { 

        for (int i = 0; i < maxSlots; i++)
        {
            // Always process all 5 slots
            bool hasSummling = i < summlingsOwned.Count;

            highlightPreview[i].SetActive(false); // Reset highlight state

            if (hasSummling)
            {
                SummlingStats stats = summlingsOwned[i].GetComponent<SummlingStats>();
                if (stats == null)
                {
                    continue;
                }
                icons[i].sprite = stats.icon;

                switch (stats.mark)
                {
                    case Mark.Newborn:
                        iconBorder[i].color = Color.grey;
                        break;

                    case Mark.Child:
                        iconBorder[i].color = Color.blue;
                        break;

                    case Mark.Pre:
                        iconBorder[i].color = Color.red;
                        break;
                }

            }
        }
        
    }

    public void UpdateEmptySlots()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            bool isSlotEmpty = i >= summlingsOwned.Count || summlingsOwned[i] == null;

            // Update icon and appearance
            if (isSlotEmpty)
            {
                icons[i].sprite = defaultIcon;
                iconBorder[i].color = Color.grey;
                highlightPreview[i].SetActive(false);
            }

            // Always disable buttons for empty slots
            iconButtons[i].interactable = !isSlotEmpty && ShouldButtonBeInteractable();
        }
    }

    //Confirm Decline Functions
    public void ConfirmSummon()
    {
        if (currentPendingSummon == null)
        {
            Debug.LogError("Cannot confirm summon: No pending summon!");
            return;
        }

        if (summlingsOwned.Count >= maxSlots)
        {
            // Only set replacing state, don't decline
            isReplacing = true;

            UpdateButtonInteractivity();

            return; // Don't destroy the pending summon!
        }

        // Normal confirmation logic
        summlingsOwned.Add(currentPendingSummon);
        UpdateSpeciesCount();
        ApplySummlingEffects(currentPendingSummon.GetComponent<SummlingStats>());


        //Adds stat modifiers
        SummlingStats stats = currentPendingSummon.GetComponent<SummlingStats>(); // Changed here
        ApplySummlingEffects(stats);
        UpgradeManager.Instance.ApplyExistingModifiersToSummling(stats); // Add this

        UpdatePartyUI();
        currentPendingSummon = null;

       

    }

    // Modified DeclineSummon to handle replacement cancellation
    public void DeclineSummon()
    {
        if (currentPendingSummon != null)
        {
            Destroy(currentPendingSummon);
            currentPendingSummon = null;
        }

        // Reset replacement state
        isReplacing = false;

        UpdateButtonInteractivity();

        pendingReplacementIndex = -1;
    }

    //Merging Functions
    public void TryMergeSummlings()
    {
        var groups = summlingsOwned
            .GroupBy(s => new
            {
                Species = s.GetComponent<SummlingStats>().specie,
                Mark = s.GetComponent<SummlingStats>().mark
            });

        foreach (var group in groups)
        {
            if (group.Count() >= 2 && group.Key.Mark != Mark.Pre)
            {
                // Remove old summlings
                var toRemove = group.Take(2).ToList();
                foreach (var summling in toRemove)
                {
                    RemoveSummlingEffects(summling.GetComponent<SummlingStats>());
                    summlingsOwned.Remove(summling);
                    Destroy(summling);
                }

                // Create upgraded version
                var newSummling = Instantiate(toRemove[0]);
                var stats = newSummling.GetComponent<SummlingStats>();
                stats.mark = group.Key.Mark + 1;
                summlingsOwned.Add(newSummling);
                ApplySummlingEffects(stats);

                UpdateSpeciesCount();
                UpdatePartyUI();
            }
        }
    }

    //Effect application System
    public void ApplySummlingEffects(SummlingStats stats)
    {
        foreach (var mod in stats.effects)
        {
            player.GetType().GetField(mod.statType.ToString())?
                .SetValue(player, GetModifiedValue(mod));
        }
    }

    private float GetModifiedValue(SummlingStats.StatModifier mod)
    {
        float baseValue = (float)player.GetType()
            .GetField(mod.statType.ToString())
            .GetValue(player);

        return mod.isPercentage ?
            baseValue * (1 + mod.value / 100f) :
            baseValue + mod.value;
    }

    //Replacment Function
    // Modified ReplaceSummling method
    public void ReplaceSummling(int slotIndex)
    {
        if (currentPendingSummon == null) return;
        if (slotIndex < 0 || slotIndex >= summlingsOwned.Count) return;

        // Remove old effects
        RemoveSummlingEffects(summlingsOwned[slotIndex].GetComponent<SummlingStats>());
        Destroy(summlingsOwned[slotIndex]);

        // Replace with new
        summlingsOwned[slotIndex] = currentPendingSummon;
        ApplySummlingEffects(currentPendingSummon.GetComponent<SummlingStats>());

        //Apply modifier effects
        SummlingStats newStats = currentPendingSummon.GetComponent<SummlingStats>();
        ApplySummlingEffects(newStats);
        UpgradeManager.Instance.ApplyExistingModifiersToSummling(newStats); 

                UpdateSpeciesCount();
        UpdatePartyUI();

        UpdateEmptySlots();

        // Clear pending summon
        currentPendingSummon = null;
        isReplacing = false;

        UpdateButtonInteractivity();
    }

    // Add to SummlingManager.cs
    private GameObject GetPrefabBySpecies(Specie species)
    {
        // Find the matching specie list
        var specieList = summlingPrefabList.FirstOrDefault(s => s.specie == species);

        if (specieList != null && specieList.prefabs.Count > 0)
        {
            // Return random prefab from the specie's list
            int randomIndex = UnityEngine.Random.Range(0, specieList.prefabs.Count);
            return specieList.prefabs[randomIndex];
        }
        return null;
    }

    private void UpdateSpeciesCount()
    {
        ownedSpeciesCount.Clear();

        foreach (GameObject summling in summlingsOwned)
        {
            // Skip null entries
            if (summling == null) continue;

            SummlingStats stats = summling.GetComponent<SummlingStats>();
            // Skip if component missing
            if (stats == null) continue;

            Specie s = stats.specie;
            ownedSpeciesCount[s] = ownedSpeciesCount.ContainsKey(s) ? ownedSpeciesCount[s] + 1 : 1;
        }
    }

    public void RemoveSummlingEffects(SummlingStats stats)
    {
        foreach (var mod in stats.effects)
        {
            var field = player.GetType().GetField(mod.statType.ToString());
            if (field != null)
            {
                float currentValue = (float)field.GetValue(player);
                float originalValue = mod.isPercentage ?
                    currentValue / (1 + mod.value / 100f) :
                    currentValue - mod.value;

                field.SetValue(player, Mathf.Floor(originalValue));
            }
        }
    }

    public void SelectReplacementSlot(int slotIndex)
    {
        if (!isReplacing) return;

        pendingReplacementIndex = slotIndex;

        Debug.Log($"Selected slot {slotIndex} for replacement");
    }

    // New method to confirm replacement
    public void ConfirmReplacement()
    {
        if (!isReplacing || pendingReplacementIndex == -1)
        {
            Debug.LogError("No replacement pending");
            return;
        }

        if (currentPendingSummon == null)
        {
            Debug.LogError("No summon to replace with");
            return;
        }

        // Perform replacement
        ReplaceSummling(pendingReplacementIndex);

        // Reset state
        isReplacing = false;
        pendingReplacementIndex = -1;

        UpdateButtonInteractivity();

    }

    public void MergeSummlings(int index1, int index2)
    {
        if (index1 < 0 || index1 >= summlingsOwned.Count) return;
        if (index2 < 0 || index2 >= summlingsOwned.Count) return;
        if (index1 == index2) return;

        GameObject s1 = summlingsOwned[index1];
        GameObject s2 = summlingsOwned[index2];
        SummlingStats stats1 = s1.GetComponent<SummlingStats>();
        SummlingStats stats2 = s2.GetComponent<SummlingStats>();

        if (stats1.summlingName != stats2.summlingName || stats1.mark != stats2.mark) return;
        if (stats1.mark == Mark.Pre) return;

        // Proceed with merge
        int targetIndex = Mathf.Min(index1, index2);
        GameObject newSummling = Instantiate(s1);
        SummlingStats newStats = newSummling.GetComponent<SummlingStats>();
        newStats.mark = stats1.mark + 1;

        //Apply summling stat modifiers
        ApplySummlingEffects(newStats);
        UpgradeManager.Instance.ApplyExistingModifiersToSummling(newStats); // Add this

        // Remove old
        RemoveSummlingEffects(stats1);
        RemoveSummlingEffects(stats2);
        summlingsOwned.Remove(s1);
        summlingsOwned.Remove(s2);
        Destroy(s1);
        Destroy(s2);
        icons[index2].sprite = defaultIcon;

        // Insert new
        summlingsOwned.Insert(targetIndex, newSummling);
        ApplySummlingEffects(newStats);

        UpdateSpeciesCount();
        UpdatePartyUI();

        UpdateEmptySlots();
    }

    public void TransmuteSummling(int index)
    {
        if (index < 0 || index >= summlingsOwned.Count) return;

        GameObject summling = summlingsOwned[index];
        SummlingStats stats = summling.GetComponent<SummlingStats>();
        if (stats == null) return;

        player.GainSoulEssence(stats.GetTransmuteValue());
        RemoveSummlingEffects(stats);
        summlingsOwned.RemoveAt(index);
        Destroy(summling);

        UpdateSpeciesCount();
        UpdatePartyUI();

        UpdateEmptySlots() ;

    }


    public void UpdateButtonInteractivity()
    {
        foreach (Button button in iconButtons)
        {
            if (isReplacing || isTransmuting || isMerging)
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }

        UpdateEmptySlots(); // Ensure UI consistency
    }

    private bool ShouldButtonBeInteractable()
    {
        // Add any additional conditions for interactivity here
        return isReplacing || isMerging || isTransmuting;
    }

    // Add these methods to handle hover states
    public void ShowHighlight(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= highlightPreview.Length) return;
        if (slotIndex >= summlingsOwned.Count) return;

        GameObject summling = summlingsOwned[slotIndex];
        if (summling == null) return;

        SummlingStats stats = summling.GetComponent<SummlingStats>();
        if (stats == null) return;

        // Update highlight information
        highlightPreview[slotIndex].SetActive(true);
        highlightText[slotIndex].text = $"Species: {stats.specie}\nMark: {stats.mark}\n";

        foreach (var mod in stats.effects)
        {
            highlightText[slotIndex].text += $"{mod.statType}: {mod.value} {(mod.isPercentage ? "%" : "")}\n";
        }
    }

    public void HideHighlight(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= highlightPreview.Length) return;
        highlightPreview[slotIndex].SetActive(false);
    }
}