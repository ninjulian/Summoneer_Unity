using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public bool focusBullet;

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
        Destroy(gameObject, lifeSpan);
    }

    //void Update()
    //{
    //    transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

    //    // If no target and getting close to target it will destroy itself
    //    //if (!hit && Vector3.Distance(transform.position, target) > .01f)
    //    //{
    //    //    Destroy(gameObject);
    //    //}


    //    if (!hit && Vector3.Distance(transform.position, target) <= 0.01f)
    //    {
    //        Debug.Log("Tryna destroy bullety");
    //        Destroy(gameObject);
    //    }


    //}

    private void OnCollisionEnter(Collision collision)
    {
        // Get the collider from the collision
        Collider other = collision.collider;

        if (other.CompareTag(sourceTag))
        {
            return;
        }

        StatClass targetStats = other.GetComponent<StatClass>();
        DamageHandler damageHandler = other.GetComponent<DamageHandler>();

        if (targetStats != null)
        {
            damageHandler.ReceiveDamage(baseDamage);

            Debug.Log(sourceTag + "has hit");

            // Apply DOT effects
            if (applyFireDOT)
            {
                damageHandler.ApplyFireDOT(baseDamage, DOTDuration);
            }
            if (applyPoisonDOT)
            {
                damageHandler.ApplyPoisonDOT(baseDamage, DOTDuration);
            }

            Destroy(gameObject);
        }
        else
        {
            // Destroy projectile when hitting non-damageable objects
            Destroy(gameObject);
        }
    }


}
