using UnityEngine;

public class RandomRoomSpawner : MonoBehaviour
{
    public GameObject corridorPrefab; // Prefab del pasillo
    public Transform corridorSpawnPoint; // Donde se genera el pasillo

    private bool corridorSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !corridorSpawned)
        {
            // Instanciar el pasillo
            GameObject corridor = Instantiate(corridorPrefab, corridorSpawnPoint.position, corridorSpawnPoint.rotation);

            // Pasar la referencia al pasillo, no es necesario ya que usa GameManager
            Debug.Log("Pasillo generado desde la habitación");

            // Marcar que ya se ha generado el pasillo
            corridorSpawned = true;
        }
    }
}
