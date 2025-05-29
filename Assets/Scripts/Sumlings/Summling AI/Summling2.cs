using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Summling2 : SummlingStats
{
    [SerializeField] private SummlingAIState currentState;
    private NavMeshAgent navAgent;
    private Vector3 roamPosition;

    public bool alwaysShowGizmos;

    [Header("Movement Settings")]
    [SerializeField] private float stuckTimeThreshold = 0.5f; // Time before considering stuck
    private float stuckTimer;
    private bool isMoving;

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform muzzleTransform;


    [Header("Animations")]
    private Animator animator;


    private void Awake()
    {   
        animator = GetComponentInChildren<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = movementSpeed;
        currentState = SummlingAIState.Roam;
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
        // Check if we need new destination or if stuck
        if (ShouldFindNewRoamPosition())
        {
            Vector2 randomPoint = Random.insideUnitCircle * roamRadius;
            roamPosition = player.transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);
            navAgent.SetDestination(roamPosition);
            stuckTimer = 0f; // Reset stuck timer when setting new destination
        }

        // Additional check for being stuck
        CheckIfStuck();
    }

    private void ChaseBehavior()
    {
        if (currentTarget != null)
        {
            navAgent.isStopped = false;
            navAgent.SetDestination(currentTarget.position);
        }
    }

    private void AttackBehavior()
    {
        if (currentTarget == null) return;
        transform.LookAt(currentTarget);

        if (attackCooldownTimer <= 0)
        {
            animator.SetBool("IsAttacking", true);

            GameObject bullet = Instantiate(bulletPrefab, muzzleTransform.position, Quaternion.identity);
            ProjectileController projectileController = bullet.GetComponent<ProjectileController>();
            Rigidbody rb = bullet.GetComponent<Rigidbody>(); // Add this

            // Set projectile properties
            projectileController.baseDamage = CalculateDamage();
            Vector3 shotDirection = (currentTarget.position - muzzleTransform.position).normalized;

            // Physics-based movement
            rb.velocity = shotDirection * projectileController.speed;

            attackCooldownTimer = damageCooldown;
        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }
    }

    private bool ShouldFindNewRoamPosition()
    {
        // Check if we've reached our destination or player moved too far
        return navAgent.remainingDistance <= navAgent.stoppingDistance ||
        Vector3.Distance(player.transform.position, roamPosition) > roamRadius;
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
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
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

    // Draw gizmos when object is selected or when "alwaysShowGizmos" is enabled
    private void OnDrawGizmos()
    {
        if (!alwaysShowGizmos) return;

        DrawGizmosContent();
    }

    // Draw gizmos only when selected (if "alwaysShowGizmos" is disabled)
    private void OnDrawGizmosSelected()
    {
        if (alwaysShowGizmos) return;

        DrawGizmosContent();
    }

    // Common gizmo drawing logic
    //private void DrawGizmosContent()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, detectionRange);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, attackRange);

    //    // Draw target destination wire cube
    //    Gizmos.color = Color.green;
    //    Vector3 targetPosition = currentTarget != null ? currentTarget.position : roamPosition;
    //    Gizmos.DrawWireCube(targetPosition, Vector3.one);
    //}


}
