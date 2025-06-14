using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUI : MonoBehaviour
{
    private void OnEnable()
    {
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 1f).setEase(LeanTweenType.easeOutBack); // Smooth easing effect
    }

    private void OnDisable()
    {
        LeanTween.scale(gameObject, new Vector3(0f, 0f, 0f), 1f).setEase(LeanTweenType.easeOutBack); // Smooth easing effect
    }
}
