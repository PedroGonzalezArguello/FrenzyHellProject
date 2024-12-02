using UnityEngine;

public class RandomRoomSpawner : MonoBehaviour
{
    public GameObject corridorPrefab;
    public Transform corridorSpawnPoint; 

    private bool corridorSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !corridorSpawned)
        {
            GameObject corridor = Instantiate(corridorPrefab, corridorSpawnPoint.position, corridorSpawnPoint.rotation);

            GameManager.instance.AddCorridor(corridor);

            Debug.Log("Pasillo generado y añadido al GameManager");

            corridorSpawned = true;
        }
    }
}
