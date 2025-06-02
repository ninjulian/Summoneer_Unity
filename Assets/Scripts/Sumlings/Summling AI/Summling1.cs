using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Summling1 : SummlingStats
{
    [SerializeField] private SummlingAIState currentState = SummlingAIState.Roam;
    private NavMeshAgent navAgent;
    private Vector3 roamPosition;

    public bool alwaysShowGizmos = false;

    [Header("Movement Settings")]
    [SerializeField] private float stuckTimeThreshold = 0.5f;
    private float stuckTimer;
    private bool isMoving;

    [Header("Chase Settings")]
    [SerializeField] private float maxSpeedIncreaseMultiplier = 2f; 
    [SerializeField] private float speedIncreaseAcceleration = 0.1f;
    private float currentSpeedMultiplier = 1f;

    [Header("Animations")]
    private Animator animator;



    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = movementSpeed;
        //currentState = SummlingAIState.Roam;

        // On ground agents (prevents them from avoiding flying agents)
        //navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        //navAgent.avoidanceMask = ~(1 << LayerMask.NameToLayer("FlyingAgent")); // Ignores flying layer
    }

    private void OnEnable()
    {
        PlayerShoot.OnEnemyFocused += SetFocusTarget;
    }

    private void OnDisable()
    {
        PlayerShoot.OnEnemyFocused -= SetFocusTarget;
    }


    private void Update()
    {

        if (player != null)
        {
            UpdateTimers();
            CheckStateTransitions();
            UpdateCurrentState();
        }
    }

    private void UpdateCurrentState()
    {
        switch (currentState)
        {
            case SummlingAIState.Roam:

                RoamBehavior();
                break;
            case SummlingAIState.Chase:
                ChaseBehavior();


                break;
            case SummlingAIState.Attack:
                AttackBehavior();
                break;
        }
    }

    private void RoamBehavior()
    {
        animator.SetBool("IsMoving", true);

        // Resets the increasing speed to normal
        currentSpeedMultiplier = 1f;
        navAgent.speed = movementSpeed * currentSpeedMultiplier;

        // Check if we need new destination or if stuck
        if (FindNewRoamPos())
        {
            Vector2 randomPoint = Random.insideUnitCircle * roamRadius;
            roamPosition = player.transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);
            navAgent.SetDestination(roamPosition);
            stuckTimer = 0f; 
        }

        // Additional check for being stuck
        CheckIfStuck();
    }

    private void ChaseBehavior()
    {
        ChaseSpeedIncrease();

        if (currentTarget != null)
        {
            animator.SetBool("IsMoving", true);
            navAgent.isStopped = false;
            navAgent.SetDestination(currentTarget.position);
        }
    }

    private void AttackBehavior()
    {
        //animator.SetBool("IsMoving", false);

        if (currentTarget == null) return;

        ChaseSpeedIncrease();

        // Face target
        //Vector3 direction = (currentTarget.position - transform.position).normalized;
        transform.LookAt(currentTarget);

        if (attackCooldownTimer <= 0)
        {
            animator.SetBool("IsAttacking", true);
            animator.SetTrigger("Attack");

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    DamageHandler enemyDamageHandler = hitCollider.GetComponent<DamageHandler>();
                    if (enemyDamageHandler != null)
                    {
                        enemyDamageHandler.ReceiveDamage(CalculateDamage());
                    }
                }
            }

            attackCooldownTimer = damageCooldown;
        }
        else
        {

            animator.SetBool("IsAttacking", false);
        }
    }

    private bool FindNewRoamPos()
    {
        // Check if we've reached our destination or player moved too far
        return navAgent.remainingDistance <= navAgent.stoppingDistance ||
               Vector3.Distance(player.transform.position, roamPosition) > leashRadius;
    }

    private void CheckIfStuck()
    {
        // Check if we're supposed to be moving but aren't
        if (navAgent.velocity.magnitude < 0.1f && stuckTimer >= stuckTimeThreshold)
        {
            // Force new destination
            Vector2 randomPoint = Random.insideUnitCircle * roamRadius;
            roamPosition = player.transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);
            navAgent.SetDestination(roamPosition);
            stuckTimer = 0f;
        }
    }

    private void CheckStateTransitions()
    {
        if (focusTarget != null && focusTimer > 0)
        {
            currentTarget = focusTarget;
        }
        else
        {
            currentTarget = FindNearestEnemy();
        }

        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);

            if (distance <= attackRange)
            {
                currentState = SummlingAIState.Attack;
                navAgent.isStopped = true;
            }
            else
            {
                currentState = SummlingAIState.Chase;
                navAgent.isStopped = false;
            }
        }
        else
        {
            currentState = SummlingAIState.Roam;
            navAgent.isStopped = false;
        }
    }

    private Transform FindNearestEnemy()
    {
        // Combine both layers into a single bitmask
        int groundEnemyLayer = LayerMask.NameToLayer("GroundEnemy");
        int flyingEnemyLayer = LayerMask.NameToLayer("FlyingEnemy");
        int combinedLayerMask = (1 << groundEnemyLayer) | (1 << flyingEnemyLayer);

        // Find all colliders in the two layers
        Collider[] enemies = Physics.OverlapSphere(
            transform.position,
            detectionRange,
            combinedLayerMask
        );

        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy.transform;
            }
        }
        return nearest;
    }

    private void UpdateTimers()
    {
        if (focusTimer > 0) focusTimer -= Time.deltaTime;
        if (attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;

        // Update stuck timer
        if (currentState == SummlingAIState.Roam)
        {
            stuckTimer += Time.deltaTime;
        }
    }

    private void SetFocusTarget(Transform newTarget)
    {
        focusTarget = newTarget;
        focusTimer = focusTime;
    }

    // Draw gizmos all the time
    private void OnDrawGizmos()
    {
        if (!alwaysShowGizmos) return;
         
        DrawGizmosContent();
    }

    // Draw gizmos only when selected
    private void OnDrawGizmosSelected()
    {
        if (alwaysShowGizmos) return;

        DrawGizmosContent();
    }

    // Increases the speed linearly, that way Summlings tick onto enemies and not outpaced as easily
    private void ChaseSpeedIncrease()
    {
        currentSpeedMultiplier = Mathf.Min(currentSpeedMultiplier + speedIncreaseAcceleration * Time.deltaTime, maxSpeedIncreaseMultiplier);

        navAgent.speed = movementSpeed * currentSpeedMultiplier;
    }

}