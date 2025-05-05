using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] private GameObject systemUI;
    [SerializeField] private GameObject pauseUI;

    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    private InputAction systemAction;
    private InputAction pauseAction;

    private void OnEnable()
    {
        systemAction = playerInput.actions["System"];
        pauseAction = playerInput.actions["Pause"];

        systemAction.performed += _ => ToggleSystemUI();
        pauseAction.performed += _ => TogglePauseUI();

        systemAction.Enable();
        pauseAction.Enable();

        UpdateCursorState();
    }

    private void OnDisable()
    {
        systemAction.performed -= _ => ToggleSystemUI();
        pauseAction.performed -= _ => TogglePauseUI();

        systemAction.Disable();
        pauseAction.Disable();
    }

    private void ToggleSystemUI()
    {
        // Prevent opening System UI if Pause UI is active
        if (!systemUI.activeInHierarchy && pauseUI.activeInHierarchy)
            return;

        systemUI.SetActive(!systemUI.activeInHierarchy);
        UpdateCursorState();
    }

    private void TogglePauseUI()
    {
        // Prevent opening Pause UI if System UI is active
        if (!pauseUI.activeInHierarchy && systemUI.activeInHierarchy)
            return;

        bool shouldPause = !pauseUI.activeInHierarchy;
        pauseUI.SetActive(shouldPause);
        Time.timeScale = shouldPause ? 0f : 1f;
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        bool anyUIActive = systemUI.activeInHierarchy || pauseUI.activeInHierarchy;

        Cursor.lockState = anyUIActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = anyUIActive;
    }
}