using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EnableAnimationUI : MonoBehaviour
{
     public enum AvailableFunctions
    {
        None,
        PlayAnimation,
        EnableObject,
        DisableObject,
        LogMessage,
        CustomFunction1,
        CustomFunction2
    }

    [SerializeField] private AvailableFunctions functionToRun = AvailableFunctions.None;
    [SerializeField] private string logMessage = "Enabled!";
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Animator animator;
    [SerializeField] private string animationName;

    private void OnEnable()
    {
        switch (functionToRun)
        {
            case AvailableFunctions.PlayAnimation:
                if (animator != null) 
                    animator.Play(animationName);
                break;
                
            case AvailableFunctions.EnableObject:
                if (targetObject != null) 
                    targetObject.SetActive(true);
                break;
                
            case AvailableFunctions.DisableObject:
                if (targetObject != null) 
                    targetObject.SetActive(false);
                break;
                
            case AvailableFunctions.LogMessage:
                Debug.Log(logMessage);
                break;
                
            case AvailableFunctions.CustomFunction1:
                CustomFunction1();
                break;
                
            case AvailableFunctions.CustomFunction2:
                CustomFunction2();
                break;
        }
    }

    private void CustomFunction1()
    {
        // Your custom implementation
    }

    private void CustomFunction2()
    {
        // Your custom implementation
    }
}
