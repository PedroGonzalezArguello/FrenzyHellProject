using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] float swayMultiplier;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;



    private void Update()
    {
        // Mouse Input 
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        // Teclado Input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");


        // Calcular el ángulo de rotación basado en la entrada horizontal
        float swayRotationX = -horizontalInput * swayMultiplier;
        float swayRotationY = -verticalInput * (swayMultiplier*1.8f);

        // Mouse Rotation
        Quaternion rotationMouseX = Quaternion.AngleAxis(-mouseY * 2, Vector3.right);
        Quaternion rotationMouseY = Quaternion.AngleAxis(mouseX, Vector3.up);
        Quaternion targetRotationMouse = rotationMouseX * rotationMouseY;

        // Crear la rotación deseada para el sway
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, swayRotationX * 1.5f);
        Quaternion targetRotationY = Quaternion.Euler(0f, swayRotationY * 1.5f, 0f);

        // Aplicar la rotación suavizada al arma
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotationY, smooth * Time.deltaTime);

        // Aplicar rotación del mouse
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotationMouse, smooth * 1.8f * Time.deltaTime);


    }



}
