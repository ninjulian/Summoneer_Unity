using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(HealthBar))]
public class DamageHandler : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private StatClass entityStats;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem poisonParticles;

    [Header("UI")]
    private HealthBar healthBar;

    private List<ActiveDOT> activeDOTs = new List<ActiveDOT>();

    public struct ActiveDOT
    {
        public DOTType type;
        public float totalDamage;
        public float duration;
        public float timeRemaining;
        public float tickInterval;
        public float timeSinceLastTick;
        public ParticleSystem particles;
    }

    [HideInInspector] public enum DOTType { Fire, Poison }

    private void Awake()
    {
        healthBar = GetComponent<HealthBar>();
    }

    void FixedUpdate()
    {
        ProcessDOTs();
    }

    public void ReceiveDamage(float rawDamage)
    {
        float finalDamage = Mathf.Max(rawDamage - entityStats.defense, 1);

        entityStats.TakeDamage(finalDamage);

        UpdateHPUI(finalDamage);

        if (entityStats.currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void ApplyDOT(float totalDamage, float duration, float tickInterval, DOTType type, ParticleSystem particles)
    {
        // Remove existing DOT of same type
        activeDOTs.RemoveAll(d => d.type == type);

        ActiveDOT newDOT = new ActiveDOT
        {
            type = type,
            totalDamage = totalDamage,
            duration = duration,
            timeRemaining = duration,
            tickInterval = tickInterval,
            timeSinceLastTick = 0,
            particles = particles != null ? Instantiate(particles, transform) : null
        };

        if (newDOT.particles != null) newDOT.particles.Play();
        activeDOTs.Add(newDOT);
    }

    private void ProcessDOTs()
    {
        for (int i = activeDOTs.Count - 1; i >= 0; i--)
        {
            ActiveDOT dot = activeDOTs[i];
            dot.timeRemaining -= Time.deltaTime;
            dot.timeSinceLastTick += Time.deltaTime;

            // Apply damage tick by a tick loop
            if (dot.timeSinceLastTick >= dot.tickInterval)
            {
                float damagePerTick = dot.totalDamage;
                ReceiveDamage(damagePerTick);
        
                dot.timeSinceLastTick = 0;
            }

            // Remove expired DOT
            if (dot.timeRemaining <= 0)
            {
                if (dot.particles != null)
                {
                    dot.particles.Stop();
                    Destroy(dot.particles.gameObject);
                }
                activeDOTs.RemoveAt(i);
            }
            else
            {
                activeDOTs[i] = dot;
            }
        }
    }

    public void ApplyFireDOT(float baseDamage, float duration)
    {
        ApplyDOT(baseDamage * 0.10f, duration, 0.5f, DOTType.Fire, fireParticles);
    }

    public void ApplyPoisonDOT(float baseDamage, float duration)
    {
        ApplyDOT(baseDamage * 0.05f, duration, 1f, DOTType.Poison, poisonParticles);
    }



    private void HandleDeath()
    {
        // Add death logic (animation, drops, etc)
        Destroy(gameObject);
    }

    // For initialization in child classes
    public void Initialize(StatClass stats)
    {
        entityStats = stats;
    }

    public void UpdateHPUI(float value)
    {
        if (healthBar != null)
        {
            healthBar.StartHpUIUpdate(value);

        }
    }

    //Returns what DOTType is currently in action
    public DOTType? GetCurrentDOTType()
    {
        if (activeDOTs.Count > 0)
        {
            return activeDOTs[0].type;
        }
        return null;
    }

}
