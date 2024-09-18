using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    //1 Need Bottom door
    //2 Need Top door
    //3 Need Left door
    //4 Need Right door

    [SerializeField] private int _openSide;


    private RoomTemplates _templates;
    private bool _spawned = false;

    void Start()
    {
        _templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();

        Invoke("Spawn", 0.5f);
    }

    void Spawn()
    {
        if(!_spawned)
        {
            if (_openSide == 1)
            {
                //Need Bottom Door
                var rand = Random.Range(0, _templates._bottomRooms.Length);

                Instantiate(_templates._bottomRooms[rand], transform.position, _templates._bottomRooms[rand].transform.rotation);
            }
            else if (_openSide == 2)
            {
                //Need Top Door
                var rand = Random.Range(0, _templates._topRooms.Length);

                Instantiate(_templates._topRooms[rand], transform.position, _templates._topRooms[rand].transform.rotation);
            }
            else if (_openSide == 3)
            {
                //Need Left Door
                var rand = Random.Range(0, _templates._leftRooms.Length);

                Instantiate(_templates._leftRooms[rand], transform.position, _templates._leftRooms[rand].transform.rotation);
            }
            else if (_openSide == 4)
            {
                //Need Right Door
                var rand = Random.Range(0, _templates._rightRooms.Length);

                Instantiate(_templates._rightRooms[rand], transform.position, _templates._rightRooms[rand].transform.rotation);
            }

            _spawned = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("SpawnPoint"))
        {
            Destroy(gameObject);
        }
    }
}
