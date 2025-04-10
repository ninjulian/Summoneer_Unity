using UnityEngine.AI;
using UnityEngine;

public enum AIRangeState { Chase, Attack }

public class EnemyRangeAI : MonoBehaviour
{
    [SerializeField] private AIState _currentState;
    [SerializeField] private float attackRange = 2f;

    private Transform player;
    private NavMeshAgent navAgent;



    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;

    private DamageHandler damageHandler;
    private EnemyStats enemyStats;


    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        damageHandler = GetComponent<DamageHandler>();
        enemyStats = GetComponent<EnemyStats>();

        navAgent.speed = enemyStats.movementSpeed;

        SetState(AIState.Chase);
    }

    void Update()
    {
        CheckStateTransition();
        UpdateCurrentState();
    }

    void CheckStateTransition()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && _currentState != AIState.Attack)
        {
            SetState(AIState.Attack);
        }
        else if (distanceToPlayer > attackRange && _currentState != AIState.Chase)
        {
            SetState(AIState.Chase);
        }
    }

    void SetState(AIState newState)
    {
        _currentState = newState;
        // Add any state entry logic here
    }

    void UpdateCurrentState()
    {
        switch (_currentState)
        {
            case AIState.Chase:
                ChasePlayer();
                break;
            case AIState.Attack:
                AttackPlayer();
                break;
        }
    }

    void ChasePlayer()
    {
        if (navAgent.isStopped) navAgent.isStopped = false;
        navAgent.SetDestination(player.position);

        // Optional: Add acceleration when starting to chase
        navAgent.speed = Mathf.Lerp(navAgent.speed, 5f, Time.deltaTime);
    }


    void AttackPlayer()
    {
        // Stop movement while attacking
        navAgent.isStopped = true;

        if (Time.time > lastAttackTime + attackCooldown)
        {
            // Face player

            RaycastHit hit;

            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

            // Perform attack
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                Debug.Log("Range Attack the player");
                lastAttackTime = Time.time;

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, attackRange);
            }
        }
    }

}