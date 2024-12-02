using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] _topRooms;    // Prefabs con puerta en la parte superior
    public GameObject[] _bottomRooms; // Prefabs con puerta en la parte inferior
    public GameObject[] _leftRooms;   // Prefabs con puerta en la izquierda
    public GameObject[] _rightRooms;  // Prefabs con puerta en la derecha
    public GameObject _closedRoom;    // Prefab de la habitación cerrada
    public List<GameObject> _rooms;   // Lista de todas las habitaciones generadas
    public int maxRooms = 10;         // Número máximo de habitaciones a generar
}
