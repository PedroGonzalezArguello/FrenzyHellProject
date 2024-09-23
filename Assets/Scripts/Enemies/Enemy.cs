using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase enemy: existe solo para que todos los enemigos hereden de esto

public abstract class Enemy : Entity
{
    [Header("Enemy - Referencias")]
    [SerializeField] protected Transform _player;
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] protected FrenzyManager _frenzyManager;
    [SerializeField] protected SoundManager _soundManager;
    [SerializeField] protected ParticleSystem _particles;
    [SerializeField] protected LayerMask _whatIsGround, _whatIsPlayer, _whatIsWall;

    [SerializeField] protected GameObject _explosionPrefab;


    [Header("Movimiento")]
    [SerializeField] protected float _movementSpeed;
    [SerializeField] protected bool _playerInSightRange, _playerInAttackRange;
    [SerializeField] protected float _sightRange, _attackRange;
    [SerializeField] protected float _pointsOnKill;

    protected bool _hasExploded;


    public override void TakeDamage(int dmg)
    {
        PopUp();
        base.TakeDamage(dmg);
    }

    protected override void Death()
    {
        Destroy(gameObject);
    }

    protected virtual void Explode()
    {
        if (_explosionPrefab != null)
        {
            SoundManager.PlaySound(SoundType.DRONEXPLODE, SoundManager.Instance.GetSFXVolume());
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    protected virtual void Explode(float timer)
    {
        if (_explosionPrefab != null)
        {
            SoundManager.PlaySound(SoundType.DRONEXPLODE, SoundManager.Instance.GetSFXVolume());
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, timer);
    }

    protected virtual void Awake()
    {
        _player = GameObject.Find("Player")?.transform;
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        isAlive = true;
        _hasExploded = false;
    }

    protected virtual void Start()
    {
        var frenzyManagerInstance = GameObject.FindGameObjectWithTag("FrenzyManager");
        if (frenzyManagerInstance != null)
        {
            _frenzyManager = frenzyManagerInstance.GetComponent<FrenzyManager>();
        }
    }
}
