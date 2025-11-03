using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class UpgradeUIAnimations : MonoBehaviour
{

    public Sequence spawnSequence;
    public Sequence destroySequence;

    [Header("Sway Settings")]
    public float swayAngle = 5f;      // How far to sway in degrees
    public float swayDuration = 2f;   // Time for one sway cycle
    public Ease easeType = Ease.OutSine;


    public void OpenAnimation(GameObject obj)
    {
        if (obj == null) return;

        // Kill any existing animations on this object
        DOTween.Kill(obj.transform);

        spawnSequence = DOTween.Sequence();
        spawnSequence.SetTarget(obj.transform); // Link sequence to object
        spawnSequence.Append(obj.transform.DOScale(new Vector3(0.01f, 0.1f, 1f), 0f));
        spawnSequence.Append(obj.transform.DOScaleY(1f, 0.2f).SetEase(Ease.OutBack));
        spawnSequence.Append(obj.transform.DOScale(new Vector3(1f, 1f, 1f), Random.Range(0.1f, 0.3f)).SetEase(Ease.OutBack));
    }

    public void CloseAnimation(GameObject obj)
    {
        if (obj == null) return;

        // Kill any existing animations on this object
        DOTween.Kill(obj.transform);

        destroySequence = DOTween.Sequence();
        destroySequence.SetTarget(obj.transform); // Link sequence to object
        destroySequence.Append(obj.transform.DOScaleX(0.1f, 0.3f).SetEase(Ease.InBack));
        destroySequence.OnComplete(() => OnCloseComplete(obj)); // Optional callback
    }

    private void OnCloseComplete(GameObject obj)
    {
        // Optional: Do something after close animation completes
        // obj.SetActive(false); // Example: disable object after animation
        obj.SetActive(false);
    }

    // Method to stop all animations
    public void StopAllAnimations(GameObject obj)
    {
        if (obj != null)
        {
            DOTween.Kill(obj.transform);
        }
    }

    public void SwayingSidewaysAnimation(RectTransform rt)
    {
        // Kill any existing sway animations first
        DOTween.Kill("ui_sway");

        // Create a continuous back-and-forth rotation
        rt.DOLocalRotate(new Vector3(0, 0, swayAngle), swayDuration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo)     // Infinite yoyo motion
            .SetId("ui_sway");
    }
    public void StopSwayingAnimation()
    {
                // Kill the sway animation using its ID
        DOTween.Kill("ui_sway");
    }
}

