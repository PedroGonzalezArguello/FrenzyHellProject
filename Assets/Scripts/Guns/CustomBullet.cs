using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    //Assignables
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    //Stats
    [Range(0f,1f)]
    public float bounciness;
    public bool useGravity;
    public Color materialColor;
    

    //Damage
    public int explosionDamage;
    public float explosionRange;
    //public float explosionForce;

    //Lifetime
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;

    int collisions;
    bool hasExploded; // Flag to track if an explosion has occurred
    //public bool isPlayerBullet; // Indica si el proyectil fue disparado por el jugador

    public FrenzyManager frenzyManager;


    PhysicMaterial physics_mat;

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
        //When to explode:
        if (collisions > maxCollisions) Explode();

        //Count down lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();

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
                Destroy(explosionInstance, 2f); // Destroy the explosion after 2 seconds
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);

            foreach (Collider collider in colliders)
            {
                // Verifica si el objeto detectado es el jugador
                if (collider.CompareTag("Player"))
                {
                    frenzyManager.TakeDamage(explosionDamage);
                    rb = collider.GetComponentInParent<Rigidbody>();
                    //rb.AddExplosionForce(explosionForce, transform.position, explosionRange);
                    //Debug.Log("RigidBody y fuerza aplicada");
                    
                }
                else
                {
                    /*
                        //Check for enemies 
                        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
                        for (int i = 0; i < enemies.Length; i++)
                        {
                        //Get component of enemy and call Take Damage

                        //Just an example!
                        enemies[i].GetComponent<FrenzyManager>().TakeDamage(explosionDamage);

                        //Add explosion force (if enemy has a rigidbody)
                        if (enemies[i].GetComponent<Rigidbody>())
                        enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
                        }
                    */
                }
            }

            //Add a little delay, just to make sure everything works fine
            Invoke("Delay", 0.05f);
        }
    }
    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Don't count collisions with other bullets
        if (collision.collider.CompareTag("Bullet")) return;

        //Count up collisions
        collisions++;

        //Explode if bullet hits an enemy directly and explodeOnTouch is activated
        if (collision.collider.CompareTag("Player") && explodeOnTouch) Explode();
    }

    private void Setup()
    {
        //Create a new Physic material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;

        // Asignar color al material visual (renderer)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = materialColor;
        }


        //Assign material to collider
        GetComponent<SphereCollider>().material = physics_mat;

        //Set gravity
        rb.useGravity = useGravity;
    }

    /// Just to visualize the explosion range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
