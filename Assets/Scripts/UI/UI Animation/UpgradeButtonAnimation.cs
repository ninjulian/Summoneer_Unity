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
        if (gameObject != null)
        {
            transform.DOScaleX(0f, 0.5f).SetEase(Ease.OutBounce);//.OnComplete(MyFunction);

        }

    }

    public void HoverScale()
    {
        if (gameObject != null)
        {
            transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).SetEase(Ease.OutBack);//.OnComplete(MyFunction);

        }
    }

    public void LeaveScale()
    {
        if (gameObject != null)
        {
            transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f).SetEase(Ease.InBack);//.OnComplete(MyFunction);

        }

    }

    public void SpawnItem()
    {
        if (gameObject != null)
        {

            spawnSequence = DOTween.Sequence();
            // rectTransform.DOAnchorPosY(-50f, 1f, false);
            spawnSequence.Append(transform.DOScale(new Vector3(0.01f, 0.1f, 1f), 0f));

            //rectTransform.DOAnchorPosY(0f, 1f, false).SetEase(Ease.OutBounce);
            spawnSequence.Append((transform.DOScaleY(1f, 0.2f)).SetEase(Ease.OutBack));

            spawnSequence.Append(transform.DOScale(new Vector3(1f, 1f, 1f), Random.Range(0.1f, 0.3f)).SetEase(Ease.OutBack));
        }
    }

    public void DestroyItem()
    {

        if (gameObject != null)
        {
            destroySequence = DOTween.Sequence();
            // rectTransform.DOAnchorPosY(-50f, 1f, false);
            destroySequence.Append((transform.DOScaleX(0.1f, 0.3f)).SetEase(Ease.InBack));

        }

    }
}
