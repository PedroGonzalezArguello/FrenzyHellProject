using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRoomToList : MonoBehaviour
{
    private RoomTemplates _roomTemplates;

    void Start()
    {
        _roomTemplates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        _roomTemplates._rooms.Add(this.gameObject); // Agregar la habitación a la lista de habitaciones
    }
}
