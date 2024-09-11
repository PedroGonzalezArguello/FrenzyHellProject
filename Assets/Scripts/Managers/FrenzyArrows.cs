using UnityEngine;
using System.Collections;

public class FrenzyArrows : MonoBehaviour
{
    public SpriteRenderer Arrow1;
    public SpriteRenderer Arrow2;
    public SpriteRenderer Arrow3;
    public float displayTime;

    void Start()
    {
        StartCoroutine(SwitchArrows());
    }

    IEnumerator SwitchArrows()
    {
        // Mostrar el primer sprite
        Arrow1.enabled = true;
        Arrow2.enabled = false;
        Arrow3.enabled = false;

        // Esperar el tiempo especificado
        yield return new WaitForSeconds(displayTime);

        // Apagar el primer sprite y prender el segundo
        Arrow1.enabled = false;
        Arrow2.enabled = true;
        Arrow3.enabled = false;

        yield return new WaitForSeconds(displayTime);

        // Apagar el segundo sprite y prender el tercero
        Arrow2.enabled = false;
        Arrow3.enabled = true;
        Arrow1.enabled = false;

        yield return new WaitForSeconds(displayTime);

        // Apagar el tercer sprite y prender el primero
        Arrow3.enabled = false;
        Arrow1.enabled = true;
        Arrow2.enabled = false;

        yield return new WaitForSeconds(displayTime);

        // Llamar a la corrutina nuevamente para crear un bucle sin usar while (true)
        StartCoroutine(SwitchArrows());
    }
}
