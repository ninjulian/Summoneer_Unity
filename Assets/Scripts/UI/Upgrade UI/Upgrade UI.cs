using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private TMP_Text descriptionText;
    //[SerializeField] private Button confirmButton;

    private HealthBar playerHealthBar;
    private PlayerStats playerStats;

    public enum UpgradeType
    {
        None,
        FireRate,
        MaxHealth,
        MovementSpeed,
        JumpHeight,
        Damage
    }

    private UpgradeType selectedUpgrade = UpgradeType.None;

    void OnEnable()
    {
        playerStats = player.GetComponent<PlayerStats>();
        playerHealthBar = player.GetComponent<HealthBar>();
        //confirmButton.interactable = false;
    }

    // Selection buttons
    public void SelectFireRate() => SetUpgrade(UpgradeType.FireRate);
    public void SelectMaxHealth() => SetUpgrade(UpgradeType.MaxHealth);
    public void SelectMovementSpeed() => SetUpgrade(UpgradeType.MovementSpeed);
    public void SelectJumpHeight() => SetUpgrade(UpgradeType.JumpHeight);
    public void SelectDamage() => SetUpgrade(UpgradeType.Damage);

    private void SetUpgrade(UpgradeType type)
    {
        selectedUpgrade = type;
        //confirmButton.interactable = true;
        UpdateDescription();
    }

    public void ConfirmSelection()
    {
        if (selectedUpgrade == UpgradeType.None) return;

        switch (selectedUpgrade)
        {
            case UpgradeType.FireRate:
                playerStats.fireRate += 1f;
                break;
            case UpgradeType.MaxHealth:
                playerStats.maxHealth += 10f;
                playerStats.currentHealth = Mathf.Min(playerStats.currentHealth + 10f, playerStats.maxHealth);
                if (playerHealthBar != null) playerHealthBar.HealthIncrease();
                break;
            case UpgradeType.MovementSpeed:
                playerStats.movementSpeed += 1f;
                break;
            case UpgradeType.JumpHeight:
                playerStats.jumpHeight += 1f;
                break;
            case UpgradeType.Damage:
                playerStats.damage += 2f;
                break;
        }

        ResetSelection();
    }

    private void ResetSelection()
    {
        selectedUpgrade = UpgradeType.None;
        //confirmButton.interactable = false;
        ClearDescription();
    }

    // Description updates
    private void UpdateDescription()
    {
        switch (selectedUpgrade)
        {
            case UpgradeType.FireRate:
                ShowFireRateDescription();
                break;
            case UpgradeType.MaxHealth:
                ShowMaxHealthDescription();
                break;
            case UpgradeType.MovementSpeed:
                ShowMovementSpeedDescription();
                break;
            case UpgradeType.JumpHeight:
                ShowJumpHeightDescription();
                break;
            case UpgradeType.Damage:
                ShowDamageDescription();
                break;
        }
    }

    private void ShowFireRateDescription()
    {
        descriptionText.text = $"Fire Rate\nShoot faster\nCurrent: {playerStats.fireRate}\nAfter Upgrade: {playerStats.fireRate + 1f}";
    }

    private void ShowMaxHealthDescription()
    {
        descriptionText.text = $"Max Health\nIncrease health capacity\nCurrent: {playerStats.maxHealth}\nAfter Upgrade: {playerStats.maxHealth + 10f}";
    }

    private void ShowMovementSpeedDescription()
    {
        descriptionText.text = $"Movement Speed\nMove faster\nCurrent: {playerStats.movementSpeed}\nAfter Upgrade: {playerStats.movementSpeed + 1f}";
    }

    private void ShowJumpHeightDescription()
    {
        descriptionText.text = $"Jump Height\nJump higher\nCurrent: {playerStats.jumpHeight}\nAfter Upgrade: {playerStats.jumpHeight + 1f}";
    }

    private void ShowDamageDescription()
    {
        descriptionText.text = $"Damage\nDeal more damage\nCurrent: {playerStats.damage}\nAfter Upgrade: {playerStats.damage + 2f}";
    }

    public void ClearDescription()
    {
        descriptionText.text = "";
    }
}