using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class RoachEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshAgent navMesh;

    [Header("Layers")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask ObjectMask;

    [Header("patrol Settings")]
    [SerializeField] private float targetRadius;
    private Vector3 targetPoint;
    private bool targetPointSet;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange;
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float lastAttackTime;
    private bool canAttack;

    [Header("Detection Settings")]
    [SerializeField] private float hearingRange;

    private bool isPlayerInRange;
    private bool canHearPlayer;

    private void Awake()
    {

        navMesh = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        detectPlayer();
        updateBehaviorState();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, targetRadius);
    }
    private void detectPlayer()
    {
        canHearPlayer = Physics.CheckSphere(transform.position, hearingRange, playerMask);
        isPlayerInRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        canAttack = Time.time - lastAttackTime >= attackCooldown && isPlayerInRange && canHearPlayer;

        player = Physics.CheckSphere(transform.position, hearingRange, playerMask) ? GameObject.FindGameObjectWithTag("Player") : null;
        if (player != null)
        {
            target = player.transform;
        }
    }
    private void findPatrolPoint()
    {
        float randomX = Random.Range(-targetRadius, targetRadius);
        float randomZ = Random.Range(-targetRadius, targetRadius);

        Vector3 potentialpoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(potentialpoint, -transform.up, 2f, groundMask))
        {
            targetPoint = potentialpoint;
            targetPointSet = true;
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void performPatrol()
    {
        if (!targetPointSet)
        {
            findPatrolPoint();
        }
        if (targetPointSet)
        {
            navMesh.SetDestination(targetPoint);
        }
        if (Vector3.Distance(transform.position, targetPoint) < 1f)
        {
            targetPointSet = false;
        }
    }

    private void performAttack()
    {

    }

    private void performChase()
    {
        
            navMesh.SetDestination(player.transform.position);
        
    }

    private void updateBehaviorState()
    {
        if (!canHearPlayer && !isPlayerInRange)
        {
            performPatrol();
        }
        else if (canHearPlayer && !isPlayerInRange)
        {
            performChase();
        }
        else if (canAttack && canHearPlayer)
        {
            performAttack();
        }
    }
}


