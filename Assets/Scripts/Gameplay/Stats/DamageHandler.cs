using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;


[RequireComponent(typeof(HealthBar))]
public class DamageHandler : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private StatClass entityStats;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem poisonParticles;
    [SerializeField] private GameObject deathParticles;

    [SerializeField] private GameObject healthBarUI;

    [SerializeField] private Renderer meshRenderer;
    private Color originalColour;

    public Color dmgTakenFlash = Color.red;

    // [SerializeField] private Transform DOTLocation;

    [Header("UI")]
    private HealthBar healthBar;

    private List<ActiveDOT> activeDOTs = new List<ActiveDOT>();

    // Damage over time variable
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
    {   //Gets rid of particles and stops them before needing to show them with DOT Triggers
        poisonParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        healthBar = GetComponent<HealthBar>();

        originalColour = meshRenderer.material.color;
    }

    void FixedUpdate()
    {
        ProcessDOTs();
    }

    private void OnDestroy()
    {   
        if (deathParticles != null)
        {
            //ParticleSystem ps = deathParticles.GetComponent<ParticleSystem>();
            //var sh = ps.shape;
            //sh.enabled = true;
            //sh.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
            //sh.skinnedMeshRenderer = (SkinnedMeshRenderer)meshRenderer; 
            Instantiate(deathParticles, gameObject.transform);
            
        }
        
    }


    public void ReceiveDamage(float rawDamage, DOTType? dotType = null)
    {
        // Unfinished damage calculation
        float finalDamage = Mathf.Max(rawDamage - entityStats.defense, 1);
        entityStats.TakeDamage(finalDamage, dotType);

        if (meshRenderer != null)
        {
            StopCoroutine(DamageFlashRoutine());
            StartCoroutine(DamageFlashRoutine());
        }
        else Debug.Log("Mesh Renderer is null");


        if (!healthBarUI.activeInHierarchy)
        {
            healthBarUI.SetActive(true);
        }

        if (entityStats.currentHealth <= 0)
        {
            HandleDeath();
        }

        // Updates health bar component of owner
        UpdateHPUI(finalDamage);
    }

    private void ApplyDOT(float totalDamage, float duration, float tickInterval, DOTType type, ParticleSystem particles)
    {

        foreach (var existingDOT in activeDOTs.FindAll(d => d.type == type))
        {

            if (particles != null)
            {
                // Stops and clears the particle system to reset it
                particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        // Cleans up pre existing DOTS of the same type
        activeDOTs.RemoveAll(d => d.type == type);

        // Creates new DOT 
        //ActiveDOT newDOT = new ActiveDOT
        ActiveDOT newDOT = new()
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

        // Adds new DOT 
        activeDOTs.Add(newDOT);
    }

    private void ProcessDOTs()
    {   // Backwards loop to prevent any skipping
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

    // Fire Damage over time effect
    public void ApplyFireDOT(float baseDamage, float duration)
    {
        ApplyDOT(baseDamage * 0.10f, duration, 0.5f, DOTType.Fire, fireParticles);
    }

    // Poison Damage over time effect
    public void ApplyPoisonDOT(float baseDamage, float duration)
    {
        ApplyDOT(baseDamage * 0.05f, duration, 1f, DOTType.Poison, poisonParticles);
    }


    private IEnumerator DamageFlashRoutine()
    {
        // Change to white
        meshRenderer.material.color = Color.red;

        // Wait for 0.1 seconds
        yield return new WaitForSeconds(0.1f);

        // Return to original color
        meshRenderer.material.color = originalColour;

    }

    // Death logic
    private void HandleDeath()
    {

        Destroy(gameObject);
    }

    // For initialization in child classes
    public void Initialize(StatClass stats)
    {
        entityStats = stats;
    }

    // Updates health bar UI
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
