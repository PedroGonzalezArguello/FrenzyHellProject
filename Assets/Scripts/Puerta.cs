using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puerta : MonoBehaviour
{
    private bool abierta = false;
    private Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    public void AbrirPuerta()
    {
        if (!abierta)
        {
            abierta = true;
            //transform.Translate(Vector3.right * 10f);
            anim.Play();
            Debug.Log("Puerta abierta");
        }
    }
}
