using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionAnimation : MonoBehaviour
{
    public List<GameObject> instructionObjects; // Assign in Inspector in order
    public float fadeDuration = 1.5f;
    public float floatHeight = 0.2f;
    public float floatSpeed = 1f;

    private CanvasGroup[] canvasGroups;
    private Vector3[] originalPositions;
    private int currentActiveCount = 0;
    private float floatTimer = 0f;

    private void Start()
    {
        // Initialize arrays
        canvasGroups = new CanvasGroup[instructionObjects.Count];
        originalPositions = new Vector3[instructionObjects.Count];

        for (int i = 0; i < instructionObjects.Count; i++)
        {
            if (instructionObjects[i] != null)
            {
                // Set up CanvasGroup for each object
                canvasGroups[i] = instructionObjects[i].GetComponent<CanvasGroup>();
                if (canvasGroups[i] == null)
                {
                    canvasGroups[i] = instructionObjects[i].AddComponent<CanvasGroup>();
                }

                // Store original position and set initial state
                originalPositions[i] = instructionObjects[i].transform.position;
                canvasGroups[i].alpha = 0f;
                instructionObjects[i].SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        // Apply floating animation to all active objects
        floatTimer += Time.fixedDeltaTime;
        for (int i = 0; i < currentActiveCount; i++)
        {
            if (i < instructionObjects.Count && instructionObjects[i] != null)
            {
                float yOffset = Mathf.Sin(floatTimer * floatSpeed) * floatHeight;
                instructionObjects[i].transform.position =
                    originalPositions[i] + new Vector3(0, yOffset, 0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentActiveCount < instructionObjects.Count)
        {

            // Activate and fade in the next object in sequence
            if (instructionObjects.Count > 0) 
            {
                StartCoroutine(FadeIn(currentActiveCount));
                currentActiveCount++;
            }

            
        }
    }

    private IEnumerator FadeIn(int index)
    {
        if (index >= instructionObjects.Count || instructionObjects[index] == null)
            yield break;

        instructionObjects[index].SetActive(true);
        instructionObjects[index].transform.position = originalPositions[index];

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            if (canvasGroups[index] != null)
            {
                canvasGroups[index].alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (canvasGroups[index] != null)
        {
            canvasGroups[index].alpha = 1f;
        }
    }
}