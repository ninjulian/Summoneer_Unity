using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{

    // public float pickUpRadius;

    // public PlayerStats playerStats;

    public SphereCollider pickUpCollider;
    public PlayerStats playerStats;


    private void Awake()
    {
        UpdatePlayerRadius();
    }

    // SE moves towards player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soul Essence"))
        {
            //SoulEssencePickup soulEssencePickup = other.GetComponent<SoulEssencePickup>();
            ItemMovement itemMovement = other.GetComponent<ItemMovement>();
            itemMovement.MoveToTarget(transform);
            itemMovement.inRange = true;
            //Debug.Log("Found Target");

        }
    }

    // SE floats back into position
    private void OnTriggerExit(Collider other)
    {   
        // Make sure item is Soul Essence before picking up
        if (other.CompareTag("Soul Essence"))
        {
            //SoulEssencePickup soulEssencePickup = other.GetComponent<SoulEssencePickup>();
            ItemMovement itemMovement = other.GetComponent<ItemMovement>();
            itemMovement.UpdateFloatAnchor();
           // itemMovement.MoveToTarget(transform);
            itemMovement.inRange = false;
            //Debug.Log("Found Target");

        }
    }


    // Pick up radius of where SE will move towards player
    public void UpdatePlayerRadius()
    {
        pickUpCollider.radius = playerStats.pickUpRadius;
    }
    

    //private void OnTriggerEnter(Collider other)
    ////{
    ////    if (other.CompareTag("Soul Essence"))
    ////    {
    ////        SoulEssencePickup soulEssencePickup = other.GetComponent<SoulEssencePickup>();

    ////        playerStats.GainSoulEssence(soulEssencePickup.soulEssenceValue);
    ////        Debug.Log("Picking up SE");
    ////        Destroy(other.gameObject);  
    ////       //DestroyPickUp(other.gameObject);

    ////    }
    //}

    //public IEnumerator DestroyPickUp(GameObject item)
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    Destroy(item);
    //}
}


//private void FloatInPlace()
//{
//    // Vertical floating motion relative to original height
//    float newY = originalBasePosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

//    // Maintain current x/z position but float vertically
//    transform.position = new Vector3(currentBasePosition.x, newY, currentBasePosition.z);

//    // Gentle rotation
//    transform.rotation = startRotation * Quaternion.Euler(0, Time.time * rotationSpeed, 0);
//}