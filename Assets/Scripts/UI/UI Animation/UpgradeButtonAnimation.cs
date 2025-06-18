using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonAnimation : MonoBehaviour
{
    private RectTransform rectTransform;
    public Sequence spawnSequence;
    public Sequence destroySequence;

    private void Start()
    {
        
    }

    public void BuyItem()
    {
        transform.DOScaleX(0f, 0.5f).SetEase(Ease.OutBounce);//.OnComplete(MyFunction);

    }

    public void HoverScale()
    {
 
        transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).SetEase(Ease.OutBack);//.OnComplete(MyFunction);
    }

    public void LeaveScale()
    {
        transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f).SetEase(Ease.InBack);//.OnComplete(MyFunction);
    }

    public void SpawnItem()
    {
        spawnSequence = DOTween.Sequence();
        // rectTransform.DOAnchorPosY(-50f, 1f, false);
        spawnSequence.Append(transform.DOScale(new Vector3(0.01f, 0.1f, 1f), 0f));

        //rectTransform.DOAnchorPosY(0f, 1f, false).SetEase(Ease.OutBounce);
        spawnSequence.Append((transform.DOScaleY(1f, 0.2f)).SetEase(Ease.OutBack));

        spawnSequence.Append((transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f)).SetEase(Ease.OutBack));
    }

    public void DestroyItem()
    {
        destroySequence = DOTween.Sequence();
        // rectTransform.DOAnchorPosY(-50f, 1f, false);
        destroySequence.Append((transform.DOScaleX(0.1f, 0.3f)).SetEase(Ease.InBack));

    }
}
