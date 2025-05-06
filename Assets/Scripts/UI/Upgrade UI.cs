using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UpgradeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    private HealthBar playerHealthBar;
     private PlayerStats playerStats;
    [SerializeField] private TMP_Text descriptionText;

    void OnEnable()
    {
        playerStats = player.GetComponent<PlayerStats>();
        playerHealthBar = GetComponent<HealthBar>();
    }

    // Button click functions
    public void FireRateButton()
    {
        playerStats.fireRate += 1f;
    }

    public void MaxHealthButton()
    {
        playerStats.maxHealth += 10f;
        playerStats.currentHealth += 10f;
       // playerHealthBar.HealthIncrease();
    }

    public void MovementSpeedButton()
    {
        playerStats.movementSpeed += 1f;
    }

    public void JumpHeightButton()
    {
        playerStats.jumpHeight += 1f;
    }

    public void DamageButton()
    {
        playerStats.damage += 2f;
    }

    // Hover description functions
    public void ShowFireRateDescription()
    {
        descriptionText.text = "Fire Rate\n" +
                              "Shoot faster\n" +
                              "Current: " + playerStats.fireRate + "\n" +
                              "After Upgrade: " + (playerStats.fireRate + 1f);
    }

    public void ShowMaxHealthDescription()
    {
        descriptionText.text = "Max Health\n" +
                              "Increase health capacity\n" +
                              "Current: " + playerStats.maxHealth + "\n" +
                              "After Upgrade: " + (playerStats.maxHealth + 10f);
    }

    public void ShowMovementSpeedDescription()
    {
        descriptionText.text = "Movement Speed\n" +
                              "Move faster\n" +
                              "Current: " + playerStats.movementSpeed + "\n" +
                              "After Upgrade: " + (playerStats.movementSpeed + 1f);
    }

    public void ShowJumpHeightDescription()
    {
        descriptionText.text = "Jump Height\n" +
                              "Jump higher\n" +
                              "Current: " + playerStats.jumpHeight + "\n" +
                              "After Upgrade: " + (playerStats.jumpHeight + 1f);
    }

    public void ShowDamageDescription()
    {
        descriptionText.text = "Damage\n" +
                              "Deal more damage\n" +
                              "Current: " + playerStats.damage + "\n" +
                              "After Upgrade: " + (playerStats.damage + 2f);
    }

    public void ClearDescription()
    {
        descriptionText.text = "";
    }
}