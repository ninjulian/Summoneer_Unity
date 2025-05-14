using Unity.VisualScripting;
using UnityEngine;

public class SoulEssencePickup : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public LayerMask groundLayer;
    public float floatLerpDuration = 0.5f;

    private Rigidbody rb;
    private Transform cachedTransform;
    private Vector3 startPos;
    private float timeOffset;
    private bool isFloating;

    [HideInInspector] public float soulEssenceValue;
    [HideInInspector] public float xpValue;

    private SphereCollider sphereCollider;

    private void Awake()
    {
        cachedTransform = transform;
        rb = GetComponent<Rigidbody>();
        timeOffset = Random.Range(0f, 2f * Mathf.PI); // Vary start time for instancing efficiency

        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = false;

    }

    private void Start()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    private void Update()
    {
        if (isFloating)
        {
            // Calculate position once per frame using precomputed values
            float newY = startPos.y + Mathf.Sin((Time.time * frequency) + timeOffset) * amplitude;
            cachedTransform.position = new Vector3(startPos.x, newY, startPos.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isFloating && (groundLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            sphereCollider.isTrigger = true;
            StartCoroutine(StartFloatingRoutine());
        }
    }

    private System.Collections.IEnumerator StartFloatingRoutine()
    {
        isFloating = true;
        rb.isKinematic = true;
        rb.useGravity = false;

        Vector3 floatStartPos = cachedTransform.position;
        startPos = floatStartPos + Vector3.up * 0.5f;
        float elapsed = 0f;

        while (elapsed < floatLerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / floatLerpDuration);
            cachedTransform.position = Vector3.Lerp(floatStartPos, startPos, t);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.GainSoulEssence(soulEssenceValue);
                playerStats.GainXP(xpValue);
            }
            

            // Consider object pooling instead of Destroy for better performance
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}