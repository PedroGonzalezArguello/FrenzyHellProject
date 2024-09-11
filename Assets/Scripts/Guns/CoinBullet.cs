using UnityEngine;

public class CoinBullet : MonoBehaviour
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

    //Lifetime
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;
    public float rotationBulletSpeed;

    int collisions;
    bool hasExploded; // Flag to track if an explosion has occurred

    public FrenzyManager frenzyManager;

    PhysicMaterial physics_mat;

    private void Start()
    {
        Setup();       
    }
    private void Setup()
    {
        GameObject frenzyManagerInstance = GameObject.FindGameObjectWithTag("FrenzyManager");
        if (frenzyManagerInstance != null)
        {
            frenzyManager = frenzyManagerInstance.GetComponent<FrenzyManager>();
        }

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

    private void Update()
    {
        transform.Rotate(Vector3.right, rotationBulletSpeed * Time.deltaTime);

        //When to explode:
        if (collisions > maxCollisions) Destroy(gameObject);

        //Count down lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Destroy(gameObject);

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Don't count collisions with other bullets
        if (collision.collider.CompareTag("Player")) return;
        collisions++;
    }

    public void Explode()
    {
        if (!hasExploded) // Only execute explosion logic if it hasn't already exploded
        {
            hasExploded = true; // Set the flag to true to prevent further explosions

            // Instantiate explosion
            if (explosion != null)
            {
                GameObject explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity);
                //explosionInstance.transform.localScale = new Vector3(2,2,2);
                Destroy(explosionInstance, 2f); // Destroy the explosion after 2 seconds
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);

            foreach (Collider collider in colliders)
            {
                //Check for enemies 
                Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies != null)
                    { 
                        //Get component of enemy and call Take Damage
                        enemies[i].GetComponent<EnemyDrone>().TakeDamage(explosionDamage);
                    }       
                }

                Collider[] kamikaze = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
                for (int i = 0; i < kamikaze.Length; i++)
                {
                    if (kamikaze != null)
                    {
                        //Get component of enemy and call Take Damage
                        kamikaze[i].GetComponent<EnemyDrone>().TakeDamage(explosionDamage);
                    }
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


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
