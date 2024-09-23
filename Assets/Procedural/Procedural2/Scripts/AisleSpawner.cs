using UnityEngine;

public class AisleSpawner : MonoBehaviour
{
    public GameObject[] roomPrefabs;   
    public GameObject finalRoomPrefab; 
    public Transform roomSpawnPoint;

    public int maxRooms; 

    private bool roomSpawned = false; 

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
                int randomIndex = Random.Range(0, roomPrefabs.Length);
                newRoom = Instantiate(roomPrefabs[randomIndex], roomSpawnPoint.position, roomSpawnPoint.rotation);
                Debug.Log("Generando habitación normal: " + roomPrefabs[randomIndex].name);
            }

            if (newRoom != null)
            {
                GameManager.instance.AddRoom(newRoom);
            }

            roomSpawned = true;
        }
    }
}
