using UnityEngine;

public class AisleSpawner : MonoBehaviour
{
    public GameObject[] roomPrefabs;   // Prefabs de habitaciones normales
    public GameObject finalRoomPrefab; // Prefab de la �ltima habitaci�n
    public Transform roomSpawnPoint;   // Punto donde se genera la habitaci�n

    public int maxRooms = 10;  // M�ximo de habitaciones antes de generar la �ltima

    private bool roomSpawned = false;  // Evita m�ltiples instancias

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !roomSpawned)
        {
            GameObject newRoom;

            // Verificar si ya se ha alcanzado el n�mero m�ximo de habitaciones
            if (GameManager.instance.GetRoomCount() >= maxRooms)
            {
                // Instanciar la �ltima habitaci�n
                newRoom = Instantiate(finalRoomPrefab, roomSpawnPoint.position, roomSpawnPoint.rotation);
                Debug.Log("Generando la �ltima habitaci�n: " + finalRoomPrefab.name);
            }
            else
            {
                // Seleccionar una habitaci�n aleatoria del array de habitaciones normales
                int randomIndex = Random.Range(0, roomPrefabs.Length);
                newRoom = Instantiate(roomPrefabs[randomIndex], roomSpawnPoint.position, roomSpawnPoint.rotation);
                Debug.Log("Generando habitaci�n normal: " + roomPrefabs[randomIndex].name);
            }

            // Verificar si la nueva habitaci�n fue instanciada correctamente
            if (newRoom != null)
            {
                // A�adir la nueva habitaci�n generada a la lista global en el GameManager
                GameManager.instance.AddRoom(newRoom);
            }

            // Marcar que la habitaci�n ya fue instanciada
            roomSpawned = true;
        }
    }
}
