using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrops : MonoBehaviour
{
    [SerializeField] private GameObject soulEssencePrefab;
    private SoulEssencePickup soulEssence;

    public StatClass statClass;

    public float xpValue;
    public float soulEssenceValue;

    private void Awake()
    {

    }

    public void DropItems()
    {

        GameObject soulEssencePickup = Instantiate(soulEssencePrefab, transform.position, transform.rotation);
        soulEssence = soulEssencePickup.GetComponent<SoulEssencePickup>();
        soulEssence.xpValue = xpValue;
        soulEssence.soulEssenceValue = soulEssenceValue;
    }

}
