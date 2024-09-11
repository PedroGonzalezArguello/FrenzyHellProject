using UnityEngine;

public class RotatorBoss : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public Transform shootPoint1;
    public Transform shootPoint2;
    public Transform shootPoint3;
    public GameObject projectile;
    public float shootInterval = 1f;
    public float bulletSpeed = 10f;

    private float shootTimer;
    public float detectionRadius = 5f;
    public LayerMask playerLayer;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        // Verificar si el jugador está dentro del área de detección
        Collider[] playerDetected = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (playerDetected.Length > 0)
        {
            // Rotar el objeto alrededor de su eje Y
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            // Actualizar el temporizador de disparo
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootInterval;
            }
        }
    }

    void Shoot()
    {
        ShootProjectile(shootPoint1);
        ShootProjectile(shootPoint2);
        ShootProjectile(shootPoint3);
    }

    // Método auxiliar para instanciar y configurar el proyectil
    void ShootProjectile(Transform shootPoint)
    {
        GameObject instantiatedProjectile = Instantiate(projectile, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = instantiatedProjectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootPoint.forward * bulletSpeed;
        }
    }

    // Método para dibujar el área de detección en la vista de escena
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
