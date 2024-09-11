using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    public List<Transform> waypoints;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public Transform target;
    public float health;
    public FrenzyManager frenzyManager;
    public Transform shootPoint;
    public Rigidbody rb;
    public SoundManager soundManager;
    public ParticleSystem particles;

    public bool isAlive;

    // Attacking
    public float timeBetweenAttacks;
    public float upwardForce;
    public float fowardForce;
    public bool alreadyAttacked;
    public GameObject projectile;
    public float movementSpeed;

    // Death Animation
    public float fowardAnimForce;
    public float downAnimForce;
    public bool isDestroyable;
    public GameObject explosion;

    //Eventos
    public delegate void EnemigoEliminadoHandler(EnemyDrone enemydrone);
    public static event EnemigoEliminadoHandler EnemigoEliminado;

    //Animation
    public Animation Anim;
    public Animator animator;

    public enum IAStates
    {
        WALK,
        SEARCH,
        CHASE,
        ATTACK,
        DEATH
    }

    public Dictionary<IAStates, BaseState> _posibleStates { private set; get; }

    private BaseState _actualState;

    public float speed;
    public BaseState ActualState
    {
        get => _actualState;
        set
        {
            if (value != _actualState) _actualState = value;
        }
    }

    void Awake()
    {
        var frenzyManagerInstance = GameObject.FindGameObjectWithTag("FrenzyManager");
        if (frenzyManagerInstance != null)
        {
            frenzyManager = frenzyManagerInstance.GetComponent<FrenzyManager>();
        }

        _posibleStates = new Dictionary<IAStates, BaseState>();

        _posibleStates.Add(IAStates.WALK, new DroneWalk(waypoints));
        _posibleStates.Add(IAStates.SEARCH, new DroneSearch());
        _posibleStates.Add(IAStates.CHASE, new DroneChase());
        _posibleStates.Add(IAStates.ATTACK, new DroneAttack());
        _posibleStates.Add(IAStates.DEATH, new DroneDeath());

        foreach (var state in _posibleStates.Values) { state.SetUp(this); }

        _actualState = _posibleStates[IAStates.WALK];
    }

    void Update()
    {
        _actualState.OnUpdate();
    }

    public void ChangeState(IAStates targetState)
    {
        _actualState?.OnExit();
        _actualState = _posibleStates[targetState];
        _actualState.OnEnter();
    }
}


public abstract class BaseState
{
    protected EnemyIA _enemy;
    protected LayerMask _playerMask;

    //public BaseState(EnemyIA enemy)
    //{
    //    _enemy = enemy; 
    //}

    public void SetUp(EnemyIA enemy)
    {
        _enemy = enemy;
        _playerMask = _enemy.playerMask;
    }

    public abstract void OnUpdate();
    public abstract void OnEnter();
    public abstract void OnExit();
}


