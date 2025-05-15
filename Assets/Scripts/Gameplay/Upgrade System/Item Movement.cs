using UnityEngine;

public class ItemMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Floating Settings")]
    [SerializeField] private float floatAmplitude = 0.2f;
    [SerializeField] private float floatFrequency = 1f;

    private Transform target;
    public Vector3 floatAnchor;
    public bool inRange = false;
    private float randomOffset;
    private float originalY;

    private void Awake()
    {
        floatAnchor = transform.position;
        originalY = floatAnchor.y;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);
    }


    // Sets up the target transform
    public void MoveToTarget(Transform newTarget)
    {
        target = newTarget;
    }


    private void Update()
    {   
        // If target not in range it will float 
        if (!inRange)
        {
            FloatInPlace();
            return;
        }

        if (target == null) return;

        Vector3 targetPosition = target.position;
        Vector3 currentPosition = transform.position;

        if (CheckLineOfSight(currentPosition, targetPosition))
        {
            transform.position = Vector3.MoveTowards(
                currentPosition,
                targetPosition,
                movementSpeed * Time.deltaTime
            );
        }
    }


    // Floating Idle animation
    private void FloatInPlace()
    {
        float yOffset = Mathf.Sin((Time.time * floatFrequency) + randomOffset) * floatAmplitude;
        transform.position = new Vector3(
            floatAnchor.x,
            originalY + yOffset,
            floatAnchor.z
        );
    }

    private bool CheckLineOfSight(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        float distance = direction.magnitude;

        if (distance < 0.1f) return true;

        return !Physics.Raycast(from, direction.normalized, distance, obstacleLayer);
    }

    public void UpdateFloatAnchor()
    {
        floatAnchor = transform.position;
        originalY = floatAnchor.y;
    }

    //private void OnDisable()
    //{
    //    target = null;
    //    inRange = false;
    //}
}