using UnityEngine;
using System.Collections;

public class SpawnerSelector : MonoBehaviour
{
    // Intervalo de tiempo en segundos
    public float interval;

    void Start()
    {
        // Inicia la corutina que seleccionar� un hijo aleatorio en intervalos regulares
        StartCoroutine(SelectRandomChildAtIntervals());
    }

    IEnumerator SelectRandomChildAtIntervals()
    {
        while (true)
        {
            // Espera el intervalo de tiempo especificado
            yield return new WaitForSeconds(interval);

            // Selecciona un hijo aleatorio
            SelectRandomChild();
        }
    }

    void SelectRandomChild()
    {
        int childCount = transform.childCount;

        if (childCount == 0)
        {
            Debug.LogWarning("No hay hijos en el objeto padre.");
            return;
        }

        // Selecciona un �ndice aleatorio en la lista de hijos
        int randomIndex = Random.Range(0, childCount);
        // Obt�n el hijo aleatorio
        Transform selectedChild = transform.GetChild(randomIndex);

        // Llama a la funci�n 'SpawnDrone' en el hijo seleccionado, si existe
        DroneSpawner spawner = selectedChild.GetComponent<DroneSpawner>();
        if (spawner != null)
        {
            spawner.SpawnDrone();
        }
        else
        {
            Debug.LogWarning("El objeto hijo seleccionado no tiene el componente DroneSpawner.");
        }
    }

}
