using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KamikazeDrone : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer, whatIsWall;
    public float health;
    public FrenzyManager frenzyManager;
    public Rigidbody rb;
    public SoundManager soundManager;
    public ParticleSystem particles;

    public bool isAlive;
    bool hasExploded;

    //Animation
    public Animation Anim;
    
    [Header("Movimiento")]
    public float movementSpeed;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public float pointsOnKill;
    public float spawnForce;

    // Death Animation
    public float fowardAnimForce;
    public float downAnimForce;
    public bool isDestroyable;
    public GameObject explosion;
    
    [Header("Daño")]
    public int explosionDamage;
    public float explosionRange;

    private void Awake()
    {
        player = GameObject.Find("Player")?.transform;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        isDestroyable = false;
        isAlive = true;
        hasExploded = false;

        if (rb != null)
        {
            rb.AddForce(transform.forward * spawnForce, ForceMode.Acceleration);
        }
    } 

    void Start()
    {
        var frenzyManagerInstance = GameObject.FindGameObjectWithTag("FrenzyManager");
        if (frenzyManagerInstance != null)
        {
            frenzyManager = frenzyManagerInstance.GetComponent<FrenzyManager>();
        }
    }

    void Update()
    {
        if (isAlive)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (playerInSightRange && !playerInAttackRange)
            {
                ChasePlayer();
                
                transform.LookAt(player.position);
            }
            if (playerInAttackRange && playerInSightRange)
            {
                PrepareAndAttackPlayer();
                transform.LookAt(player.position);
            }
        }
    }

    private void ChasePlayer()
    {
        // Actualizar la posición del enemigo hacia el jugador, incluyendo el eje Y
        Vector3 playerPosition = player.position;
        Vector3 moveDirection = (playerPosition - transform.position).normalized;
        transform.position += moveDirection * movementSpeed * Time.deltaTime;
        Anim.Play();
    }


    private void PrepareAndAttackPlayer()
    {
        if (!hasExploded)
        {        
            // Realizar el ataque
            Explode();
        }
    }

    private void Explode()
    {
        if (!hasExploded) // Only execute explosion logic if it hasn't already exploded
        {
            hasExploded = true; // Set the flag to true to prevent further explosions

            // Instantiate explosion
            if (explosion != null)
            {
                GameObject explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(explosionInstance, 1f); // Destroy the explosion after 2 seconds
                Destroy(gameObject);
            }

            SoundManager.PlaySound(SoundType.DRONEXPLODE, SoundManager.Instance.GetSFXVolume());

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange, whatIsPlayer);

            foreach (Collider collider in colliders)
            {
                // Verifica si el objeto detectado es el jugador
                if (collider.CompareTag("Player"))
                {
                    frenzyManager.TakeDamage(explosionDamage);
                    rb = collider.GetComponentInParent<Rigidbody>();
                }
            }
            
        }
    }
    

    public void TakeDamage(int damage)
    {
        GameObject explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity);

        string DamagePopUp = "X";
        PopUpManager._current.PopUp(transform.position, DamagePopUp, Color.red);


        Destroy(explosionInstance, 2f); // Destroy the explosion after 2 seconds

        Destroy(gameObject);
        health -= damage;
        SoundManager.PlaySound(SoundType.DRONEXPLODE, SoundManager.Instance.GetSFXVolume());

        if (health <= 0 && isAlive)
        {
            isAlive = false;
            frenzyManager.AddPoints(pointsOnKill);
        }
    }


    private void DestroyEnemy()
    {
        if (explosion != null)
        {
            GameObject explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(explosionInstance, 2f);
        }

        Destroy(gameObject, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireCube(patrolCenter, new Vector3(patrolXDistance * 2, 1, patrolZDistance * 2)); // Dibujar el área de patrullaje en forma de cubo
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
            SoundManager.PlaySound(SoundType.DRONEXPLODE, SoundManager.Instance.GetSFXVolume());
            Invoke(nameof(DestroyEnemy), 0.1f);
        }

        if (isDestroyable)
        {
            Explode();
            Destroy(gameObject);
        }
    }
}