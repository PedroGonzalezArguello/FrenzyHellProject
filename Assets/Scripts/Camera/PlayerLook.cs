using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerLook : MonoBehaviour
{
    // Sensibilidad del ratón en los ejes X e Y
    [SerializeField] public float sensX;
    [SerializeField] public float sensY;

    // Transformación de la cámara y la orientación del jugador
    [SerializeField] Transform Cam;
    [SerializeField] Transform Orientation;
    public Transform camHolder;

    // Variables para almacenar el movimiento del ratón
    float mouseX;
    float mouseY;

    // Factor de multiplicación para ajustar la sensibilidad
    float multiplier = 0.01f;

    // Rotaciones en los ejes X e Y
    float xRotation;
    float yRotation;

    public bool pauseActive = false;

    private void Start()
    {
        // Bloquea el cursor del ratón y lo hace invisible
        
    }

    private void Update()
    {
        if(pauseActive != true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Captura la entrada del ratón y actualiza las rotaciones de la cámara y la orientación del jugador
            MyInput();

            camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            Orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
        
    }

    // Obtiene la entrada del ratón
    void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        // Calcula las rotaciones en los ejes X e Y
        yRotation += mouseX * sensX * multiplier;
        xRotation += mouseY * sensY * multiplier * -1;

        // Limita la rotación en el eje X entre -90 y 90 grados para evitar que la cámara gire completamente
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0f, 0f, zTilt), 0.25f);
    }
}
