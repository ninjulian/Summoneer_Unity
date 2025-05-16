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

    [Header("References")]
    public PlayerStats player;
    public WaveManager waveManager;
    [Header("Spawning")]
    public Transform summlingSpawnPoint;
    private bool isPartyFull = false;
    public bool canSummon;
    public bool keepSummon;

    public List<GameObject> summlingPrefabs;
    public List<GameObject> summlingsOwned = new List<GameObject>();

    // In SummlingManager.cs
    [Header("Species Weights")]
    public int perSpecieWeight = 20;
    public float summonCostMultiplier = 5f;

    [Header("UI References")]
    public Image previewImage;
    public TMP_Text previewStats;
    public Image[] partySlots;
    private Dictionary<Specie, int> ownedSpeciesCount = new();

    private GameObject currentPendingSummon;
    private int summonCountInWave = 0;

    //Summongin Functions
    public void GenerateSummling()
    {
        if (player.soulEssence >= GetSummonCost())
        {   
            canSummon = true;
            
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
        float[] weights = { 50f, 30f, 20f }; // Newborn, Child, Pre
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
        previewStats.text = $"Species: {stats.specie}\nMark: {stats.mark}\n";

        foreach (var mod in stats.effects)
        {
            previewStats.text += $"{mod.statType}: {mod.value} {(mod.isPercentage ? "%" : "")}\n";
        }
    }

    private void UpdatePartyUI()
    {
        // Debug current state
        Debug.Log($"Updating Party UI. Summlings: {summlingsOwned.Count}, Max Slots: {maxSlots}");

        for (int i = 0; i < maxSlots; i++)
        {
            // Always process all 5 slots
            bool hasSummling = i < summlingsOwned.Count;

            if (hasSummling)
            {
                SummlingStats stats = summlingsOwned[i].GetComponent<SummlingStats>();
                if (stats == null)
                {
                    Debug.LogError($"Slot {i}: Missing SummlingStats component");
                    continue;
                }

                partySlots[i].sprite = stats.icon;
                Debug.Log($"Slot {i} updated with {stats.specie} icon");
            }
            else
            {
                Debug.Log($"Slot {i}: No summling (intentional empty slot)");
            }

            // Force immediate UI update
            partySlots[i].SetAllDirty();
        }

        // Final UI refresh
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(partySlots[0].transform.parent as RectTransform);
    }

    //Confirm Decline Functions
    public void ConfirmSummon()
    {
        // Add this check
        if (currentPendingSummon == null)
        {
            Debug.LogError("Cannot confirm summon: No pending summon!");
            return;
        }

        if (summlingsOwned.Count > maxSlots)
        {   
            Debug.Log("Party full - must replace");
            DeclineSummon();
            return;
        }

        summlingsOwned.Add(currentPendingSummon);
        Debug.Log($"Added Summling. Total: {summlingsOwned.Count}"); //  Add this
        UpdateSpeciesCount();
        ApplySummlingEffects(currentPendingSummon.GetComponent<SummlingStats>());
        UpdatePartyUI();
        currentPendingSummon = null;
    }

    public void DeclineSummon()
    {
        if (currentPendingSummon != null)
        {
            Destroy(currentPendingSummon);
            currentPendingSummon = null;
        }
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

    private void ApplySummlingEffects(SummlingStats stats)
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
    public void ReplaceSummling(int slotIndex)
    {
        if (currentPendingSummon == null) return;

        // Remove old effects
        RemoveSummlingEffects(summlingsOwned[slotIndex].GetComponent<SummlingStats>());

        // Replace with new
        summlingsOwned[slotIndex] = currentPendingSummon;
        ApplySummlingEffects(currentPendingSummon.GetComponent<SummlingStats>());

        UpdateSpeciesCount();
        UpdatePartyUI();
        currentPendingSummon = null;
    }

    // Add to SummlingManager.cs
    private GameObject GetPrefabBySpecies(Specie species)
    {
        foreach (GameObject prefab in summlingPrefabs)
        {
            SummlingStats stats = prefab.GetComponent<SummlingStats>();
            if (stats != null && stats.specie == species)
            {
                return prefab;
            }
        }
        Debug.LogError($"No prefab found for species: {species}");
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

    private void RemoveSummlingEffects(SummlingStats stats)
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
}