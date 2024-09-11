using UnityEngine;
using UnityEngine.AI;

public class EnemiesScript : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    public FrenzyManager frenzyManager;

    public Transform shootPoint;

    //Daño Melee
    public int MeleeDamage;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public Animator animator;

    //[SerializeField] private BoxCollider boxCollider;

    //Attacking
    public float timeBetweenAttacks;
    public float upwardForce;
    public float fowardForce;
    public bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public float pointsOnKill;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

    }

    private void Start()
    {
        GameObject frenzyManagerInstance = GameObject.FindGameObjectWithTag("FrenzyManager");
        if (frenzyManagerInstance != null)
        {
            frenzyManager = frenzyManagerInstance.GetComponent<FrenzyManager>();
        }

    }

    private void Update()
    {
        //Animacion Caminar enemigo.

        //animator.SetFloat("Walk", walkPointRange);
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();

        transform.LookAt(player);
    }

    private void Patroling()
    {
        animator.SetBool("Walking", true);

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        animator.SetBool("Walking", true);

        agent.SetDestination(player.transform.position);
    }

    public virtual void AttackPlayer()
    {
        animator.SetBool("Walking", false);
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("AttackAnim"))
            {
                animator.SetTrigger("Attack");
                //Make sure enemy doesn't move

                transform.LookAt(player);
                
                ///Attack code here
                Rigidbody rb = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * fowardForce, ForceMode.Impulse);
                rb.AddForce(transform.up * upwardForce, ForceMode.Impulse);
                ///End of attack code
                
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    }
    public void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        { 
            Invoke(nameof(DestroyEnemy), 0.1f); 
        }
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
        frenzyManager.AddPoints(pointsOnKill);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    void EnableAttack()
    {
        //boxCollider.enabled = true;
    }

    void DisableAttack()
    {
        //boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<DavesPM>();

        if (player != null)
        {
            Debug.Log("Hit");
            if (other.CompareTag("Player"))
            {
                frenzyManager.TakeDamage(MeleeDamage);

            }
        }
        else
        {
            Debug.Log("no encuentro player");
        }
    }
}
