using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalaEnemy : MonoBehaviour
{
    public Puerta puerta;
    private int cantidadEnemigos;

    private void OnEnable()
    {
        EnemyDrone.EnemigoEliminado += OnEnemigoEliminado;
    }

    private void OnDisable()
    {
        EnemyDrone.EnemigoEliminado -= OnEnemigoEliminado;
    }

    private void Start()
    {
        cantidadEnemigos = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            cantidadEnemigos--;
            Debug.Log("Enemigo salió de la sala. Total enemigos: " + cantidadEnemigos);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            cantidadEnemigos--;
            Debug.Log("Enemigo salió de la sala. Total enemigos: " + cantidadEnemigos);
        }
    }

    private void OnEnemigoEliminado(EnemyDrone enemigo)
    {
        // Verificar si el enemigo eliminado estaba dentro de la sala
        Collider enemigoCollider = enemigo.GetComponent<Collider>();
        if (enemigoCollider != null && enemigoCollider.bounds.Intersects(GetComponent<Collider>().bounds))
        {
            cantidadEnemigos--;
            Debug.Log("Enemigo eliminado en la sala. Total enemigos: " + cantidadEnemigos);

            if (cantidadEnemigos <= 0)
            {
                puerta.AbrirPuerta();
            }
        }
    }
}
