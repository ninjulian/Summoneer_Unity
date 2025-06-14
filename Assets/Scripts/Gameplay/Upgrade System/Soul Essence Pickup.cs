// SoulEssencePickup.cs
using System.Collections;
using UnityEngine;

public class SoulEssencePickup : MonoBehaviour
{
    public float soulEssenceValue = 1;
    public float xpValue = 10;

    private ItemMovement movement;
    private Collider pickupCollider;

    [SerializeField] private GameObject spawnEffect;

    private MeshRenderer meshRenderer;

    public float duration = 1f;

    private void Start()
    {
        
        //SpawnEffect();
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, new Vector3(0.3f, 0.3f, 0.3f), 1f).setEase(LeanTweenType.easeOutBack); // Smooth easing effect

        //StartCoroutine(ScaleOverTime());
    }

    private void Awake()
    {   
        meshRenderer = GetComponent<MeshRenderer>();
       
        movement = GetComponent<ItemMovement>();
        pickupCollider = GetComponent<Collider>();
        pickupCollider.isTrigger = true;
    }
    

    // Checks if it is Player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect(other.GetComponent<PlayerStats>());
        }
    }

    // Add XP and SE gain for player
    private void Collect(PlayerStats playerStats)
    {
        if (playerStats != null)
        {
            playerStats.GainSoulEssence(soulEssenceValue);
            playerStats.GainXP(xpValue);

            Destroy(gameObject);    
        }

    }

    private void SpawnEffect()
    {
        var se = Instantiate(spawnEffect, gameObject.transform);

    }

    IEnumerator ScaleOverTime()
    {
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = new Vector3(0.3f, 0.3f, 0.3f);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure final scale is exact
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
