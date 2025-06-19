using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonButtonAnimation : MonoBehaviour
{

    private Sequence replaceOpen;
    private Sequence replaceClose;

    private void Start()
    {

    }

    private void OnEnable()
    {
        transform.localScale = new Vector3(1,1,1);  
    }

    public void HoverScale()
    {

        transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).SetEase(Ease.OutBack);//.OnComplete(MyFunction);
    }

    public void LeaveScale()
    {
        transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f).SetEase(Ease.InBack);//.OnComplete(MyFunction);
    }

    public void replaceOpenUI()
    {
        replaceOpen = DOTween.Sequence();
        replaceOpen.Append(transform.DOScale(new Vector3(0.01f, 0.1f, 1f), 0f));

        //rectTransform.DOAnchorPosY(0f, 1f, false).SetEase(Ease.OutBounce);
        replaceOpen.Append((transform.DOScaleY(1f, 0.2f)).SetEase(Ease.OutBack));

        replaceOpen.Append((transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f)).SetEase(Ease.OutBack));
    }

    public void replaceCloseUI()
    {
        transform.DOScaleX(0.1f, 0.3f).SetEase(Ease.InBack);
    }

    public void SummonOpenUI()
    {
        replaceOpen = DOTween.Sequence();
        replaceOpen.Append(transform.DOScale(new Vector3(0.01f, 0.1f, 1f), 0f));

        //rectTransform.DOAnchorPosY(0f, 1f, false).SetEase(Ease.OutBounce);
        replaceOpen.Append((transform.DOScaleY(1f, 0.2f)).SetEase(Ease.OutBack));

        replaceOpen.Append((transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f)).SetEase(Ease.OutBack));
    }

    public void SummonCloseUI()
    {
        transform.DOScaleX(0.1f, 0.3f).SetEase(Ease.InBack);
    }


}
