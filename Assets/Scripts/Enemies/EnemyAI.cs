using UnityEngine.AI;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

public enum AIState { Chase, Attack }

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private AIState _currentState;

    private Transform player;
    private NavMeshAgent navAgent;

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
        if (player != null)
        {
            CheckStateTransition();
            UpdateCurrentState();
        }
    }

    void CheckStateTransition()
    {
        
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);


            if (distanceToPlayer <= enemyStats.attackRange && _currentState != AIState.Attack)
            {
                SetState(AIState.Attack);
            }
            else if (distanceToPlayer > enemyStats.attackRange && _currentState != AIState.Chase)
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
        //if (navAgent.isStopped) navAgent.isStopped = false;
        navAgent.SetDestination(player.position);

        // Optional: Add acceleration when starting to chase
        navAgent.speed = Mathf.Lerp(navAgent.speed, 5f, Time.deltaTime);
    }


    void AttackPlayer()
    { 

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
        }
    }

    

}