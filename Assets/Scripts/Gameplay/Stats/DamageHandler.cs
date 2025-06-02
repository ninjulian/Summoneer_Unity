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

    [SerializeField] private GameObject healthBarUI;
    
   // [SerializeField] private Transform DOTLocation;

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
        poisonParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // Stop and clear particles
        fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // Stop and clear particles
        healthBar = GetComponent<HealthBar>();
    }

    void FixedUpdate()
    {
        ProcessDOTs();
    }

    public void ReceiveDamage(float rawDamage, DOTType? dotType = null) // Modified line
    {
        float finalDamage = Mathf.Max(rawDamage - entityStats.defense, 1);
        entityStats.TakeDamage(finalDamage, dotType); // Modified line

        if (!healthBarUI.activeInHierarchy)
        {
            healthBarUI.SetActive(true);
        }

        if (entityStats.currentHealth <= 0)
        {
            HandleDeath();
        }

        UpdateHPUI(finalDamage);
    }

    private void ApplyDOT(float totalDamage, float duration, float tickInterval, DOTType type, ParticleSystem particles)
    {
        // Cleans up existing DOTs of the same type
        foreach (var existingDOT in activeDOTs.FindAll(d => d.type == type))
        {
            // Stops and clears the particle system to reset it
            if (particles != null)
            {
                particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // Stop and clear particles
            }
        }
        activeDOTs.RemoveAll(d => d.type == type);

        // Creates new DOT 
        ActiveDOT newDOT = new ActiveDOT
        {
            type = type,
            totalDamage = totalDamage,
            duration = duration,
            timeRemaining = duration,
            tickInterval = tickInterval,
            timeSinceLastTick = 0,


            particles = particles 
        };

        if (newDOT.particles != null)
        {
            // Configure particles to match DOT duration
            var main = newDOT.particles.main;

            // Emission rate matches the DOT
            main.duration = duration; 

            // No loop
            main.loop = false;

            // Start emitting
            newDOT.particles.Play(); 
        }

        activeDOTs.Add(newDOT);
    }

    private void ProcessDOTs()
    {
        for (int i = activeDOTs.Count - 1; i >= 0; i--)
        {
            ActiveDOT dot = activeDOTs[i];
            dot.timeRemaining -= Time.deltaTime;
            dot.timeSinceLastTick += Time.deltaTime;

            // Apply damage tick
            if (dot.timeSinceLastTick >= dot.tickInterval)
            {
                ReceiveDamage(dot.totalDamage, dot.type);
                dot.timeSinceLastTick = 0;
            }

            // Remove expired DOT
            if (dot.timeRemaining <= 0)
            {
                if (dot.particles != null)
                {
                    // Stop emitting new particles
                    dot.particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
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
