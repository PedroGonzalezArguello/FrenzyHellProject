using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDrone : Enemy
{
    [Header("EnemyDrone - Referencias")]
    public Transform shootPoint;
    //public NavMeshAgent agent;

    
    //Animation
    public Animation Anim;
    public Animator animator;

    // Attacking
    public float timeBetweenAttacks;
    public float upwardForce;
    public float fowardForce;
    public bool alreadyAttacked;
    public GameObject projectile;

    // Patrolling
    private bool isPatrolling = false;
    public float patrolXDistance = 5f; // Distancia en el eje X
    public float patrolZDistance = 5f; // Distancia en el eje Z
    public float patrolSpeed = 2f;
    private Vector3 patrolCenter;
    private Vector3 patrolTarget;
    private Vector3 direction;
    public float rotationSpeed = 5f; // Nueva variable para la velocidad de rotación


    // Death Animation
    public float fowardAnimForce;
    public float downAnimForce;

    //Eventos
    public delegate void EnemigoEliminadoHandler(EnemyDrone enemydrone);
    public static event EnemigoEliminadoHandler EnemigoEliminado;


    protected override void Start()
    {
        base.Start();
        patrolCenter = transform.position; // Fijar el centro del patrullaje a la posición inicial del dron
    }

    void Update()
    {
        if (isAlive)
        {
            _playerInSightRange = Physics.CheckSphere(transform.position, _sightRange, _whatIsPlayer);
            _playerInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _whatIsPlayer);

            if (!_playerInAttackRange && !_playerInSightRange)
            {
                if (!isPatrolling) StartCoroutine(Patrol());
            }
            if (_playerInSightRange && !_playerInAttackRange)
            {
                ChasePlayer();
                isPatrolling = false;
                transform.LookAt(_player.position);
            }
            if (_playerInAttackRange && _playerInSightRange)
            {
                if (!alreadyAttacked)
                {
                    StartCoroutine(PrepareAndAttackPlayer());
                }
                transform.LookAt(_player.position);
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
            direction = patrolTarget - transform.position;
            // Suavizar la rotación hacia el objetivo del patrullaje
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(patrolTarget - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            Vector3 newPosition = Vector3.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);
            newPosition.y = transform.position.y;
            transform.position = newPosition;
            yield return null;
        }
    }

    private void ChasePlayer()
    {
        Vector3 playerPosition = new Vector3(_player.position.x, transform.position.y, _player.position.z);
        Vector3 moveDirection = (playerPosition - transform.position).normalized;

        Vector3 moveDirectionXZ = new Vector3(moveDirection.x, 0f, moveDirection.z);
        Vector3 newPosition = transform.position + moveDirectionXZ * _movementSpeed * Time.deltaTime;
        newPosition.y = transform.position.y; // Mantener la posición Y constante
        transform.position = newPosition;

        //SoundManager.PlaySound(SoundType.ONDRONEDETECT, SoundManager.Instance.GetSFXVolume());
    }


    private IEnumerator PrepareAndAttackPlayer()
    {
        alreadyAttacked = true;
        

        // Reproducir sonido de preparación
        SoundManager.PlaySound(SoundType.DRONECHARGE, SoundManager.Instance.GetSFXVolume());
        _particles.Play();

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

    public override void TakeDamage(int dmg)
    {
        SoundManager.PlaySound(SoundType.METALHIT, SoundManager.Instance.GetSFXVolume());
        base.TakeDamage(dmg);
    }

    protected override void Death()
    {
        base.Death();
        isAlive = false;
        _frenzyManager.AddPoints(_pointsOnKill);
        PlayDeathAnimation();

        if (EnemigoEliminado != null)
        {
            EnemigoEliminado(this);
        }
    }


    


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(patrolCenter, new Vector3(patrolXDistance * 2, 1, patrolZDistance * 2)); // Dibujar el área de patrullaje en forma de cubo
    }
    private void PlayDeathAnimation()
    {
        fowardAnimForce = 6;
        Anim.Play();
        _rb.useGravity = true;
        _rb.velocity = Vector3.zero; // Detener el movimiento actual
        _rb.AddForce(transform.forward * fowardAnimForce + Vector3.down * downAnimForce, ForceMode.Impulse);
        SoundManager.PlaySound(SoundType.DRONEDEATH, SoundManager.Instance.GetSFXVolume());
        Explode(2f);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<DavesPM>() != null)
        {
            Death();
        }
    }
}