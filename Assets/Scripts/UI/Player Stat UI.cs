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
    [SerializeField] private TMP_Text dashSpeed;
    [SerializeField] private TMP_Text dashCooldown;
}
