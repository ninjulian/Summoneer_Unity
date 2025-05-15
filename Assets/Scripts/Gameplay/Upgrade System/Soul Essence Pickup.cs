// SoulEssencePickup.cs
using UnityEngine;

public class SoulEssencePickup : MonoBehaviour
{
    public float soulEssenceValue = 1;
    public float xpValue = 10;

    private ItemMovement movement;
    private Collider pickupCollider;

    private void Awake()
    {
        movement = GetComponent<ItemMovement>();
        pickupCollider = GetComponent<Collider>();
        pickupCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect(other.GetComponent<PlayerStats>());
        }
    }

    private void Collect(PlayerStats playerStats)
    {
        if (playerStats != null)
        {
            playerStats.GainSoulEssence(soulEssenceValue);
            playerStats.GainXP(xpValue);

            Destroy(gameObject);
        }
        
    }

    //// Called by PlayerPickupRange
    //public void EnablePickup(Transform player)
    //{
    //    movement.StartFollowing(player);
    //}

    //// Called by PlayerPickupRange
    //public void DisablePickup()
    //{
    //    movement.StopFollowing();
    //}
}