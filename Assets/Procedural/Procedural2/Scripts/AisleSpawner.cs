using UnityEngine;

public class AisleSpawner : MonoBehaviour
{
    public GameObject[] roomPrefabs;   // Prefabs de habitaciones normales
    public GameObject finalRoomPrefab; // Prefab de la última habitación
    public Transform roomSpawnPoint;   // Punto donde se genera la habitación

    public int maxRooms = 10;  // Máximo de habitaciones antes de generar la última

    private bool roomSpawned = false;  // Evita múltiples instancias

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !roomSpawned)
        {
            GameObject newRoom;

            // Verificar si ya se ha alcanzado el número máximo de habitaciones
            if (GameManager.instance.GetRoomCount() >= maxRooms)
            {
                // Instanciar la última habitación
                newRoom = Instantiate(finalRoomPrefab, roomSpawnPoint.position, roomSpawnPoint.rotation);
                Debug.Log("Generando la última habitación: " + finalRoomPrefab.name);
            }
            else
            {
                // Seleccionar una habitación aleatoria del array de habitaciones normales
                int randomIndex = Random.Range(0, roomPrefabs.Length);
                newRoom = Instantiate(roomPrefabs[randomIndex], roomSpawnPoint.position, roomSpawnPoint.rotation);
                Debug.Log("Generando habitación normal: " + roomPrefabs[randomIndex].name);
            }

            // Verificar si la nueva habitación fue instanciada correctamente
            if (newRoom != null)
            {
                // Añadir la nueva habitación generada a la lista global en el GameManager
                GameManager.instance.AddRoom(newRoom);
            }

            // Marcar que la habitación ya fue instanciada
            roomSpawned = true;
        }
    }
}
