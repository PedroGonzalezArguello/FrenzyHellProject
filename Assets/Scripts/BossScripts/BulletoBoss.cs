using UnityEngine;

public class BulletoBoss : MonoBehaviour
{
    public float speed; 
    public float damage = 10f; 
    public GameObject explosionEffect;
    [SerializeField] private FrenzyManager frenzyManager;

    private void Awake()
    {
        if (frenzyManager != null)
        {
            frenzyManager = FrenzyManager.Instance;
        }
    }
    void Update()
    {
        // Mover el proyectil hacia adelante en la direcci�n que apunta
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
       
        if (frenzyManager != null)
        {
            // Infligir da�o al jugador
            frenzyManager.TakeDamage(damage);
        }

        // Instanciar el efecto de explosi�n en la posici�n de la bala y con la rotaci�n por defecto
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Destruir el proyectil
        Destroy(gameObject);
    }
}
