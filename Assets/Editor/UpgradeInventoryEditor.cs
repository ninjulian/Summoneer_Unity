using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(UpgradeInventory))]
public class UpgradeInventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UpgradeInventory inventory = (UpgradeInventory)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Owned Upgrades", EditorStyles.boldLabel);

        if (inventory.ownedUpgrades != null && inventory.ownedUpgrades.Count > 0)
        {
            foreach (var pair in inventory.ownedUpgrades)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(pair.Key, GUILayout.Width(150));
                EditorGUILayout.LabelField("Count: " + pair.Value);
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.LabelField("No upgrades owned yet.");
        }
    }
}