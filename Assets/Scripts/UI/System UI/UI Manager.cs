using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] private GameObject systemUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private GameObject summlingManagerUI;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject soulEssenceText;
    public GameObject deathScreen;
    public GameObject playerHud;

    [Header("Open Timer")]
    public GameObject openSystemTimer;
    public TMP_Text openSystemTimerText;
    public float openTimer = 3f;

    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    private InputAction systemAction, pauseAction , moveAction, lookAction, jumpAction, dashAction, shootAction, focusAction;

    public WaveManager waveManager;

    private void Awake()
    {
        Time.timeScale = 1f;

        // Cache all input actions
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        dashAction = playerInput.actions["Dash"];
        shootAction = playerInput.actions["Shoot"];
        focusAction = playerInput.actions["Focus"];
        systemAction = playerInput.actions["System"];
        pauseAction = playerInput.actions["Pause"];

        UpdateCursorState();
        SetPlayerInputs(true);
    }

    private void OnEnable()
    {
        systemAction.performed += _ => ToggleSystemUI();
        pauseAction.performed += _ => TogglePauseUI();

        systemAction.Enable();
        pauseAction.Enable();

        UpdateCursorState();
        SetPlayerInputs(true);
    }

    private void OnDestroy()
    {
        systemAction.performed -= _ => ToggleSystemUI();
        pauseAction.performed -= _ => TogglePauseUI();

        systemAction.Disable();
        pauseAction.Disable();
    }

    public void ToggleSystemUI()
    {
        if (systemUI == null || pauseUI == null) return; // Add null checks

        // Prevent opening System UI if Pause UI is active
        if (!systemUI.activeInHierarchy && pauseUI.activeInHierarchy) return;

        systemUI.SetActive(!systemUI.activeInHierarchy);
        UpdateCursorState();
    }

    public void TogglePauseUI()
    {
        if (systemUI == null || pauseUI == null) return; // Add null checks

        // Prevent opening Pause UI if System UI is active
        if (!pauseUI.activeInHierarchy && systemUI.activeInHierarchy) return;

        bool shouldPause = !pauseUI.activeInHierarchy;
        pauseUI.SetActive(shouldPause);
        Time.timeScale = shouldPause ? 0f : 1f;
        UpdateCursorState();
    }

    public void ToggleUpgradeUI()
    {
        // Prevent opening if System or Pause UI is active
        //if (!upgradeUI.activeInHierarchy && (systemUI.activeInHierarchy || pauseUI.activeInHierarchy))
        //    return;

        upgradeUI.SetActive(!upgradeUI.activeInHierarchy);

        UpdateCursorState();
    }

    public void OnUpgradeDone()
    {
        ToggleUpgradeUI(); // Close the upgrade UI
        waveManager.CompleteWave();
        //waveManager.StartCountdown(); // Start the next wave countdown
    }

    public void ToggleSummlingManagerUI()
    {
        summlingManagerUI.SetActive(!summlingManagerUI.activeInHierarchy);
        UpdateCursorState();
    }

    public void UpdateCursorState()
    {
        bool anyUIActive = systemUI.activeInHierarchy || pauseUI.activeInHierarchy || upgradeUI.activeInHierarchy ||
            summlingManagerUI.activeInHierarchy || deathScreen.activeInHierarchy;

        bool anyUIwSE = upgradeUI.activeInHierarchy || summlingManagerUI.activeInHierarchy;
       
        soulEssenceText.SetActive(anyUIwSE); 
        

        // Set cursor lock state and visibility
        if (anyUIActive)
        {
            Cursor.lockState = CursorLockMode.Confined; // Or None if you want cursor to leave window
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Properly locks the cursor
            Cursor.visible = false;
        }

        // Toggle the Input actions
        SetPlayerInputs(!anyUIActive);

        // Handle crosshair - show only when no UI is active
        if (crosshair != null)
        {
            crosshair.SetActive(!anyUIActive);
        }
    }

    public void SetPlayerInputs(bool enable)
    {
        // Enable/disable each action
        if (enable)
        {
            moveAction.Enable();
            lookAction.Enable();
            jumpAction.Enable();
            dashAction.Enable();
            shootAction.Enable();
            focusAction.Enable();
        }
        else
        {
            moveAction.Disable();
            lookAction.Disable();
            jumpAction.Disable();
            dashAction.Disable();
            shootAction.Disable();
            focusAction.Disable();
        }
    }
}