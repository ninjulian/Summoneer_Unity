using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionAnimation : MonoBehaviour
{
    public List<GameObject> instructionObjects; 


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
                //Gets Canvas component to fade with
                canvasGroups[i] = instructionObjects[i].GetComponent<CanvasGroup>();
                if (canvasGroups[i] == null)
                {
                    canvasGroups[i] = instructionObjects[i].AddComponent<CanvasGroup>();
                }

                // Store original position and then hides the object
                originalPositions[i] = instructionObjects[i].transform.position;
                canvasGroups[i].alpha = 0f;
                instructionObjects[i].SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        // Flaoting animation to objects inside the list
        floatTimer += Time.fixedDeltaTime;
        for (int i = 0; i < currentActiveCount; i++)
        {
            if (i < instructionObjects.Count && instructionObjects[i] != null)
            {   
                // Updates only the 7 axis
                float yOffset = Mathf.Sin(floatTimer * floatSpeed) * floatHeight;
                instructionObjects[i].transform.position =
                    originalPositions[i] + new Vector3(0, yOffset, 0);
            }
        }
    }

    // IF there are more than 1 object to fade it in and will move to the next
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

        // Reactivate the object and resets its transform position

        instructionObjects[index].SetActive(true);
        instructionObjects[index].transform.position = originalPositions[index];

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            if (canvasGroups[index] != null)
            {   
                // Slowly increases the alpha/transparancy
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