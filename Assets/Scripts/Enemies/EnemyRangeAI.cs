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

    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;

    private DamageHandler damageHandler;
    private EnemyStats enemyStats;

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform muzzleTransform;

    private bool hasLOS;


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
        if (player != null)
        {
            CheckLOS();
            CheckStateTransition();
            UpdateCurrentState();
        }
      
    }

    void CheckStateTransition()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && hasLOS && _currentState != AIState.Attack)
        {
            SetState(AIState.Attack);
        }
        else if ((distanceToPlayer > attackRange || !hasLOS) && _currentState != AIState.Chase)
        {
            SetState(AIState.Chase);
        }
    }

    void SetState(AIState newState)
    {
        _currentState = newState;
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
        //if (navAgent.isStopped) navAgent.isStopped = false;
        navAgent.SetDestination(player.position);

        // Optional: Add acceleration when starting to chase
        navAgent.speed = Mathf.Lerp(navAgent.speed, 5f, Time.deltaTime);
    }


    private void CheckLOS()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        int layerToIgnore1 = 7;
        int layerToIgnore2 = 9;
        int ignoreMask = ~(1 << layerToIgnore1 | 1 << layerToIgnore2);

        hasLOS = Physics.Raycast(transform.position, direction, out RaycastHit hit, attackRange, ignoreMask) && hit.collider.CompareTag("Player");
    }

    IEnumerator AttackPlayer()
    {
        // Stop movement while attacking
        //navAgent.isStopped = true;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        transform.LookAt(player.position);

        if (Time.time > lastAttackTime + attackCooldown)
        {
            RaycastHit hit;
            GameObject bullet = GameObject.Instantiate(bulletPrefab, muzzleTransform.position, Quaternion.identity);
            ProjectileController projectileController = bullet.GetComponent<ProjectileController>();
            projectileController.baseDamage = enemyStats.damage;

            // Ignore Layer 7 and 9
            int layerToIgnore1 = 7;
            int layerToIgnore2 = 9;
            int ignoreMask = ~(1 << layerToIgnore1 | 1 << layerToIgnore2);


            Vector3 shotDirection = (player.position - muzzleTransform.position).normalized;
            float maxDistance = 100f;

            // Calculate target point in the shooting direction
            projectileController.target = muzzleTransform.position + shotDirection * maxDistance;


            // Perform attack
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                lastAttackTime = Time.time;

                if (Physics.Raycast(transform.position, shotDirection, out hit, maxDistance, ignoreMask))
                {

                    //Debug.DrawRay(transform.position, shotDirection * hit.distance, Color.green, 1f);
                    //projectileController.target = hit.point;
                    projectileController.hit = true;
                    yield return new WaitForSeconds(attackCooldown);
                }

            }

        }

    }

}