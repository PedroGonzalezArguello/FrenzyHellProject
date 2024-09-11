using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MiniBoss : MonoBehaviour
{
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer, whatIsWall;
    public float health;
    public FrenzyManager frenzyManager;
    public Transform shootPoint;
    public Rigidbody rb;
    public SoundManager soundManager;
    public ParticleSystem particles;
    public NavMeshAgent agent;
    public GameManager gameManager;

    public bool isAlive;

    //Animation
    public Animation Anim;
    public Animator animator;
    // Attacking
    public float timeBetweenAttacks;
    public float upwardForce;
    public float fowardForce;
    public bool alreadyAttacked;
    public GameObject projectile;
    public float movementSpeed;

    // Patrolling
    private bool isPatrolling = false;
    public float patrolXDistance = 5f; // Distancia en el eje X
    public float patrolZDistance = 5f; // Distancia en el eje Z
    public float patrolSpeed = 2f;
    private Vector3 patrolCenter;
    private Vector3 patrolTarget;
    public float rotationSpeed = 5f; // Nueva variable para la velocidad de rotación

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public float pointsOnKill;

    // Death Animation
    public float fowardAnimForce;
    public float downAnimForce;
    public bool isDestroyable;
    public GameObject explosion;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        player = GameObject.Find("Player")?.transform;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        isDestroyable = false;
        isAlive = true;
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        var frenzyManagerInstance = GameObject.FindGameObjectWithTag("FrenzyManager");
        if (frenzyManagerInstance != null)
        {
            frenzyManager = frenzyManagerInstance.GetComponent<FrenzyManager>();
        }
        patrolCenter = transform.position; // Fijar el centro del patrullaje a la posición inicial del dron
    }

    void Update()
    {
        if (isAlive)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInAttackRange && !playerInSightRange)
            {
                if (!isPatrolling) StartCoroutine(Patrol());
            }
            if (playerInSightRange && !playerInAttackRange)
            {
                ChasePlayer();
                isPatrolling = false;
                transform.LookAt(player.position);
            }
            if (playerInAttackRange && playerInSightRange)
            {
                if (!alreadyAttacked)
                {
                    StartCoroutine(PrepareAndAttackPlayer());
                }
                transform.LookAt(player.position);
            }
        }
    }

    private IEnumerator Patrol()
    {
        isPatrolling = true;
        while (isPatrolling)
        {
            if (patrolTarget == Vector3.zero || Vector3.Distance(transform.position, patrolTarget) < 1f)
            {
                float randomX = Random.Range(-patrolXDistance, patrolXDistance);
                float randomZ = Random.Range(-patrolZDistance, patrolZDistance);
                patrolTarget = new Vector3(patrolCenter.x + randomX, transform.position.y, patrolCenter.z + randomZ);
            }

            // Suavizar la rotación hacia el objetivo del patrullaje
            Quaternion targetRotation = Quaternion.LookRotation(patrolTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 newPosition = Vector3.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);
            newPosition.y = transform.position.y;
            transform.position = newPosition;
            yield return null;
        }
    }

    private void ChasePlayer()
    {
        Vector3 playerPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 moveDirection = (playerPosition - transform.position).normalized;

        Vector3 moveDirectionXZ = new Vector3(moveDirection.x, 0f, moveDirection.z);
        Vector3 newPosition = transform.position + moveDirectionXZ * movementSpeed * Time.deltaTime;
        newPosition.y = transform.position.y; // Mantener la posición Y constante
        transform.position = newPosition;

        //SoundManager.PlaySound(SoundType.ONDRONEDETECT, SoundManager.Instance.GetSFXVolume());
    }


    private IEnumerator PrepareAndAttackPlayer()
    {
        alreadyAttacked = true;


        // Reproducir sonido de preparación
        SoundManager.PlaySound(SoundType.DRONECHARGE, SoundManager.Instance.GetSFXVolume());
        particles.Play();

        // Esperar un momento antes de atacar
        yield return new WaitForSeconds(.8f); // Ajusta el tiempo según sea necesario
        animator.SetTrigger("Attack");
        // Reproducir sonido de ataque
        SoundManager.PlaySound(SoundType.DRONEATTACK, SoundManager.Instance.GetSFXVolume());

        // Realizar el ataque
        Rigidbody rb = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * fowardForce, ForceMode.Impulse);
        rb.AddForce(transform.up * upwardForce, ForceMode.Impulse);

        // Esperar el tiempo entre ataques antes de permitir otro ataque
        yield return new WaitForSeconds(timeBetweenAttacks);
        ResetAttack();
    }

    public void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        string DamagePopUp = "X";
        PopUpManager._current.PopUp(transform.position, DamagePopUp, Color.red);
        SoundManager.PlaySound(SoundType.METALHIT, SoundManager.Instance.GetSFXVolume());

        if (health <= 0 && isAlive)
        {
            isAlive = false;
            frenzyManager.AddPoints(pointsOnKill);
            PlayDeathAnimation();
        }
    }

    private void PlayDeathAnimation()
    {
        Anim.Play();
        rb.useGravity = true;
        rb.velocity = Vector3.zero; // Detener el movimiento actual
        rb.AddForce(transform.forward * fowardAnimForce + Vector3.down * downAnimForce, ForceMode.Impulse);
        isDestroyable = true;
        SoundManager.PlaySound(SoundType.DRONEDEATH, SoundManager.Instance.GetSFXVolume());
        Invoke(nameof(DestroyEnemy), 0.1f);
    }

    private void DestroyEnemy()
    {
        
        if (explosion != null)
        {
            GameObject explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(explosionInstance, 0.2f);
        }
        
        Destroy(gameObject, 2f );

        gameManager.ShowVictoryScreen();


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(patrolCenter, new Vector3(patrolXDistance * 2, 1, patrolZDistance * 2)); // Dibujar el área de patrullaje en forma de cubo
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            fowardAnimForce = 6;
            Anim.Play();
            rb.useGravity = true;
            rb.velocity = Vector3.zero; // Detener el movimiento actual
            rb.AddForce(-transform.forward * fowardAnimForce + Vector3.down * -downAnimForce, ForceMode.Impulse);
            //isDestroyable = true;
            SoundManager.PlaySound(SoundType.DRONEDEATH, SoundManager.Instance.GetSFXVolume());
            Invoke(nameof(DestroyEnemy), 2f);
        }

        if (isDestroyable)
        {
            SoundManager.PlaySound(SoundType.DRONECOLLISION, SoundManager.Instance.GetSFXVolume());
            gameManager.ShowVictoryScreen();
            Destroy(gameObject);
            
        }
    }
}