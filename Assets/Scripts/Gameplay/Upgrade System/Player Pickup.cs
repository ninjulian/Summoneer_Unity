using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{

    public float pickUpRadius;

    public PlayerStats playerStats;

    public SphereCollider pickUpCollider;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soul Essence"))
        {
            SoulEssencePickup soulEssencePickup = other.GetComponent<SoulEssencePickup>();

            playerStats.GainSoulEssence(soulEssencePickup.soulEssenceValue);
            Debug.Log("Picking up SE");
            Destroy(other.gameObject);  
           //DestroyPickUp(other.gameObject);

        }
    }

    public IEnumerator DestroyPickUp(GameObject item)
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(item);
    }
}
