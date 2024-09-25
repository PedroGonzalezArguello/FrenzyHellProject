using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] _bottomRooms;
    public GameObject[] _topRooms;
    public GameObject[] _leftRooms;
    public GameObject[] _rightRooms;

    public GameObject _closedRoom;
    public List<GameObject> _rooms;

    public int maxRooms = 10; // Máximo número de habitaciones permitidas

    public GameObject DroneEnemies;

    private void Start()
    {
        Invoke("SpawnDroneEnemies", 3f);
    }

    void SpawnDroneEnemies()
    {
        for (int i = 0; i < _rooms.Count - 1; i++)
        {
            Instantiate(DroneEnemies, _rooms[i].transform.position, Quaternion.identity);
        }
    }
}
