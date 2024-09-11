using UnityEngine;
using System.Collections;

public class PrensaScript : MonoBehaviour
{
    [SerializeField] private float moveDownDistance;
    public float moveDownSpeed;
    public float moveUpSpeed;
    public float waitTime;
    public LayerMask playerLayer;
    public Vector3 detectionBoxSize = new Vector3();
    public AudioClip prensaSound;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool playerInRange;
    private AudioSource audioSource;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition - new Vector3(0, moveDownDistance, 0);
        audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(MoveDownAndUp());
    }

    IEnumerator MoveDownAndUp()
    {
        // Mueve el objeto hacia abajo
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveDownSpeed * Time.deltaTime);
            yield return null; // Espera al siguiente frame
        }

        CheckPlayerInRange();

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

        if (playerInRange)
        {
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
            float maxDistance = detectionBoxSize.magnitude / 0.5f; // Distancia máxima basada en la diagonal del cubo
            float volume = Mathf.Clamp01(0.2f - (minDistance / maxDistance));
            audioSource.volume = volume;
            audioSource.PlayOneShot(prensaSound);
        }
    }

    // Método para dibujar el rango de detección en el editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, detectionBoxSize);
    }
}