using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class OpenUI : MonoBehaviour
{
    private void OnEnable()
    {
        //Vector3 currentScale = transform.localScale;
        //currentScale.x = 0f; // Reset X to 0
        transform.localScale = new Vector3 (0f, 1f, 1f);

        // Animate only X scale to 1 (keeping Y and Z as they are)
        transform.DOScaleX(1f, 0.3f).SetEase(Ease.OutBack);
    }

    //private void OnDisable()
    //{
    //    transform.DOScale(Vector3.zero, 1f).SetEase(Ease.OutBack); // Smooth easing effect
    //}

    private void OnDestroy()
    {
        // Clean up any active tweens when the object is destroyed
        transform.DOKill();
    }
}
