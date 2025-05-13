using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEssencePickup : MonoBehaviour
{
    public float amplitude = 0.5f;     // How high the object moves
    public float frequency = 1f;       // Speed of the movement

    private Vector3 startPos;

    void Start()
    {
        // Force the start position to Y = 1
        startPos = new Vector3(transform.position.x, 1f, transform.position.z);
        transform.position = startPos; // Move object to Y = 1 at start
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate new Y position
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // Apply new position
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit player");
        }
    }
}
