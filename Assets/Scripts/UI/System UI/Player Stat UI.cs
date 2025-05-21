using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerStatUI : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private TMP_Text health;
    [SerializeField] private TMP_Text defense;
    [SerializeField] private TMP_Text damage;
    [SerializeField] private TMP_Text critChance;
    [SerializeField] private TMP_Text critMultiplier;
    [SerializeField] private TMP_Text fireRate;
    [SerializeField] private TMP_Text movementSpeed;
    [SerializeField] private TMP_Text dashStrength;
    //[SerializeField] private TMP_Text dashCooldown;

    [Header("Player Reference")]
    [SerializeField] private GameObject player;
    private PlayerStats playerStats;

    //private void Start()
    //{
    //    playerStats = player.GetComponent<PlayerStats>();
    //}

    void OnEnable()
    {
        playerStats = player.GetComponent<PlayerStats>();
        health.text = playerStats.currentHealth.ToString() + " / " + playerStats.maxHealth.ToString();

        defense.text = playerStats.defense.ToString();
        damage.text = playerStats.damage.ToString("F1"); // One decimal place
        critChance.text = playerStats.critChance.ToString("F1") + "%";
        critMultiplier.text = playerStats.critMultiplier.ToString("F1") + "x";
        fireRate.text = playerStats.fireRate.ToString("F2");
        movementSpeed.text = playerStats.movementSpeed.ToString("F2");
        dashStrength.text = playerStats.dashStrength.ToString("F1");
        //dashCooldown.text = playerStats.dashCooldown.ToString("F1") + "s";
    }

}
