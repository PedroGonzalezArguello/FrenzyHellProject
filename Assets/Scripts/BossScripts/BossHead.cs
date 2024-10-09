using UnityEngine;

public class BossHead : MonoBehaviour
{
    [SerializeField] private float firstBossLife;

    public Transform player;
    public Transform rayOrigin;
    public LineRenderer lineRenderer;
    public float damageAmount;  
    public float visionRange;
    public float rotationSpeed;
    [SerializeField] private FrenzyManager frenzyManager;
    [SerializeField] private GameManager gameManager;

    private float patrolTimer;
    private bool rotateClockwise = true;
    [SerializeField] private float patrolTime = 3f;

    private float timeSinceLastDamage = 0f;
    private float damageInterval = 3f;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        if (player != null && frenzyManager == null)
        {
            frenzyManager = FrenzyManager.Instance;
        }

        patrolTimer = patrolTime;
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            lineRenderer.enabled = true;

            if (distanceToPlayer <= visionRange)
            {
                // Rotación suavizada hacia el jugador
                Vector3 direction = player.position - transform.position;
                Quaternion desiredRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);

                // Disparar rayo
                ShootRay();
            }
            else
            {
                // Si el jugador está fuera del rango
                lineRenderer.SetPosition(0, rayOrigin.position);
                lineRenderer.SetPosition(1, rayOrigin.position + rayOrigin.forward * 100f);
                Patrol();
                lineRenderer.enabled = false;
            }
        }
    }

    void ShootRay()
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        RaycastHit hit;

        lineRenderer.SetPosition(0, rayOrigin.position);

        if (Physics.Raycast(ray, out hit))
        {
            lineRenderer.SetPosition(1, hit.point);

            // Aquí infligimos daño al jugador si el rayo lo golpea
            if (hit.transform == player && frenzyManager != null)
            {
                // Verificar si han pasado 5 segundos desde el último daño
                if (timeSinceLastDamage >= damageInterval)
                {
                    frenzyManager.TakeDamage(damageAmount); // Infligir el daño
                    timeSinceLastDamage = 0f; // Reiniciar el temporizador
                    Debug.Log("Jugador detectado y alcanzado por el rayo. Daño infligido.");
                }
                else
                {
                    timeSinceLastDamage += Time.deltaTime;
                }
            }
        }
        else
        {
            lineRenderer.SetPosition(1, rayOrigin.position + rayOrigin.forward * 100f); // Longitud del rayo
        }
    }

    public void TakeDamage(float damage)
    {
        firstBossLife -= damage;

        if (firstBossLife <= 0)
        {
            // Acciones cuando la vida del jefe llegue a 0
        }
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0)
        {
            rotateClockwise = !rotateClockwise;
            patrolTimer = patrolTime;
        }

        float rotationDirection = rotateClockwise ? 1f : -1f;
        transform.Rotate(Vector3.up, rotationSpeed * rotationDirection * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
