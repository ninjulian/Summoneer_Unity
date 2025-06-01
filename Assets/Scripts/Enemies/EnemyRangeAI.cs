using UnityEngine.AI;
using UnityEngine;
using System.Collections;

//
public class EnemyRangeAI : MonoBehaviour
{
    [SerializeField] private AIState _currentState;
    private float attackRange = 2f;

    private Transform player;
    private NavMeshAgent navAgent;

    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;

    private DamageHandler damageHandler;
    private EnemyStats enemyStats;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzleTransform;

    private bool hasLOS;
    private Animator animator;
    private bool initializeSpawn = false;
    private bool isAttackCoroutineRunning = false; // Track attack state

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        damageHandler = GetComponent<DamageHandler>();
        enemyStats = GetComponent<EnemyStats>();
        animator.SetTrigger("Spawn");

        StartCoroutine(InitializeSpawn());
    }

    void Update()
    {
        if (player != null && initializeSpawn)
        {
            CheckLOS();
            CheckStateTransition();
            UpdateCurrentState();
        }
    }

    void CheckStateTransition()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && hasLOS)
        {
            SetState(AIState.Attack);
        }
        else
        {
            SetState(AIState.Chase);
        }
    }

    void SetState(AIState newState)
    {
        // Handle state exit logic
        if (_currentState == AIState.Attack && newState != AIState.Attack)
        {
            navAgent.isStopped = false;
        }

        _currentState = newState;

        // Handle state enter logic
        if (_currentState == AIState.Attack)
        {
            navAgent.isStopped = true; // Stop movement when attacking
        }
    }

    void UpdateCurrentState()
    {
        switch (_currentState)
        {
            case AIState.Chase:
                ChasePlayer();
                break;
            case AIState.Attack:
                if (!isAttackCoroutineRunning)
                {
                    StartCoroutine(AttackPlayer());
                }
                break;
        }
    }

    void ChasePlayer()
    {
        navAgent.SetDestination(player.position);
        //animator.SetBool("IsChasing", true);
    }

    private void CheckLOS()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        int layerToIgnore1 = 7;
        int layerToIgnore2 = 9;
        int layerToIgnore3 = 13;
        int ignoreMask = ~(1 << layerToIgnore1 | 1 << layerToIgnore2 | 1 << layerToIgnore3);

        RaycastHit hit;
        hasLOS = Physics.Raycast(transform.position, direction, out hit, attackRange, ignoreMask);
        if (hasLOS)
        {
            hasLOS = hit.collider.CompareTag("Player");
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttackCoroutineRunning = true;
        //animator.SetBool("IsChasing", false);

        // Face player
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation; // Immediate rotation

        // Attack if cooldown finished
        if (Time.time > lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            FireProjectile();
            lastAttackTime = Time.time;
        }

        yield return new WaitForSeconds(0.1f); // Brief delay before next check
        isAttackCoroutineRunning = false;
    }

    private void FireProjectile()
    {
        GameObject bullet = Instantiate(bulletPrefab, muzzleTransform.position, Quaternion.identity);
        ProjectileController projectileController = bullet.GetComponent<ProjectileController>();
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        projectileController.baseDamage = enemyStats.damage;
        Vector3 shotDirection = (player.position - muzzleTransform.position).normalized;
        rb.velocity = shotDirection * projectileController.speed;
    }

    private IEnumerator InitializeSpawn()
    {
        yield return new WaitForSeconds(1f);
        initializeSpawn = true;
        navAgent.speed = enemyStats.movementSpeed;
        attackRange = enemyStats.attackRange; // Ensure range is set from stats
        SetState(AIState.Chase);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player Projectile"))
        {

            Debug.Log("The player has hit me wah wah wah ");


            ProjectileController projectileController = other.gameObject.GetComponent<ProjectileController>();

            damageHandler.ReceiveDamage(projectileController.baseDamage);

            // Apply DOT effects
            if (projectileController.applyFireDOT)
            {
                damageHandler.ApplyFireDOT(projectileController.baseDamage, projectileController.DOTDuration);
            }
            if (projectileController.applyPoisonDOT)
            {
                damageHandler.ApplyPoisonDOT(projectileController.baseDamage, projectileController.DOTDuration);
            }

        }
    }
}