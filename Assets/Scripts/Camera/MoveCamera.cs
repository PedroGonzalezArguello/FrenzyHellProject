using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    // Transformaci�n que representa la posici�n de la c�mara
    [SerializeField] Transform cameraPosition;
    void Update()
    {
        // Actualiza la posici�n del objeto aline�ndola con la posici�n de la c�mara
        transform.position = cameraPosition.position;
    }
}
