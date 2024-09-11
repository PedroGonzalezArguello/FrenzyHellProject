using UnityEngine;
using System.Collections;

public class TroncoScript : MonoBehaviour
{
    [SerializeField] private float moveDownDistance = 5f;
    public float moveDownSpeed = 10f;
    public float moveUpSpeed = 5f;
    public float waitTime = 2f;
    public LayerMask playerLayer;
    public Vector3 detectionBoxSize = new Vector3();
    public AudioClip prensaSound;
    public float rotationSpeed = 45f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool playerInRange;
    private AudioSource audioSource;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition - new Vector3(0, moveDownDistance, 0); // Calcula la posición objetivo hacia abajo
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = prensaSound;
        audioSource.loop = true;
        StartCoroutine(MoveDownAndUp());
    }

    void Update()
    {
        // Rotar constantemente el objeto alrededor de su eje Y
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // Verificar si el jugador está en rango
        CheckPlayerInRange();
    }

    IEnumerator MoveDownAndUp()
    {
        // Mueve el objeto hacia abajo
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveDownSpeed * Time.deltaTime);
            yield return null; // Espera al siguiente frame
        }

        // Espera por un tiempo
        yield return new WaitForSeconds(waitTime);

        // Mueve el objeto de regreso a su posición inicial
        while (transform.position != initialPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveUpSpeed * Time.deltaTime);
            yield return null; // Espera al siguiente frame
        }

        // Espera por un tiempo antes de volver a bajar
        yield return new WaitForSeconds(waitTime);

        // Llama de nuevo a la corutina para repetir el ciclo
        StartCoroutine(MoveDownAndUp());
    }

    private void CheckPlayerInRange()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, detectionBoxSize / 2, Quaternion.identity, playerLayer);
        playerInRange = hitColliders.Length > 0;

        if (playerInRange && !audioSource.isPlaying)
        {
            // Ajuste del volumen basado en la distancia mínima al jugador
            float minDistance = float.MaxValue;
            foreach (var hitCollider in hitColliders)
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }

            // Ajuste del volumen basado en la distancia mínima al jugador
            float maxDistance = detectionBoxSize.magnitude / 1.3f; // Distancia máxima basada en la diagonal del cubo
            float volume = Mathf.Clamp01(0.7f - (minDistance / maxDistance));
            audioSource.volume = volume;
            audioSource.Play();
        }
        else if (!playerInRange && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Método para dibujar el rango de detección en el editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, detectionBoxSize);
    }
}