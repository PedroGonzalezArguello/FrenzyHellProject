using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    // 1 Need Bottom door
    // 2 Need Top door
    // 3 Need Left door
    // 4 Need Right door

    [SerializeField] private int _openSide;
    [SerializeField] private int rand;

    private RoomTemplates _templates;
    private bool _spawned = false;

    void Start()
    {
        _templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("Spawn", 1f);
    }

    void Spawn()
    {
        if (!_spawned && _templates._rooms.Count < _templates.maxRooms)
        {
            if (_openSide == 1) // Need Bottom Door
            {
                rand = Random.Range(0, _templates._bottomRooms.Length);
                Instantiate(_templates._bottomRooms[rand], transform.position, _templates._bottomRooms[rand].transform.rotation);
            }
            else if (_openSide == 2) // Need Top Door
            {
                rand = Random.Range(0, _templates._topRooms.Length);
                Instantiate(_templates._topRooms[rand], transform.position, _templates._topRooms[rand].transform.rotation);
            }
            else if (_openSide == 3) // Need Left Door
            {
                rand = Random.Range(0, _templates._leftRooms.Length);
                Instantiate(_templates._leftRooms[rand], transform.position, _templates._leftRooms[rand].transform.rotation);
            }
            else if (_openSide == 4) // Need Right Door
            {
                rand = Random.Range(0, _templates._rightRooms.Length);
                Instantiate(_templates._rightRooms[rand], transform.position, _templates._rightRooms[rand].transform.rotation);
            }

            _spawned = true;
        }
        else
        {
            // Si se ha alcanzado el límite de habitaciones, se genera una habitación cerrada
            Instantiate(_templates._closedRoom, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            RoomSpawner otherSpawner = other.GetComponent<RoomSpawner>();

            // Si ni este ni el otro SpawnPoint han generado una habitación, crear una habitación cerrada
            if (!otherSpawner._spawned && !_spawned)
            {
                Instantiate(_templates._closedRoom, transform.position, Quaternion.identity);
                Destroy(gameObject); // Destruir este SpawnPoint
            }

            _spawned = true; // Marcar este SpawnPoint como utilizado
        }
    }
}

