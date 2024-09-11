using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    [Header("<color=blue>NormalShake</color>")]

    // Duración total del shake
    public float shakeDuration = 0.5f;
    // Intensidad del shake
    public float shakeMagnitude = 0.2f;
    // Velocidad del shake
    public float shakeSpeed = 1.0f;
    // Offset del shake
    public Vector2 offset = Vector2.zero;

    [Header("<color=red>AttackShake</color>")]

    // Duración total del attackshake
    public float _attackShakeDuration = 0.5f;
    // Intensidad del attackshake
    public float _attackShakeMagnitude = 0.2f;
    // Velocidad del attackshake
    public float _attackShakeSpeed = 1.0f;
    // Offset del attackshake
    public Vector2 _attackOffset = Vector2.zero;

    public static CameraShake instance;

    private Transform camTransform;


    private Vector3 originalPos;

    void Awake()
    {
        // Obtener el transform de la cámara
        camTransform = transform;
        // Guardar la posición original de la cámara
        originalPos = camTransform.localPosition;

        if (instance == null)
        {
            instance = this;
        }

    }

    // Método para iniciar el shake
    public void StartShake()
    {
        StopAllCoroutines();
        StartCoroutine(StandardShake());
    }

    public void StartAttackShake(float value)
    {
        StopAllCoroutines();
        StartCoroutine(AttackShake(value));
    }

    IEnumerator StandardShake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Generar una posición aleatoria para el shake con el offset
            float x = Random.Range(-1f, 1f) * shakeMagnitude + offset.x;
            float y = Random.Range(-1f, 1f) * shakeMagnitude + offset.y;

            // Modificar la posición de la cámara
            camTransform.localPosition = originalPos + new Vector3(x, y, 0);

            // Esperar un frame
            elapsedTime += Time.deltaTime * shakeSpeed;
            yield return null;
        }

        // Restaurar la posición original de la cámara
        camTransform.localPosition = originalPos;
    }

    IEnumerator AttackShake(float value)
    {
        float elapsedTime = 0f;

        while (elapsedTime < _attackShakeDuration)
        {
            // Generar una posición aleatoria para el shake con el offset
            float x = Random.Range(-1f, 1f) * (_attackShakeMagnitude + offset.x) * value;
            float y = Random.Range(-1f, 1f) * (_attackShakeMagnitude + offset.y) * value;

            // Modificar la posición de la cámara
            camTransform.localPosition = originalPos + new Vector3(x, y, 0);

            // Esperar un frame
            elapsedTime += Time.deltaTime * shakeSpeed;
            yield return null;
        }

        // Restaurar la posición original de la cámara
        camTransform.localPosition = originalPos;
    }
}
