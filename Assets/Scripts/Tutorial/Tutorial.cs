using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    [Header("Settings")]
    public bool resetPosition = false;
    public bool spawnUI = false;
    public bool enableInputActions = false;
    public bool disableObject = false;

    [Header("References")]
    public Transform resetPos;
    public GameObject uiObject;
    public List<InputActionReference> inputActionsToEnable; // Assign in Inspector
    public GameObject playerObject;
    public GameObject oldTriggers;
    public GameObject objectToDisable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (resetPosition && resetPos != null)
            {
                CharacterController cc = playerObject.GetComponent<CharacterController>();

                // Disable the CharacterController before moving
                if (cc != null)
                    cc.enabled = false;

                // Reset position
                playerObject.transform.position = resetPos.position;
                if (cc != null)
                    cc.enabled = true;
            }


            if (enableInputActions)
            {
                EnableInputActions();
            }
            else
            {
                DisableInputActions();
            }
            if (spawnUI && uiObject != null)
            {
                uiObject.SetActive(true);
            }

            if (oldTriggers != null)
            {
                Destroy(oldTriggers);
            }

            if (disableObject && objectToDisable != null)
            {
                Destroy(objectToDisable);
            }
        }
    }

    private void EnableInputActions()
    {
        foreach (InputActionReference actionRef in inputActionsToEnable)
        {
            if (actionRef != null)
            {
                actionRef.action.Enable();
            }
        }
    }

    private void DisableInputActions()
    {
        foreach (InputActionReference actionRef in inputActionsToEnable)
        {
            if (actionRef != null)
            {
                actionRef.action.Disable();
            }
        }
    }
}