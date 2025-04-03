using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class SwitchCamera : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] PlayerInput playerInput;
    private InputAction aimAction;

    [SerializeField] private int cameraPriorityBoost = 10;

    [Header("Camera")]
    private CinemachineVirtualCamera virtualCamera;


    private void Awake()
    {   
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        aimAction = playerInput.actions["Aim"];
    }

    private void OnEnable()
    {
        aimAction.performed += _ => StartAim();
        aimAction.performed += _ => StopAim();
    }

    private void OnDisable()
    {
        aimAction.performed -= _ => StartAim();
        aimAction.performed -= _ => StopAim();
    }

    private void StartAim()
    {
        virtualCamera.Priority += cameraPriorityBoost;
    }

    private void StopAim()
    {
        virtualCamera.Priority -= cameraPriorityBoost;
    }
}
