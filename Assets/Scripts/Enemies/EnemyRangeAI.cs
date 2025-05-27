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


        //navAgent.speed = Mathf.Lerp(navAgent.speed, 5f, Time.deltaTime);
    }


    private void CheckLOS()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        int layerToIgnore1 = 7;
        int layerToIgnore2 = 9;
        int layerToIgnore3 = 13;
        int ignoreMask = ~(1 << layerToIgnore1 | 1 << layerToIgnore2 | 1 << layerToIgnore3);

        hasLOS = Physics.Raycast(transform.position, direction, out RaycastHit hit, attackRange, ignoreMask) && hit.collider.CompareTag("Player");
    }

    IEnumerator AttackPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        if (Time.time > lastAttackTime + attackCooldown)
        {
            GameObject bullet = Instantiate(bulletPrefab, muzzleTransform.position, Quaternion.identity);
            ProjectileController projectileController = bullet.GetComponent<ProjectileController>();
            Rigidbody rb = bullet.GetComponent<Rigidbody>(); // Add this

            projectileController.baseDamage = enemyStats.damage;
            Vector3 shotDirection = (player.position - muzzleTransform.position).normalized;

            // Physics-based movement
            rb.velocity = shotDirection * projectileController.speed;

            // Maintain raycast verification
            if (Physics.Raycast(muzzleTransform.position, shotDirection, out RaycastHit hit, attackRange))
            {
                projectileController.hit = true;
            }

            lastAttackTime = Time.time;
            yield return new WaitForSeconds(attackCooldown);
        }
    }

}