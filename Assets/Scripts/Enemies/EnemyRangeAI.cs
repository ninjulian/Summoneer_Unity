using UnityEngine.AI;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections;

public enum AIRangeState { Chase, Attack }

public class EnemyRangeAI : MonoBehaviour
{
    [SerializeField] private AIState _currentState;
    private float attackRange = 2f;

    private Transform player;
    private NavMeshAgent navAgent;


    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;

    private DamageHandler damageHandler;
    private EnemyStats enemyStats;

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform muzzleTransform;


    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        damageHandler = GetComponent<DamageHandler>();
        enemyStats = GetComponent<EnemyStats>();

        attackRange = enemyStats.attackRange;

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
                StartCoroutine(AttackPlayer());
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


    IEnumerator AttackPlayer()
    {
        // Stop movement while attacking
        navAgent.isStopped = true;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);


        if (Time.time > lastAttackTime + attackCooldown)
        {
            // Face player

            RaycastHit hit;
            GameObject bullet = GameObject.Instantiate(bulletPrefab, muzzleTransform.position, Quaternion.identity);
            ProjectileController projectileController = bullet.GetComponent<ProjectileController>();

            // Ignore Layer 7 and 9
            int layerToIgnore1 = 7;
            int layerToIgnore2 = 9;
            int ignoreMask = ~(1 << layerToIgnore1 | 1 << layerToIgnore2);

            
            // Perform attack
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                Debug.Log("Range Attack the player");
                lastAttackTime = Time.time;

                Debug.DrawRay(transform.position, direction * 100f, Color.yellow, 1f);

                if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, ignoreMask))
                {
                    Debug.DrawRay(transform.position, direction * hit.distance, Color.red, 1f);
                    projectileController.target = hit.point;
                    projectileController.hit = true;
                    //Debug.Log("hit something");
                    yield return new WaitForSeconds(attackCooldown);
                    // canShoot = true;
                }
                else
                {
                    Debug.DrawRay(transform.position, direction * 100, Color.blue, 1f);
                    projectileController.target = transform.position + direction * 100;
                    projectileController.hit = true;
                    //Debug.Log("nothing");
                    yield return new WaitForSeconds(attackCooldown);
                    //canShoot = true;
                }



            }



        }


    }

}