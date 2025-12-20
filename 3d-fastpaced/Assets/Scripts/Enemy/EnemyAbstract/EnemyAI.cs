using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAI : MonoBehaviour
{
    protected Transform player;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;
    protected NavMeshAgent agent;
    [SerializeField] protected EnemyDataSO enemyData;

    //Enemy Stats
    protected float damage;

    //Patroling
    [SerializeField] private Vector3 walkPoint;

    //Attacking
    [SerializeField] private float timeBetweenAttacks;
    private bool alreadyAttacked;

    //States
    [SerializeField] private float sightRange, attackRange;
    public bool isSightRange, isAttackRange;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;   
        damage = enemyData.enemyDamage;
    }

    private void Update()
    {
        isSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        isAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!isSightRange && !isAttackRange) Patrolling();
        if (isSightRange && !isAttackRange) ChasePlayer();
        if (isSightRange && isAttackRange) Attack();

        //Debug.Log($"Sight: {isSightRange} | Attack: {isAttackRange}");
    }

    protected virtual void Patrolling()
    {
        
    }

    protected virtual void ChasePlayer()
    {
        Debug.Log("Chasing Player");
        agent.SetDestination(player.position);
    }

    private void Attack()
    {
        EnemyAttack();
    }

    protected void SetMovementSpeed()
    {
        agent.speed = enemyData.moveSpeed;
    }

    protected abstract void EnemyAttack();
    

    

    // Scene view'da seçiliyken gizmo çizimleri
    private void OnDrawGizmosSelected()
    {
        // Sight range
        Gizmos.color = new Color(0f, 0.6f, 1f, 1f); // mavi
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Attack range
        Gizmos.color = new Color(1f, 0.3f, 0f, 1f); // turuncu
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Walk point (varsa anlamlý bir deðer)
        if (walkPoint != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(walkPoint, 0.2f);
            Gizmos.DrawLine(transform.position, walkPoint);
        }
    }
}
