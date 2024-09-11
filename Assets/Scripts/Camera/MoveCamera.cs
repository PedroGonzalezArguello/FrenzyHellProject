using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    // Transformación que representa la posición de la cámara
    [SerializeField] Transform cameraPosition;
    void Update()
    {
        // Actualiza la posición del objeto alineándola con la posición de la cámara
        transform.position = cameraPosition.position;
    }
}
