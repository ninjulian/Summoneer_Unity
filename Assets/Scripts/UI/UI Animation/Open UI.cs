using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class OpenUI : MonoBehaviour
{
    public Sequence openSequence;
    public Sequence closeSequence;

    private void OnEnable()
    {
        openAnimation();
    }


    private void OnDestroy()
    {
        // Clean up any active tweens when the object is destroyed
        transform.DOKill();
    }

    public void openAnimation()
    {
        openSequence = DOTween.Sequence();
        openSequence.Append(transform.DOScale(new Vector3(0.1f, 0.1f, 1f), 0f));//.SetEase(Ease.OutBack);
        // Animate only X scale to 1 (keeping Y and Z as they are)
        openSequence.Append(transform.DOScaleY(1f, 0.2f).SetEase(Ease.OutBack));
        openSequence.Append(transform.DOScaleX(1f, 0.2f).SetEase(Ease.OutBack));
    }

    public void closeAnimation()
    {
        if (gameObject != null)
        {
            closeSequence = DOTween.Sequence();
            closeSequence.Append(transform.DOScaleX(0.1f, 0.1f));
            closeSequence.Append(transform.DOScaleY(0.1f, 0.1f));

        }
    }
}
