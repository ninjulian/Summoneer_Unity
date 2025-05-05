using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{


    public float speed = 50f;
    public float lifeSpan = 3f;

    public Vector3 target { get; set; }
    public bool hit { get; set; }

    [HideInInspector] public float baseDamage;
    public string sourceTag; 


    [Header("Damage Over Time")]
    public bool applyFireDOT;
    public bool applyPoisonDOT;
    public float DOTDuration = 3f;

    private void OnEnable()
    {
        //Destroy(gameObject, lifeSpan);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // If no target and getting close to target it will destroy itself
        //if (!hit && Vector3.Distance(transform.position, target) > .01f)
        //{
        //    Destroy(gameObject);
        //}


        if (!hit && Vector3.Distance(transform.position, target) <= 0.01f)
        {
            Debug.Log("Tryna destroy bullety");
            Destroy(gameObject);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        //ContactPoint contact = other.GetContact(0);
        //GameObject.Instantiate(bulletDecal, contact.point + contact.normal * .001f, Quaternion.LookRotation(contact.normal));
        Destroy(gameObject);
        if (other.CompareTag(sourceTag)) 
        {   
            Debug.Log("HIT MESELFG" + sourceTag);
            return;
        } 

        StatClass targetStats = other.GetComponent<StatClass>();
        DamageHandler damageHandler = other.GetComponent<DamageHandler>();
        if (targetStats != null)
        {

            Debug.Log(sourceTag + " Shot projectile");
            damageHandler.ReceiveDamage(baseDamage);
            Destroy(gameObject);

            // Apply DOT effects
            if (applyFireDOT)
            {
                damageHandler.ApplyFireDOT(baseDamage, DOTDuration);
            }

            if (applyPoisonDOT)
            {
                damageHandler.ApplyPoisonDOT(baseDamage, DOTDuration);
            }

        }

        int layerToIgnore1 = 6;
        int ignoreMask = ~(1 << layerToIgnore1);

        //if ((ignoreMask & (1 << other.gameObject.layer)) == 0)
        //{
        //    StartCoroutine(DelayedDestroy(0.1f));
        //    return; // Exit early, don't apply damage
        //}

    }

    private IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
