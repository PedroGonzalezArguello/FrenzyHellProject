using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpCam : MonoBehaviour
{
    private Camera _cam;

    private void Awake()
    {
        // Intenta encontrar la c�mara principal
        _cam = Camera.main;

        // Si no se encuentra una c�mara principal, busca una c�mara alternativa
        if (_cam == null)
        {
            _cam = FindObjectOfType<Camera>();
            if (_cam == null)
            {
                Debug.LogError("No se ha encontrado ninguna c�mara en la escena.");
            }
        }
    }

    private void Update()
    {
        if (_cam != null)
        {
            Vector3 cam = transform.position - _cam.transform.position;
            transform.forward = cam.normalized;
        }
        else
        {
            Debug.LogWarning("No hay c�mara asignada.");
        }
    }
}
