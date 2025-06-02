using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public enum AIState { Chase, Attack }

public class EnemyAI : MonoBehaviour
{
    // Enemy AI state
    [SerializeField] private AIState currentState;

    private Transform player;
    private NavMeshAgent navAgent;

    // Enemy attack variable
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;

    private DamageHandler damageHandler;
    private EnemyStats enemyStats;

    // Animation Variables
    private Animator animator;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isDead;

    private bool initializeSpawn = false;

    private void Awake()
    {
        //animator = GetComponentInChildren<Animator>();
        //navAgent = GetComponent<NavMeshAgent>();
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        //damageHandler = GetComponent<DamageHandler>();
        //enemyStats = GetComponent<EnemyStats>();

        //navAgent.speed = enemyStats.movementSpeed;
        //SetState(AIState.Chase);


        // References neccesary components
        animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Spawn");   // Run spawn animation
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        damageHandler = GetComponent<DamageHandler>();
        enemyStats = GetComponent<EnemyStats>();

        // Initializes state
        StartCoroutine(InitializeSpawn());
    }

    private void Update()
    {
        if (player != null && initializeSpawn)
        {
            // Checks for any state changes and updates them
            CheckStateTransition();
            UpdateCurrentState();
        }
    }

    // If close to Player change to attack State
    private void CheckStateTransition()
    {

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If Player outside Attack range  Attack the Player
        if (distanceToPlayer <= enemyStats.attackRange && currentState != AIState.Attack)
        {
            SetState(AIState.Attack);
        }
        // If Player outside Attack range  Chase the Player
        else if (distanceToPlayer > enemyStats.attackRange && currentState != AIState.Chase)
        {
            SetState(AIState.Chase);
        }

    }

    private void SetState(AIState newState)
    {
        currentState = newState;

    }

    private void UpdateCurrentState()
    {
        switch (currentState)
        {
            case AIState.Chase:
                ChasePlayer();

                break;
            case AIState.Attack:
                AttackPlayer();
                break;
        }
    }

    // Set Player position as NavAgent destination, aka the target
    private void ChasePlayer()
    {
        //if (navAgent.isStopped) navAgent.isStopped = false;
        navAgent.SetDestination(player.position);

        animator.SetBool("IsChasing", true);

        //navAgent.speed = Mathf.Lerp(navAgent.speed, 5f, Time.deltaTime);
    }


    private void AttackPlayer()
    {

        animator.SetBool("IsChasing", false);

        if (Time.time > lastAttackTime + attackCooldown)
        {
            // Face player
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);


            // Perform attack
            if (Vector3.Distance(transform.position, player.position) <= enemyStats.attackRange)
            {
                //Debug.Log("Attacking the player");
                lastAttackTime = Time.time;

                navAgent.isStopped = true; // Enemy stops when attacking
                animator.SetBool("IsAttacking", true);
                animator.SetTrigger("Attack");

                Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyStats.attackRange);

                // If sphere hits Player it will get the Player's damage handler and apply damage
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Player"))
                    {
                        //Debug.Log("Hit player!");

                        DamageHandler playerDamageHandler = hitCollider.GetComponent<DamageHandler>();

                        if (playerDamageHandler != null)
                        {
                            playerDamageHandler.ReceiveDamage(enemyStats.damage);
                        }
                    }
                }

            }
            else
            {
                animator.SetBool("IsAttacking", false);
            }
        }
        else
        {
            navAgent.isStopped = false;
            animator.SetBool("IsAttacking", false);
        }
    }

    private IEnumerator InitializeSpawn()
    {
        // Lets spawn animation to play
        yield return new WaitForSeconds(1f);


        initializeSpawn = true;
        navAgent.speed = enemyStats.movementSpeed;

        SetState(AIState.Chase);
    }

}