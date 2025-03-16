using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    [SerializeField] private int _openSide; // 1=Bottom, 2=Top, 3=Left, 4=Right
    private int rand;
    private RoomTemplates _templates;
    private bool _spawned = false;
    private Collider _collider;

    private float spawnDelay = 0.1f;

    void Start()
    {
        _templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        _collider = GetComponent<Collider>();
        Invoke("Spawn", spawnDelay);
    }

    void Spawn()
    {
        if (!_spawned && _templates._rooms.Count < _templates.maxRooms - 1)
        {
            GameObject roomToSpawn = null;

            // Selecciona la habitación basada en el lado abierto
            switch (_openSide)
            {
                case 1: //puerta inferior
                    rand = Random.Range(0, _templates._bottomRooms.Length);
                    roomToSpawn = _templates._bottomRooms[rand];
                    break;
                case 2: //puerta superior
                    rand = Random.Range(0, _templates._topRooms.Length);
                    roomToSpawn = _templates._topRooms[rand];
                    break;
                case 3: // puerta izquierda
                    rand = Random.Range(0, _templates._leftRooms.Length);
                    roomToSpawn = _templates._leftRooms[rand];
                    break;
                case 4: // puerta derecha
                    rand = Random.Range(0, _templates._rightRooms.Length);
                    roomToSpawn = _templates._rightRooms[rand];
                    break;
            }

            if (roomToSpawn != null)
            {
                Instantiate(roomToSpawn, transform.position, roomToSpawn.transform.rotation);
                _spawned = true;
            }
        }
        else if (!_spawned) // Si se genera la última habitación, asegúrate de que sea una habitación cerrada
        {
            Instantiate(_templates._closedRoom, transform.position, Quaternion.identity);
            _spawned = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            RoomSpawner otherSpawner = other.GetComponent<RoomSpawner>();

            if (!otherSpawner._spawned && !_spawned)
            {
                Instantiate(_templates._closedRoom, transform.position, Quaternion.identity);
            }

            _spawned = true;
            Destroy(gameObject); // Destruir este spawn point para evitar duplicaciones
        }
    }
}
