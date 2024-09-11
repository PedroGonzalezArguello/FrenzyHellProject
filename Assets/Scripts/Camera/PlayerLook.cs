using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerLook : MonoBehaviour
{
    // Sensibilidad del rat�n en los ejes X e Y
    [SerializeField] public float sensX;
    [SerializeField] public float sensY;

    // Transformaci�n de la c�mara y la orientaci�n del jugador
    [SerializeField] Transform Cam;
    [SerializeField] Transform Orientation;
    public Transform camHolder;

    // Variables para almacenar el movimiento del rat�n
    float mouseX;
    float mouseY;

    // Factor de multiplicaci�n para ajustar la sensibilidad
    float multiplier = 0.01f;

    // Rotaciones en los ejes X e Y
    float xRotation;
    float yRotation;

    public bool pauseActive = false;

    private void Start()
    {
        // Bloquea el cursor del rat�n y lo hace invisible
        
    }

    private void Update()
    {
        if(pauseActive != true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Captura la entrada del rat�n y actualiza las rotaciones de la c�mara y la orientaci�n del jugador
            MyInput();

            camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            Orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
        
    }

    // Obtiene la entrada del rat�n
    void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        // Calcula las rotaciones en los ejes X e Y
        yRotation += mouseX * sensX * multiplier;
        xRotation += mouseY * sensY * multiplier * -1;

        // Limita la rotaci�n en el eje X entre -90 y 90 grados para evitar que la c�mara gire completamente
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
