using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonAnimation : MonoBehaviour
{
    [Header("References")]
    public RectTransform upgradeCardRectTransform; // Public reference to the card's RectTransform
    public Sequence spawnSequence;
    public Sequence destroySequence;

    [Header("3D Hover Settings")]
    [SerializeField] private float rotationDuration = 0.12f;
    [SerializeField] private float returnDuration = 0.18f;
    [SerializeField] private float maxRotationAngle = 12f;
    [SerializeField] private Ease rotationEase = Ease.OutQuad;

    private Vector3 originalLocalEuler;
    private bool isHovering = false;
    private Vector3 currentTargetEuler = Vector3.zero;

    private void Start()
    {
        if (upgradeCardRectTransform != null)
        {
            originalLocalEuler = upgradeCardRectTransform.localEulerAngles;
        }
        else
        {
            Debug.LogWarning("UpgradeCardRectTransform not assigned in " + gameObject.name);
            originalLocalEuler = transform.localEulerAngles;
        }
    }

    private void Update()
    {
        if (isHovering && upgradeCardRectTransform != null)
        {
            currentTargetEuler = ComputeTargetEulerForMouse(Input.mousePosition);
            float t = rotationDuration > 0f ? (Time.deltaTime / rotationDuration) : 1f;
            upgradeCardRectTransform.localRotation = Quaternion.Slerp(upgradeCardRectTransform.localRotation, 
                                                                     Quaternion.Euler(currentTargetEuler), t);
        }
    }

    public void BuyItem()
    {
        if (gameObject != null)
        {
            transform.DOScaleX(0f, 0.5f).SetEase(Ease.OutBounce);
        }
    }

    public void HoverScale()
    {
        if (gameObject != null)
        {   
            isHovering = true;
            transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).SetEase(Ease.OutBack);
        }
    }

    public void LeaveScale()
    {
        if (gameObject != null)
        {   
            isHovering = false; 
            transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f).SetEase(Ease.InBack);
            Hover3DLeave();
        }
    }

    public void SpawnItem()
    {
        if (gameObject != null)
        {
            spawnSequence = DOTween.Sequence();
            spawnSequence.Append(transform.DOScale(new Vector3(0.01f, 0.1f, 1f), 0f));
            spawnSequence.Append((transform.DOScaleY(1f, 0.2f)).SetEase(Ease.OutBack));
            spawnSequence.Append(transform.DOScale(new Vector3(1f, 1f, 1f), Random.Range(0.1f, 0.3f)).SetEase(Ease.OutBack));
        }
    }

    public void DestroyItem()
    {
        if (gameObject != null)
        {
            destroySequence = DOTween.Sequence();
            destroySequence.Append((transform.DOScaleX(0.1f, 0.3f)).SetEase(Ease.InBack));
        }
    }

    // Called when pointer enters / starts hovering.
    // Enables continuous updating in Update().
    public void Hover3DEnter()
    {
        if (upgradeCardRectTransform == null) return;

        // Stop any DOTween rotation tweens so they don't fight the Update-driven interpolation.
        upgradeCardRectTransform.DOKill(false);

        // Set hover state to true so Update() begins recalculating and applying rotation
        isHovering = true;

        // Compute initial target immediately (so it doesn't wait one frame)
        currentTargetEuler = ComputeTargetEulerForMouse(Input.mousePosition);

        // Optionally snap slightly toward target immediately using a short DOTween to feel responsive:
        upgradeCardRectTransform.DOLocalRotate(currentTargetEuler, Mathf.Min(rotationDuration, 0.06f)).SetEase(rotationEase);
    }

    // Called when pointer exits / stops hovering.
    // Disables continuous updating and returns card to flat rotation.
    public void Hover3DLeave()
    {
        if (upgradeCardRectTransform == null) return;

        // stop continuous follow
        isHovering = false;

        // Smoothly return to the original local rotation using DOTween
        upgradeCardRectTransform.DOLocalRotate(originalLocalEuler, returnDuration).SetEase(rotationEase);
    }

    // Compute the target euler angles (-rotX, rotY, 0) based on screen mouse position
    private Vector3 ComputeTargetEulerForMouse(Vector2 mouseScreenPos)
    {
        if (upgradeCardRectTransform == null) return Vector3.zero;

        // Determine correct camera for conversion depending on canvas render mode
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        Camera eventCamera = null;
        if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            eventCamera = parentCanvas.worldCamera;
        if (parentCanvas == null && Camera.main != null)
            eventCamera = Camera.main;

        // Convert mouse position to local point in rectTransform space
        Vector2 localPoint;
        bool converted = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            upgradeCardRectTransform, mouseScreenPos, eventCamera, out localPoint);

        // If conversion failed, try parent rect as fallback
        if (!converted && upgradeCardRectTransform.parent is RectTransform parentRect)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, mouseScreenPos, eventCamera, out localPoint);
        }

        // Use half extents relative to the pivot for normalization
        float halfWidth = upgradeCardRectTransform.rect.width * 0.5f;
        float halfHeight = upgradeCardRectTransform.rect.height * 0.5f;

        if (halfWidth <= 0f) halfWidth = 1f;
        if (halfHeight <= 0f) halfHeight = 1f;

        // Normalize to [-1, 1]
        Vector2 normalized = new Vector2(
            Mathf.Clamp(localPoint.x / halfWidth, -1f, 1f),
            Mathf.Clamp(localPoint.y / halfHeight, -1f, 1f)
        );

        // Map normalized positions to rotation angles.
        float rotX = Mathf.Clamp(-normalized.y * maxRotationAngle, -maxRotationAngle, maxRotationAngle); // up -> tilt toward viewer (negative X)
        float rotY = Mathf.Clamp(normalized.x * maxRotationAngle, -maxRotationAngle, maxRotationAngle);  // right -> positive Y tilt

        return new Vector3(rotX, rotY, 0f);
    }
}
