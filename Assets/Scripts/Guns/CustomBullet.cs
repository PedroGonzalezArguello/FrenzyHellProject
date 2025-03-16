using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CustomBullet : MonoBehaviour, IParryable
{
    // Asignables y propiedades de la bala
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    public float bounciness;
    public bool useGravity;
    public Color materialColor;

    public int explosionDamage;
    public float explosionRange;

    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;

    private int collisions;
    private bool hasExploded = false;

    public FrenzyManager frenzyManager;
    [SerializeField] private EnemyTurret enemyTurret; 

    private void Start()
    {
        Setup();
        GameObject frenzyManagerInstance = GameObject.FindGameObjectWithTag("FrenzyManager");
        if (frenzyManagerInstance != null)
        {
            frenzyManager = frenzyManagerInstance.GetComponent<FrenzyManager>();
        }
    }

    private void Update()
    {
        if (collisions > maxCollisions) Explode();
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }
    public void SetTurret(EnemyTurret _enemyTurret)
    {
        enemyTurret = _enemyTurret;
    }

    // Implementación del parry
    public IEnumerator Parry()
    {
        // Congelar el tiempo
        Time.timeScale = 0f;
        SoundManager.PlaySound(SoundType.FREEZETIME, SoundManager.Instance.GetSFXVolume());

        // Esperar 1 segundo en tiempo real (sin ser afectado por Time.timeScale)
        yield return new WaitForSecondsRealtime(0.6f);

        // Restaurar el tiempo
        Time.timeScale = 1f;

        enemyTurret.TurretOffTime();


        // Destruir la bala después del parry
        Destroy(gameObject);
    }

    private void Explode()
    {
        if (!hasExploded)
        {
            hasExploded = true;

            if (explosion != null)
            {
                GameObject explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(explosionInstance, 2f);
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    frenzyManager.TakeDamage(explosionDamage);
                }
            }

            Destroy(gameObject);
        }
    }
    
   





    private void Setup()
    {
        // Crear un material físico para el rebote
        PhysicMaterial physics_mat = new PhysicMaterial
        {
            bounciness = bounciness,
            frictionCombine = PhysicMaterialCombine.Minimum,
            bounceCombine = PhysicMaterialCombine.Maximum
        };

        GetComponent<SphereCollider>().material = physics_mat;
        rb.useGravity = useGravity;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = materialColor;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet")) return;
        collisions++;

        if (collision.collider.CompareTag("Player") && explodeOnTouch)
        {
            Explode();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
