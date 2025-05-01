using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletDecal;

    public float speed = 50f;
    public float lifeSpan = 3f;

    public Vector3 target { get; set; }
    public bool hit { get; set; }

    [HideInInspector] public float baseDamage;
    public string sourceTag; // Who shot this projectile


    [Header("Damage Over Time")]
    public bool applyFireDOT;
    public bool applyPoisonDOT;
    public float DOTDuration = 3f;

    private void OnEnable()
    {
        Destroy(gameObject, lifeSpan);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // If no target and getting close to target it will destroy itself
        if (!hit && Vector3.Distance(transform.position, target) > .01f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //ContactPoint contact = other.GetContact(0);
        //GameObject.Instantiate(bulletDecal, contact.point + contact.normal * .001f, Quaternion.LookRotation(contact.normal));

        if (other.CompareTag(sourceTag)) return; 

        StatClass targetStats = other.GetComponent<StatClass>();
        DamageHandler damageHandler = other.GetComponent<DamageHandler>();
        if (targetStats != null)
        {
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



    }
}
