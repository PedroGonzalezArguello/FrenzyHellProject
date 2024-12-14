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

    [Header("Obstacle Avoidance")]
    [SerializeField] protected float obstacleRange = 1.5f;
    [SerializeField] protected LayerMask _obstacleMask;

    protected bool _hasExploded;
    [SerializeField]protected int _pointsOnDeath;



    public int PointsOnDeath
    {
        get { return _pointsOnDeath; }
    }

    public override void TakeDamage(int dmg)
    {
        PopUp();
        base.TakeDamage(dmg);
    }

    protected override void Death()
    {
        PointsManager.Instance.AddPoints(_pointsOnDeath);
    }

    //Esto solo explota
    protected virtual void Explode()
    {
        if (_explosionPrefab != null)
        {
            SoundManager.PlaySound(SoundType.DRONEXPLODE, SoundManager.Instance.GetSFXVolume());
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    //Esto explota con timer
    protected virtual IEnumerator Explode(float timer)
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (timer < 0)
        {
            if (_explosionPrefab != null)
            {
                SoundManager.PlaySound(SoundType.DRONEXPLODE, SoundManager.Instance.GetSFXVolume());
                Debug.Log("EXPLODE");
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            }
            Destroy(gameObject);
        }
    }
    public static bool LineOfSight(Vector3 from, Vector3 to)
    {
        var dir = to - from;
        return !Physics.Raycast(from, dir, dir.magnitude, LayerMask.GetMask("Wall"));
    }
    public Vector3 ObstacleAvoidance()
    {
        var obstacles = Physics.OverlapSphere(transform.position, obstacleRange, _obstacleMask);
        Debug.Log(obstacles.Length);

        if (obstacles.Length <= 0) return Vector3.zero;

        var obstacleDir = Vector3.zero;

        foreach (var obstacle in obstacles)
        {
            Debug.Log(obstacle.gameObject.name);
            obstacleDir += transform.position - obstacle.transform.position;
        }

        obstacleDir.y = 0f;

        return obstacleDir;
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
