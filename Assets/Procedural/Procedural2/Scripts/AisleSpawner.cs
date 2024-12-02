using UnityEngine;

public class AisleSpawner : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public GameObject finalRoomPrefab;
    public Transform roomSpawnPoint;

    public int maxRooms;
    private bool roomSpawned = false;
    private int lastRoomIndex = -1; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !roomSpawned)
        {
            GameObject newRoom;

            if (GameManager.instance.GetRoomCount() >= maxRooms)
            {
              
                newRoom = Instantiate(finalRoomPrefab, roomSpawnPoint.position, roomSpawnPoint.rotation);
                Debug.Log("Generando la última habitación: " + finalRoomPrefab.name);
            }
            else
            {
                int randomIndex;

                if (roomPrefabs.Length > 1)
                {
                    do
                    {
                        randomIndex = Random.Range(0, roomPrefabs.Length);
                    } while (randomIndex == lastRoomIndex);
                }
                else
                {
                    randomIndex = 0;
                }

                newRoom = Instantiate(roomPrefabs[randomIndex], roomSpawnPoint.position, roomSpawnPoint.rotation);
                Debug.Log("Generando habitación normal: " + roomPrefabs[randomIndex].name);

                lastRoomIndex = randomIndex;
            }

            if (newRoom != null)
            {
                GameManager.instance.AddRoom(newRoom);
            }

            roomSpawned = true;
        }
    }
}
