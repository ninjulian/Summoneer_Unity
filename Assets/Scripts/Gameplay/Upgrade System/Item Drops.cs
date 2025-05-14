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
      
            Debug.Log("Found the thing I think");
        
        
        soulEssence = GetComponent<SoulEssencePickup>();


    }

    private void OnDestroy()
    {
        soulEssence.xpValue = xpValue;
        soulEssence.soulEssenceValue = soulEssenceValue;
        Instantiate(soulEssencePrefab, transform);
    }

}
