using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KamikazeDrone : Enemy
{
    //Animation
    public Animation Anim;
    public float spawnForce;

    // Death Animation
    public float fowardAnimForce;
    public float downAnimForce;
    
    [Header("Daño")]
    public int explosionDamage;
    public float explosionRange;

    protected override void Awake()
    {
        base.Awake();

        if (_rb != null)
        {
            _rb.AddForce(transform.forward * spawnForce, ForceMode.Acceleration);
        }
    } 
    void Update()
    {
        if (isAlive)
        {
            _playerInSightRange = Physics.CheckSphere(transform.position, _sightRange, _whatIsPlayer);
            _playerInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _whatIsPlayer);
            if (_playerInSightRange && !_playerInAttackRange)
            {
                ChasePlayer();    
                transform.LookAt(_player.position);
            }
            if (_playerInAttackRange && _playerInSightRange)
            {
                PrepareAndAttackPlayer();
                transform.LookAt(_player.position);
            }
        }
    }

    private void ChasePlayer()
    {
        // Actualizar la posición del enemigo hacia el jugador, incluyendo el eje Y
        Vector3 playerPosition = _player.position;
        Vector3 moveDirection = (playerPosition - transform.position).normalized;
        transform.position += moveDirection * _movementSpeed * Time.deltaTime;
        Anim.Play();
    }


    private void PrepareAndAttackPlayer()
    {
        if (!_hasExploded)
        {        
            // Realizar el ataque
            Explode();
        }
    }

    protected override void Explode()
    {
        _hasExploded = true; // Set the flag to true to prevent further explosions

        base.Explode();

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange, _whatIsPlayer);

        foreach (Collider collider in colliders)
        {
            // Verifica si el objeto detectado es el jugador
            if (collider.GetComponent<DavesPM>() != null)
            {
                _frenzyManager.TakeDamage(explosionDamage);
            }
        }
    }
    

    public override void TakeDamage(int dmg)
    {
        PopUp();
        if (_health <= 0)
        {
            _frenzyManager.AddPoints(_pointsOnKill);
            isAlive = false;
            Death();
        }
    }


    protected override void Death()
    {
        if (_explosionPrefab != null)
        {

            SoundManager.PlaySound(SoundType.DRONEXPLODE, SoundManager.Instance.GetSFXVolume());
            GameObject explosionInstance = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireCube(patrolCenter, new Vector3(patrolXDistance * 2, 1, patrolZDistance * 2)); // Dibujar el área de patrullaje en forma de cubo
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<DavesPM>() != null)
        {
            Death();
        }
    }

}