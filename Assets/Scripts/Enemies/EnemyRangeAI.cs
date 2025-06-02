using UnityEngine.AI;
using UnityEngine;
using System.Collections;

//
public class EnemyRangeAI : MonoBehaviour
{   
    // Enemy AI state
    [SerializeField] private AIState currentState;
    private float attackRange = 2f;

    private Transform player;
    private NavMeshAgent navAgent;

    // Enemy Attack variables
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;
    private bool hasLOS;

    private DamageHandler damageHandler;
    private EnemyStats enemyStats;

    // Bullet variables
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzleTransform;

    //  Animation varaibles
    private Animator animator;
    private bool initializeSpawn = false;
    private bool isAttackCoroutineRunning = false; // Track attack state

    private void Awake()
    {   
        // Reference neccesary components
        animator = GetComponentInChildren<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        damageHandler = GetComponent<DamageHandler>();
        enemyStats = GetComponent<EnemyStats>();
        animator.SetTrigger("Spawn");

        // Initializes state
        StartCoroutine(InitializeSpawn());
    }

    void Update()
    {
        if (player != null && initializeSpawn)
        {
            // Checks for any state changes and updates them
            CheckLOS();
            CheckStateTransition();
            UpdateCurrentState();
        }
    }
    // If close to Player change to attack State
    void CheckStateTransition()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If Player outside Attack range  Attack the Player
        if (distanceToPlayer <= attackRange && hasLOS)
        {
            SetState(AIState.Attack);
        }
        // If Player outside Attack range  Chase the Player
        else
        {
            SetState(AIState.Chase);
        }
    }

    void SetState(AIState newState)
    {
        // Chase state
        if (currentState == AIState.Attack && newState != AIState.Attack)
        {
            navAgent.isStopped = false; // Able to move 
        }

        currentState = newState;

        // Attack state
        if (currentState == AIState.Attack)
        {
            navAgent.isStopped = true; // Stops moving when attacking
        }
    }

    void UpdateCurrentState()
    {
        switch (currentState)
        {
            case AIState.Chase:
                ChasePlayer();
                break;
            case AIState.Attack:
                if (!isAttackCoroutineRunning)
                {   
                    // Coroutine for Shooting
                    StartCoroutine(AttackPlayer());
                }
                break;
        }
    }
    // Set Player position as NavAgent destination, aka the target
    void ChasePlayer()
    {
        navAgent.SetDestination(player.position);
        //animator.SetBool("IsChasing", true);

    }

    // Checks if the Enemy has a line of sight with the Player
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

        // Looks at shooting rotation
        transform.rotation = lookRotation; 

        // Attack if cooldown finished
        if (Time.time > lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            FireProjectile();
            lastAttackTime = Time.time;
        }

        // Brief delay before next check
        yield return new WaitForSeconds(0.1f); 
        isAttackCoroutineRunning = false;
    }

    private void FireProjectile()
    {   
        // Creates bullet instantiate 
        GameObject bullet = Instantiate(bulletPrefab, muzzleTransform.position, Quaternion.identity);
        ProjectileController projectileController = bullet.GetComponent<ProjectileController>();
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // Apply Enemy stat damage to instantiated bullet projectile
        projectileController.baseDamage = enemyStats.damage;
        Vector3 shotDirection = (player.position - muzzleTransform.position).normalized;
        rb.velocity = shotDirection * projectileController.speed;
    }

    private IEnumerator InitializeSpawn()
    {   
        // Lets spawn animation play 
        yield return new WaitForSeconds(1f);
        initializeSpawn = true;
        navAgent.speed = enemyStats.movementSpeed;
        attackRange = enemyStats.attackRange;
        SetState(AIState.Chase);
    }

    // Old script to try and fix collision not working
    //void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.CompareTag("Player Projectile"))
    //    {


    //        ProjectileController projectileController = other.gameObject.GetComponent<ProjectileController>();

    //        damageHandler.ReceiveDamage(projectileController.baseDamage);

    //        // Apply DOT effects
    //        if (projectileController.applyFireDOT)
    //        {
    //            damageHandler.ApplyFireDOT(projectileController.baseDamage, projectileController.DOTDuration);
    //        }
    //        if (projectileController.applyPoisonDOT)
    //        {
    //            damageHandler.ApplyPoisonDOT(projectileController.baseDamage, projectileController.DOTDuration);
    //        }

    //    }
    //}
}