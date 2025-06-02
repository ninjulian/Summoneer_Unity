
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] PlayerInput playerInput;
    private InputAction aimAction;

    //[SerializeField] private int cameraPriorityBoost = 10;

    [Header("Camera")]
    private CinemachineVirtualCamera virtualCamera;


    private void Awake()
    {   
        // Depricated aiming feature
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        aimAction = playerInput.actions["Shoot"];
    }

    private void Start()
    {   
        // Locks cursor to screen and makes cursor invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //private void OnEnable()
    //{
    //    aimAction.performed += _ => StartAim();
    //    aimAction.canceled += _ => StopAim();
    //}

    //private void OnDisable()
    //{
    //    aimAction.performed -= _ => StartAim();
    //    aimAction.canceled -= _ => StopAim();
    //}

    //private void StartAim()
    //{
    //    virtualCamera.Priority += cameraPriorityBoost;
    //}

    //private void StopAim()
    //{
    //    virtualCamera.Priority -= cameraPriorityBoost;
    //}
}
