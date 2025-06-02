using System.Collections;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float lifespan = 1f;

    // How fast it moves downzward
    public float floatSpeed = 1f;

    // How long it takes to fade
    public float fadeDuration = 1f;      

    private TextMeshPro tmpText;
    private Color startColor;

    void Start()
    {
        tmpText = GetComponent<TextMeshPro>();
        if (tmpText != null)
        {
            startColor = tmpText.color;
        }

        Destroy(gameObject, lifespan);
    }
    
    void Update()
    {
        // Face the camera
        transform.forward = Camera.main.transform.forward;

        // Move downward over time
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out over time
        if (tmpText != null)
        {
            float fadeAmount = Mathf.Clamp01(1 - (Time.timeSinceLevelLoad - Time.time + lifespan) / fadeDuration);
            tmpText.color = new Color(startColor.r, startColor.g, startColor.b, fadeAmount);
        }
    }
}
